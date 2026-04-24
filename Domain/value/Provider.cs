using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.value
{
    public class Provider
    {
        public string Name { get; set; }

        public Uri Uri {get; set;}

        public Provider(string name, Uri uri) {
            Uri = uri;
            Name = name;
        }
    }
}
