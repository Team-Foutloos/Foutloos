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
        private char exerciseNextChar;
        private string userInput = "";
        private string userInputCorrect = "";
        private string exerciseStringLeft;

        public Exercise()
        {
            InitializeComponent();

            //Set exercise text
            Exercise_TextBlock.Text = exerciseText;

            //Fill queue with characters to be typed by the user
            foreach(char c in exerciseText)
            {
                exerciseCharsLeft.Enqueue(c);
            }

            //Determine next character to be typed by the user
            exerciseNextChar = exerciseCharsLeft.Dequeue();
            //Determine string value of characters left to be typed besides previous correct input and the following character
            exerciseStringLeft = exerciseText.Remove(0, 1);
        }

        private void UserInput_TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Disable the backspace key
            if (e.Key == Key.Back)
            {
                e.Handled = true;
            }
        }

        private void UserInput_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Set users curser to the end of the inputbox
            UserInput_TextBox.CaretIndex = UserInput_TextBox.Text.Length;

            //Make sure when the user is holding down a key, it's only typed once
            if(e.IsRepeat)
            {
                UserInput_TextBox.Text = userInput;
                UserInput_TextBox.CaretIndex = UserInput_TextBox.Text.Length;
            }
            else
            {
                userInput = UserInput_TextBox.Text;
            }
        }

        private void UserInput_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            //Event only does something when certain keys are pressed
            if(e.Key != Key.LeftShift && UserInput_TextBox.Text.Length > 0)
            {
                //Make sure the user can't delete his/her input
                if(userInput.Length <= UserInput_TextBox.Text.Length)
                {
                    userInput = UserInput_TextBox.Text;
                    Exercise_TextBlock.Text = userInput;
                }
                else
                {
                    UserInput_TextBox.Text = userInput;
                    UserInput_TextBox.CaretIndex = UserInput_TextBox.Text.Length;
                }

                //Determine if the exercise is finished
                if(exerciseStringLeft.Length > 0)
                {
                    //Determine if the user's input is correct
                    if (Convert.ToChar(userInput.Last()) == exerciseNextChar)
                    {
                        //Update exercise visualisation
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        if(e.Key == Key.Space)
                        {
                            Exercise_TextBlock.Inlines.Add(new Run("_") { Foreground = Brushes.LightGreen });
                        }
                        else
                        {
                            Exercise_TextBlock.Inlines.Add(new Run(exerciseNextChar.ToString()) { Foreground = Brushes.LightGreen });
                        }
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft));

                        //Update variables
                        userInputCorrect += exerciseNextChar.ToString();
                        exerciseNextChar = exerciseCharsLeft.Dequeue();
                        exerciseStringLeft = exerciseStringLeft.Remove(0, 1);
                    }
                    else
                    {
                        //Remove user's mistake from the inputbox
                        UserInput_TextBox.Text = UserInput_TextBox.Text.Remove(UserInput_TextBox.Text.Length - 1);
                        UserInput_TextBox.CaretIndex = UserInput_TextBox.Text.Length;

                        //Update exercise visualisation
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        if (e.Key == Key.Space)
                        {
                            Exercise_TextBlock.Inlines.Add(new Run("_") { Foreground = Brushes.Red });
                        }
                        else
                        {
                            Exercise_TextBlock.Inlines.Add(new Run(exerciseNextChar.ToString()) { Foreground = Brushes.Red });
                        }
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft));
                    }
                }
                else
                {
                    //If the exercise is finished the user can't type anymore
                    UserInput_TextBox.Text = userInput.Remove(userInput.Length - 1);
                }
            }
        }
    }
}
