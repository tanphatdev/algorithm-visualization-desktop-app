using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace KnapsackProblem
{
    class Algorithm
    {
        public List<Item> Greedy(List<Item> listItem, int maxWeight)
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

            for (int i = 0; i < listItem.Count; i++)
            {
                listItem[i].Amount = (int)maxWeight / listItem[i].Weight;
                if (listItem[i].MaxAmount > 0 && listItem[i].Amount > listItem[i].MaxAmount)
                    listItem[i].Amount = listItem[i].MaxAmount;
                maxWeight = maxWeight - listItem[i].Weight * (int)listItem[i].Amount;
            }

            return listItem;
        }

        public List<Item> DynamicPrograming(List<Item> listItem, int maxWeight)
        {
            int[,] BangF = new int[listItem.Count, maxWeight + 1];
            int[,] BangX = new int[listItem.Count, maxWeight + 1];
            int FMax, XMax, yk;
            int v;

            for (v = 0; v < maxWeight + 1; v++)
            {
                BangX[0, v] = v / listItem[0].Weight;
                BangF[0, v] = listItem[0].Value * BangX[0, v];
            }

            for (int k = 1; k < listItem.Count; k++)
            {
                BangX[k, 0] = 0;
                BangF[k, 0] = 0;

                for (v = 1; v < maxWeight + 1; v++)
                {
                    FMax = BangF[k - 1, v];
                    XMax = 0;
                    yk = v / listItem[k].Weight;
                    if (listItem[k].MaxAmount > 0 && yk > listItem[k].MaxAmount)
                        yk = listItem[k].MaxAmount;

                    for (int xk = 1; xk <= yk; xk++)
                    {
                        if (BangF[k - 1, v - xk * listItem[k].Weight] + xk * listItem[k].Value > FMax)
                        {
                            FMax = BangF[k - 1, v - xk * listItem[k].Weight] + xk * listItem[k].Value;
                            XMax = xk;
                        }
                    }

                    BangF[k, v] = FMax;
                    BangX[k, v] = XMax;
                }
            }

            v = maxWeight;
            for (int k = listItem.Count - 1; k >= 0; k--)
            {
                if (BangX[k, v] > 0)
                {
                    listItem[k].Amount = BangX[k, v];
                    v -= listItem[k].Weight * listItem[k].Amount;
                } else
                    listItem[k].Amount = 0;
            }

            return listItem;
        }

        public List<Item> BranchAndBound(List<Item> listItem, int maxWeight)
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
            {
                solutionTemp.Add(0);
            }

            RecursiveBranchAndBound(curNodeIndex, ref totalValue, ref CanTren, ref V, ref maxValueTemp, solutionTemp, listItem);

            return listItem;
        }

        private void RecursiveBranchAndBound(int curNodeIndex, ref double totalValue, ref double CanTren, ref int V, ref double maxValueTemp, List<int> solutionTemp, List<Item> listItems)
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


                if (CanTren > maxValueTemp)
                {
                    solutionTemp[curNodeIndex] = j;
                    if ((curNodeIndex == (listItems.Count() - 1)) || (V == 0))
                        UpdateMaxValueTemp(totalValue, ref maxValueTemp, solutionTemp, listItems);
                    else
                        RecursiveBranchAndBound(curNodeIndex + 1, ref totalValue, ref CanTren, ref V, ref maxValueTemp, solutionTemp, listItems);
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
    }
}
