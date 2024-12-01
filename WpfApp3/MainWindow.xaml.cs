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
        private string selectedFilePath;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        //анимации
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

        //кнопка выбора файла
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

        //кнопка сохранения изменений
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                try
                {
                    var textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
                    using (var fileStream = new FileStream(selectedFilePath, FileMode.Create))
                    {
                        textRange.Save(fileStream, DataFormats.XamlPackage);
                    }
                    CustomMessageBox customMessageBox = new CustomMessageBox("Сохранения изменены.");
                    customMessageBox.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}");
                }
            }
            else
            { 
                CustomMessageBox customMessageBox = new CustomMessageBox("Ты че...Долбаеб?");
                customMessageBox.ShowDialog();
            }
            
        }

        //кнопка печати
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                FlowDocument flowDocument = new FlowDocument(new Paragraph(new Run(new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd).Text)));
                flowDocument.ColumnWidth = double.MaxValue;

                IDocumentPaginatorSource idpSource = flowDocument;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Печать документа");
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



        
        private void Font_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty,
                    ((ComboBoxItem)Font.SelectedItem).Content);
            }
        }

        
        private void FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty,
                    Convert.ToDouble(((ComboBoxItem)FontSize.SelectedItem).Content));
            }
        }

        
        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, RichTextBox);
        }

        
        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, RichTextBox);
        }

        
        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleUnderline.Execute(null, RichTextBox);
        }

        
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
            }
        }

        
        private void CenterButton_Click(object sender, RoutedEventArgs e)
        {
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            }
        }

        
        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
            }
        }
    }
}