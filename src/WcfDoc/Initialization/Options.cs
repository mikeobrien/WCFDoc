using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfDoc.Initialization
{
	internal class Options
	{
        // ────────────────────────── Public Methods ──────────────────────────

        public static void Display(params Type[] optionGroups)
        {
            OptionWriter writer = new WcfDoc.Initialization.OptionWriter(optionGroups, Console.WindowWidth, true);
            Console.Write(writer.ToString());
        }

        public static void Load(string args, object options)
        {
            OptionLoader.Load(options, args);
        }
	}
}
