using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Foutloos
{
    public partial class Exercise : Page
    {
        private string exerciseSpecialText = "Néqué pôrrô quïsquam èst quï dôlörem ïpsum quïa dôlör sit amet, consëctétur, adïpisci velit...";
        private string exerciseText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor  massa ultricies. Neque volutpat ac tincidunt vitae semper quis. Adipiscing elit pellentesque habitant morbi tristique. Gravida rutrum quisque non tellus. Mauris commodo quis imperdiet massa tincidunt nunc pulvinar sapien et. Viverra nibh cras pulvinar mattis. Urna nunc id cursus metus aliquam eleifend mi in. Netus et malesuada fames ac turpis egestas maecenas pharetra convallis. Malesuada pellentesque elit eget gravida cum. Varius sit amet mattis vulputate enim nulla. Eu mi bibendum neque egestas congue quisque.";
        private string exerciseStringLeft;
        private string userInputCorrect = "";
        private bool exerciseFinished = false;
        private Dictionary<char, int> userMistakes = new Dictionary<char, int>();
        private bool mistake = false;
        private MainWindow owner;
        private Thickness userInput_WithoutKeyboard = new Thickness(183, 352, 183, 0);
        private Thickness userInput_WithKeyboard = new Thickness(183, 452, 183, 0);

        public Exercise(MainWindow o)
        {
            InitializeComponent();

            //Save mainwindow in a variable
            owner = o;

            //Show keyboard
            UserInput_TextBox.Margin = userInput_WithKeyboard;
            Test.Visibility = Visibility.Visible;

            //Save remaining exercise text into variable
            exerciseStringLeft = exerciseText;
            //Visualize exercise text
            Exercise_TextBlock.Text = exerciseText;
        }

        //Constructor for special characters excersice
        public Exercise(bool special)
        {
            InitializeComponent();
            if (special)
            {
                //Save remaining exercise text into variable
                exerciseStringLeft = exerciseSpecialText;
                //Visualize exercise text
                Exercise_TextBlock.Text = exerciseSpecialText;
            } else {
                //Save remaining exercise text into variable
                exerciseStringLeft = exerciseText;
                //Visualize exercise text
                Exercise_TextBlock.Text = exerciseText;
            }
        }
        private void UserInput_TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Functionality Toggle
            //Check if toggle is true
            if (Toggle.Toggled)
            {
                //Check which key is pressed
                if (e.Key == Key.D1)
                {
                    Key1_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D2)
                {
                    Key2_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D3)
                {
                    Key3_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D4)
                {
                    Key4_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D5)
                {
                    Key5_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D6)
                {
                    Key6_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D7)
                {
                    Key7_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D8)
                {
                    Key8_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D9)
                {
                    Key9_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D0)
                {
                    Key0_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.OemMinus)
                {
                    KeyDash_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Q)
                {
                    Keyq_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.W)
                {
                    Keyw_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.E)
                {
                    Keye_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.R)
                {
                    Keyr_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.T)
                {
                    Keyt_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Y)
                {
                    Keyy_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.U)
                {
                    Keyu_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.I)
                {
                    Keyi_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.O)
                {
                    Keyo_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.P)
                {
                    Keyp_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.A)
                {
                    Keya_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.S)
                {
                    Keys_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D)
                {
                    Keyd_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.F)
                {
                    Keyf_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.G)
                {
                    Keyg_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.H)
                {
                    Keyh_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.J)
                {
                    Keyj_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.K)
                {
                    Keyk_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.L)
                {
                    Keyl_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Oem1)
                {
                    KeyColon_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Oem7)
                {
                    KeyAccolade_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.LeftShift)
                {
                    KeyShift_Back.Fill = Brushes.Black;
                    Key1_Text.Text = "!";
                    Key2_Text.Text = "@";
                    Key3_Text.Text = "#";
                    Key4_Text.Text = "$";
                    Key5_Text.Text = "%";
                    Key6_Text.Text = "^";
                    Key7_Text.Text = "&";
                    Key8_Text.Text = "*";
                    Key9_Text.Text = "(";
                    Key0_Text.Text = ")";
                    KeyDash_Text.Text = "_";
                    KeyColon_Text.Text = ":";
                    KeyAccolade_Text.Text = "\"";
                    KeyComma_Text.Text = "<";
                    KeyDot_Text.Text = ">";
                    KeySlash_Text.Text = "?";
                }
                else if (e.Key == Key.Z)
                {
                    Keyz_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.X)
                {
                    Keyx_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.C)
                {
                    Keyc_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.V)
                {
                    Keyv_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.B)
                {
                    Keyb_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.N)
                {
                    Keyn_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.M)
                {
                    Keym_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.OemComma)
                {
                    KeyComma_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.OemPeriod)
                {
                    KeyDot_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Oem2)
                {
                    KeySlash_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.LeftCtrl)
                {
                    KeyControl_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.LeftAlt)
                {
                    KeyAlt_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Space)
                {
                    KeySpace_Back.Fill = Brushes.Black;
                }
            }

            //Set user's cursor to the end of line
            UserInput_TextBox.CaretIndex = UserInput_TextBox.Text.Length;

            //Disable the use of backspace
            if (e.Key == Key.Back)
            {
                e.Handled = true;
            }

            //Check if the exercise is finished
            if (!exerciseFinished)
            {
                //Check if the user pressed the spacebar
                if (e.Key == Key.Space)
                {
                    //Check if the next character of the exercise is spacebar
                    if (exerciseStringLeft.First() == 32)
                    {
                        //Used for saving user's mistakes
                        mistake = false;

                        //Visualize correct input
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Foreground = Brushes.LightGreen });

                        //Update variables
                        userInputCorrect += exerciseStringLeft.First().ToString();
                        exerciseStringLeft = exerciseStringLeft.Remove(0, 1);

                        //Add remaining exercise text if there is any
                        if (exerciseStringLeft.Length > 0)
                        {
                            Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Background = Brushes.Yellow });
                            Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.Remove(0, 1)));
                        }

                        //Check if the exercise is finished
                        if (exerciseStringLeft.Length == 0)
                        {
                            //Change exercise text when exercise is finished
                            exerciseFinished = true;
                            Exercise_TextBlock.Text = "";
                            Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGreen });
                        }
                    }
                    else
                    {
                        //Disable incorrect input to be shown in user's inputbox
                        e.Handled = true;

                        //Check if the next character of the exercise was a mistake, by the user, before
                        if(!mistake)
                        {
                            //Update dictionary containing user's mistakes
                            try
                            {
                                userMistakes.Add((char)32, 1);
                            }
                            catch(Exception)
                            {
                                userMistakes[(char)32] += 1;
                            }
                            mistake = true;
                        }                     

                        //Visualize incorrect input
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Foreground = Brushes.Red, Background = Brushes.Yellow });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.Remove(0, 1)));
                    }
                }
            }
            else
            {
                //Show exercise in light green when it's finished
                Exercise_TextBlock.Text = "";
                Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGreen });
                e.Handled = true;
            }
        }

        private void UserInput_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Check if exercise is finished
            if (!exerciseFinished)
            {
                //Check if the user's input is correct
                if (e.Text == exerciseStringLeft.First().ToString())
                {
                    //Used for saving user's mistakes
                    mistake = false;

                    //Visualize correct input
                    Exercise_TextBlock.Text = "";
                    Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                    Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Foreground = Brushes.LightGreen });

                    //Update variables
                    userInputCorrect += exerciseStringLeft.First().ToString();
                    exerciseStringLeft = exerciseStringLeft.Remove(0, 1);

                    //Add remaining exercise text if there is any
                    if (exerciseStringLeft.Length > 0)
                    {
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Background = Brushes.Yellow });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.Remove(0, 1)));
                    }

                    //Check if the exercise is finished
                    if (exerciseStringLeft.Length == 0)
                    {
                        //Change exercise text when exercise is finished
                        exerciseFinished = true;
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGreen });
                    }
                }
                else
                {
                    //Disable incorrect input to be shown in user's inputbox
                    e.Handled = true;

                    //Check if the next character of the exercise was a mistake, by the user, before
                    if (!mistake)
                    {
                        //Update dictionary containing user's mistakes
                        try
                        {
                            userMistakes.Add(exerciseStringLeft.First(), 1);
                        }
                        catch(Exception)
                        {
                            userMistakes[exerciseStringLeft.First()] += 1;
                        }
                        mistake = true;
                    }

                    //Check if the next character in the exercise is a space
                    if (exerciseStringLeft.First() == 32)
                    {
                        //Visualize incorrect input (spacebar)
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Background = Brushes.Red });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.Remove(0, 1)));
                    }
                    else
                    {
                        //Visualize incorrect input (non-spacebar)
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Foreground = Brushes.Red, Background = Brushes.Yellow });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.Remove(0, 1)));
                    }
                }
            }
        }

        private void UserInput_TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Disable copy/cut/paste commands in user's textbox
            if(e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        //Home button functionality
        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            owner.Content = new HomeScreen(owner);
        }

        //Toggle button functionality
        private void Toggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(Toggle.Toggled)
            {
                //Show keyboard
                UserInput_TextBox.Margin = userInput_WithKeyboard;
                Test.Visibility = Visibility.Visible;
            }
            else
            {
                //Hide keyboard
                UserInput_TextBox.Margin = userInput_WithoutKeyboard;
                Test.Visibility = Visibility.Hidden;
            }
        }

        private void UserInput_TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            //Functionality Toggle
            //Check if toggle is true
            if (Toggle.Toggled)
            {
                //Check which key is pressed
                if (e.Key == Key.D1)
                {
                    Key1_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D2)
                {
                    Key2_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D3)
                {
                    Key3_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D4)
                {
                    Key4_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D5)
                {
                    Key5_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D6)
                {
                    Key6_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D7)
                {
                    Key7_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D8)
                {
                    Key8_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D9)
                {
                    Key9_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D0)
                {
                    Key0_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.OemMinus)
                {
                    KeyDash_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Q)
                {
                    Keyq_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.W)
                {
                    Keyw_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.E)
                {
                    Keye_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.R)
                {
                    Keyr_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.T)
                {
                    Keyt_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Y)
                {
                    Keyy_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.U)
                {
                    Keyu_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.I)
                {
                    Keyi_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.O)
                {
                    Keyo_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.P)
                {
                    Keyp_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.A)
                {
                    Keya_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.S)
                {
                    Keys_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D)
                {
                    Keyd_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.F)
                {
                    Keyf_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.G)
                {
                    Keyg_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.H)
                {
                    Keyh_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.J)
                {
                    Keyj_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.K)
                {
                    Keyk_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.L)
                {
                    Keyl_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Oem1)
                {
                    KeyColon_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Oem7)
                {
                    KeyAccolade_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.LeftShift)
                {
                    KeyShift_Back.Fill = Brushes.LightGray;
                    Key1_Text.Text = "1";
                    Key2_Text.Text = "2";
                    Key3_Text.Text = "3";
                    Key4_Text.Text = "4";
                    Key5_Text.Text = "5";
                    Key6_Text.Text = "6";
                    Key7_Text.Text = "7";
                    Key8_Text.Text = "8";
                    Key9_Text.Text = "9";
                    Key0_Text.Text = "0";
                    KeyDash_Text.Text = "-";
                    KeyColon_Text.Text = ";";
                    KeyAccolade_Text.Text = "\'";
                    KeyComma_Text.Text = ",";
                    KeyDot_Text.Text = ".";
                    KeySlash_Text.Text = "/";
                }
                else if (e.Key == Key.Z)
                {
                    Keyz_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.X)
                {
                    Keyx_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.C)
                {
                    Keyc_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.V)
                {
                    Keyv_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.B)
                {
                    Keyb_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.N)
                {
                    Keyn_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.M)
                {
                    Keym_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.OemComma)
                {
                    KeyComma_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.OemPeriod)
                {
                    KeyDot_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Oem2)
                {
                    KeySlash_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.LeftCtrl)
                {
                    KeyControl_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.LeftAlt)
                {
                    KeyAlt_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Space)
                {
                    KeySpace_Back.Fill = Brushes.LightGray;
                }
            }
        }
    }
}      