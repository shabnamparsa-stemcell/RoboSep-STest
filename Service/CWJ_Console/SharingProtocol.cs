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
