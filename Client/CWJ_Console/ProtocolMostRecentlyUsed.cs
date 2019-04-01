using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace GUI_Console
{

    public class ProtocolMostRecentlyUsed
    {
        public ProtocolMostRecentlyUsed()
        {
            ClearProtocolMRU();
        }
        
        public ProtocolMostRecentlyUsed(string ProtocolLabel, int QuadrantNumber, double SampleVolumeUl, DateTime usageTime)
        {
            this.ProtocolLabel = ProtocolLabel;
            this.QuadrantNumber = QuadrantNumber;
            this.SampleVolumeUl = SampleVolumeUl;
            this.usageTime = usageTime;
        }

        public void ClearProtocolMRU()
        {
            this.ProtocolLabel = "";
            this.QuadrantNumber = -1;
            this.SampleVolumeUl = 0.0;
            this.usageTime = new DateTime();
        }

        public string ProtocolLabel { get; set; }
        public int QuadrantNumber { get; set; }
        public double SampleVolumeUl { get; set; }
        public DateTime usageTime { get; set; } 

    }

    public class ProtocolMRUList
    {
        ProtocolMostRecentlyUsed[] arrProtocolMRU;
        int MaxProtocolMRU = 4;

        private const int MinQuadrantNumber = 1;
        private const int MaxQuadrantNumber = 4;

        private const string PrefixProtocolLabel = "MruProtocolLabelQ";
        private const string PrefixSampleVolumeUl = "MruSampleVolumeUlQ";
        private const string PrefixUsageTime = "MruUsageTimeQ";
        private const string PrefixProtocol = "Protocol";
        private const string UserInfoFileName = "UserInfo.ini";


        public ProtocolMRUList() 
        {
            arrProtocolMRU = new ProtocolMostRecentlyUsed[MaxProtocolMRU];
            for (int i = 0; i < MaxProtocolMRU; i++)
            {
                arrProtocolMRU[i] = new ProtocolMostRecentlyUsed();
                arrProtocolMRU[i].ClearProtocolMRU();
            }
        }

        private void ClearMRUList()
        {
            for (int i = 0; i < arrProtocolMRU.Length; i++)
            {
                arrProtocolMRU[i].ClearProtocolMRU();
            }
        }

        public bool LoadUserProtocolsMRU(string inUser)
        {
            if (string.IsNullOrEmpty(inUser))
                return false;

            string fullPathUserFileName = RoboSep_UserDB.getInstance().sysPath;

            fullPathUserFileName += "protocols\\";
            fullPathUserFileName += UserInfoFileName;

            if (!File.Exists(fullPathUserFileName))
            {
                return false;
            }

            // read values
            string[] lines = File.ReadAllLines(fullPathUserFileName, System.Text.Encoding.UTF8);
            List<string> lst = new List<string>();
            lst.AddRange(lines);

            ClearMRUList();

            string User = inUser.Trim();
            string tempPrefixLabel = User + PrefixProtocolLabel;
            string tempPrefixSampleVolume = User + PrefixSampleVolumeUl; 
            string tempPrefixUsageTime = User + PrefixUsageTime;
            string temp1, temp2, temp3;
            int index0 = 0, index1 = 0, index2 = 0, index3 = 0;
            string[] parts;
            bool bProtocolNameNotEmpty = false;

            for (int i = 0; i < arrProtocolMRU.Length; i++)
            {
                // Quadrant starts at 1 instead of 0
                index0 = i +1;
                bProtocolNameNotEmpty = false;
                temp1 = tempPrefixLabel + index0.ToString();
                temp2 = tempPrefixSampleVolume + index0.ToString();
                temp3 = tempPrefixUsageTime + index0.ToString();

                index1 = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp1.Length && x.ToLower().Contains(temp1.ToLower())); });
                index2 = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp2.Length && x.ToLower().Contains(temp2.ToLower())); });
                index3 = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp3.Length && x.ToLower().Contains(temp3.ToLower())); });

                arrProtocolMRU[i].QuadrantNumber = i;
                if (0 <= index1)
                {
                    parts = lst[index1].Split('=');
                    if (parts.Length > 1 && parts[0].Trim() == temp1 && parts[0].Trim().Substring(0,2) != @"//")                    
                    {
                        if (!string.IsNullOrEmpty(parts[1]) && parts[1].Trim().Length > 0)
                        {
                            arrProtocolMRU[i].ProtocolLabel = parts[1].Trim();
                            bProtocolNameNotEmpty = true;
                        }
                    }
                }

                if (bProtocolNameNotEmpty)
                {
                    if (0 <= index2)
                    {
                        parts = lst[index2].Split('=');
                        if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]) && parts[1].Trim().Length > 0)
                        {
                            arrProtocolMRU[i].SampleVolumeUl = Convert.ToDouble(parts[1].Trim());
                        }
                    }
                    if (0 <= index3)
                    {
                        parts = lst[index3].Split('=');
                        if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]) && parts[1].Trim().Length > 0)
                        {
                            arrProtocolMRU[i].usageTime = Convert.ToDateTime(parts[1].Trim());
                        }
                    }
                }
            }
            return true;
        }

        public bool UpdateUserProtocolsMRU(string inUser, ProtocolMostRecentlyUsed[] arrProtocols)
        {
            if (string.IsNullOrEmpty(inUser) || arrProtocols == null || arrProtocols.Length == 0)
            {
                return false;
            }
            string fullPathUserFileName = RoboSep_UserDB.getInstance().sysPath;

            fullPathUserFileName += "protocols\\";
            fullPathUserFileName += UserInfoFileName;

            if (!File.Exists(fullPathUserFileName))
            {
                return false;
            }

            // read values
            string[] lines = File.ReadAllLines(fullPathUserFileName, System.Text.Encoding.UTF8);
            List<string> lst = new List<string>();
            lst.AddRange(lines);
            
            string User = inUser.Trim();
            string tempPrefixProtocol = User + PrefixProtocol;
            string tempPrefixLabel = User + PrefixProtocolLabel;
            string tempPrefixSampleVolume = User + PrefixSampleVolumeUl; 
            string tempPrefixUsageTime = User + PrefixUsageTime;
            string temp1, temp2, temp3, temp4;
            int index0 = 0, index1 = 0, index2 = 0, index3 = 0, index4 = 0, index5 = -1;
            string[] parts;
            string[] newParts = new string[2];
            bool bItemUpdated = false;
            bool bDelete = false;

            for (int i = 0; i < arrProtocols.Length; i++)
            {
                if (arrProtocols[i].QuadrantNumber < MinQuadrantNumber || arrProtocols[i].QuadrantNumber > MaxQuadrantNumber)
                    continue;

                index0 = arrProtocols[i].QuadrantNumber;
                temp1 = tempPrefixLabel + index0.ToString();

                // protocol name 
                index1 = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp1.Length && x.ToLower().Contains(temp1.ToLower())); });
                index4 = lst.FindLastIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= tempPrefixProtocol.Length && x.ToLower().Contains(tempPrefixProtocol.ToLower()) && x.Trim().Substring(0, tempPrefixProtocol.Length) == tempPrefixProtocol); });
                
                newParts[0] = temp1;
                newParts[1] = arrProtocols[i].ProtocolLabel;
                temp4 = "";
                if (!string.IsNullOrEmpty(newParts[1].Trim()))
                {
                    temp4 = string.Join(" = ", newParts);
                    bDelete = false;
                }
                else
                {
                    bDelete = true;
                }
 
                bItemUpdated = false;
 
                if (0 <= index1)
                {
                    if (bDelete == true)
                    {
                        lst.RemoveAt(index1);
                        bItemUpdated = true;
                    }
                    else
                    {
                        parts = lst[index1].Split('=');
                        if (0 < parts.Length && parts[0].Trim() == temp1)
                        {
                            // replace 
                            lst[index1] = temp4;
                            bItemUpdated = true;
                        }
                    }
                }
                if (!bItemUpdated && bDelete == false)
                {
                    // add new 
                    index5 = 0 <= index1 ? index1 : 0 <= index5 ? index5 : index4;
                    if (0 <= index5)
                    {
                        lst.Insert(++index5, temp4);
                        index1 = index5;
                    }
                    else
                    {
                        lst.Add(temp4);
                    }
                }

                // sample volume
                temp2 = tempPrefixSampleVolume + index0.ToString();
                index2 = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp2.Length && x.ToLower().Contains(temp2.ToLower())); });
                index4 = lst.FindLastIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= tempPrefixProtocol.Length && x.ToLower().Contains(tempPrefixProtocol.ToLower()) && x.Trim().Substring(0, tempPrefixProtocol.Length) == tempPrefixProtocol); });

                bItemUpdated = false;
                newParts[0] = temp2;
                if (bDelete == false)
                {
                    if (arrProtocols[i].SampleVolumeUl < 0)
                    {
                        newParts[1] = "";
                    }
                    else
                        newParts[1] = arrProtocols[i].SampleVolumeUl.ToString();

                    temp4 = string.Join(" = ", newParts);
                }
                if (0 <= index2)
                {
                    if (bDelete == true)
                    {
                        lst.RemoveAt(index2);
                        bItemUpdated = true;
                    }
                    else
                    {
                        parts = lst[index2].Split('=');
                        if (0 < parts.Length && parts[0].Trim() == temp2)
                        {
                            // replace 
                            lst[index2] = temp4;
                            bItemUpdated = true;
                        }
                    }
                }

                if (!bItemUpdated && bDelete == false)
                {
                    // add new 
                    index5 = 0 <= index2 ? index2 : 0 <= index1 ? index1 : index4;
                    if (0 <= index5)
                    {
                        lst.Insert(++index5, temp4);
                        index2 = index5;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(arrProtocols[i].ProtocolLabel))
                            lst.Add(temp4);
                    }
                }

                // date time
                temp3 = tempPrefixUsageTime + index0.ToString();
                index3 = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp3.Length && x.ToLower().Contains(temp3.ToLower())); });
                index4 = lst.FindLastIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= tempPrefixProtocol.Length && x.ToLower().Contains(tempPrefixProtocol.ToLower()) && x.Trim().Substring(0, tempPrefixProtocol.Length) == tempPrefixProtocol); });
  
                bItemUpdated = false;
                newParts[0] = temp3;

                temp4 = "";
                if (bDelete == false)
                {
                   newParts[1] = arrProtocols[i].usageTime.ToLongDateString();
                   temp4 = string.Join(" = ", newParts);
                }

                if (0 <= index3)
                {
                    if (bDelete == true)
                    {
                        lst.RemoveAt(index3);
                        bItemUpdated = true;
                    }
                    else
                    {
                        parts = lst[index3].Split('=');
                        if (0 < parts.Length && parts[0].Trim() == temp3)
                        {
                            // replace 
                            lst[index3] = temp4;
                            bItemUpdated = true;
                            index5 = index3;
                        }
                    }
                }

                if (!bItemUpdated && bDelete == false)
                {
                    // add new 
                    index5 = 0 <= index3 ? index3 : 0 <= index2 ? index2 : index4;
                    if (0 <= index5)
                    {
                        lst.Insert(++index5, temp4);
                    }
                }
            }

            // write text to file
            File.WriteAllLines(fullPathUserFileName, lst.ToArray());

            return true;
        }

        public bool DeleteMostRecentlyUsedProtocol(string inUser, string inProtocolName)
        {
            if (string.IsNullOrEmpty(inUser) || string.IsNullOrEmpty(inProtocolName) )
            {
                return false;
            }
            string fullPathUserFileName = RoboSep_UserDB.getInstance().sysPath;

            fullPathUserFileName += "protocols\\";
            fullPathUserFileName += UserInfoFileName;

            if (!File.Exists(fullPathUserFileName))
            {
                return false;
            }

            // read values
            string[] lines = File.ReadAllLines(fullPathUserFileName, System.Text.Encoding.UTF8);
            List<string> lst = new List<string>();
            lst.AddRange(lines);
            
            string User = inUser.Trim();
            string ProtocolName = inProtocolName.Trim();

            string tempPrefixLabel = User + PrefixProtocolLabel;               // "MruProtocolLabelQ"
            string tempPrefixSampleVolume = User + PrefixSampleVolumeUl;       // "MruSampleVolumeUlQ"
            string tempPrefixUsageTime = User + PrefixUsageTime;               // "MruUsageTimeQ"
            string temp1, temp2, temp3;
            int index = 0;
            string[] parts;
            for (int i = MinQuadrantNumber; i <= MaxQuadrantNumber; i++)
            {
                // protocol name 
                temp1 = tempPrefixLabel + i.ToString();
                index = lst.FindIndex(0, x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp1.Length && x.ToLower().Contains(temp1.ToLower())); });
                while (0 <= index)
                {
                    if (0 <= index)
                    {
                        parts = lst[index].Split('=');
                        if (0 < parts.Length && parts[0].Trim().Substring(0, 2) != @"//" && parts[0].Trim() == temp1)
                        {
                            if (parts[1].Trim().ToLower() == ProtocolName.ToLower())
                            {
                                lst.RemoveAt(index--);

                                // sample volume
                                temp2 = tempPrefixSampleVolume + i.ToString();
                                RemoveEntry(temp2 , ref lst, ref index);

                                // usage time
                                temp3 = tempPrefixUsageTime + i.ToString();
                                RemoveEntry(temp3, ref lst, ref index);
                            }
                        }
                    }
                    index++;
                    index = lst.FindIndex(index, x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp1.Length && x.ToLower().Contains(temp1.ToLower())); });
                }
            }

            // write text to file
            File.WriteAllLines(fullPathUserFileName, lst.ToArray());
            return true;
        }


        public bool DeleteAllMostRecentlyUsedProtocols(string inUser)
        {
            if (string.IsNullOrEmpty(inUser))
            {
                return false;
            }
            string fullPathUserFileName = RoboSep_UserDB.getInstance().sysPath;

            fullPathUserFileName += "protocols\\";
            fullPathUserFileName += UserInfoFileName;

            if (!File.Exists(fullPathUserFileName))
            {
                return false;
            }

            // read values
            string[] lines = File.ReadAllLines(fullPathUserFileName, System.Text.Encoding.UTF8);
            List<string> lst = new List<string>();
            lst.AddRange(lines);

            string User = inUser.Trim();

            string tempPrefixLabel = User + PrefixProtocolLabel;               // "MruProtocolLabelQ"
            string tempPrefixSampleVolume = User + PrefixSampleVolumeUl;       // "MruSampleVolumeUlQ"
            string tempPrefixUsageTime = User + PrefixUsageTime;               // "MruUsageTimeQ"
            string temp1, temp2, temp3;
            int index = 0;
            string[] parts;
            for (int i = MinQuadrantNumber; i <= MaxQuadrantNumber; i++)
            {
                // protocol name 
                temp1 = tempPrefixLabel + i.ToString();
                index = lst.FindIndex(0, x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp1.Length && x.ToLower().Contains(temp1.ToLower())); });
                while (0 <= index)
                {
                    if (0 <= index)
                    {
                        parts = lst[index].Split('=');
                        if (0 < parts.Length && parts[0].Trim().Substring(0, 2) != @"//" && parts[0].Trim() == temp1)
                        {
                            lst.RemoveAt(index--);

                            // sample volume
                            temp2 = tempPrefixSampleVolume + i.ToString();
                            RemoveEntry(temp2, ref lst, ref index);

                            // usage time
                            temp3 = tempPrefixUsageTime + i.ToString();
                            RemoveEntry(temp3, ref lst, ref index);

                        }
                    }
                    index++;
                    index = lst.FindIndex(index, x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp1.Length && x.ToLower().Contains(temp1.ToLower())); });
                }
            }

            // write text to file
            File.WriteAllLines(fullPathUserFileName, lst.ToArray());
            return true;
        }

        public static bool RenameProtocolsAndMRUs(string inOldUserName, string inNewUserName)
        {
            if (string.IsNullOrEmpty(inOldUserName) || string.IsNullOrEmpty(inNewUserName) )
            {
                return false;
            }
            string fullPathUserFileName = RoboSep_UserDB.getInstance().sysPath;

            fullPathUserFileName += "protocols\\";
            fullPathUserFileName += UserInfoFileName;

            if (!File.Exists(fullPathUserFileName))
            {
                return false;
            }

            // read values
            string[] lines = File.ReadAllLines(fullPathUserFileName, System.Text.Encoding.UTF8);
            List<string> lst = new List<string>();
            lst.AddRange(lines);

            string OldUserName = inOldUserName.Trim();
            string NewUserName = inNewUserName.Trim();

            string tempPrefixProtocol = OldUserName + PrefixProtocol;                 // Protocol
            string tempPrefixLabel = OldUserName + PrefixProtocolLabel;               // "MruProtocolLabelQ"
            string tempPrefixSampleVolume = OldUserName + PrefixSampleVolumeUl;       // "MruSampleVolumeUlQ"
            string tempPrefixUsageTime = OldUserName + PrefixUsageTime;               // "MruUsageTimeQ"
            string temp1, temp2;
            int index = 0;
            // rename Protocols
            do
            {
                index = lst.FindIndex(index, x => { return (!string.IsNullOrEmpty(x) && x.Length >= tempPrefixProtocol.Length && x.ToLower().Contains(tempPrefixProtocol.ToLower())); });
                if (0 <= index)
                {
                    temp1 = NewUserName + PrefixProtocol;

                    lst[index] = lst[index].Replace(tempPrefixProtocol, temp1);
                    index++;
                }

            } while (0 <= index);

            //rename MRUs for all quadrant
            for (int i = MinQuadrantNumber; i <= MaxQuadrantNumber; i++)
            {
                // label 
                index = 0;
                temp1 = tempPrefixLabel + i.ToString();
                index = lst.FindIndex(0, x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp1.Length && x.ToLower().Contains(temp1.ToLower())); });
                if (0 <= index)
                {
                    temp2 = NewUserName + PrefixProtocolLabel + i.ToString(); 
                    lst[index] = lst[index].Replace(temp1, temp2);
                }

                // sample volume
                temp1 = tempPrefixSampleVolume + i.ToString();
                index = lst.FindIndex(0, x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp1.Length && x.ToLower().Contains(temp1.ToLower())); });
                if (0 <= index)
                {
                    temp2 = NewUserName + PrefixSampleVolumeUl + i.ToString();
                    lst[index] = lst[index].Replace(temp1, temp2);
                }

                // time
                temp1 = tempPrefixUsageTime + i.ToString();
                index = lst.FindIndex(0, x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp1.Length && x.ToLower().Contains(temp1.ToLower())); });
                if (0 <= index)
                {
                    temp2 = NewUserName + PrefixUsageTime + i.ToString();
                    lst[index] = lst[index].Replace(temp1, temp2);
                }
            }

            // write text to file
            File.WriteAllLines(fullPathUserFileName, lst.ToArray());
            return true;
        }
 
        private void RemoveEntry(string EntryName, ref List<string> lst, ref int lstIndex)
        {
            if (lst == null || string.IsNullOrEmpty(EntryName))
                return;

            int index = 0;
            string[] parts;

            index = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= EntryName.Length && x.ToLower().Contains(EntryName.ToLower())); });
            if (0 <= index)
            {
                parts = lst[index].Split('=');
                if (0 < parts.Length && parts[0].Trim().Substring(0, 2) != @"//" && parts[0].Trim().ToLower() == EntryName.ToLower())
                {
                    lst.RemoveAt(index);

                    if (index < lstIndex)
                    {
                        // update the line number
                        lstIndex--;
                    }
                }
            }
        }

        public ProtocolMostRecentlyUsed[] ListProtocolMRU
        {
            get { return arrProtocolMRU; }
        }
    }


}
