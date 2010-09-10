using System;

namespace WcfDoc.Initialization
{
	internal class Options
	{
        // ────────────────────────── Public Methods ──────────────────────────

        public static void Display(params Type[] optionGroups)
        {
            var writer = new OptionWriter(optionGroups, Console.WindowWidth, true);
            Console.Write(writer.ToString());
        }

        public static void Load(string args, object options)
        {
            OptionLoader.Load(options, args);
        }
	}
}
