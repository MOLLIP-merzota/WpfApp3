using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System;
using System.IO;

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
            RotateTransform rotateTransform = new RotateTransform();
            this.RenderTransform = rotateTransform;
            this.RenderTransformOrigin = new Point(0.5, 0.5);

            DoubleAnimation rotateAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(2)),
                RepeatBehavior = new RepeatBehavior(1)
            };

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(2))
            };

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            this.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _filePath = openFileDialog.FileName;
                _fileContent = File.ReadAllText(_filePath);

                SetRichTextBoxContent(_fileContent);
                ApplyDefaultFormatting();

                RichTextBox.Visibility = Visibility.Visible;
                DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5));
                RichTextBox.BeginAnimation(OpacityProperty, fadeInAnimation);
            }
        }

        private void ApplyDefaultFormatting()
        {
            TextRange textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
            textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily("Arial"));
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, 12.0);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                CustomMessageBox2 customMessageBox = new CustomMessageBox2("Ты че...Долбаеб?.");
                customMessageBox.ShowDialog();
                return;
            }

            try
            {
                string content = GetRichTextBoxContent();
                File.WriteAllText(_filePath, content);
                CustomMessageBox customMessageBox = new CustomMessageBox("Сохранения изменены.");
                customMessageBox.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
            else
            {
                MessageBox.Show("Печать отменена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SetRichTextBoxContent(string text)
        {
            RichTextBox.Document.Blocks.Clear();
            RichTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        private string GetRichTextBoxContent()
        {
            TextRange textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
            return textRange.Text.TrimEnd('\r', '\n');
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