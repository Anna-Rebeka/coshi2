using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace coshi2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private string currentFilePath;
        private const string NewLineCharacter = "\r\n";
        public Canvas current_canvas;
        public bool map_is_focused = false;
        public bool terminal_is_focused = false;
        public bool is_running = false;
        public string soundPackage;
        private int caretInd;
        public Ellipse robot;
        double sirkaC;
        double vyskaC;


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
            //WindowStyle = WindowStyle.None; // Skryjte okraj okna
            WindowState = WindowState.Maximized; // Maximalizujte

            DrawGrid();
            UpdateLineNumbers();
            //nastavenia
            Settings.MAP = this.get_map();


            //nacitaj zvukove balicky
            string[] subdirectories = Directory.GetDirectories(SoundsHandler.mainDirectory);
            if (subdirectories != null && subdirectories.Length > 0)
            {
                string firstPackageName = System.IO.Path.GetFileName(subdirectories[0]);
                Settings.set_sound_package(firstPackageName);
            }
            changeSize(Settings.PACKAGE_SIZE);
            WritePackagesMenu();
            DrawLabels();
            Draw_User(0, 0);

            textBox.Focus();
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
                foreach (MenuItem item in soundPackagesMenu.Items)
                {
                    item.IsChecked = false;
                }
                string selectedSoundPackage = menuItem.Header as string;
                Settings.set_sound_package(selectedSoundPackage);
                changeSize(Settings.PACKAGE_SIZE);
                menuItem.IsChecked = true;
                DrawLabels();
            }
        }

        public void DrawGrid() {
            uniformGrid.Rows = Settings.MAP_SQRT_SIZE;
            uniformGrid.Columns = Settings.MAP_SQRT_SIZE;

            for (int i = 1; i <= Settings.MAP_SQRT_SIZE * Settings.MAP_SQRT_SIZE; i++)
            {


                Border border = new Border();
                border.BorderBrush = Settings.FG;
                border.BorderThickness = new Thickness(0.5);

                Canvas canvas = new Canvas();
                canvas.Background = Settings.BG;
                canvas.Name = "c" + i;
                canvas.Focusable = true;

                /*
                canvas.SizeChanged += (sender, e) =>
                {
                    sirkaC = canvas.ActualWidth;
                    vyskaC = canvas.ActualHeight;
                    Canvas.SetTop(canvas, sirkaC / 2);
                    Canvas.SetLeft(canvas, vyskaC / 2);

                }; */

                //canvas.Height = 50;

                if (i == 1)
                {
                    this.current_canvas = canvas;
                }

                border.Child = canvas;

                uniformGrid.Children.Add(border);
            }
        }


        public void DrawLabels()
        {
            Commands.labelnames.Clear();

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
                                Foreground = Settings.FG
                            };

                            if (!Commands.labelnames.Contains(SoundsHandler.sounds_map[i, j].Name)) {
                                Commands.labelnames.Add(SoundsHandler.sounds_map[i, j].Name);
                            }

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
                    List<string> commands = Commands.find_command(zmeneneSlovo.ToLower());
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
            newFile();
        }

        private void newFile() {
            Settings.CURRENTFILEPATH = null;
            textBox.Text = string.Empty;
            Title = "Coshi2";
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            openFile();
        }

        private void openFile()
        {
            string code = FilesHandler.open();
            Title = "Coshi2 - " + Settings.CURRENTFILEPATH;
            changeSize(Settings.MAP_SQRT_SIZE);
            DrawLabels();
            textBox.Text = code;
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveToFile();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Textové súbory (*.txt)|*.txt|Všetky súbory (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                Settings.CURRENTFILEPATH = saveFileDialog.FileName;
                SaveToFile();
            }
        }

        private void SaveToFile()
        {
            FilesHandler.save(textBox.Text);
            Title = "Coshi2 - " + Settings.CURRENTFILEPATH;
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

        public void changeSize(int size) {
            Settings.set_size(size);
            uniformGrid.Children.Clear();
            DrawGrid();
            Settings.MAP = this.get_map();
            SoundsHandler.fill_sound_map();
            DrawLabels();
            Draw_User(0, 0);
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MessageBox.Show("My message here");

        }


        private void CloseMyApp(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("Neuložené zmeny budú stratené. Chcete ich uložiť a zatvoriť aplikáciu?", "Upozornenie", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                SaveToFile(); // Uložiť zmeny
                Application.Current.Shutdown(); // Zatvoriť aplikáciu
                e.Handled = true; // Zastaviť ďalšie spracovanie klávesnice
            }
            else if (result == MessageBoxResult.No)
            {
                Application.Current.Shutdown(); // Zatvoriť aplikáciu
                e.Handled = true; // Zastaviť ďalšie spracovanie klávesnice
            }
            else
            {
                e.Handled = true; // Zastaviť ďalšie spracovanie klávesnice
            }
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Tab) {
                if (predictionBox.Items.Count > 0)
                {
                    caretInd = textBox.CaretIndex;
                    predictionBox.Focus();
                }
                else
                {
                    e.Handled = true;
                }
            }
        }


      

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                if (terminal_is_focused)
                {
                    textBox.Focus();
                    terminal_is_focused = false;
                }
                else
                {
                    Terminal.Focus();
                    terminal_is_focused = true;
                }
            }

            if (e.Key == Key.F2)
            {
                subor_volba.Focus();
            }

            if (e.Key == Key.F3)
            {
                FindNextKeyword();
            }


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
                    cmp.jumpOverVariables();
                    tree.generate();
                    VirtualMachine.execute_all();


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
                    textBox.IsReadOnly = false;
                    this.map_is_focused = false;
                    //FocusMe.Visibility = Visibility.Hidden;
                    //this.textBox.CaretIndex = this.CarotIndex;
                    //textBox.Focus();
                }
                else
                {
                    //F6 preloz do Key_Preview na Textbox a potom focus na FocusMe kde bude Key_Preview na sipky a pohyb
                    textBox.IsReadOnly = true;
                    this.map_is_focused = true;
                    //FocusMe.Visibility = Visibility.Visible;
                    //this.CarotIndex = textBox.CaretIndex;
                    //FocusMe.Focus();
                }

            }

            if (e.Key == Key.F7)
            {
                SwitchColorTheme(sender, null);
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.S)
            {
                // Zavolajte vašu funkciu 
                SaveToFile();

                // Zastavte ďalšie spracovanie klávesnice
                e.Handled = true;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.S)
            {
                // Zavolajte vašu funkciu 
                SaveAs_Click(sender, null);

                // Zastavte ďalšie spracovanie klávesnice
                e.Handled = true;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.O)
            {
                // Zavolajte vašu funkciu 
                openFile();

                // Zastavte ďalšie spracovanie klávesnice
                e.Handled = true;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.N)
            {
                // Zavolajte vašu funkciu 
                newFile();

                // Zastavte ďalšie spracovanie klávesnice
                e.Handled = true;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.OemPlus)
            {
                Increase_Font(sender, null);

                // Zastavte ďalšie spracovanie klávesnice
                e.Handled = true;
                    
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.OemMinus)
            {
                // Zavolajte vašu funkciu 
                Decrease_Font(sender, null);

                // Zastavte ďalšie spracovanie klávesnice
                e.Handled = true;

            }

            if (Keyboard.IsKeyDown(Key.LeftAlt) && e.Key == Key.F4)
            {
                //
                 
            }


            if (this.map_is_focused)
            {
                int name = int.Parse(this.current_canvas.Name.Replace("c", "")) - 1;
                int i = name / Settings.MAP_SQRT_SIZE;
                int j = name % Settings.MAP_SQRT_SIZE;

                int i0 = i;
                int j0 = j;

                if (e.Key == Key.A && j != 0)
                {
                    j -= 1;
                }
                else if (e.Key == Key.W && i != 0)
                {
                    i -= 1;
                }
                else if (e.Key == Key.S && i + 1 < Settings.MAP_SQRT_SIZE)
                {
                    i += 1;
                }
                else if (e.Key == Key.D && j + 1 < Settings.MAP_SQRT_SIZE)
                {
                    j += 1;
                }

                if (i != i0 || j != j0)
                {
                    Draw_User(i, j);
                    SoundsHandler.play_sound(i, j);
                }

            }
            
        }

        private void FindNextKeyword()
        {
            var headerPattern = string.Join("|", Commands.get_block_starts());
            var regex = new System.Text.RegularExpressions.Regex(headerPattern);
            var matches = regex.Matches(textBox.Text);
            int caret = textBox.CaretIndex;

            // prejdi vsetky nalezi kluc. slov a najdi najblizsi dalsi
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Index > caret)
                {
                    textBox.CaretIndex = match.Index;
                    return;
                }
            }

            // ak ani jeden nie je za aktualny caretom, nastav prvy vyskyt - cyklicke haldanie
            if (matches.Count > 0)
            {
                textBox.CaretIndex = matches[0].Index;
                return;
            }

            //inak zostan kde si
        }


        public void Draw_User(int riadok, int stlpec) {
            this.current_canvas.Children.Remove(robot);

            this.current_canvas = Settings.MAP[riadok, stlpec];
            this.robot = new Ellipse();
            this.robot.Width = 80;
            this.robot.Height = 80;

            if (Settings.MAP_SQRT_SIZE == 3)
            {
                Canvas.SetLeft(this.robot, this.robot.Width - 30);
                Canvas.SetTop(this.robot, this.robot.Width - 30);
            }
                

            if (Settings.MAP_SQRT_SIZE == 5)
            {
                Canvas.SetLeft(this.robot, this.robot.Width - 40);
                Canvas.SetTop(this.robot, this.robot.Width - 40);
                this.robot.Width = 50;
                this.robot.Height = 50;
            }
            else if (Settings.MAP_SQRT_SIZE == 7)
            {
                Canvas.SetLeft(this.robot, this.robot.Width - 50);
                Canvas.SetTop(this.robot, this.robot.Width - 50);
                this.robot.Width = 50;
                this.robot.Height = 50;
            }
            this.robot.Fill = Settings.FG;
            //Canvas.SetLeft(this.robot, sirkaC / 2);
            //Canvas.SetTop(this.robot, vyskaC / 2); 
            Canvas.SetZIndex(this.robot, 1); // Nastavíme z-index elipsy na 1

            this.current_canvas.Children.Add(this.robot);
        }
        
        public void Draw_Robot(object sender, EventArgs e)
        {
            if (Robot.positions.Count == 1 || this.index >= Robot.positions.Count)
            {
                this.timer.Stop();
                Terminal.Text = "Program úspešne zbehol.";
                return;
            }
            int riadok = Robot.positions[this.index][0];
            int stlpec = Robot.positions[this.index][1];

            if (riadok == 100 && stlpec == 100)
            {
                Settings.SILENCE = false;
                this.index += 1;
            }
            else if (riadok == -100 && stlpec == -100)
            {
                Settings.SILENCE = true;
                this.index += 1;
            }
            else
            {
                if (riadok < 0 || riadok >= Settings.MAP_SQRT_SIZE || stlpec < 0 || stlpec >= Settings.MAP_SQRT_SIZE)
                {
                    Terminal.Focus();
                    this.timer.Stop();
                    return;
                }
                //this.current_canvas.Children.Clear();
                this.current_canvas.Children.Remove(robot);

                this.current_canvas = Settings.MAP[riadok, stlpec];
                this.robot = new Ellipse();

                this.robot.Width = 80;
                this.robot.Height = 80;

                if (Settings.MAP_SQRT_SIZE == 3)
                {
                    Canvas.SetLeft(this.robot, this.robot.Width - 30);
                    Canvas.SetTop(this.robot, this.robot.Width - 30);
                }


                if (Settings.MAP_SQRT_SIZE == 5)
                {
                    Canvas.SetLeft(this.robot, this.robot.Width - 40);
                    Canvas.SetTop(this.robot, this.robot.Width - 40);
                    this.robot.Width = 50;
                    this.robot.Height = 50;
                }
                else if (Settings.MAP_SQRT_SIZE == 7)
                {
                    Canvas.SetLeft(this.robot, this.robot.Width - 50);
                    Canvas.SetTop(this.robot, this.robot.Width - 50);
                    this.robot.Width = 50;
                    this.robot.Height = 50;
                }
                this.robot.Fill = Settings.FG;

                Canvas.SetZIndex(this.robot, 1); // Nastavíme z-index elipsy na 1

                this.current_canvas.Children.Add(this.robot);
                SoundsHandler.play_sound(riadok, stlpec);



                this.index += 1;
                if (this.index >= Robot.positions.Count)
                {
                    this.timer.Stop();
                    Terminal.Text = "Program úspešne zbehol.";
                }
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
                        caretInd += selected.Length - 2;
                    }

                }
                catch
                {

                }
                textBox.Focus();
                textBox.CaretIndex = caretInd;
            }
            if (e.Key == Key.Escape) {
                textBox.Focus();
                textBox.CaretIndex = caretInd;
            }
            
        }
        private void predictionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //textBox.Focus();
        }

        private void predictionBox_GotFocus(object sender, RoutedEventArgs e)
        {
    
        }

        private void SwitchColorTheme(object sender, RoutedEventArgs e)
        {
            // ak je dark daj light inak daj dark
            Settings.THEME = (Theme.Dark == Settings.THEME) ? Theme.Light : Theme.Dark;
            Settings.FG = (Theme.Dark == Settings.THEME) ? Brushes.White : Brushes.Black;
            Settings.BG = (Theme.Dark == Settings.THEME) ? Brushes.Black : Brushes.White;

            if(Settings.THEME == Theme.Dark)
            {
                ThemeState.Header = "_Svetlý režim";
            }
            else
            {
                ThemeState.Header = "_Tmavý režim";
            }

            textBox.Background = Settings.BG;
            textBox.Foreground = Settings.FG;

            Terminal.Background = Settings.BG;
            Terminal.Foreground = Settings.FG;
            
            lineNumberTextBox.Background = Settings.BG;
            lineNumberTextBox.Foreground = Settings.FG;

            predictionBox.Background = Settings.BG;
            predictionBox.Foreground = Settings.FG;

            uniformGrid.Children.Clear();
            DrawGrid();
            Settings.MAP = this.get_map();
            //SoundsHandler.fill_sound_map();
            DrawLabels();

            int x = (Robot.position - 1) / Settings.MAP_SQRT_SIZE;
            int y = (Robot.position - 1) % Settings.MAP_SQRT_SIZE;
            Draw_User(x, y);
        }

        private void Increase_Font(object sender, RoutedEventArgs e)
        {
            if (textBox.FontSize >= 32)
            {
                return;
            }
            lineNumberTextBox.FontSize += 2.0;
            textBox.FontSize += 2.0;
        }

        private void Decrease_Font(object sender, RoutedEventArgs e)
        {
            if (textBox.FontSize <= 10)
            {
                return;
            }
            lineNumberTextBox.FontSize -= 2.0;
            textBox.FontSize -= 2.0;
        }

        private void Show_Help(object sender, RoutedEventArgs e)
        {
            Window pomocneOkno = new Window();
            pomocneOkno.Title = "Pomoc - Klávesové skratky";
            pomocneOkno.Width = 500;
            pomocneOkno.Height = 400;
            pomocneOkno.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Text s klávesovými skratkami
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Klávesové skratky:\n\n" +
                "CTRL + N - Nový\n" +
                "CTRL + S - Uložiť\n" +
                "CTRL + O - Otvoriť\n" +
                "Alt + F4 - Koniec\n\n\n" +
                "Fn Funkcie:\n\n"+
                "F1 - Prepínač medzi terminálom a kódom\n" +
                "F2 - Menu\n" +
                "F3 - Skok po blokoch kódu\n" +
                "F4 - Editor\n" +
                "F5 - Spusti program\n" +
                "F6 - Pohyb robotom\n" +
                "F7 - Zmena témy\n";
            textBlock.TextAlignment = TextAlignment.Center;

            pomocneOkno.Content = textBlock;

            pomocneOkno.ShowDialog();
        }
    }
}

