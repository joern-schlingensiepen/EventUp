using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EventUpLib
{
    public partial class Person
    {
        public override string ToString()
        {
            return $"{Name} {FamilyName} ({Id})";
        }
        public string AsString => ToString();
    }
}
