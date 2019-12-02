using System;
using System.Windows;
using System.Windows.Threading;

namespace Foutloos.Modals
{
    /// <summary>
    /// Interaction logic for Countdown.xaml
    /// </summary>
    public partial class Countdown : Window
    {
        //Timer for displaying the countdown.
        private DispatcherTimer timer = new DispatcherTimer();


        //The initial time
        private int countdownTimer = 3;

        public Countdown()
        {
            InitializeComponent();

            //Configuring the timer and adding an event
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        //Timer functionality
        private void Timer_Tick(object sender, EventArgs e)
        {
            countdownTimer--;
            timerUI.Content = countdownTimer;

            if (countdownTimer == 0)
            {
                this.Close();
            }
        }
    }

}
