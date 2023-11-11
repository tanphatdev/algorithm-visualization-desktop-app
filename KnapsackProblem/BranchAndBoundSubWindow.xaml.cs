using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = Microsoft.Msagl.Drawing.Color;

namespace KnapsackProblem
{
    /// <summary>
    /// Interaction logic for BranchAndBoundSubWindow.xaml
    /// </summary>
    public partial class BranchAndBoundSubWindow : Window
    {
        private List<Item> listItem = new List<Item>();
        private int maxWeight;
        private GraphViewer graphViewer = new GraphViewer();
        private Graph graph = new Graph();
        private Thread thread;
        private ManualResetEvent mrse = new ManualResetEvent(true);
        private bool isNewThread = false;
        private bool isStarted = false;

        public BranchAndBoundSubWindow(List<Item> listItem, int maxWeight)
        {
            InitializeComponent();
            this.listItem = listItem;
            this.maxWeight = maxWeight;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateGraph(null, null);
            CreateHideBranchAndBound();
        }

        private void CreateGraph(object sender, ExecutedRoutedEventArgs ex)
        {
            graphViewer.BindToPanel(graphViewerPanel);
            graphViewer.Graph = graph;
            graphViewer.LayoutEditingEnabled = false;
        }

        private void CreateHideBranchAndBound()
        {
            listItem.ForEach(item => { item.Cost = (double)item.Value / item.Weight; });

            for (int i = 0; i <= listItem.Count - 2; i++)
            {
                for (int j = listItem.Count - 1; j >= i + 1; j--)
                {
                    if (listItem[j].Cost > listItem[j - 1].Cost)
                    {
                        Item temp = listItem[j - 1];
                        listItem[j - 1] = listItem[j];
                        listItem[j] = temp;
                    }
                }
            }

            double totalValue = 0.0;
            int V = maxWeight;
            double CanTren = V * listItem[0].Cost;
            double maxValueTemp = 0.0;
            int curNodeIndex = 0;
            List<int> solutionTemp = new List<int>();
            for (int i = 0; i < listItem.Count(); i++)
                solutionTemp.Add(0);

            Microsoft.Msagl.Drawing.Node rootNode = graph.AddNode("root");
            rootNode.Attr.LineWidth *= 0.3;
            rootNode.Label.FontSize = 5;
            rootNode.LabelText = "    " + "TGT = " + totalValue + "    " + "\n" +
                                 "    " + "W = " + V + "    " + "\n" +
                                 "    " + "CT = " + CanTren + "    ";

            Microsoft.Msagl.Drawing.Node deleteNode = graph.AddNode("deleteNode");
            deleteNode.Attr.LineWidth *= 0.3;
            deleteNode.Label.FontSize = 5;
            deleteNode.LabelText = "Cắt Bỏ";
            deleteNode.IsVisible = false;

            graphViewer.Graph = graph;

            RecursiveHideBranchAndBound(curNodeIndex, ref totalValue, ref CanTren, ref V, ref maxValueTemp, solutionTemp, listItem, "root");
        }

