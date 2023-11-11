using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnapsackProblem
{
    public class Item
    {
        #region Attribute
        private string name;
        private int weight;
        private int value;
        private int amount;
        private double cost;
        private int maxAmount;
        private string stringMaxAmount;
        #endregion

        #region Constructor
        public Item(string name, int weight, int value)
        {
            this.name = name;
            this.weight = weight;
            this.value = value;
            this.amount = 0;
            this.cost = 0;
            this.maxAmount = -1;
            this.stringMaxAmount = "Vô Hạn";
        }

        public Item(string name, int weight, int value, int maxAmount)
        {
            this.name = name;
            this.weight = weight;
            this.value = value;
            this.amount = 0;
            this.cost = 0;
            this.maxAmount = maxAmount;
            if (maxAmount < 0)
                this.stringMaxAmount = "Vô Hạn";
            else
                this.stringMaxAmount = maxAmount.ToString();
        }
        #endregion

        #region GET SET
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Weight
        {
            get { return weight; }
            set { this.weight = value; }
        }

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public int Amount
        {
            get { return amount; }
            set { this.amount = value; }
        }

        public double Cost
        {
            get { return cost; }
            set { this.cost = value; }
        }

        public int MaxAmount
        {
            get { return maxAmount; }
        }

        public string StringMaxAmount
        {
            get { return stringMaxAmount; }
        }
        #endregion

        public Item Clone()
        {
            Item item = new Item(this.Name, this.Weight, this.Value, this.MaxAmount);
            item.Amount = this.Amount;
            item.Cost = this.Cost;
            return item;
        }
    }
}
