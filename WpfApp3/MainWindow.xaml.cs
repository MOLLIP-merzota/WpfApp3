using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using Path = System.IO.Path;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        private string _filePath;
        private string _fileContent;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Создаем трансформацию вращения
            RotateTransform rotateTransform = new RotateTransform();
            this.RenderTransform = rotateTransform;
            this.RenderTransformOrigin = new Point(0.5, 0.5);

            // Создаем анимацию вращения
            DoubleAnimation rotateAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(2)),
                RepeatBehavior = new RepeatBehavior(1)
            };

            // Создаем анимацию для свойства Opacity
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(2))
            };

            // Запускаем анимации
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            this.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }
        
        
        
        private MainWindow(string fileContent, string filePath)
        {
            InitializeComponent(); // Установка содержимого в TextBox
            TextBox.Text = fileContent; // Сохранение пути к файлу
            _filePath = filePath;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _filePath = openFileDialog.FileName;
                _fileContent = File.ReadAllText(_filePath);

                // Устанавливаем содержимое файла в TextBox
                TextBox.Text = _fileContent;
                
                TextBox.Visibility = Visibility.Visible;
                DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5));
                TextBox.BeginAnimation(OpacityProperty, fadeInAnimation);
            }
        }
        
        
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            // Сохранение изменений в файл
            File.WriteAllText(_filePath, TextBox.Text);//файл-класс, метод, из текстбокса в файл
            CustomMessageBox customMessageBox = new CustomMessageBox("Сохранения изменены.");
            customMessageBox.ShowDialog();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument flowDocument = new FlowDocument(new Paragraph(new Run(TextBox.Text)));
                IDocumentPaginatorSource idpSource = flowDocument;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Printing Document");
            }
            
        }
        
    }
}