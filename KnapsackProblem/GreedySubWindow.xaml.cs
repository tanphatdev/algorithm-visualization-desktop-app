using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KnapsackProblem
{
    /// <summary>
    /// Interaction logic for GreedySubWindow.xaml
    /// </summary>
    public partial class GreedySubWindow : Window
    {
        private List<Item> listItem = new List<Item>();
        private List<Item> listItemTemp = new List<Item>();
        private int maxWeight;
        Thread thread;
        ManualResetEvent mrse = new ManualResetEvent(true);
        private bool isNewThread = false;
        private bool isStarted = false;

        public GreedySubWindow(List<Item> listItem, int maxWeight)
        {
            InitializeComponent();
            this.listItem = listItem;
            for (int i = 0; i < listItem.Count; i++)
            {
                listItemTemp.Add(listItem[i].Clone());
            }
            this.maxWeight = maxWeight;
            CreateTable();
        }

        private void CreateTable()
        {
            for (int i = 0; i <= listItem.Count; i++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
                // Tạo hàng tiêu đề
                #region
                if (i == 0)
                {
                    Grid titleRow = new Grid();
                    Grid.SetRow(titleRow, 0);
                    Border titleCell;
                    string content;

                    titleRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120) });
                    content = "Tên Đồ Vật";
                    titleCell = CreateCell("TitleBorderStyle", "TitleTextBlockStyle", content, 120);
                    Grid.SetColumn(titleCell, 0);
                    titleRow.Children.Add(titleCell);

                    titleRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120) });
                    content = "Trọng lượng";
                    titleCell = CreateCell("TitleBorderStyle", "TitleTextBlockStyle", content, 120);
                    Grid.SetColumn(titleCell, 1);
                    titleRow.Children.Add(titleCell);

                    titleRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });
                    content = "Giá Trị";
                    titleCell = CreateCell("TitleBorderStyle", "TitleTextBlockStyle", content, 80);
                    Grid.SetColumn(titleCell, 2);
                    titleRow.Children.Add(titleCell);

                    titleRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120) });
                    content = "Đơn Giá";
                    titleCell = CreateCell("TitleBorderStyle", "TitleTextBlockStyle", content, 120);
                    Grid.SetColumn(titleCell, 3);
                    titleRow.Children.Add(titleCell);

                    titleRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });
                    content = "Số Lượng";
                    titleCell = CreateCell("TitleBorderStyle", "TitleTextBlockStyle", content, 80);
                    Grid.SetColumn(titleCell, 4);
                    titleRow.Children.Add(titleCell);

                    titleRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(140) });
                    content = "Số Lượng Tối Đa";
                    titleCell = CreateCell("TitleBorderStyle", "TitleTextBlockStyle", content, 140);
                    Grid.SetColumn(titleCell, 5);
                    titleRow.Children.Add(titleCell);

                    mainGrid.Children.Add(titleRow);
                }
                #endregion

                // Tạo các hàng còn lại
                #region
                if (i != 0)
                {
                    Grid normalRow = new Grid();
                    Grid.SetRow(normalRow, i);
                    Border normalCell;
                    string content;

                    normalRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120) });
                    content = listItem[i-1].Name;
                    normalCell = CreateCell("NormalBorderStyle", "NormalTextBlockStyle", content, 120);
                    Grid.SetColumn(normalCell, 0);
                    normalRow.Children.Add(normalCell);

                    normalRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120) });
                    content = listItem[i - 1].Weight.ToString();
                    normalCell = CreateCell("NormalBorderStyle", "NormalTextBlockStyle", content, 120);
                    Grid.SetColumn(normalCell, 1);
                    normalRow.Children.Add(normalCell);

                    normalRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });
                    content = listItem[i - 1].Value.ToString();
                    normalCell = CreateCell("NormalBorderStyle", "NormalTextBlockStyle", content, 80);
                    Grid.SetColumn(normalCell, 2);
                    normalRow.Children.Add(normalCell);

                    normalRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120) });
                    content = "";
                    normalCell = CreateCell("NormalBorderStyle", "NormalTextBlockStyle", content, 120);
                    Grid.SetColumn(normalCell, 3);
                    normalRow.Children.Add(normalCell);

                    normalRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });
                    content = "";
                    normalCell = CreateCell("NormalBorderStyle", "NormalTextBlockStyle", content, 80);
                    Grid.SetColumn(normalCell, 4);
                    normalRow.Children.Add(normalCell);

                    normalRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(140) });
                    content = listItem[i - 1].StringMaxAmount;
                    normalCell = CreateCell("NormalBorderStyle", "NormalTextBlockStyle", content, 140);
                    Grid.SetColumn(normalCell, 5);
                    normalRow.Children.Add(normalCell);

                    mainGrid.Children.Add(normalRow);
                }
                #endregion
            }
        }

        private Border CreateCell(string resourceBorder, string resourceTextBlock, string content, int Width)
        {
            Style borderStyle = this.FindResource(resourceBorder) as Style;
            Border currentBorder = new Border() { Style = borderStyle, Width = Convert.ToInt32(Width), Height = 40 };
            Style textblockStyle = this.FindResource(resourceTextBlock) as Style;
            TextBlock currentTextBlock = new TextBlock() { Style = textblockStyle };
            currentTextBlock.Text = content;
            currentBorder.Child = currentTextBlock;
            return currentBorder;
        }

        private void GreedyDemo()
        {
            int i,j;
            int maxWei = maxWeight;

            // Tính đơn giá các đồ vật
            #region
            for (i = 0; i < listItem.Count; i++)
            {
                listItem[i].Cost = (double) listItem[i].Value / listItem[i].Weight;
                string str = listItem[i].Value + " / " + listItem[i].Weight;

                ChangeBackgroundRowAnimation(i+1, Color.FromRgb(147, 205, 255));
                Thread.Sleep(600);
                mrse.WaitOne();

                LeftRightAnimation(i+1, 3, str, 120);
                Thread.Sleep(1000);
                mrse.WaitOne();

                str = String.Format("{0:0.##}", listItem[i].Cost);
                LeftRightAnimation(i+1, 3, str, 120);
                Thread.Sleep(1000);
                mrse.WaitOne();
                ClearChangeBackgroundRowAnimation(i + 1);
            }
            #endregion

            // Sắp xếp đồ vật theo đơn giá
            #region
            for (i = 0; i <= listItem.Count-2; i++)
            {
                for (j = listItem.Count-1; j >= i+1; j--)
                {

                    ChangeBackgroundRowAnimation(j+1, Color.FromRgb(147, 205, 255));
                    ChangeBackgroundRowAnimation(j, Color.FromRgb(147, 205, 255));
                    Thread.Sleep(1000);
                    mrse.WaitOne();

                    if (listItem[j].Cost > listItem[j-1].Cost)
                    {
                        Item temp = listItem[j-1];
                        listItem[j-1] = listItem[j];
                        listItem[j] = temp;

                        TranslateAnimationBottomRow(j);
                        TranslateAnimationTopRow(j-1);
                        Thread.Sleep(1500);
                        mrse.WaitOne();

                        UpdateContentRow(j);
                        UpdateContentRow(j - 1);
                    }

                    ClearChangeBackgroundRowAnimation(j+1);
                    ClearChangeBackgroundRowAnimation(j);
                }
            }
            #endregion

            // Tính số lượng đồ vật
            #region
            for (i = 0; i < listItem.Count; i++)
            {
                ChangeBackgroundRowAnimation(i + 1, Color.FromRgb(147, 205, 255));
                Thread.Sleep(600);
                mrse.WaitOne();

                listItem[i].Amount = maxWei / listItem[i].Weight;

                string str = maxWei + " / " + listItem[i].Weight;
                LeftRightAnimation(i + 1, 4, str, 80);
                Thread.Sleep(1000);
                mrse.WaitOne();

                str = listItem[i].Amount.ToString();
                LeftRightAnimation(i + 1, 4, str, 80);
                Thread.Sleep(1000);
                mrse.WaitOne();

                if (listItem[i].MaxAmount > 0 && listItem[i].Amount > listItem[i].MaxAmount)
                {
                    listItem[i].Amount = listItem[i].MaxAmount;
                    str = listItem[i].Amount.ToString();
                    LeftRightAnimation(i + 1, 4, str, 80);
                    Thread.Sleep(1000);
                    mrse.WaitOne();
                }

                maxWei = maxWei - listItem[i].Weight * listItem[i].Amount;

                ClearChangeBackgroundRowAnimation(i + 1);
            }
            #endregion

            this.Dispatcher.Invoke(() => { btnStartAndPause.Content = "Bắt Đầu"; btnStartAndPause.IsEnabled = false; });
        }

        private void TranslateAnimationTopRow(int rowIndex)
        {
            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowIndex + 1] as Grid;
                Border borderCell;

                borderCell = row.Children[0] as Border;
                CreateTranslateTransformTopCellAnimation(borderCell);

                borderCell = row.Children[1] as Border;
                CreateTranslateTransformTopCellAnimation(borderCell);

                borderCell = row.Children[2] as Border;
                CreateTranslateTransformTopCellAnimation(borderCell);

                borderCell = row.Children[3] as Border;
                CreateTranslateTransformTopCellAnimation(borderCell);

                borderCell = row.Children[4] as Border;
                CreateTranslateTransformTopCellAnimation(borderCell);

                borderCell = row.Children[5] as Border;
                CreateTranslateTransformTopCellAnimation(borderCell);
            });
        }

        private void TranslateAnimationBottomRow(int rowIndex)
        {
            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowIndex + 1] as Grid;
                Border borderCell;

                borderCell = row.Children[0] as Border;
                CreateTranslateTransformBottomCellAnimation(borderCell);

                borderCell = row.Children[1] as Border;
                CreateTranslateTransformBottomCellAnimation(borderCell);

                borderCell = row.Children[2] as Border;
                CreateTranslateTransformBottomCellAnimation(borderCell);

                borderCell = row.Children[3] as Border;
                CreateTranslateTransformBottomCellAnimation(borderCell);

                borderCell = row.Children[4] as Border;
                CreateTranslateTransformBottomCellAnimation(borderCell);

                borderCell = row.Children[5] as Border;
                CreateTranslateTransformBottomCellAnimation(borderCell);
            });
        }

        private void UpdateContentRow(int rowIndex)
        {
            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowIndex+1] as Grid;
                Border borderCell;
                TextBlock textblockCell;
                TranslateTransform myTranslateTransform = new TranslateTransform();

                borderCell = row.Children[0] as Border;
                textblockCell = borderCell.Child as TextBlock;
                borderCell.RenderTransform = myTranslateTransform;
                textblockCell.Text = listItem[rowIndex].Name;

                borderCell = row.Children[1] as Border;
                textblockCell = borderCell.Child as TextBlock;
                borderCell.RenderTransform = myTranslateTransform;
                textblockCell.Text = listItem[rowIndex].Weight.ToString();

                borderCell = row.Children[2] as Border;
                textblockCell = borderCell.Child as TextBlock;
                borderCell.RenderTransform = myTranslateTransform;
                textblockCell.Text = listItem[rowIndex].Value.ToString();

                borderCell = row.Children[3] as Border;
                textblockCell = borderCell.Child as TextBlock;
                borderCell.RenderTransform = myTranslateTransform;
                textblockCell.Text = String.Format("{0:0.##}", listItem[rowIndex].Cost);

                borderCell = row.Children[4] as Border;
                textblockCell = borderCell.Child as TextBlock;
                borderCell.RenderTransform = myTranslateTransform;
                textblockCell.Text = "";

                borderCell = row.Children[5] as Border;
                textblockCell = borderCell.Child as TextBlock;
                borderCell.RenderTransform = myTranslateTransform;
                textblockCell.Text = listItem[rowIndex].StringMaxAmount;
            });
        }

        private void ChangeBackgroundRowAnimation(int rowIndex, Color backgroudColor)
        {
            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowIndex] as Grid;

                for (int i = 0; i < row.Children.Count; i++)
                {
                    Border borderCell = row.Children[i] as Border;
                    TextBlock textblockCell = borderCell.Child as TextBlock;
                    CreateBackgroundChangeAnimation(borderCell, backgroudColor);
                    CreateOpacityAnimation(textblockCell);
                }
            });            
        }

        private void ClearChangeBackgroundRowAnimation(int rowIndex)
        {
            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowIndex] as Grid;

                for (int i = 0; i < row.Children.Count; i++)
                {
                    Border borderCell = row.Children[i] as Border;
                    TextBlock textblockCell = borderCell.Child as TextBlock;
                    borderCell.Background = new SolidColorBrush(Color.FromRgb(219, 241, 255));
                    textblockCell.BeginAnimation(TextBlock.OpacityProperty, null);
                    textblockCell.Opacity = 1;
                }
            });
        }

        private void LeftRightAnimation(int rowIndex, int colIndex, string content, int width)
        {
            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowIndex] as Grid;

                Border borderCell = row.Children[colIndex] as Border;
                TextBlock textblockCell = borderCell.Child as TextBlock;
                textblockCell.Text = content;
                CreateWidthAnimation(borderCell, width);
                CreateKeyFrameOpacityAnimation(textblockCell);
            });
        }

        private void CreateBackgroundChangeAnimation(Border borderCell, Color backgroudColor)
        {
            SolidColorBrush background = new SolidColorBrush(Color.FromRgb(219, 241, 255));
            ColorAnimation backgroundAnimation = new ColorAnimation();
            backgroundAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            backgroundAnimation.From = Color.FromRgb(219, 241, 255);
            backgroundAnimation.To = backgroudColor;
            background.BeginAnimation(SolidColorBrush.ColorProperty, backgroundAnimation);
            borderCell.Background = background;
        }

        private void CreateOpacityAnimation(TextBlock textblockCell)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            opacityAnimation.From = 0.0;
            opacityAnimation.To = 1.0;
            textblockCell.BeginAnimation(TextBlock.OpacityProperty, opacityAnimation);
        }

        private void CreateWidthAnimation(Border borderCell, int width)
        {
            DoubleAnimationUsingKeyFrames widthAnimation = new DoubleAnimationUsingKeyFrames();
            widthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            LinearDoubleKeyFrame widthKey1 = new LinearDoubleKeyFrame();
            widthKey1.Value = width;
            widthKey1.KeyTime = TimeSpan.FromMilliseconds(0);
            LinearDoubleKeyFrame widthKey2 = new LinearDoubleKeyFrame();
            widthKey2.Value = 0;
            widthKey2.KeyTime = TimeSpan.FromMilliseconds(400);
            LinearDoubleKeyFrame widthKey3 = new LinearDoubleKeyFrame();
            widthKey3.Value = width;
            widthKey3.KeyTime = TimeSpan.FromMilliseconds(800);
            widthAnimation.KeyFrames.Add(widthKey1);
            widthAnimation.KeyFrames.Add(widthKey2);
            widthAnimation.KeyFrames.Add(widthKey3);
            borderCell.BeginAnimation(Border.WidthProperty, widthAnimation);
        }

        private void CreateKeyFrameOpacityAnimation(TextBlock textblockCellX)
        {
            DoubleAnimationUsingKeyFrames opacityAnimation = new DoubleAnimationUsingKeyFrames();
            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            LinearDoubleKeyFrame opacityKey1 = new LinearDoubleKeyFrame();
            opacityKey1.Value = 0.0;
            opacityKey1.KeyTime = TimeSpan.FromMilliseconds(0);
            LinearDoubleKeyFrame opacityKey2 = new LinearDoubleKeyFrame();
            opacityKey2.Value = 0.0;
            opacityKey2.KeyTime = TimeSpan.FromMilliseconds(400);
            LinearDoubleKeyFrame opacityKey3 = new LinearDoubleKeyFrame();
            opacityKey3.Value = 1.0;
            opacityKey3.KeyTime = TimeSpan.FromMilliseconds(800);
            opacityAnimation.KeyFrames.Add(opacityKey1);
            opacityAnimation.KeyFrames.Add(opacityKey2);
            opacityAnimation.KeyFrames.Add(opacityKey3);
            textblockCellX.BeginAnimation(TextBlock.OpacityProperty, opacityAnimation);
        }

        private void CreateTranslateTransformTopCellAnimation(Border borderCell)
        {
            DoubleAnimation translateDownAnimation = new DoubleAnimation();
            translateDownAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            translateDownAnimation.From = 0;
            translateDownAnimation.To = 40;

            TranslateTransform myTranslateDownTransform = new TranslateTransform();
            myTranslateDownTransform.BeginAnimation(TranslateTransform.YProperty, translateDownAnimation);
            borderCell.RenderTransform = myTranslateDownTransform;
        }

        private void CreateTranslateTransformBottomCellAnimation(Border borderCell)
        {
            DoubleAnimation translateUpAnimation = new DoubleAnimation();
            translateUpAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            translateUpAnimation.From = 0;
            translateUpAnimation.To = -40;

            TranslateTransform myTranslateUpTransform = new TranslateTransform();
            myTranslateUpTransform.BeginAnimation(TranslateTransform.YProperty, translateUpAnimation);
            borderCell.RenderTransform = myTranslateUpTransform;
        }

        private void StartPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isStarted == false)
            {
                if (isNewThread == false)
                {
                    thread = new Thread(GreedyDemo);
                    thread.IsBackground = true;
                    thread.Start();
                    isNewThread = true;
                }
                else mrse.Set(); ;
                btnStartAndPause.Content = "Tạm Dừng";
                isStarted = true;

            }
            else
            {
                mrse.Reset();
                btnStartAndPause.Content = "Bắt Đầu";
                isStarted = false;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            thread.Abort();
            mainGrid.RowDefinitions.Clear();
            mainGrid.Children.Clear();
            listItem.Clear();
            for (int i = 0; i < listItemTemp.Count; i++)
            {
                listItem.Add(listItemTemp[i].Clone());
            }
            CreateTable();
            isNewThread = false;
            isStarted = false;
            mrse.Set();
            btnStartAndPause.Content = "Bắt Đầu";
            btnStartAndPause.IsEnabled = true;
        }
    }
}
