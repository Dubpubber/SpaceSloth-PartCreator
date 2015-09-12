using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpaceSloth_Part_Creator {
    /**
     * Rank
     * Health
     * Repair
     * Cost
     * Visible
     * LocalName
     * FileName
     * RGB
     * Alpha
     * Shorthand
     * PartType
     * Properties
     **/
    public class Part {

        [JsonProperty(Order=1)]
        public string Rank { get; set; }

        [JsonProperty(Order = 2)]
        public float Health { get; set; }

        [JsonProperty(Order = 3)]
        public float RepairFactor { get; set; }

        [JsonProperty(Order = 4)]
        public float Cost { get; set; }

        [JsonProperty(Order = 5)]
        public bool Visible { get; set; }

        [JsonProperty(Order = 6)]
        public string LocalName { get; set; }

        [JsonProperty(Order = 7)]
        public string FileName { get; set; }

        [JsonProperty(Order = 8)]
        public string RGB { get; set; }

        [JsonProperty(Order = 9)]
        public float Alpha { get; set; }

        [JsonProperty(Order = 10)]
        public string ShortHand { get; set; }

        [JsonProperty(Order = 11)]
        public string PartType { get; set; }

        [JsonProperty(Order = 12)]
        public List<Property> Properties;

        public Part() {
            Properties = new List<Property>();
            Init();
        }

        public void cleanEntries() {
            foreach(var prop in this.GetType().GetProperties()) {
                if(prop.GetValue(this, null) is string) {
                    prop.SetValue(this, prop.GetValue(this, null).ToString().Replace(" ", ""));
                }
            }
        }

        public void printPart() {
            foreach(var prop in this.GetType().GetProperties()) {
                Console.WriteLine(prop.Name + ": " + prop.GetValue(this, null));
            }
        }

        public void addProps(ObservableCollection<Property> prop) {
            foreach(Property property in prop) {
                Properties.Add(property);
            }
        }

        public string serializeToJson() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public void ResetPart() {
            Init();
        }

        private void Init() {
            Rank = "A";
            Health = 100.0f;
            RepairFactor = 1.0f;
            Cost = 100.0f;
            Visible = true;
            LocalName = "partrankdefault";
            FileName = "partrankdefault.png";
            RGB = "000000";
            Alpha = 1.0f;
            ShortHand = "prtdef";
            PartType = "COCKPIT";
            Properties.Clear();
        }

    }
}
