using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Input;


namespace Foutloos.Modals
{
    /// <summary>
    /// Interaction logic for YesCancelModal.xaml
    /// </summary>
    public partial class YesCancelModal : Window
    {
        SpeechSynthesizer _synthesizer;

        public YesCancelModal()
        {
            InitializeComponent();
        }

        public YesCancelModal(SpeechSynthesizer synthesizer)
        {
            InitializeComponent();
            this._synthesizer = synthesizer;
        }

        private void ThemedButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();

            Application.Current.MainWindow.Content = new Foutloos.ExercisesPage();
            if (_synthesizer != null)
            {
                _synthesizer.Pause();
            }
        }

        private void ThemedButton_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
