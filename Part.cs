using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Brickstore
{
    internal class Part
    {
        string id, name, category, color;
        int qty;

        public Part(string id, string name, string category, string color, int qty)
        {
            this.id = id;
            this.name = name;
            this.category = category;
            this.color = color;
            this.qty = qty;
        }

        public string Id => id;
        public string Name => name;
        public string Category => category;
        public string Color => color;
        public int Qty => qty;
    }
}
