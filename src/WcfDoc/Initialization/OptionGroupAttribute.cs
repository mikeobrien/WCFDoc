using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfDoc.Initialization
{
    internal class OptionGroupAttribute : Attribute 
    {
        // ────────────────────────── Constructors ──────────────────────────

        public OptionGroupAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        // ────────────────────────── Public Properties ──────────────────────────

        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}
