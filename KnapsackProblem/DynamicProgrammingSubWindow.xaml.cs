using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Xml.Linq;

namespace KnapsackProblem
{
    /// <summary>
    /// Interaction logic for DynamicProgrammingSubWindow.xaml
    /// </summary>
    public partial class DynamicProgrammingSubWindow : Window
    {
        private List<Item> listItem = new List<Item>();
        private int maxWeight;
        int[,] tableF; int[,] tableX;
        Thread thread;
        ManualResetEvent mrse = new ManualResetEvent(true);
        private bool isNewThread = false;
        private bool isStarted = false;

        public DynamicProgrammingSubWindow(List<Item> listItem, int maxWeight)
        {
            InitializeComponent();
            this.listItem = listItem;
            this.maxWeight = maxWeight;
            CreateTable();
        }

        private void StartPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isStarted == false)
            {
                if (isNewThread == false)
                {
                    thread = new Thread(DynamicProgrammingDemo);
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
            CreateTable();
            isNewThread = false;
            isStarted = false;
            mrse.Set();
            btnStartAndPause.Content = "Bắt Đầu";
            btnStartAndPause.IsEnabled = true;
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

                    for (int j = 0; j <= maxWeight+1; j++)
                    {
                        titleRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(104) });
                        string content = j != 0 ? (j - 1).ToString() : "";
                        Border titleCell = CreateCell("TitleBorderStyle", "TitleTextBlockStyle", content.ToString(), 104);
                        Grid.SetColumn(titleCell, j);
                        titleRow.Children.Add(titleCell);
                    }
                    mainGrid.Children.Add(titleRow);
                }
                #endregion

                // Tạo các hàng còn lại
                #region
                if (i != 0)
                {
                    Grid normalRow = new Grid();
                    Grid.SetRow(normalRow, i);

                    normalRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(104) });
                    Border titleCell = CreateCell("TitleBorderStyle", "TitleTextBlockStyle", i.ToString(), 104);
                    Grid.SetColumn(titleCell, 0);
                    normalRow.Children.Add(titleCell);
                    

                    for (int k = 1; k <= maxWeight+1; k++)
                    {
                        normalRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(82) });
                        Border cellF = CreateCell("NormalBorderStyle", "NormalTextBlockStyle", "", 82);
                        Grid.SetColumn(cellF, k*2-1);
                        normalRow.Children.Add(cellF);

                        normalRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(22) });
                        Border cellX = CreateCell("NormalBorderStyle", "NormalTextBlockStyle", "", 22);
                        Grid.SetColumn(cellX, k*2);
                        normalRow.Children.Add(cellX);
                    }
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

        private void DynamicProgrammingDemo()
        {
            tableF = new int[listItem.Count, maxWeight + 1];
            tableX = new int[listItem.Count, maxWeight + 1];
            int FMax, XMax, yk, k, v;

            // Xử lý hàng đầu tiên
            #region
            for (v = 0; v < maxWeight + 1; v++)
            {
                mrse.WaitOne();
                tableX[0, v] = v / listItem[0].Weight;
                tableF[0, v] = listItem[0].Value * tableX[0, v];
                // Gọi hàm Animation
                if (v == 0)
                    ChangeBackgroundAnimation(0, v);
                else
                {
                    ClearAnimation(0, v - 1);
                    ChangeBackgroundAnimation(0, v);
                }
                Thread.Sleep(1000);
            }
            #endregion

            // Xử lý các hàng còn lại
            #region
            int preRow = 0;
            int preCol = maxWeight;
            for (k = 1; k < listItem.Count; k++)
            {
                mrse.WaitOne();
                tableX[k, 0] = 0;
                tableF[k, 0] = 0;

                ClearAnimation(preRow, preCol);
                ChangeBackgroundAnimation(k, 0);

                preRow = k;
                preCol = 0;
                Thread.Sleep(1000);

                for (v = 1; v < maxWeight + 1; v++)
                {
                    mrse.WaitOne();
                    FMax = tableF[k - 1, v];
                    XMax = 0;
                    yk = v / listItem[k].Weight;
                    if (listItem[k].MaxAmount > 0 && yk > listItem[k].MaxAmount)
                        yk = listItem[k].MaxAmount;

                    // Gọi hàm Animation
                    ClearAnimation(preRow, preCol);
                    ChangeBackgroundAnimationFCell(k, v, Color.FromRgb(147, 205, 255));
                    ChangeBackgroundAnimationXCell(k, v, Color.FromRgb(147, 205, 255));
                    Thread.Sleep(600);
                    mrse.WaitOne();
                    UpDownAnimation(k, v, XMax.ToString());
                    Thread.Sleep(1000);
                    mrse.WaitOne();
                    LeftRightAnimation(k, v, FMax.ToString());
                    Thread.Sleep(1000);
                    mrse.WaitOne();

                    for (int xk = 1; xk <= yk; xk++)
                    {
                        int aboveRowIndex = v - xk * listItem[k].Weight;
                        if (tableF[k - 1, v - xk * listItem[k].Weight] + xk * listItem[k].Value > FMax)
                        {
                            FMax = tableF[k - 1, v - xk * listItem[k].Weight] + xk * listItem[k].Value;
                            XMax = xk;

                            // Gọi hàm Animation
                            UpDownAnimation(k, v, XMax.ToString());
                            Thread.Sleep(1000);
                            mrse.WaitOne();
                            string str = xk + " * " + listItem[k].Value;
                            LeftRightAnimation(k, v, str);
                            Thread.Sleep(1000);
                            mrse.WaitOne();
                            ChangeBackgroundAnimationFCell(k-1, aboveRowIndex, Color.FromRgb(199, 133, 236));
                            Thread.Sleep(600);
                            mrse.WaitOne();
                            str = str + " + " + tableF[k-1, aboveRowIndex];
                            LeftRightAnimation(k, v, str);
                            Thread.Sleep(1000);
                            mrse.WaitOne();
                            ClearAnimation(k-1, aboveRowIndex);
                            LeftRightAnimation(k, v, FMax.ToString());
                            Thread.Sleep(1000);
                            mrse.WaitOne();
                        }
                    }
                    tableF[k, v] = FMax;
                    tableX[k, v] = XMax;

                    // Gọi hàm Animation
                    ChangeBackgroundAnimation(k, v);
                    preCol = v;
                    Thread.Sleep(1000);
                    mrse.WaitOne();
                }
            }

            ClearAnimation(preRow, preCol);
            #endregion

            // Duyệt bảng
            #region
            v = maxWeight;
            for (k = listItem.Count - 1; k >= 0; k--)
            {
                if (tableX[k, v] > 0)
                {
                    listItem[k].Amount = tableX[k, v];
                    ChangeBackgroundAnimationXCell(k, v, Color.FromRgb(199, 133, 236));
                    Thread.Sleep(200);
                    mrse.WaitOne();
                    v -= listItem[k].Weight * listItem[k].Amount;
                }
                else
                {
                    listItem[k].Amount = 0;
                    ChangeBackgroundAnimationXCell(k, v, Color.FromRgb(199, 133, 236));
                    Thread.Sleep(200);
                    mrse.WaitOne();
                }
            }
            #endregion

            this.Dispatcher.Invoke(() => { btnStartAndPause.Content = "Bắt Đầu"; btnStartAndPause.IsEnabled = false; });
        }

        private void ChangeBackgroundAnimation(int rowIndex, int colIndex)
        {
            int rowPosition = rowIndex + 1;
            int columnPositionX = (colIndex + 1) * 2;
            int columnPositionF = columnPositionX - 1;

            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowPosition] as Grid;

                // Thêm nội dung và animation cho ô F
                Border borderCellF = row.Children[columnPositionF] as Border;
                TextBlock textblockCellF = borderCellF.Child as TextBlock;
                textblockCellF.Text = tableF[rowIndex, colIndex].ToString();
                CreateBackgroundChangeAnimation(borderCellF, Color.FromRgb(147, 205, 255));
                CreateOpacityAnimation(textblockCellF);

                // Thêm nội dung và animation cho ô X
                Border borderCellX = row.Children[columnPositionX] as Border;
                TextBlock textblockCellX = borderCellX.Child as TextBlock;
                textblockCellX.Text = tableX[rowIndex, colIndex].ToString();
                CreateBackgroundChangeAnimation(borderCellX, Color.FromRgb(147, 205, 255));
                CreateOpacityAnimation(textblockCellX);
            });
        }

        private void ChangeBackgroundAnimationFCell(int rowIndex, int colIndex, Color backgroundColor)
        {
            int rowPosition = rowIndex + 1;
            int columnPositionX = (colIndex + 1) * 2;
            int columnPositionF = columnPositionX - 1;

            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowPosition] as Grid;

                // Thêm animation cho ô F
                Border borderCellF = row.Children[columnPositionF] as Border;
                TextBlock textblockCellF = borderCellF.Child as TextBlock;
                CreateBackgroundChangeAnimation(borderCellF, backgroundColor);
                CreateOpacityAnimation(textblockCellF);
            });
        }

        private void ChangeBackgroundAnimationXCell(int rowIndex, int colIndex, Color backgroundColor)
        {
            int rowPosition = rowIndex + 1;
            int columnPositionX = (colIndex + 1) * 2;
            int columnPositionF = columnPositionX - 1;

            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowPosition] as Grid;

                // Thêm animation cho ô X
                Border borderCellX = row.Children[columnPositionX] as Border;
                TextBlock textblockCellX = borderCellX.Child as TextBlock;
                CreateBackgroundChangeAnimation(borderCellX, backgroundColor);
                CreateOpacityAnimation(textblockCellX);
            });
        }

        private void UpDownAnimation(int rowIndex, int colIndex, string content)
        {
            int rowPosition = rowIndex + 1;
            int columnPositionX = (colIndex + 1) * 2;
            int columnPositionF = columnPositionX - 1;

            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowPosition] as Grid;

                Border borderCellX = row.Children[columnPositionX] as Border;
                TextBlock textblockCellX = borderCellX.Child as TextBlock;
                textblockCellX.Text = content;
                CreateHeightAnimation(borderCellX);
                CreateKeyFrameOpacityAnimation(textblockCellX);
            });
        }

        private void LeftRightAnimation(int rowIndex, int colIndex, string content)
        {
            int rowPosition = rowIndex + 1;
            int columnPositionX = (colIndex + 1) * 2;
            int columnPositionF = columnPositionX - 1;

            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowPosition] as Grid;

                Border borderCellF = row.Children[columnPositionF] as Border;
                TextBlock textblockCellF = borderCellF.Child as TextBlock;
                textblockCellF.Text = content;
                CreateWidthAnimation(borderCellF);
                CreateKeyFrameOpacityAnimation(textblockCellF);
            });
        }

        private void ClearAnimation(int rowIndex, int colIndex)
        {
            int rowPosition = rowIndex + 1;
            int columnPositionX = (colIndex + 1) * 2;
            int columnPositionF = columnPositionX - 1;

            this.Dispatcher.Invoke(() =>
            {
                Grid row = mainGrid.Children[rowPosition] as Grid;

                Border borderCellF = row.Children[columnPositionF] as Border;
                TextBlock textblockCellF = borderCellF.Child as TextBlock;
                borderCellF.Width = 82;
                borderCellF.Background = new SolidColorBrush(Color.FromRgb(219, 241, 255));
                borderCellF.BeginAnimation(Border.WidthProperty, null);
                textblockCellF.BeginAnimation(TextBlock.OpacityProperty, null);
                textblockCellF.Opacity = 1.0;

                Border borderCellX = row.Children[columnPositionX] as Border;
                TextBlock textblockCellX = borderCellX.Child as TextBlock;
                borderCellX.Height = 40;
                borderCellX.BeginAnimation(Border.HeightProperty, null);
                borderCellX.Background = new SolidColorBrush(Color.FromRgb(219, 241, 255));
                textblockCellX.BeginAnimation(TextBlock.OpacityProperty, null);
                textblockCellX.Opacity = 1.0;
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

        private void CreateHeightAnimation(Border borderCellX)
        {
            DoubleAnimationUsingKeyFrames heightAnimation = new DoubleAnimationUsingKeyFrames();
            heightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            LinearDoubleKeyFrame heightKey1 = new LinearDoubleKeyFrame();
            heightKey1.Value = 40;
            heightKey1.KeyTime = TimeSpan.FromMilliseconds(0);
            LinearDoubleKeyFrame heightKey2 = new LinearDoubleKeyFrame();
            heightKey2.Value = 0;
            heightKey2.KeyTime = TimeSpan.FromMilliseconds(400);
            LinearDoubleKeyFrame heightKey3 = new LinearDoubleKeyFrame();
            heightKey3.Value = 40;
            heightKey3.KeyTime = TimeSpan.FromMilliseconds(800);
            heightAnimation.KeyFrames.Add(heightKey1);
            heightAnimation.KeyFrames.Add(heightKey2);
            heightAnimation.KeyFrames.Add(heightKey3);
            borderCellX.BeginAnimation(Border.HeightProperty, heightAnimation);
        }

        private void CreateWidthAnimation(Border borderCellF)
        {
            DoubleAnimationUsingKeyFrames widthAnimation = new DoubleAnimationUsingKeyFrames();
            widthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            LinearDoubleKeyFrame widthKey1 = new LinearDoubleKeyFrame();
            widthKey1.Value = 82;
            widthKey1.KeyTime = TimeSpan.FromMilliseconds(0);
            LinearDoubleKeyFrame widthKey2 = new LinearDoubleKeyFrame();
            widthKey2.Value = 0;
            widthKey2.KeyTime = TimeSpan.FromMilliseconds(400);
            LinearDoubleKeyFrame widthKey3 = new LinearDoubleKeyFrame();
            widthKey3.Value = 82;
            widthKey3.KeyTime = TimeSpan.FromMilliseconds(800);
            widthAnimation.KeyFrames.Add(widthKey1);
            widthAnimation.KeyFrames.Add(widthKey2);
            widthAnimation.KeyFrames.Add(widthKey3);
            borderCellF.BeginAnimation(Border.WidthProperty, widthAnimation);
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
    }
}
