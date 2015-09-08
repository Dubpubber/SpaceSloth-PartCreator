using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Rank { get; set; }
        public float Health { get; set; }
        public float RepairFactor { get; set; }
        public float Cost { get; set; }
        public bool Visible { get; set; }
        public string LocalName { get; set; }
        public string FileName { get; set; }
        public string RGB { get; set; }
        public float Alpha { get; set; }
        public string ShortHand { get; set; }
        public string PartType { get; set; }
        public Dictionary<string, string> properties;

        public Part() {
            properties = new Dictionary<string,string>();
        }

        public void addProperty(string key, string value) {
            if (!properties.ContainsKey(key)) {
                properties.Add(key, value);
            }
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

    }
}
