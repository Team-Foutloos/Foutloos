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

        public Exercise()
        {
            InitializeComponent();

            Exercise_TextBlock.Text = exerciseText;
        }

        private void User_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            //Make textblock empty
            Exercise_TextBlock.Text = "";

            //create queue with chars of exercise 
            Queue<char> exerciseChars = new Queue<char>();
            foreach(char c in exerciseText)
            {
                exerciseChars.Enqueue(c);
            }

            //Get user input and save into queue
            Queue<char> userInput = new Queue<char>();
            foreach(char c in User_TextBox.Text)
            {
                userInput.Enqueue(c);
            }

            //Counter for amount of wrong chars
            int wrongCounter = 0;

            //Check for correct input user and sort chars into string for representation
            while(userInput.Count > 0)
            {
                char nextUserChar = userInput.Dequeue();
                char nextExerciseChar = exerciseChars.Dequeue();

                if(nextUserChar == nextExerciseChar)
                {
                    //If there is a mistake you're not allowed to type the next correct char
                    if(wrongCounter > 0)
                    {
                        var items = exerciseChars.ToArray();
                        exerciseChars.Clear();
                        exerciseChars.Enqueue(nextExerciseChar);
                        foreach(var item in items)
                        {
                            exerciseChars.Enqueue(item);
                        }
                        User_TextBox.Text = User_TextBox.Text.Remove(User_TextBox.Text.Length - 1);
                        User_TextBox.CaretIndex = User_TextBox.Text.Length;
                    }
                    else
                    {
                        if (userInput.Count > 0)
                        {
                            Exercise_TextBlock.Inlines.Add(new Run(nextExerciseChar.ToString()) { Foreground = Brushes.LightGray });
                        }
                        else
                        {
                            Exercise_TextBlock.Inlines.Add(new Run(nextExerciseChar.ToString()) { Foreground = Brushes.LightGreen });
                        }
                    }           
                }
                else
                {
                    wrongCounter++;

                    //After 10 mistakes the user is not allowed to type anymore
                    if(wrongCounter == 10)
                    {
                        var items = exerciseChars.ToArray();
                        exerciseChars.Clear();
                        exerciseChars.Enqueue(nextExerciseChar);
                        foreach (var item in items)
                        {
                            exerciseChars.Enqueue(item);
                        }
                        User_TextBox.Text = User_TextBox.Text.Remove(User_TextBox.Text.Length - 1);
                        User_TextBox.CaretIndex = User_TextBox.Text.Length;
                    }
                    else
                    {
                        if (nextExerciseChar == 32)
                        {
                            Exercise_TextBlock.Inlines.Add(new Run("_") { Foreground = Brushes.Red });
                        }
                        else
                        {
                            Exercise_TextBlock.Inlines.Add(new Run(nextExerciseChar.ToString()) { Foreground = Brushes.Red });
                        }
                    }
                }
            }

            //Check for left over chars and add them to text
            string charsLeft = "";
            if (exerciseChars.Count > 0)
            {
                Exercise_TextBlock.Inlines.Add(new Run(exerciseChars.Dequeue().ToString()) { Background = Brushes.LightGoldenrodYellow });
                while (exerciseChars.Count > 0)
                {
                    charsLeft += exerciseChars.Dequeue();
                }
            }
            Exercise_TextBlock.Inlines.Add(new Run(charsLeft));
        }
    }
}
