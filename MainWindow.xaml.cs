﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace coshi2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentFilePath;
        private const string NewLineCharacter = "\r\n";
        public Interpreter interpreter;
        public Canvas current_canvas;
        //public Handler handler;
        //public SoundsHandler soundsHandler;
        public int map_size = 3;
        public bool map_is_focused = false;
        public bool is_running = false;
        SoundsHandler soundsHandler;

        public Canvas[,] map;
        public List<int[]> positions;


        private DispatcherTimer timer = new DispatcherTimer();
        public int index = 0;


        public MainWindow()
        {
            Console.WriteLine();
            InitializeComponent();
            DrawGrid();
            UpdateLineNumbers();

            this.current_canvas = c1;
            this.interpreter = new Interpreter(Terminal, map_size);

            this.map = this.get_map();
            this.soundsHandler = new SoundsHandler(this.map);

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Draw_Robot;
        }

        public void DrawGrid() {
            uniformGrid.Rows = map_size;
            uniformGrid.Columns = map_size;

            for (int i = 2; i <= this.map_size * this.map_size; i++)
            {
                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(0.5);

                Canvas canvas = new Canvas();
                canvas.Name = "c" + i;
                canvas.Focusable = true;

                border.Child = canvas;

                uniformGrid.Children.Add(border);
            }
        }

        private void UpdateLineNumbers()
        {
            string[] lines = textBox.Text.Split(new string[] { NewLineCharacter }, System.StringSplitOptions.None);
            int lineCount = lines.Length;

            lineNumberTextBox.Text = string.Join(NewLineCharacter, Enumerable.Range(1, lineCount).Select(i => i.ToString().PadLeft(3) + "  "));
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateLineNumbers();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            currentFilePath = null;
            textBox.Text = string.Empty;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                currentFilePath = openFileDialog.FileName;
                textBox.Text = File.ReadAllText(currentFilePath);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                var saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                {
                    currentFilePath = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }
            File.WriteAllText(currentFilePath, textBox.Text);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public Canvas[,] get_map()
        {
            Canvas[,] canvases = new Canvas[this.map_size, this.map_size];

            int index = 0;

            for (int i = 0; i < this.map_size; i++)
            {
                for (int j = 0; j < this.map_size; j++)
                {
                    Border border = (Border)uniformGrid.Children[index];
                    canvases[i, j] = (Canvas)border.Child;
                    index++;
                }
            }
            return canvases;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void size3_Click(object sender, RoutedEventArgs e)
        {
            changeSize(3);
        }

        private void size5_Click(object sender, RoutedEventArgs e)
        {
            changeSize(5);
        }

        private void size7_Click(object sender, RoutedEventArgs e)
        {
            changeSize(7);
        }

        private void changeSize(int size) {
            this.map_size = size;
            this.interpreter.setMap_size(this.map_size);
            uniformGrid.Children.Clear();

            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1);
            Canvas canvas = new Canvas();
            canvas.Name = "c1";
            canvas.Focusable = true;
            border.Child = canvas;
            this.current_canvas = canvas;
            
            this.robot = new Ellipse();
            this.robot.Width = 50;
            this.robot.Height = 50;
            if (size == 5) {
                this.robot.Width = 35;
                this.robot.Height = 35;
            }
            else if (size == 7)
            {
                this.robot.Width = 20;
                this.robot.Height = 20;
            }
            this.robot.Fill = Brushes.Black;
            Canvas.SetLeft(this.robot, 10);
            Canvas.SetTop(this.robot, 10);
            this.current_canvas.Children.Add(this.robot);

            uniformGrid.Children.Add(border);
            DrawGrid();
            this.map = this.get_map();

        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MessageBox.Show("My message here");

        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {          
            
            if (e.Key == Key.F5 && !this.is_running)
            {
                Terminal.Text = "Program beží...";
                this.is_running = true;
                this.index = 0;
                this.interpreter.load(textBox.Text);
                this.positions = this.interpreter.get_positions();
                this.index += 1;
                this.timer.Start();
                this.is_running = false;

            }
            if (e.Key == Key.F6)
            {
                if (this.map_is_focused)
                {
                    this.textBox.Focus();

                    this.map_is_focused = false;
                }
                else
                {
                    robot.Focus();
                    this.map_is_focused = true;
                }

            }
            if (this.map_is_focused)
            {
                int name = int.Parse(this.current_canvas.Name.Replace("c", "")) - 1;
                int i = name / this.map_size;
                int j = name % this.map_size;
                this.current_canvas.Children.Clear();


                try
                {
                    if (e.Key == Key.Left)
                    {
                       
                        j -= 1;
                    }
                    else if (e.Key == Key.Up)
                    {
                        i -= 1;
                    }
                    else if (e.Key == Key.Down)
                    {
                        i += 1;
                    }
                    else if (e.Key == Key.Right)
                    {
                        j += 1;
                    }
                    this.current_canvas = this.map[i, j];

                }
                catch (IndexOutOfRangeException){}
                this.soundsHandler.play_sound(i,j );
                this.robot = new Ellipse();
                this.robot.Width = 50;
                this.robot.Height = 50;
                this.robot.Fill = Brushes.Black;
                Canvas.SetLeft(this.robot, 10);
                Canvas.SetTop(this.robot, 10);
                this.current_canvas.Children.Add(this.robot);
            }

        }

        public void Draw_Robot(object sender, EventArgs e)
        {
            int riadok = this.positions[this.index][0];
            int stlpec = this.positions[this.index][1];

            if (riadok < 0 || riadok >= this.map_size || stlpec < 0 || stlpec >= this.map_size) {
                Terminal.Focus();
              
                this.timer.Stop();
                return;
            }
            this.current_canvas.Children.Clear();
            this.current_canvas = this.map[riadok, stlpec];
            this.robot = new Ellipse();
            this.robot.Width = 50;
            this.robot.Height = 50;
            if (map_size == 5)
            {
                this.robot.Width = 35;
                this.robot.Height = 35;
            }
            else if (map_size == 7)
            {
                this.robot.Width = 20;
                this.robot.Height = 20;
            }
            this.robot.Fill = Brushes.Black;
            Canvas.SetLeft(this.robot, 10);
            Canvas.SetTop(this.robot, 10);
            this.current_canvas.Children.Add(this.robot);
            soundsHandler.play_sound(riadok, stlpec);

            

            this.index += 1;
            if (this.index >= this.positions.Count)
            {
                this.timer.Stop();
                Terminal.Text = "Program úpešne zbehol.";
            }
        }
    }


}
