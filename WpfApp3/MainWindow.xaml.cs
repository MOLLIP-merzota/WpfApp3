using System;
using System.IO;
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
        private string _filePath; // Путь к текущему открытому файлу
        private string _fileContent; // Содержимое текущего открытого файла

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded; // Обработчик события загрузки окна
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyWindowAnimations(); // Применение анимаций при загрузке окна
        }

        private void ApplyWindowAnimations()
        {
            // Создание трансформации вращения и применение её к окну
            var rotateTransform = new RotateTransform();
            RenderTransform = rotateTransform;
            RenderTransformOrigin = new Point(0.5, 0.5);

            // Определение анимации вращения
            var rotateAnimation = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(2))
            {
                RepeatBehavior = new RepeatBehavior(1)
            };

            // Определение анимации появления
            var fadeInAnimation = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(2));

            // Запуск анимаций
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Открытие диалога выбора файла для выбора текстового файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Clown files (*.clw)|*.clw"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _filePath = openFileDialog.FileName; // Сохранение пути к выбранному файлу
                _fileContent = File.ReadAllText(_filePath); // Чтение содержимого файла

                SetRichTextBoxContent(_fileContent); // Отображение содержимого в RichTextBox
                ApplyDefaultFormatting(); // Применение стандартного форматирования к тексту
                AnimateRichTextBoxVisibility(); // Анимация видимости RichTextBox
            }
        }

        private void AnimateRichTextBoxVisibility()
        {
            // Сделать RichTextBox видимым с анимацией появления
            RichTextBox.Visibility = Visibility.Visible;
            var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5));
            RichTextBox.BeginAnimation(OpacityProperty, fadeInAnimation);
        }

        private void ApplyDefaultFormatting()
        {
            // Применение стандартного шрифта и размера к тексту в RichTextBox
            var textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
            textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily("Arial"));
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, 12.0);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            // Сохранение содержимого RichTextBox обратно в файл
            if (string.IsNullOrEmpty(_filePath))
            {
                ShowCustomMessageBox("Ты че...Долбаеб?."); // Показать сообщение, если файл не загружен
                return;
            }

            try
            {
                string content = GetRichTextBoxContent(); // Получение текущего содержимого RichTextBox
                File.WriteAllText(_filePath, content); // Запись содержимого обратно в файл
                ShowCustomMessageBox("Сохранения изменены."); // Показать сообщение об успешном сохранении
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowCustomMessageBox(string message)
        {
            // Отображение пользовательского сообщения
            CustomMessageBox customMessageBox = new CustomMessageBox(message);
            customMessageBox.ShowDialog();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            // Открытие диалога печати для печати документа
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                PrintDocument(); // Печать документа, если пользователь подтвердил
            }
            else
            {
                MessageBox.Show("Печать отменена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void PrintDocument()
        {
            // Создание FlowDocument из содержимого RichTextBox и его печать
            FlowDocument flowDocument = new FlowDocument(new Paragraph(new Run(GetRichTextBoxContent())))
            {
                ColumnWidth = double.MaxValue
            };

            IDocumentPaginatorSource idpSource = flowDocument;
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintDocument(idpSource.DocumentPaginator, "Печать документа");
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            // Поиск и выделение текста в RichTextBox
            string findText = FindTextBox.Text;

            if (!string.IsNullOrEmpty(findText))
            {
                Find(findText);
            }
        }

        private void Find(string searchText)
        {
            // Поиск указанного текста в RichTextBox и его выделение
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
                MessageBox.Show("Текст не найден.");
            }
        }

        private TextPointer GetTextPositionAtOffset(TextPointer start, int offset)
        {
            // Получение TextPointer на указанном смещении
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

        private void FindTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Очистка текста-заполнителя при получении фокуса FindTextBox
            if (FindTextBox.Text == "Введите текст для поиска")
            {
                FindTextBox.Text = "";
                FindTextBox.Foreground = Brushes.Black;
            }
            return;
        }

        private void FindTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Восстановление текста-заполнителя, если FindTextBox пуст при потере фокуса
            if (string.IsNullOrWhiteSpace(FindTextBox.Text))
            {
                FindTextBox.Text = "Введите текст для поиска";
                FindTextBox.Foreground = Brushes.Gray;
            }

            return;
        }

        private void SetRichTextBoxContent(string text)
        {
            // Установка содержимого RichTextBox
            RichTextBox.Document.Blocks.Clear();
            RichTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        private string GetRichTextBoxContent()
        {
            // Получение текущего содержимого RichTextBox в виде строки
            TextRange textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
            return textRange.Text.TrimEnd('\r', '\n');
        }

        private void Font_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Изменение шрифта выделенного текста в RichTextBox
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty,
                    ((ComboBoxItem)Font.SelectedItem).Content);
            }
        }

        private void FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Изменение размера шрифта выделенного текста в RichTextBox
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty,
                    Convert.ToDouble(((ComboBoxItem)FontSize.SelectedItem).Content));
            }
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            // Переключение жирного форматирования для выделенного текста
            EditingCommands.ToggleBold.Execute(null, RichTextBox);
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            // Переключение курсивного форматирования для выделенного текста
            EditingCommands.ToggleItalic.Execute(null, RichTextBox);
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            // Переключение подчеркивания для выделенного текста
            EditingCommands.ToggleUnderline.Execute(null, RichTextBox);
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            // Выравнивание выделенного текста по левому краю
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
            }
        }

        private void CenterButton_Click(object sender, RoutedEventArgs e)
        {
            // Центрирование выделенного текста
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            // Выравнивание выделенного текста по правому краю
            if (RichTextBox.Selection != null)
            {
                RichTextBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
            }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            // Отмена последнего действия в RichTextBox
            if (RichTextBox.CanUndo)
            {
                RichTextBox.Undo();
            }
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            // Повтор последнего отмененного действия в RichTextBox
            if (RichTextBox.CanRedo)
            {
                RichTextBox.Redo();
            }
        }

        private void PrintPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Показать предварительный просмотр печати документа
            PrintDocument();
        }
    }
}