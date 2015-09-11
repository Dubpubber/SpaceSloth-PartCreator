using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpaceSloth_Part_Creator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private JArray jAr;

        private ObservableCollection<Property> properties;

        public MainWindow() {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.healthValue.KeyDown += new KeyEventHandler(textbox_KeyDown);
            this.healthValue.TextChanged += new TextChangedEventHandler(textbox_DecimalCheck);

            this.repairFactorValue.KeyDown += new KeyEventHandler(textbox_KeyDown);
            this.repairFactorValue.TextChanged += new TextChangedEventHandler(textbox_DecimalCheck);

            this.costValue.KeyDown += new KeyEventHandler(textbox_KeyDown);
            this.costValue.TextChanged += new TextChangedEventHandler(textbox_DecimalCheck);

            properties = new ObservableCollection<Property>();
        }

        /**
         * Generate the part from all the components!
         */
        private void Button_Click(object sender, RoutedEventArgs e) {
            Part part = loadPart();
            part.cleanEntries();
            part.printPart();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".json";
            dlg.Filter = "Json Files (.json)|*.json";

            Nullable<bool> result = dlg.ShowDialog();

            if(result == true) {
                string asset = dlg.FileName;
                jAr = JArray.Parse(File.ReadAllText(asset));
                Console.WriteLine("File: " + (jAr != null));
            }
        }

        private void textbox_KeyDown(object sender, KeyEventArgs e) {
            bool digit = e.Key.ToString().Contains("NumPad") || e.Key.ToString().Contains("D") && !e.Key.ToString().Equals("D");
            Console.WriteLine(digit);
            if (digit)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void textbox_DecimalCheck(object sender, TextChangedEventArgs e) {
            TextBox box = sender as TextBox;
            decimal dec;
            if(!decimal.TryParse(box.Text, out dec) && box.Text.Length > 0) {
                MessageBox.Show(box.Text + " isn't a valid floating point.");
                box.Clear();
            }
        }

        private void listView_Click(object sender, MouseButtonEventArgs e) {
            var item = ItemsControl.ContainerFromElement(partTypeValue, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null) {
                Console.WriteLine("Loading properties for item clicked...");
                if (properties.Count() > 0)
                    properties.Clear();
                loadPartProperties(item);
                Console.WriteLine("Completed. Count: " + properties.Count());
            }
        }

        private void loadPartProperties(ListBoxItem item) {
            Part tempPart = new Part();
            string value = item.Content.ToString();
            switch (value) {
                case "Cockpit":
                    properties.Add(new Property { Key = "HudValue", Value = "0"});
                    break;
                case "GunMount":
                    properties.Add(new Property { Key = "FireRate", Value = "10.0" });
                    break;
                case "Hull":
                    properties.Add(new Property { Key = "Capacity", Value = "100.0" });
                    break;
                case "Thrusters":
                    properties.Add(new Property { Key = "BoostCap", Value = "100.0" });
                    break;
                case "Wing1":
                    properties.Add(new Property { Key = "Torque", Value = "50.0" });
                    break;
                case "Wing2":
                    properties.Add(new Property { Key = "Torque", Value = "50.0" });
                    break;
                case "Shield Generator":
                    properties.Add(new Property { Key = "Power", Value = "25.0" });
                    break;
                case "Reactor":
                    properties.Add(new Property { Key = "MaxSpeed", Value = "0.080" });
                    break;
                case "Armory":
                    properties.Add(new Property { Key = "Capacity", Value = "1" });
                    break;
                case "Tractor Beam":
                    properties.Add(new Property { Key = "Range", Value = "5.0" });
                    break;
                case "Refinery":
                    properties.Add(new Property { Key = "RefineSpeed", Value = "1.0" });
                    break;
            }
            
            // Set the curent dictionary to the new properties.
            tempPart.addProps(properties);

            props.ItemsSource = null;
            props.ItemsSource = properties;

            tempPart.printPart();

        }

        public Part loadPart() {
            Part tempPart = new Part();

            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;

            // set the rank
            if (rankValue.SelectedItem != null) {
                string rankType = ((ListBoxItem)rankValue.SelectedValue).Content.ToString();
                tempPart.Rank = rankType.Substring(4).Replace(" ", "");
            }
            else
                tempPart.Rank = "A";

            // set the health
            if (healthValue.Text.Length > 0) {
                tempPart.Health = float.Parse(healthValue.Text, currentCulture);
            }
            else
                tempPart.Health = 1f;

            // set repair factor
            if (repairFactorValue.Text.Length > 0) {
                tempPart.RepairFactor = float.Parse(repairFactorValue.Text, currentCulture);
            }
            else
                tempPart.RepairFactor = 1.0f;

            // set cost
            if (costValue.Text.Length > 0) {
                tempPart.Cost = float.Parse(costValue.Text, currentCulture);
            }
            else
                tempPart.Cost = 100.0f;

            // set local name
            if (localNameValue.Text.Length > 0) {
                tempPart.LocalName = localNameValue.Text;
            }
            else
                tempPart.LocalName = " ";

            // set file name
            if (fileNameValue.Text.Length > 0) {
                tempPart.FileName = fileNameValue.Text;
            }
            else
                tempPart.FileName = " ";

            Color selectedColor = new Color();
            // set rgb
            if (rgbValue.SelectedColor != null) {
                selectedColor = (Color)rgbValue.SelectedColor;
                tempPart.RGB = selectedColor.R.ToString() + selectedColor.G.ToString() + selectedColor.B.ToString();
            }
            else
                tempPart.RGB = "000000";

            // set alpha
            if (rgbValue.SelectedColor != null) {
                tempPart.Alpha = float.Parse(selectedColor.A.ToString(), currentCulture);
            }
            else
                tempPart.Alpha = 1.0f;

            // set shorthand
            if (shortHandValue.Text.Length > 0) {
                tempPart.ShortHand = shortHandValue.Text;
            }
            else
                tempPart.ShortHand = " ";

            // finally, set part type and visibility
            if (partTypeValue.SelectedItem != null) {
                string value = ((ListBoxItem)partTypeValue.SelectedValue).Content.ToString();
                switch (value) {
                    case "Cockpit":
                        tempPart.PartType = value.ToUpper();
                        tempPart.Visible = true;
                        break;
                    case "GunMount":
                        tempPart.PartType = value.ToUpper();
                        tempPart.Visible = true;
                        break;
                    case "Hull":
                        tempPart.PartType = value.ToUpper();
                        tempPart.Visible = true;
                        break;
                    case "Thrusters":
                        tempPart.PartType = value.ToUpper();
                        tempPart.Visible = true;
                        break;
                    case "Wing1":
                        tempPart.PartType = "WING1";
                        tempPart.Visible = true;
                        break;
                    case "Wing2":
                        tempPart.PartType = "WING2";
                        tempPart.Visible = true;
                        break;
                    case "Shield Generator":
                        tempPart.PartType = "SGENERATOR";
                        tempPart.Visible = true;
                        break;
                    case "Reactor":
                        tempPart.PartType = value.ToUpper();
                        tempPart.Visible = false;
                        break;
                    case "Armory":
                        tempPart.PartType = value.ToUpper();
                        tempPart.Visible = false;
                        break;
                    case "Tractor Beam":
                        tempPart.PartType = "TRACKBEAM";
                        tempPart.Visible = false;
                        break;
                    case "Refinery":
                        tempPart.PartType = value.ToUpper();
                        tempPart.Visible = false;
                        break;
                }
            }
            else
                tempPart.PartType = "COCKPIT";

            return tempPart;
        }

    }
}