        private void RecursiveHideBranchAndBound(int curNodeIndex, ref double totalValue, ref double CanTren, ref int V, ref double maxValueTemp, List<int> solutionTemp, List<Item> listItems, string parrentNodeId)
        {
            int yk = V / listItems[curNodeIndex].Weight;
            if (listItems[curNodeIndex].MaxAmount > 0 && yk > listItems[curNodeIndex].MaxAmount)
                yk = listItems[curNodeIndex].MaxAmount;

            for (int j = yk; j >= 0; j--)
            {
                totalValue = totalValue + j * listItems[curNodeIndex].Value;
                V = V - j * listItems[curNodeIndex].Weight;
                if (curNodeIndex == listItems.Count() - 1)
                    CanTren = totalValue;
                else
                    CanTren = totalValue + V * listItems[curNodeIndex + 1].Cost;

                string childNodeId = parrentNodeId + ":" + listItems[curNodeIndex].Name + "=" + j;
                var node = graph.AddNode(childNodeId);
                FillNode(node, curNodeIndex, j, totalValue, V, CanTren);
                var edge = graph.AddEdge(parrentNodeId, childNodeId);
                FillEdge(edge);
                edge.Attr.Id = parrentNodeId + "->" + childNodeId;
                graphViewer.Graph = graph;


                if (CanTren > maxValueTemp)
                {
                    solutionTemp[curNodeIndex] = j;
                    if ((curNodeIndex == (listItems.Count() - 1)) || (V == 0))
                        UpdateMaxValueTemp(totalValue, ref maxValueTemp, solutionTemp, listItems);
                    else
                        RecursiveHideBranchAndBound(curNodeIndex + 1, ref totalValue, ref CanTren, ref V, ref maxValueTemp, solutionTemp, listItems, childNodeId);
                }
                else
                {
                    var ed = graph.AddEdge(childNodeId, "deleteNode");
                    ed.Attr.Id = childNodeId + "->deleteNode";
                    FillEdge(ed);
                    graphViewer.Graph = graph;
                }

                solutionTemp[curNodeIndex] = 0;
                totalValue = totalValue - j * listItems[curNodeIndex].Value;
                V = V + j * listItems[curNodeIndex].Weight;
            }
        }

        private void UpdateMaxValueTemp(double totalValue, ref double maxValueTemp, List<int> solutionTemp, List<Item> listItems)
        {
            if (totalValue > maxValueTemp)
            {
                maxValueTemp = totalValue;
                for (int i = 0; i < listItems.Count(); i++)
                {
                    listItems[i].Amount = solutionTemp[i];
                }
            }
        }

        private void FillNode(Microsoft.Msagl.Drawing.Node node, int index, int amount, double totalValue, int V, double CanTren)
        {
            node.Attr.LineWidth = 0.3;
            node.Label.FontSize = 5;
            node.LabelText = listItem[index].Name + " = " + amount + "\t" + "TGT = " + totalValue + "\n" +
                             "\t" + "W = " + V + "\n" +
                             "\t" + "CT = " + String.Format("{0:0.##}", CanTren);
            node.Label.FontColor = Color.Transparent;
            node.Attr.Color = Color.Transparent;            
        }

        private void FillEdge(Microsoft.Msagl.Drawing.Edge edge)
        {
            edge.Attr.Color = Color.Transparent;
            edge.Attr.LineWidth *= 0.3;
            edge.Attr.ArrowheadLength = 5;
        }

        private void ActionBranchAndBound()
        {
            listItem.ForEach(item => { item.Cost = (double)item.Value / item.Weight; });

            for (int i = 0; i <= listItem.Count - 2; i++)
            {
                for (int j = listItem.Count - 1; j >= i + 1; j--)
                {
                    if (listItem[j].Cost > listItem[j - 1].Cost)
                    {
                        Item temp = listItem[j - 1];
                        listItem[j - 1] = listItem[j];
                        listItem[j] = temp;
                    }
                }
            }

            double totalValue = 0.0;
            int V = maxWeight;
            double CanTren = V * listItem[0].Cost;
            double maxValueTemp = 0.0;
            int curNodeIndex = 0;
            List<int> solutionTemp = new List<int>();
            for (int i = 0; i < listItem.Count(); i++)
                solutionTemp.Add(0);

            RecursiveBranchAndBound(curNodeIndex, ref totalValue, ref CanTren, ref V, ref maxValueTemp, solutionTemp, listItem, "root");

            this.Dispatcher.Invoke(() => { btnStartAndPause.Content = "Bắt Đầu"; btnStartAndPause.IsEnabled = false; });
        }

