﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace coshi2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
            AddText();
        }

        private void AddText()
        {
            helptextbox.Text = "Hello sweetie";
            helptextbox.Focus();
        }

        private void Help_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            this.Close();
        }
        


    }
}
