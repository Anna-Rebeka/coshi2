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
        public Canvas current_canvas;
        public bool map_is_focused = false;
        public bool is_running = false;
        public string soundPackage;

        public List<int[]> positions;

        private int lastCursorPosition = 0;
        private int startIndex = 0;


        private DispatcherTimer timer = new DispatcherTimer();
        public int index = 0;


        public MainWindow()
        {
            //priprav canvas
            Console.WriteLine();
            InitializeComponent();
            DrawGrid();
            UpdateLineNumbers();
            this.current_canvas = c1;

            //nastavenia
            Settings.MAP = this.get_map();

            //nacitaj zvukove balicky
            string[] subdirectories = Directory.GetDirectories(SoundsHandler.mainDirectory);
            if (subdirectories != null && subdirectories.Length > 0)
            {
                string firstPackageName = System.IO.Path.GetFileName(subdirectories[0]);
                Settings.set_sound_package(firstPackageName);
            }
            WritePackagesMenu();
            DrawLabels();

            //spusti kreslenie
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Draw_Robot;
        }

        public void WritePackagesMenu()
        {
            string soundsDirectory = "../../../sounds"; // Nahraďte skutočnou cestou

            if (Directory.Exists(soundsDirectory))
            {
                string[] packageDirectories = Directory.GetDirectories(soundsDirectory);

                foreach (string packageDirectory in packageDirectories)
                {  
                    string packageName = new DirectoryInfo(packageDirectory).Name;
                    MenuItem packageMenuItem = new MenuItem { Header = packageName };
                    packageMenuItem.Click += SoundPackageMenuItem_Click;
                    soundPackagesMenu.Items.Add(packageMenuItem);
                    if (soundPackagesMenu.Items.Count == 1)
                    {
                        packageMenuItem.IsChecked = true;
                    }
                }
                
            }
        }

        private void SoundPackageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                foreach(MenuItem item in soundPackagesMenu.Items)
                {
                    item.IsChecked = false;
                }
                string selectedSoundPackage = menuItem.Header as string;
                Settings.set_sound_package(selectedSoundPackage);
                menuItem.IsChecked = true;
                DrawLabels();
            }
        }

        public void DrawGrid() {
            uniformGrid.Rows = Settings.MAP_SQRT_SIZE;
            uniformGrid.Columns = Settings.MAP_SQRT_SIZE;

            for (int i = 2; i <= Settings.MAP_SQRT_SIZE * Settings.MAP_SQRT_SIZE; i++)
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


        public void DrawLabels()
        {
            for (int i = 0; i < Settings.MAP.GetLength(0); i++)
            {
                for (int j = 0; j < Settings.MAP.GetLength(1); j++)
                {
                    //odstranime stary label
                    Label foundLabel = null;

                    foreach (var child in Settings.MAP[i, j].Children)
                    {
                        if (child is Label oldlabel)
                        {
                            foundLabel = oldlabel;
                            break;
                        }
                    }
                    if (foundLabel is not null)
                    {
                        Settings.MAP[i, j].Children.Remove(foundLabel);
                    }

                    //mame novy label
                    if (i >= 0 && i < Math.Sqrt(SoundsHandler.sounds_map.Length) && j >= 0 && j < Math.Sqrt(SoundsHandler.sounds_map.Length))
                    {
                        if (SoundsHandler.sounds_map[i, j] is SoundItem) {
                            // Vytvorenie labelu
                            Label label = new Label
                            {
                                Content = SoundsHandler.sounds_map[i, j].Name, // Názov zo sounds_map
                                FontSize = 12,
                                Foreground = Brushes.Black,
                            };


                            // Nastavenie pozície labelu na Canvas
                            Canvas.SetLeft(label, 0);
                            Canvas.SetTop(label, 0);
                            Canvas.SetZIndex(label, 2); // Nastavte z-index podľa potreby

                            // Pridanie labelu do príslušného Canvas
                            Settings.MAP[i, j].Children.Add(label);
                        }
                    }  
                }
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
                Predict_Commands();
                UpdateLineNumbers();
            
        }

        private void Predict_Commands() //TODO fix
        {
            // Získajte aktuálnu pozíciu kurzoru
            int currentCursorPosition = textBox.CaretIndex - 1;
            startIndex = currentCursorPosition; //soon to be start

            // Ak pozícia kurzoru sa zmenila, získať slovo, na ktorom bola vykonaná zmena
            if (currentCursorPosition != lastCursorPosition)
            {
                string zmeneneSlovo = "";

                //NAJDI KTORE SLOVO
                while (startIndex > 0 && !char.IsWhiteSpace(textBox.Text[startIndex - 1]))
                {
                    startIndex -= 1;
                }

                if (startIndex < currentCursorPosition)
                {
                    zmeneneSlovo = textBox.Text.Substring(startIndex, currentCursorPosition - startIndex + 1);
                }
                
                if (zmeneneSlovo != null && zmeneneSlovo.Length >= 2)
                {                    
                    predictionBox.Items.Clear();
                    string[] commands = Commands.find_command(zmeneneSlovo.ToLower());
                    foreach (string command in commands)
                    {
                        predictionBox.Items.Add(command);
                    }
                }
                else
                {
                    predictionBox.Items.Clear();
                }

                // Aktualizovať pozíciu kurzoru
                lastCursorPosition = currentCursorPosition;
            }
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
            Canvas[,] canvases = new Canvas[Settings.MAP_SQRT_SIZE, Settings.MAP_SQRT_SIZE];

            int index = 0;

            for (int i = 0; i < Settings.MAP_SQRT_SIZE; i++)
            {
                for (int j = 0; j < Settings.MAP_SQRT_SIZE; j++)
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
            Settings.set_size(size);
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
            Canvas.SetLeft(this.robot, this.robot.Width - 10);
            Canvas.SetTop(this.robot, this.robot.Width - 10);
            this.current_canvas.Children.Add(this.robot);

            uniformGrid.Children.Add(border);
            DrawGrid();
            Settings.MAP = this.get_map();
            SoundsHandler.fill_sound_map();
            DrawLabels();

        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MessageBox.Show("My message here");

        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {          
            
            if (e.Key == Key.F5 && !this.is_running)
            {
                try
                {
                    Terminal.Text = "Program beží...";
                    this.is_running = true;
                    this.index = 0;
                    VirtualMachine.reset();
                    VirtualMachine.SetTextBoxReference(Terminal);
                    Robot.reset();
                    Compiler cmp = new Compiler(textBox.Text);
                    Block tree = cmp.parse();
                    //jump divoky
                    cmp.jumpOverVariables();
                    tree.generate();
                    VirtualMachine.execute_all();
                    //Robot.position = 1;
                    //MessageBox.Show(Robot.position.ToString());
                    //naplnime compilatorom POSITIONS a potom to budeme kreslit ako PREDTYM GG EZ
                    //this.interpreter.load(textBox.Text);
                    //this.positions = this.interpreter.get_positions();

                    this.index += 1;
                    this.timer.Start();
                }
                catch (Exception ex)
                {
                    Terminal.Text = "Chyba: " + ex.Message;
                }
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
                int i = name / Settings.MAP_SQRT_SIZE;
                int j = name % Settings.MAP_SQRT_SIZE;
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
                    this.current_canvas = Settings.MAP[i, j];

                }
                catch (IndexOutOfRangeException){}
                SoundsHandler.play_sound(i,j );
                this.robot = new Ellipse();
                this.robot.Width = 50;
                this.robot.Height = 50;
                this.robot.Fill = Brushes.Black;
                Canvas.SetLeft(this.robot, this.robot.Width - 10);
                Canvas.SetTop(this.robot, this.robot.Width - 10);
                this.current_canvas.Children.Add(this.robot);
            }

        }

        public void Draw_Robot(object sender, EventArgs e)
        {
            if (Robot.positions.Count == 1)
            {
                this.timer.Stop();
                Terminal.AppendText("\n" + "Program úpešne zbehol.");
                return;
            }
            int riadok = Robot.positions[this.index][0];
            int stlpec = Robot.positions[this.index][1];

            if (riadok < 0 || riadok >= Settings.MAP_SQRT_SIZE   || stlpec < 0 || stlpec >= Settings.MAP_SQRT_SIZE) {
                Terminal.Focus();
              
                this.timer.Stop();
                return;
            }
            //this.current_canvas.Children.Clear();
            this.current_canvas.Children.Remove(robot);

            this.current_canvas = Settings.MAP[riadok, stlpec];
            this.robot = new Ellipse();
            this.robot.Width = 50;
            this.robot.Height = 50;
            if (Settings.MAP_SQRT_SIZE == 5)
            {
                this.robot.Width = 35;
                this.robot.Height = 35;
            }
            else if (Settings.MAP_SQRT_SIZE == 7)
            {
                this.robot.Width = 20;
                this.robot.Height = 20;
            }
            this.robot.Fill = Brushes.Black;
            Canvas.SetLeft(this.robot, this.robot.Width - 10);
            Canvas.SetTop(this.robot, this.robot.Width - 10);
            Canvas.SetZIndex(this.robot, 1); // Nastavíme z-index elipsy na 1

            this.current_canvas.Children.Add(this.robot);
            SoundsHandler.play_sound(riadok, stlpec);

            

            this.index += 1;
            if (this.index >= Robot.positions.Count)
            {
                this.timer.Stop();
                Terminal.AppendText("\n" + "Program úpešne zbehol.");
            }
        }

        private void lineNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }



        private void ListBox_Selection(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Space)
            {
                try
                {
                    if (predictionBox.SelectedItem != null)
                    {
                        string selected = predictionBox.SelectedItem.ToString();
                        string part1 = textBox.Text[0..startIndex];
                        string part2 = textBox.Text.Substring(textBox.CaretIndex);
                        textBox.Text = part1 + selected + part2;
                    }

                }
                catch
                {

                }
            }
            
        }
    }


}
