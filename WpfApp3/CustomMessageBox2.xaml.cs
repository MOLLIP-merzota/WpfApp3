using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfApp3
{
    public partial class CustomMessageBox2 : Window
    {
        public CustomMessageBox2(string message)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
            Loaded += CustomMessageBox_Loaded;
        }

        private void CustomMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Анимация для ScaleTransform
            DoubleAnimation scaleAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // Анимация для Opacity
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };

            // Применение анимации к ScaleX и ScaleY
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            // Применение анимации к Opacity
            this.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}