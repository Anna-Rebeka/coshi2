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
        public static string[] commands =
        {
            "vľavo", "vlavo", 
            "vpravo", 
            "hore", 
            "dole", 
            "kým", "kym", 
            "opakuj",
            "krát",
            "zobraz",
            "koniec"
        };

        public static string[] find_command(string prefix)
        {
            return commands.Where(comm => comm.StartsWith(prefix)).ToArray();
        }
    }
}
