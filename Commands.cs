﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace coshi2
{
    public static class Commands
    {
        public static List<string> commands = new List<string>
        {
            "vľavo", "vlavo", 
            "vpravo", 
            "hore", 
            "dole", 
            "kým", "kym", 
            "opakuj",
            "krát",
            "zobraz",
            "koniec",
            "menší",
            "vacsi", "väčší",
            "rovný",
            "zvuk"
        };

        public static List<string> labelnames = new List<string>();

        public static List<string> find_command(string prefix)
        {
            List<string> foundcomm = new List<string>(commands.Where(comm => comm.StartsWith(prefix)).ToArray());
            List<string> foundnames = new List<string>(labelnames.Where(comm => comm.StartsWith(prefix)).ToArray());

            HashSet<string> found = new HashSet<string>(foundcomm.Concat(foundnames));
            
            return found.ToList();
        }
    }
}
