using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Foutloos
{
    class BorderButton
    {
        Border borderButton = new Border();

        public BorderButton(int dif)
        {
            TextBlock l1 = new TextBlock();
            Grid borderButtonGrid = new Grid() { Margin = new Thickness(10) };
            TextBlock level = new TextBlock();

            //Get the completed logo
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            
            logo.UriSource = new Uri(@"/assets/tick.png", UriKind.RelativeOrAbsolute);
            logo.EndInit();

            Image completedIcon = new Image();
            completedIcon.Source = logo;
            completedIcon.Width = 40;
            completedIcon.Opacity = .4;

            var availableColors = new List<Color>();

            //Set all the button colors
            var allColor = Color.FromRgb(0, 102, 255);
            var easyColor = Color.FromRgb(51, 204, 51);
            var mediumColor = Color.FromRgb(255, 153, 0);
            var hardColor = Color.FromRgb(204, 0, 0);
            
            //Put all of the colors in a list so they can be easily picked out later.
            availableColors.Add(easyColor);
            availableColors.Add(mediumColor);
            availableColors.Add(hardColor);
            availableColors.Add(allColor);


            //Set all the properties for the label.
            borderButton.Child = borderButtonGrid;

            //Set the properties for the children of the grid of the borderbutotn.
            borderButtonGrid.Children.Add(l1);
            borderButtonGrid.Children.Add(level);
            borderButtonGrid.Children.Add(completedIcon);


            borderButton.CornerRadius = new CornerRadius(10);
            borderButton.BorderBrush = new SolidColorBrush(availableColors[dif]) { Opacity = 1 };
            borderButton.BorderThickness = new Thickness(5);
            borderButton.Background = new SolidColorBrush(availableColors[dif]) { Opacity = .5 };
            level.FontSize = 15;
            level.HorizontalAlignment = HorizontalAlignment.Left;
            level.VerticalAlignment = VerticalAlignment.Bottom;
            l1.HorizontalAlignment = HorizontalAlignment.Center;
            l1.VerticalAlignment = VerticalAlignment.Center;
            completedIcon.HorizontalAlignment = HorizontalAlignment.Right;
            completedIcon.VerticalAlignment = VerticalAlignment.Bottom;



            //Set the levelText
            switch (dif)
            {
                case 0:
                    level.Text = "Amateur";
                    break;
                case 1:
                    level.Text = "Normal";
                    break;
                case 2:
                    level.Text = "Expert";
                    break;
                case 3:
                    level.Text = "";
                    break;
            }
        }

        //Homescreen
        public BorderButton()
        {
            TextBlock l1 = new TextBlock();
            Grid borderButtonGrid = new Grid() { Margin = new Thickness(10) };

            //Get the completed logo
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(@"/assets/lockIcon.png", UriKind.RelativeOrAbsolute);
            logo.EndInit();

            Image completedIcon = new Image();
            completedIcon.Source = logo;
            completedIcon.Width = 40;
            completedIcon.Opacity = .4;


            //Set all the button colors
            var color = Color.FromRgb(51, 204, 51);
            


            //Set all the properties for the label.
            borderButton.Child = borderButtonGrid;

            //Set the properties for the children of the grid of the borderbutotn.
            borderButtonGrid.Children.Add(l1);
            borderButtonGrid.Children.Add(completedIcon);


            borderButton.CornerRadius = new CornerRadius(10);
            borderButton.BorderBrush = new SolidColorBrush(color) { Opacity = 1 };
            borderButton.BorderThickness = new Thickness(5);
            borderButton.Background = new SolidColorBrush(color) { Opacity = .7 };
           
            l1.HorizontalAlignment = HorizontalAlignment.Center;
            l1.VerticalAlignment = VerticalAlignment.Center;
            completedIcon.HorizontalAlignment = HorizontalAlignment.Right;
            completedIcon.VerticalAlignment = VerticalAlignment.Bottom;

        }

        public Border getButton()
        {
            return borderButton;
        }



    }
}
