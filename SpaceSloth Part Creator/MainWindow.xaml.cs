using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SpaceSloth_Part_Creator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private string jAr;

        private Part part;

        private ObservableCollection<Property> properties;

        private string jsonString = "Click make part button to preview json.";

        private Random r;

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
            part = new Part();
            r = new Random();
        }

        /**
         * Generate the part from all the components!
         */
        private void Button_Click(object sender, RoutedEventArgs e) {
            Part part = loadPart();
            if (jAr == null) {
                MessageBox.Show("No precreated Json file has been loaded!");
                return;
            }
            else {
                var jsonData = File.ReadAllText(jAr);
                var partList = JsonConvert.DeserializeObject<List<Part>>(jsonData) ?? new List<Part>();
                partList.Add(part);

                using (FileStream fs = File.Open(jAr, FileMode.OpenOrCreate))
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter jw = new JsonTextWriter(sw)) {
                    jw.Formatting = Formatting.Indented;

                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jw, partList);
                    jsonPreview.Text = JsonConvert.SerializeObject(partList, Formatting.Indented);
                }
                part.ResetPart();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".json";
            dlg.Filter = "Json Files (.json)|*.json";

            Nullable<bool> result = dlg.ShowDialog();

            if(result == true) {
                jAr = dlg.FileName;
                if (jAr != null)
                    MessageBox.Show("Successfully Loaded " + dlg.FileName + "!");
                else
                    MessageBox.Show("Couldn't load file!");
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
                loadPartProperties(item.Content.ToString());
                Console.WriteLine("Completed. Count: " + properties.Count());
            }
        }

        private void loadPartProperties(String str) {
            switch (str) {
                case "Cockpit":
                    properties.Add(new Property { key = "HudValue", value = "0" });
                    fileNameValue.IsEnabled = true;
                    break;
                case "GunMount":
                    properties.Add(new Property { key = "FireRate", value = "1.0" });
                    fileNameValue.IsEnabled = true;
                    break;
                case "Hull":
                    properties.Add(new Property { key = "Capacity", value = "100.0" });
                    fileNameValue.IsEnabled = true;
                    break;
                case "Thrusters":
                    properties.Add(new Property { key = "BoostCap", value = "100.0" });
                    fileNameValue.IsEnabled = true;
                    break;
                case "Wing1":
                    properties.Add(new Property { key = "Torque", value = "1.80" });
                    fileNameValue.IsEnabled = true;
                    break;
                case "Wing2":
                    properties.Add(new Property { key = "Torque", value = "1.80" });
                    fileNameValue.IsEnabled = true;
                    break;
                case "Shield Generator":
                    properties.Add(new Property { key = "Power", value = "25.0" });
                    fileNameValue.IsEnabled = true;
                    break;
                case "Reactor":
                    properties.Add(new Property { key = "MaxSpeed", value = "0.080" });
                    fileNameValue.IsEnabled = false;
                    break;
                case "Armory":
                    properties.Add(new Property { key = "Capacity", value = "1" });
                    fileNameValue.IsEnabled = false;
                    break;
                case "Tractor Beam":
                    properties.Add(new Property { key = "Range", value = "5.0" });
                    fileNameValue.IsEnabled = false;
                    break;
                case "Refinery":
                    properties.Add(new Property { key = "RefineSpeed", value = "1.0" });
                    fileNameValue.IsEnabled = false;
                    break;
            }
            props.ItemsSource = null;
            props.ItemsSource = properties;
        }

        public Part loadPart() {

            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;

            // set the rank
            if (rankValue.SelectedItem != null) {
                string rankType = ((ListBoxItem)rankValue.SelectedValue).Content.ToString();
                part.Rank = rankType.Substring(4).Replace(" ", "");
            }
            else
                part.Rank = "A";

            // set the health
            if (healthValue.Text.Length > 0) {
                part.Health = float.Parse(healthValue.Text, currentCulture);
            }
            else
                part.Health = 1f;

            // set repair factor
            if (repairFactorValue.Text.Length > 0) {
                part.RepairFactor = float.Parse(repairFactorValue.Text, currentCulture);
            }
            else
                part.RepairFactor = 1.0f;

            // set cost
            if (costValue.Text.Length > 0) {
                part.Cost = float.Parse(costValue.Text, currentCulture);
            }
            else
                part.Cost = 100.0f;

            // set local name
            if (localNameValue.Text.Length > 0) {
                part.LocalName = localNameValue.Text;
            }
            else
                part.LocalName = " ";

            // set file name
            part.FileName = ((ComboBoxItem) fileNameValue.SelectedItem).Content.ToString();

            Color selectedColor = new Color();
            // set rgb
            if (rgbValue.SelectedColor != null) {
                selectedColor = (Color)rgbValue.SelectedColor;
                part.RGB = selectedColor.R.ToString("X2") + selectedColor.G.ToString("X2") + selectedColor.B.ToString("X2");
            }
            else
                part.RGB = "000000";

            // set alpha
            if (rgbValue.SelectedColor != null) {
                part.Alpha = float.Parse(selectedColor.A.ToString(), currentCulture) / 100;
            }
            else
                part.Alpha = 1.0f;

            // set shorthand
            if (shortHandValue.Text.Length > 0) {
                part.ShortHand = shortHandValue.Text;
            }
            else
                part.ShortHand = " ";

            // finally, set part type and visibility
            if (partTypeValue.SelectedItem != null) {
                string value = ((ListBoxItem)partTypeValue.SelectedValue).Content.ToString();
                switch (value) {
                    case "Cockpit":
                        part.PartType = value.ToUpper();
                        part.Visible = true;
                        break;
                    case "GunMount":
                        part.PartType = value.ToUpper();
                        part.Visible = true;
                        break;
                    case "Hull":
                        part.PartType = value.ToUpper();
                        part.Visible = true;
                        break;
                    case "Thrusters":
                        part.PartType = value.ToUpper();
                        part.Visible = true;
                        break;
                    case "Wing1":
                        part.PartType = "WING1";
                        part.Visible = true;
                        break;
                    case "Wing2":
                        part.PartType = "WING2";
                        part.Visible = true;
                        break;
                    case "Shield Generator":
                        part.PartType = "SGENERATOR";
                        part.Visible = true;
                        break;
                    case "Reactor":
                        part.PartType = value.ToUpper();
                        part.Visible = false;
                        break;
                    case "Armory":
                        part.PartType = value.ToUpper();
                        part.Visible = false;
                        break;
                    case "Tractor Beam":
                        part.PartType = "TRACKBEAM";
                        part.Visible = false;
                        break;
                    case "Refinery":
                        part.PartType = value.ToUpper();
                        part.Visible = false;
                        break;
                }
            }
            else
                part.PartType = "COCKPIT";

            // Load properties //
            loadPartProperties(partTypeValue.SelectedItem.ToString());
            part.addProps(properties);

            return part;
        }

        private void fillWithSelection_Click(object sender, RoutedEventArgs e) {
            fillFieldsOfPartType("");
        }

        private void getRandPart_Click(object sender, RoutedEventArgs e) {
            if((bool)rankToggle.IsChecked) {
                int ranRank = r.Next(0, int.Parse(rankValue.Items.Count.ToString()));
                rankValue.SelectedIndex = ranRank;
            }

            if((bool)partTypeToggle.IsChecked) {
                int ranPartType = r.Next(0, int.Parse(partTypeValue.Items.Count.ToString()));
                partTypeValue.SelectedIndex = ranPartType;

                if (ranPartType > 6) {
                    fileNameValue.IsEnabled = false;
                    fileNameValue.SelectedIndex = -1;
                }
                else
                    fileNameValue.IsEnabled = true;
            }

            if (partTypeValue.SelectedItem != null) { 
                string partType = ((ListBoxItem)partTypeValue.SelectedValue).Content.ToString();
                properties.Clear();
                loadPartProperties(partType);
                fillFieldsOfPartType("");
            }

            if((bool) colorToggle.IsChecked) {
                Color randColor = Color.FromArgb(255, (byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255));
                rgbValue.SelectedColor = randColor;
            }

            if ((bool)healthToggle.IsChecked) {
                healthValue.Text = NextDouble(1.0, 1000.0) + "";
            }

            if ((bool)repairFactorToggle.IsChecked) {
                repairFactorValue.Text = NextDouble(0.01, 10.0) + "";
            }

            if ((bool)costToggle.IsChecked) {
                costValue.Text = NextDouble(100.0, 100000.0) + "";
            }
        }

        private void fillFieldsOfPartType(string randomAdjective) {
            if (partTypeValue.SelectedItem != null && rankValue.SelectedItem != null) {
                string rank = ((ListBoxItem)rankValue.SelectedValue).Content.ToString();
                string partType = ((ListBoxItem)partTypeValue.SelectedValue).Content.ToString();
                if (fileNameValue.IsEnabled) {
                    switch (partType) {
                        case "Cockpit":
                            fileNameValue.SelectedIndex = 0;
                            break;
                        case "GunMount":
                            fileNameValue.SelectedIndex = 1;
                            break;
                        case "Hull":
                            fileNameValue.SelectedIndex = 2;
                            break;
                        case "Thrusters":
                            fileNameValue.SelectedIndex = 4;
                            break;
                        case "Wing1":
                            fileNameValue.SelectedIndex = 5;
                            break;
                        case "Wing2":
                            fileNameValue.SelectedIndex = 6;
                            break;
                        case "Shield Generator":
                            fileNameValue.SelectedIndex = 3;
                            break;
                    }
                }
                localNameValue.Text = rank + " ";
                shortHandValue.Text = rank.ToLower().Replace(" ", "") + partType.ToLower().Replace(" ", "") + randomAdjective + " ";
            }
            else {
                MessageBox.Show("Please select a rank and part type first!");
            }
        }

        private double NextDouble(double min, double max) {
            return Math.Round(min + (r.NextDouble() * (max - min)), 2, MidpointRounding.AwayFromZero);
        }

    }
}
