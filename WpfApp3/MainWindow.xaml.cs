using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Win32;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        // Поле для хранения пути к выбранному файлу
        private string selectedFilePath;

        // Конструктор главного окна
        public MainWindow()
        {
            InitializeComponent();
            // Подписка на событие загрузки окна
            Loaded += Window_Loaded;
        }

        // Обработчик события загрузки окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Применение анимаций к окну
            ApplyWindowAnimations();
        }

        // Метод для применения анимаций к окну
        private void ApplyWindowAnimations()
        {
            var rotateTransform = new RotateTransform();
            RenderTransform = rotateTransform;
            RenderTransformOrigin = new Point(0.5, 0.5);

            // Анимация вращения
            var rotateAnimation = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(2))
            {
                RepeatBehavior = new RepeatBehavior(1)
            };

            // Анимация появления
            var fadeInAnimation = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(2));

            // Запуск анимаций
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        // Обработчик нажатия на кнопку для открытия файла
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Clown file| *.clw"
            };

            // Проверка, был ли выбран файл
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                LoadFileContent();
            }
            else
            {
                ShowCustomMessageBox("Файл не выбран.");
            }
        }

        // Метод для загрузки содержимого файла
        private void LoadFileContent()
        {
            try
            {
                TextRange textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
                using (FileStream fileStream = new FileStream(selectedFilePath, FileMode.Open))
                {
                    string extension = Path.GetExtension(selectedFilePath).ToLower();
                    textRange.Load(fileStream, extension == ".rtf" ? DataFormats.Rtf : DataFormats.Text);
                }
                AnimateRichTextBoxVisibility();
            }
            catch (Exception ex)
            {
                ShowCustomMessageBox($"Ошибка при открытии файла: {ex.Message}");
            }
        }

        // Метод для анимации видимости RichTextBox
        private void AnimateRichTextBoxVisibility()
        {
            RichTextBox.Visibility = Visibility.Visible;
            var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5));
            RichTextBox.BeginAnimation(OpacityProperty, fadeInAnimation);
        }

        // Обработчик нажатия на кнопку для сохранения файла
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Clown file| *.clw"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    selectedFilePath = saveFileDialog.FileName;
                }
                else
                {
                    ShowCustomMessageBox("Файл не выбран для сохранения.");
                    return;
                }
            }

            SaveFileContent();
        }

        // Метод для сохранения содержимого файла
        private void SaveFileContent()
        {
            try
            {
                TextRange textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
                using (FileStream fileStream = new FileStream(selectedFilePath, FileMode.Create))
                {
                    string extension = Path.GetExtension(selectedFilePath).ToLower();
                    if (extension == ".rtf")
                    {
                        textRange.Save(fileStream, DataFormats.Rtf);
                    }
                    else
                    {
                        using (StreamWriter writer = new StreamWriter(fileStream, Encoding.UTF8))
                        {
                            writer.Write(textRange.Text);
                        }
                    }
                }

                ShowCustomMessageBox("Файл успешно сохранен.");
            }
            catch (Exception ex)
            {
                ShowCustomMessageBox($"Ошибка при сохранении файла: {ex.Message}");
            }
        }

        // Обработчик нажатия на кнопку для печати документа
        private void PrintDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                PrintDocument(printDialog);
            }
            else
            {
                ShowCustomMessageBox("Печать отменена.");
            }
        }

        // Метод для печати документа
        private void PrintDocument(PrintDialog printDialog)
        {
            FlowDocument flowDocument = new FlowDocument();
            TextRange sourceRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
            TextRange targetRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);

            using (MemoryStream ms = new MemoryStream())
            {
                sourceRange.Save(ms, DataFormats.Rtf);
                ms.Seek(0, SeekOrigin.Begin);
                targetRange.Load(ms, DataFormats.Rtf);
            }

            flowDocument.PageWidth = printDialog.PrintableAreaWidth;
            flowDocument.PageHeight = printDialog.PrintableAreaHeight;
            flowDocument.PagePadding = new Thickness(40);
            flowDocument.ColumnWidth = double.MaxValue;

            IDocumentPaginatorSource idpSource = flowDocument;
            printDialog.PrintDocument(idpSource.DocumentPaginator, "Печать документа");
        }

        // Метод для отображения пользовательского сообщения
        private void ShowCustomMessageBox(string message)
        {
            CustomMessageBox customMessageBox = new CustomMessageBox(message);
            customMessageBox.ShowDialog();
        }

        // Обработчик нажатия на кнопку для поиска текста
        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            string findText = FindTextBox.Text;
            if (!string.IsNullOrEmpty(findText))
            {
                Find(findText);
            }
        }

        // Метод для поиска текста в документе
        private void Find(string searchText)
        {
            TextRange textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
            string documentText = textRange.Text;

            int index = documentText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                TextPointer start = GetTextPositionAtOffset(RichTextBox.Document.ContentStart, index);
                TextPointer end = GetTextPositionAtOffset(start, searchText.Length);

                RichTextBox.Selection.Select(start, end);
                RichTextBox.Focus();
            }
            else
            {
                ShowCustomMessageBox("Текст не найден.");
            }
        }

        // Метод для получения позиции текста с заданным смещением
        private TextPointer GetTextPositionAtOffset(TextPointer start, int offset)
        {
            TextPointer current = start;
            int currentOffset = 0;

            while (current != null && currentOffset < offset)
            {
                if (current.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    int count = current.GetTextRunLength(LogicalDirection.Forward);
                    if (currentOffset + count >= offset)
                    {
                        return current.GetPositionAtOffset(offset - currentOffset);
                    }
                    currentOffset += count;
                }
                current = current.GetNextContextPosition(LogicalDirection.Forward);
            }
            return current;
        }

        // Обработчик получения фокуса текстового поля поиска
        private void FindTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FindTextBox.Text == "Введите текст для поиска")
            {
                FindTextBox.Text = "";
                FindTextBox.Foreground = Brushes.Black;
            }
        }

        // Обработчик потери фокуса текстового поля поиска
        private void FindTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FindTextBox.Text))
            {
                FindTextBox.Text = "Введите текст для поиска";
                FindTextBox.Foreground = Brushes.Gray;
            }
        }

        // Обработчик изменения выбора шрифта
        private void Font_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty,
                    ((ComboBoxItem)Font.SelectedItem).Content);
            }
        }

        // Обработчик изменения размера шрифта
        private void FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty,
                    Convert.ToDouble(((ComboBoxItem)FontSize.SelectedItem).Content));
            }
        }

        // Обработчик нажатия на кнопку для применения жирного шрифта
        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, RichTextBox);
        }

        // Обработчик нажатия на кнопку для применения курсивного шрифта
        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, RichTextBox);
        }

        // Обработчик нажатия на кнопку для применения подчеркивания
        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleUnderline.Execute(null, RichTextBox);
        }

        // Обработчик нажатия на кнопку для выравнивания текста по левому краю
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
            }
        }

        // Обработчик нажатия на кнопку для выравнивания текста по центру
        private void CenterButton_Click(object sender, RoutedEventArgs e)
        {
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            }
        }

        // Обработчик нажатия на кнопку для выравнивания текста по правому краю
        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
            }
        }

        // Обработчик нажатия на кнопку для отмены последнего действия
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (RichTextBox.CanUndo)
            {
                RichTextBox.Undo();
            }
        }

        // Обработчик нажатия на кнопку для повторения последнего отмененного действия
        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (RichTextBox.CanRedo)
            {
                RichTextBox.Redo();
            }
        }

        // Обработчик нажатия на кнопку для предварительного просмотра печати
        private void PrintPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDocumentButton_Click(sender, e);
        }
    }
}