        private void RecursiveBranchAndBound(int curNodeIndex, ref double totalValue, ref double CanTren, ref int V, ref double maxValueTemp, List<int> solutionTemp, List<Item> listItems, string parrentNodeId)
        {
            int yk = V / listItems[curNodeIndex].Weight;
            if (listItems[curNodeIndex].MaxAmount > 0 && yk > listItems[curNodeIndex].MaxAmount)
                yk = listItems[curNodeIndex].MaxAmount;

            for (int j = yk; j >= 0; j--)
            {
                totalValue = totalValue + j * listItems[curNodeIndex].Value;
                V = V - j * listItems[curNodeIndex].Weight;
                if (curNodeIndex == listItems.Count() - 1)
                    CanTren = totalValue;
                else
                    CanTren = totalValue + V * listItems[curNodeIndex + 1].Cost;

                string childNodeId = parrentNodeId + ":" + listItems[curNodeIndex].Name + "=" + j;
                this.Dispatcher.Invoke(() =>
                {
                    var node = graph.FindNode(childNodeId);
                    node.IsVisible = true;
                    node.Attr.Color = Color.Black;
                    node.Label.FontColor = Color.Black;
                    node.Attr.LineWidth = 1;
                    string edgeId = parrentNodeId + "->" + childNodeId;
                    var edge = graph.EdgeById(edgeId);
                    edge.Attr.Color = Color.Black;
                    edge.Attr.LineWidth = 1;
                    graphViewer.Graph = graph;
                });
                Thread.Sleep(1000);
                mrse.WaitOne();
                this.Dispatcher.Invoke(() =>
                {
                    var node = graph.FindNode(childNodeId);
                    node.Attr.LineWidth = 0.3;
                    string edgeId = parrentNodeId + "->" + childNodeId;
                    var edge = graph.EdgeById(edgeId);
                    edge.Attr.LineWidth = 0.3;
                    graphViewer.Graph = graph;
                });


                if (CanTren > maxValueTemp)
                {
                    solutionTemp[curNodeIndex] = j;
                    if ((curNodeIndex == (listItems.Count() - 1)) || (V == 0))
                        UpdateMaxValueTemp(totalValue, ref maxValueTemp, solutionTemp, listItems);
                    else
                        RecursiveBranchAndBound(curNodeIndex + 1, ref totalValue, ref CanTren, ref V, ref maxValueTemp, solutionTemp, listItems, childNodeId);
                }
                else
                {
                    string edgeId = childNodeId + "->deleteNode";
                    this.Dispatcher.Invoke(() =>
                    {
                        var deleteNode = graph.FindNode("deleteNode");
                        deleteNode.IsVisible = true;
                        deleteNode.Attr.LineWidth = 1;
                        var edge = graph.EdgeById(edgeId);
                        edge.Attr.Color = Color.Black;
                        edge.Attr.LineWidth = 1;
                        graphViewer.Graph = graph;
                    });
                    Thread.Sleep(1000);
                    mrse.WaitOne();
                    this.Dispatcher.Invoke(() =>
                    {
                        var deleteNode = graph.FindNode("deleteNode");
                        deleteNode.Attr.LineWidth = 0.3;
                        var edge = graph.EdgeById(edgeId);
                        edge.Attr.LineWidth = 0.3;
                        graphViewer.Graph = graph;
                    });
                }

                solutionTemp[curNodeIndex] = 0;
                totalValue = totalValue - j * listItems[curNodeIndex].Value;
                V = V + j * listItems[curNodeIndex].Weight;
            }
        }

        private void btnStartAndPause_Click(object sender, RoutedEventArgs e)
        {
            if (isStarted == false)
            {
                if (isNewThread == false)
                {
                    thread = new Thread(ActionBranchAndBound);
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

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            thread.Abort();
            graph = new Graph();
            graphViewer.Graph = graph;
            CreateHideBranchAndBound();
            isNewThread = false;
            isStarted = false;
            mrse.Set();
            btnStartAndPause.Content = "Bắt Đầu";
            btnStartAndPause.IsEnabled = true;
        }
    }
}
