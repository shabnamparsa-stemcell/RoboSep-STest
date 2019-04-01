using System;
using System.Collections;
using Tesla.Common.Separator;

namespace GUI_Console
{
    public class SharingProtocol
    {
        private string myName;
        private double myAmount;
        private int myInitQuadrant;
        private ProtocolConsumable[,] myConsumables;
        private ArrayList sharingWith = new ArrayList();
        private int myQuadrants;
        private string[] myCustomNames;


        public SharingProtocol()
        {
        }

        public SharingProtocol(string name, double amount, int initQuadrant, ProtocolConsumable[,] consumables)
        {
            myName = name;
            myAmount = amount;
            myInitQuadrant = initQuadrant;
            myConsumables = consumables;
            myQuadrants = consumables.GetLength(0);
            myCustomNames = new string[8];
        }

        public SharingProtocol(string name, double amount, int initQuadrant, ProtocolConsumable[,] consumables, string[] ConsumablesNames)
        {
            myName = name;
            myAmount = amount;
            myInitQuadrant = initQuadrant;
            myConsumables = consumables;
            myQuadrants = consumables.GetLength(0);
            myCustomNames = ConsumablesNames;
        }

        public SharingProtocol Clone()
        {
            SharingProtocol sp = new SharingProtocol();
            sp.myName = this.myName;
            sp.myName = this.myName;
            sp.myAmount = this.myAmount;
            sp.myInitQuadrant = this.myInitQuadrant;
            sp.myQuadrants = this.myQuadrants;
            sp.myCustomNames = new string[8];
            if (sp.sharingWith.Count >= this.sharingWith.Count)
                sp.sharingWith.AddRange(this.sharingWith);
            if (this.myCustomNames != null && sp.myCustomNames != null && this.myCustomNames.Length <= sp.myCustomNames.Length)
                Array.Copy(this.myCustomNames, sp.myCustomNames, this.myCustomNames.Length);

            if (this.myConsumables != null)
            {
                int n1 = this.myConsumables.GetLength(0);
                int n2 = this.myConsumables.GetLength(1);

                sp.myConsumables = new ProtocolConsumable[n1, n2];
                for (int i = 0; i < sp.myConsumables.GetLength(0); i++)
                {
                    for (int j = 0; j < sp.myConsumables.GetLength(1); j++)
                    {
                        sp.myConsumables[i, j] = new ProtocolConsumable();
                        sp.myConsumables[i, j].AbsoluteQuadrant = this.myConsumables[i, j].AbsoluteQuadrant;
                        sp.myConsumables[i, j].Capacity = this.myConsumables[i, j].Capacity;
                        sp.myConsumables[i, j].IsRequired = this.myConsumables[i, j].IsRequired;
                        sp.myConsumables[i, j].Volume = this.myConsumables[i, j].Volume;
                    }
                }
            }
            return sp;
        }

        public ProtocolConsumable[,] Consumables
        {
            get
            {
                return myConsumables;
            }
        }
        public string Name
        {
            get
            {
                return myName;
            }
        }
        public int InitQuadrant
        {
            get
            {
                return myInitQuadrant;
            }
        }
        public int Quadrants
        {
            get
            {
                return myQuadrants;
            }
        }
        public string[] CustomNames
        {
            get
            {
                return myCustomNames;
            }
        }
        public ArrayList SharingWith
        {
            get
            {
                return sharingWith;
            }
        }
        public void addSharingWith(int quadrantID)
        {
            sharingWith.Add(quadrantID);
        }
        public void clear()
        {
            sharingWith.Clear();
        }
        public void removeSharingWith(int quadrantID)
        {
            sharingWith.Remove(quadrantID);
        }
    }
}
