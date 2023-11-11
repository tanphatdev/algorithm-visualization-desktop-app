using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KnapsackProblem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Item> listItem = new List<Item>();
        private List<Item> listItemTemp = new List<Item>();
        private Algorithm algorithm = new Algorithm();

        public MainWindow()
        {
            InitializeComponent();
            mainTable.ItemsSource = listItemTemp;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string name;
            int weight;
            int value;
            int maxAmount;

            #region Kiểm tra danh sách đồ vật đã đầy hay chưa
            //if (listItem.Count >= 5)
            //{
            //    txbName.Text = "";
            //    txbWeight.Text = "";
            //    txbValue.Text = "";
            //    MessageBox.Show("Danh sách đồ vật đã dầy");
            //    return;
            //}
            #endregion

            #region Kiểm tra tên đồ vật
            name = txbName.Text;
            if (!(Regex.IsMatch(name, @"^[a-zA-Z]+$")))
            {
                MessageBox.Show("Tên đồ vật phải là chữ cái");
                return;
            }
            else if (!(name.Length <= 2))
            {
                MessageBox.Show("Tên đồ vật không được vượt quá 2 chữ cái");
                return;
            }
            #endregion

            #region Kiểm tra trọng lượng đồ vật
            try
            {
                weight = Convert.ToInt32(txbWeight.Text);
                if (!(weight > 0))
                {
                    MessageBox.Show("Trọng lượng đồ vật phải lớn hơn 0");
                    return;
                }
                if(!(weight < 100))
                {
                    MessageBox.Show("Trọng lượng đồ vật phải nhỏ hơn 100");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Trọng lượng đồ vật phải là số");
                return;
            }
            #endregion

            #region Kiểm tra giá trị đồ vật
            try
            {
                value = Convert.ToInt32(txbValue.Text);
                if (!(value > 0))
                {
                    MessageBox.Show("Giá trị đồ vật phải lớn hơn 0");
                    return;
                }
                if (!(value < 100))
                {
                    MessageBox.Show("Giá trị đồ vật phải nhỏ hơn 100");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Giá trị đồ vật phải là số");
                return;
            }
            #endregion

            #region Kiểm tra số lượng đồ vật tối đa
            if (String.IsNullOrEmpty(txbMaxAmount.Text) || txbMaxAmount.Text.All(c => Char.IsWhiteSpace(c)))
            {
                maxAmount = -1;
            } else
            {
                try
                {
                    maxAmount = Convert.ToInt32(txbMaxAmount.Text);
                    if (!(maxAmount > 0))
                    {
                        MessageBox.Show("Số lượng đồ vật tối đa phải lớn hơn 0");
                        return;
                    }
                    if (!(maxAmount < 10))
                    {
                        MessageBox.Show("Số lượng đồ vật tối đa phải nhỏ hơn 10");
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Số lượng đồ vật tối đa phải là số");
                    return;
                }
            }
            #endregion

            listItem.Add(new Item(name, weight, value, maxAmount));
            CloneListItem(listItemTemp);

            #region Xóa dữ liệu nhập vào
            txbName.Text = "";
            txbWeight.Text = "";
            txbValue.Text = "";
            txbMaxAmount.Text = "";
            #endregion

            mainTable.Items.Refresh();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            #region Xóa bảng
            listItem.Clear();
            listItemTemp.Clear();
            mainTable.Items.Refresh();
            #endregion

            #region Xóa Tổng trọng lượng và Tổng giá trị
            txbTotalWeight.Text = "";
            txbTotalValue.Text = "";
            #endregion
        }

        private void btnOptimize_Click(object sender, RoutedEventArgs e)
        {
            int maxWeight = 0;
            try
            {
                maxWeight = Convert.ToInt32(txbMaxWeight.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Trọng lượng balo không được để trống");
                return;
            }

            #region Gọi thuật toán
            if (cbAlgorithm.SelectedIndex == 0)
            {
                listItemTemp = algorithm.Greedy(listItemTemp, maxWeight);
                mainTable.Items.Refresh();
            }
            else if (cbAlgorithm.SelectedIndex == 1)
            {
                listItemTemp = algorithm.DynamicPrograming(listItemTemp, maxWeight);
                mainTable.Items.Refresh();
            }
            else if (cbAlgorithm.SelectedIndex == 2)
            {
                listItemTemp = algorithm.BranchAndBound(listItemTemp, maxWeight);
                mainTable.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn thuật toán để tối ưu");
            }
            #endregion

            TotalWeight();
            TotalValue();
        }

        private void TotalWeight()
        {
            int totalWeight = 0;
            for (int i = 0; i < listItemTemp.Count; i++)
            {
                totalWeight += listItemTemp[i].Weight * listItemTemp[i].Amount;
            }
            txbTotalWeight.Text = totalWeight.ToString();
        }

        private void TotalValue()
        {
            int totalValue = 0;
            for (int i = 0; i < listItemTemp.Count; i++)
            {
                totalValue += listItemTemp[i].Value * listItemTemp[i].Amount;
            }
            txbTotalValue.Text = totalValue.ToString();
        }

        private void btnDemo_Click(object sender, RoutedEventArgs e)
        {
            int maxWeight = 0;
            try
            {
                maxWeight = Convert.ToInt32(txbMaxWeight.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Trọng lượng balo không được để trống");
                return;
            }

            if (cbAlgorithm.SelectedIndex == 0)
            {
                List<Item> list = new List<Item>();
                CloneListItem(list);
                GreedySubWindow subWindow = new GreedySubWindow(list, maxWeight);
                subWindow.ShowDialog();
            } else if (cbAlgorithm.SelectedIndex == 1)
            {
                List<Item> list = new List<Item>();
                CloneListItem(list);
                DynamicProgrammingSubWindow subWindow = new DynamicProgrammingSubWindow(list, maxWeight);
                subWindow.ShowDialog();
            } else if (cbAlgorithm.SelectedIndex == 2)
            {
                List<Item> list = new List<Item>();
                CloneListItem(list);
                BranchAndBoundSubWindow subWindow = new BranchAndBoundSubWindow(list, maxWeight);
                subWindow.ShowDialog();
            } else
            {
                MessageBox.Show("Vui lòng chọn thuật toán để demo");
            }
            
        }

        private void cbAlgorithm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CloneListItem(listItemTemp);
            mainTable.Items.Refresh();

            txbTotalWeight.Text = "";
            txbTotalValue.Text = "";
        }

        private void CloneListItem(List<Item> list)
        {
            list.Clear();
            for (int i = 0; i < listItem.Count; i++)
                list.Add(listItem[i].Clone());
        }

        private void openMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    String rawString = File.ReadAllText(openFileDialog.FileName);
                    String[] listRawString = rawString.Split('\n');

                    if (listRawString[0] != "###")
                    {
                        int maxWeight = Convert.ToInt32(listRawString[0]);
                        txbMaxWeight.Text = maxWeight.ToString();
                    }

                    List<Item> listTemp = new List<Item>();
                    String[] spearator = { " - " };
                    String name; int weight; int value; int maxAmount;

                    #region Xử lý đổ dữ liệu trở lại bảng
                    for (int i = 1; i < listRawString.Count()-1; i++)
                    {
                        String[] rawItem = listRawString[i].Split(spearator, StringSplitOptions.RemoveEmptyEntries);

                        name = rawItem[0];
                        weight = Convert.ToInt32(rawItem[1]);
                        value = Convert.ToInt32(rawItem[2]);
                        maxAmount = Convert.ToInt32(rawItem[3]);

                        listTemp.Add(new Item(name, weight, value, maxAmount));
                    }

                    listItem.Clear();
                    listItemTemp.Clear();

                    for (int i = 0; i < listTemp.Count; i++)
                    {
                        listItem.Add(listTemp[i].Clone());
                        CloneListItem(listItemTemp);
                    }
                    #endregion

                    mainTable.Items.Refresh();
                }
                catch (Exception)
                {
                    MessageBox.Show("Định dạng file không đúng");
                }
            }
        }

        private void saveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            String str = "";
            try
            {
                int maxWeight = Convert.ToInt32(txbMaxWeight.Text);
                str += maxWeight.ToString() + "\n";
            }
            catch (Exception)
            {
                str += "###\n";
            }

            for (int i = 0; i < listItem.Count; i++)
            {
                str += listItem[i].Name + " - ";
                str += listItem[i].Weight + " - ";
                str += listItem[i].Value + " - ";
                str += listItem[i].MaxAmount + "\n";
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, str);
        }
    }
}
