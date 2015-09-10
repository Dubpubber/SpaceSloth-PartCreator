using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
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

        public MainWindow() {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.healthValue.KeyDown += new KeyEventHandler(textbox_KeyDown);
            this.healthValue.TextChanged += new TextChangedEventHandler(textbox_DecimalCheck);
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
            if(box != null) {
                string str = box.Text;
            }
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

            // set visible
            tempPart.Visible = (bool)visibleValue.IsChecked;

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

            // finally, set part type
            if (partTypeValue.SelectedItem != null) {
                string value = ((ListBoxItem)partTypeValue.SelectedValue).Content.ToString();
                switch (value) {
                    case "Cockpit":
                        tempPart.PartType = value.ToUpper();
                        break;
                    case "GunMount":
                        tempPart.PartType = value.ToUpper();
                        break;
                    case "Hull":
                        tempPart.PartType = value.ToUpper();
                        break;
                    case "Thrusters":
                        tempPart.PartType = value.ToUpper();
                        break;
                    case "Wing1":
                        tempPart.PartType = "WING1";
                        break;
                    case "Wing2":
                        tempPart.PartType = "WING2";
                        break;
                    case "Shield Generator":
                        tempPart.PartType = "SGENERATOR";
                        break;
                    case "Reactor":
                        tempPart.PartType = value.ToUpper();
                        break;
                    case "Armory":
                        tempPart.PartType = value.ToUpper();
                        break;
                    case "Tractor Beam":
                        tempPart.PartType = "TRACKBEAM";
                        break;
                    case "Refinery":
                        tempPart.PartType = value.ToUpper();
                        break;
                }
            }
            else
                tempPart.PartType = "COCKPIT";

            return tempPart;
        }

    }
}
