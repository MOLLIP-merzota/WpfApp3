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
            InitializeComponent();
            SetRichTextBoxContent(fileContent);
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

                // Устанавливаем содержимое файла в RichTextBox
                SetRichTextBoxContent(_fileContent);

                RichTextBox.Visibility = Visibility.Visible;
                DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5));
                RichTextBox.BeginAnimation(OpacityProperty, fadeInAnimation);
            }
        }
        
        
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            string content = GetRichTextBoxContent();
            File.WriteAllText(_filePath, content);
            CustomMessageBox customMessageBox = new CustomMessageBox("Сохранения изменены.");
            customMessageBox.ShowDialog();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument flowDocument = new FlowDocument(new Paragraph(new Run(GetRichTextBoxContent())));
                IDocumentPaginatorSource idpSource = flowDocument;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Printing Document");
            }
            
        }
        private void FontStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RichTextBox.Selection != null)
            {
                string selectedStyle = (FontStyleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (selectedStyle != null)
                {
                    TextSelection selectedText = RichTextBox.Selection;
                    switch (selectedStyle)
                    {
                        case "Normal":
                            selectedText.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                            selectedText.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                            break;
                        case "Italic":
                            selectedText.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                            break;
                        case "Bold":
                            selectedText.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                            break;
                    }
                }
            }
        }

        // Вспомогательные методы для установки и получения содержимого RichTextBox
        private void SetRichTextBoxContent(string text)
        {
            RichTextBox.Document.Blocks.Clear();
            RichTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        private string GetRichTextBoxContent()
        {
            TextRange textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
            return textRange.Text.TrimEnd('\r', '\n'); // Удаляем лишние переводы строк
        }
    }
        
}