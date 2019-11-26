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

            owner = o;

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

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            owner.Content = new HomeScreen(owner);
        }

        private void Toggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(Toggle.Toggled)
            {
                UserInput_TextBox.Margin = userInput_WithKeyboard;
                Test.Visibility = Visibility.Visible;
            }
            else
            {
                UserInput_TextBox.Margin = userInput_WithoutKeyboard;
                Test.Visibility = Visibility.Hidden;
            }
        }
    }
}      