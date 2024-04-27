using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace coshi2
{
    public static class Commands
    {


        public static HashSet<string> commands = new HashSet<string>(StringComparer.Create(new CultureInfo("en-US"), true))
    {
        "vľavo", 
        "vpravo",
        "hore",
        "dole",
        "kým",
        "opakuj",
        "krát",
        "urob",
        "koniec",
        "do",
        "daj",
        "zobraz",
        "prehraj",
        "ak",
        "je",
        "menší",
        "väčší", 
        "rovný", 
        "ako",
        "zvuk",
        "voľné", 
        "od",
        "pričítaj", 
        "odčítaj", 
        "ticho",
        "nahlas",
    };


        public static List<string> labelnames = new List<string>();
        public static List<string> find_command(string prefix)
        {
            // objekt pre prorovnanie bez diakritiky z package Globalization
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;

            HashSet<string> foundCommands = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (string command in commands)
            {
                if (compareInfo.IsPrefix(command, prefix, CompareOptions.IgnoreNonSpace))
                {
                    foundCommands.Add(command); 
                }
            }

            foreach (string label in labelnames)
            {
                if (compareInfo.IsPrefix(label, prefix, CompareOptions.IgnoreNonSpace))
                {
                    foundCommands.Add(label);
                }
            }

            return foundCommands.ToList();
        }

        public static string[] get_block_starts()
        {
            return new string[]
            {
                "kým",
                "kym",
                "ak",
                "opakuj",
                "urob"
            };
        }
    }
}
