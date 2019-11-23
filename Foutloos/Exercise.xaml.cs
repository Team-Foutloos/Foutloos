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
        private string exerciseText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor  massa ultricies. Neque volutpat ac tincidunt vitae semper quis. Adipiscing elit pellentesque habitant morbi tristique. Gravida rutrum quisque non tellus. Mauris commodo quis imperdiet massa tincidunt nunc pulvinar sapien et. Viverra nibh cras pulvinar mattis. Urna nunc id cursus metus aliquam eleifend mi in. Netus et malesuada fames ac turpis egestas maecenas pharetra convallis. Malesuada pellentesque elit eget gravida cum. Varius sit amet mattis vulputate enim nulla. Eu mi bibendum neque egestas congue quisque.";
        private Queue<char> exerciseCharsLeft = new Queue<char>();
        private string exerciseStringLeft;
        private char exerciseNextChar;
        private string exerciseNextString;
        private string userInputCorrect = "";

        public Exercise()
        {
            InitializeComponent();

            //Set exercise text
            Exercise_TextBlock.Text = exerciseText;

            //Fill queue with characters to be typed by the user
            foreach (char c in exerciseText)
            {
                exerciseCharsLeft.Enqueue(c);
            }

            //Determine next character to be typed by the user
            exerciseNextChar = exerciseCharsLeft.Dequeue();
            //Determine string value of characters left to be typed besides previous correct input and the following character
            exerciseStringLeft = exerciseText.Remove(0, 1);
            //Determine next string value to be typed by the user
            exerciseNextString = exerciseStringLeft.First().ToString();
            exerciseStringLeft = exerciseStringLeft.Remove(0, 1);
        }

        private void UserInput_TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Set user's cursor to the end of line
            UserInput_TextBox.CaretIndex = UserInput_TextBox.Text.Length;
            
            //If user presses backspace a space is placed in the string to be deleted by the backspace instead of previous input
            if(e.Key == Key.Back)
            {
                UserInput_TextBox.Text += " ";
                UserInput_TextBox.CaretIndex = UserInput_TextBox.Text.Length;
            }

            //Check if the exercise is finishes
            if(exerciseCharsLeft.Count > 0)
            {
                //Check if the user pressed space
                if (e.Key == Key.Space)
                {
                    //Check if the next character in the exercise is a space
                    if (exerciseNextChar == 32)
                    {
                        //Visualize correct input
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        Exercise_TextBlock.Inlines.Add(new Run(" "));
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseNextString) { Background = Brushes.AliceBlue });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft));

                        //Update variables
                        userInputCorrect += exerciseNextChar.ToString();
                        exerciseNextChar = exerciseCharsLeft.Dequeue();
                        if (exerciseCharsLeft.Count > 0)
                        {
                            exerciseNextString = exerciseStringLeft.First().ToString();
                            exerciseStringLeft = exerciseStringLeft.Remove(0, 1);
                        }
                    }
                    else
                    {
                        //Disable incorrect input to be shown in user's inputbox
                        e.Handled = true;

                        //User's input is incorrect
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseNextChar.ToString()) { Background = Brushes.Red });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseNextString + exerciseStringLeft));
                    }
                }
            }
            else
            {
                //Exercise is finished
                Exercise_TextBlock.Text = "";
                Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGreen });
                UserInput_TextBox.Text = exerciseText;
                UserInput_TextBox.CaretIndex = exerciseText.Length;
                e.Handled = true;
            }
        }

        private void UserInput_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ////Check if the exercise is finished
            if (exerciseCharsLeft.Count > 0)
            {
                //Check if the user's input is a space
                if(exerciseNextChar != 32)
                {
                    //Check if the user's input is correct
                    if (e.Text == exerciseNextChar.ToString())
                    {
                        //User's input is correct
                        //Visualize correct input
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseNextChar.ToString()) { Foreground = Brushes.LightGreen });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseNextString) { Background = Brushes.AliceBlue });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft));

                        //Update variables
                        userInputCorrect += exerciseNextChar.ToString();
                        exerciseNextChar = exerciseCharsLeft.Dequeue();
                        if (exerciseStringLeft.Length > 0)
                        {
                            exerciseNextString = exerciseStringLeft.First().ToString();
                            exerciseStringLeft = exerciseStringLeft.Remove(0, 1);
                        }
                    }
                    else
                    {
                        //Disable incorrect input to be shown in user's inputbox
                        e.Handled = true;

                        //User's input is incorrect
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseNextChar.ToString()) { Foreground = Brushes.Red, Background = Brushes.AliceBlue });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseNextString + exerciseStringLeft));
                    }
                }
            }
            else
            {
                //Exercise is finished
                Exercise_TextBlock.Text = "";
                Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGreen });
                UserInput_TextBox.Text = exerciseText;
                UserInput_TextBox.CaretIndex = exerciseText.Length;
                e.Handled = true;
            }
        }
    }
}      