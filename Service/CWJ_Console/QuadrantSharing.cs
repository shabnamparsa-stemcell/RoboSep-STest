using System;
using System.Windows.Forms;
using System.Collections;
using Tesla.Common.Separator;
using Tesla.Common.ResourceManagement;
using GUI_Controls;

namespace GUI_Console
{
    /// <summary>
    /// Summary description for QuadrantSharing.
    /// </summary>
    public class QuadrantSharing
    {
        private SharingProtocol[] myAllProtocols;

        private int[] sharedSectorsTranslation = new int[4];
        private int[] sharedSectorsProtocolIndex = new int[4];
        private bool isSharing = false;

        public QuadrantSharing(SharingProtocol[] allProtocols)
        {
            myAllProtocols = allProtocols;

            //cycles through each protocol
            for (int i = 0; i < myAllProtocols.Length; i++)
            {
                SharingProtocol leechProtocol = myAllProtocols[i];
                ProtocolConsumable[,] consumables = leechProtocol.Consumables;

                //cycles through each protocol to find translation
                int translate = 0, current = 0;
                for (int j = 0; j < myAllProtocols.Length; j++)
                {
                    SharingProtocol sharingProtocol = myAllProtocols[j];
                    //find the first quadrant that matches to leech
                    if (sharingProtocol.Name.Equals(leechProtocol.Name))
                    {
                        //Can't share with itself
                        if (i == j)
                            break;
                        translate = sharingProtocol.InitQuadrant + 1;
                        current = leechProtocol.InitQuadrant + 1;
                        //sharingLabel+='\t'+"Q"+(current)+" will be sharing with Q"+translate+'\n';
                        sharingProtocol.addSharingWith(current);
                        break;
                    }
                }

                //Now Fill in current protocol's leeching info
                int quadrants = leechProtocol.Quadrants;
                for (int k = 0; k < quadrants; k++)
                {
                    sharedSectorsTranslation[leechProtocol.InitQuadrant + k] = translate > 0 ? translate + k : 0;
                    sharedSectorsProtocolIndex[leechProtocol.InitQuadrant + k] = i;
                }
            }
        }

        public void queryAllProtocols()
        {
            //query user and adjust info
            DialogResult result = DialogResult.Cancel;
            for (int i = 0; i < myAllProtocols.Length; i++)
            {
                SharingProtocol sharingProtocol = myAllProtocols[i];

                //Init Quadrant Selection state
                result = DialogResult.Cancel;
                QuadrantSelectionDialog.QuadrantSelectionState[] QStates = new QuadrantSelectionDialog.QuadrantSelectionState[4];
                for (int j = 0; j < 4; j++)
                {
                    QStates[j] = QuadrantSelectionDialog.QuadrantSelectionState.DISABLED;
                }
                QStates[sharingProtocol.InitQuadrant] = QuadrantSelectionDialog.QuadrantSelectionState.SELECTED_ALWAYS;
                ArrayList sharingWith = sharingProtocol.SharingWith;

                //if sharing
                if (sharingWith.Count > 0)
                {
                    // ASK USER ABOUT THIS PROTOCOL
                    result = confirmSpecificProtocol(sharingProtocol);

                    //go into quadrant selection if Yes
                    if (result == DialogResult.OK)
                    {
                        //enable relevent quadrants
                        for (int j = 0; j < sharingWith.Count; j++)
                        {
                            QStates[((int)sharingWith[j]) - 1] = QuadrantSelectionDialog.QuadrantSelectionState.ENABLED;
                        }
                        // ASK USER ABOUT SPECIFIC QUADRANT SELECTION
                        if (!querySpecificProtocol(QStates, sharingProtocol))
                        {
                            //false means not successful (i.e. Cancel) so redo loop
                            --i;
                            continue;
                        }
                    }
                    else
                    {
                        //need to remove translation if selected No
                        removeSharingWith(sharingProtocol);
                    }
                }
            }
            printAllQuadrants();
        }

        private DialogResult confirmSpecificProtocol(SharingProtocol sharingProtocol)
        {
            DialogResult result = DialogResult.Cancel;
            //MessageDialog messageDialog;      // old robosep message box
            RoboMessagePanel messageDialog;

            ArrayList sharingWith = sharingProtocol.SharingWith;
            int initQuadrantIndex = sharingProtocol.InitQuadrant;

            //build message
            string quadrantsLabel = "Q" + (initQuadrantIndex + 1);
            for (int i = 0; i < sharingWith.Count; i++)
            {
                if (i >= sharingWith.Count - 1)
                {
                    quadrantsLabel += " and Q" + sharingWith[i];
                }
                else
                {
                    quadrantsLabel += ", Q" + sharingWith[i];
                }
            }

            //ask the user
            //messageDialog = new MessageDialog();
            //result = messageDialog.ShowDialog(
                //(System.Windows.Forms.Form)this.TopLevelControl,
            string sMSG = "You have selected quadrants " + quadrantsLabel + " with the protocol " + sharingProtocol.Name + "." + '\n' + '\n' +
                " Do you wish to use a single reagent kit?";
            messageDialog = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG ,"Reagent Sharing", "Yes", "No");
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            messageDialog.ShowDialog();
            RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            return messageDialog.DialogResult;
        }

        
        private bool querySpecificProtocol(QuadrantSelectionDialog.QuadrantSelectionState[] QStates, SharingProtocol sharer)
        {
            DialogResult result = DialogResult.Retry;
            bool[] QuadrantStatus = null;
            //error checking case
            while (result == DialogResult.Retry)
            {
                QuadrantSelectionDialog dlg = new QuadrantSelectionDialog(QStates[0], QStates[1],
                    QStates[2], QStates[3]);
                dlg.Location = RoboSep_UserConsole.getInstance().Location;
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                result = dlg.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                if (result == DialogResult.OK)
                {
                    QuadrantStatus = dlg.getQuadrantSelectionStatus();
                    //error check here
                    if (!checkValidCombo(QStates, QuadrantStatus))
                    {
                        //error message
                        //new MessageDialog().ShowDialog(
                            //(System.Windows.Forms.Form)this.TopLevelControl,
                        string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgOverload");
                        sMSG = sMSG == null ? "You have selected a combination of sample volumes and/or multiple protocols that exceeds the recommended limit for the reagent vials. " +
                            "If greater than 1.1 mL is loaded into the reagent vials, then there is the potential for reagent overflow during pipetting." : sMSG;
                        
                        RoboMessagePanel errorPrompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG, "Error", "Ok");
                        RoboSep_UserConsole.getInstance().frmOverlay.Show();
                        errorPrompt.ShowDialog();
                        RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                        result = DialogResult.Retry;
                    }
                }
            }


            //redo loop if cancel
            if (result == DialogResult.Cancel)
            {
                return false;
            }
            else
            {
                //Done was pressed 
                //don't set isSharing if nothing is selected
                //remove translation (include 2nd Q...) if not selected

                bool selected = false;

                //for special case all 4 the same
                SharingProtocol nextSharer = null;

                for (int quad = 0; quad < 4; ++quad)
                {
                    //this check makes sure k is initial quadrant of protocol			
                    if (QStates[quad] == QuadrantSelectionDialog.QuadrantSelectionState.ENABLED)
                    {
                        System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++" + quad + " "
                            + QuadrantStatus[quad] + " "
                            + nextSharer + " ");
                        if (QuadrantStatus[quad])
                        {
                            selected = true;
                        }
                        else
                        {
                            //if a quadrant is not selected
                            //do not translate quadrants with the same protocol index
                            removeSharingWithProtocolIndex(sharer, sharedSectorsProtocolIndex[quad]);
                            if (nextSharer == null)
                            {
                                nextSharer = myAllProtocols[sharedSectorsProtocolIndex[quad]];
                            }
                            else
                            {
                                nextSharer.SharingWith.Add((quad + 1));
                                SharingProtocol leechProtocol = myAllProtocols[sharedSectorsProtocolIndex[quad]];

                                //Now Fill in current protocol's leeching info
                                int quadrants = leechProtocol.Quadrants;
                                int translate = nextSharer.InitQuadrant + 1;
                                for (int k = 0; k < quadrants; k++)
                                {
                                    sharedSectorsTranslation[leechProtocol.InitQuadrant + k] = translate + k;
                                }
                            }
                        }
                    }
                }
                //isSharing only if something is selected
                if (selected)
                    isSharing = true;
            }
            return true;
        }
        
        
        private void removeSharingWith(SharingProtocol sharer)
        {
            int quadrantID = sharer.InitQuadrant + 1;
            //go through sharer's quadrants
            for (int i = 0; i < sharer.Quadrants; ++i)
            {
                //make sure no quadrant maps to it
                for (int j = 0; j < 4; ++j)
                {
                    if (sharedSectorsTranslation[j] == quadrantID + i)
                    {
                        sharedSectorsTranslation[j] = 0;
                    }
                }
            }
            //clear sharer
            sharer.clear();
        }

        private void removeSharingWithProtocolIndex(SharingProtocol sharer, int protocolIndex)
        {
            int initQuad = 0;
            //make sure this protocol index doesn't map to anything
            for (int i = 0; i < 4; ++i)
            {
                if (sharedSectorsProtocolIndex[i] == protocolIndex)
                {
                    sharedSectorsTranslation[i] = 0;
                    initQuad = (initQuad == 0) ? i + 1 : initQuad;
                }
            }
            //remove from sharer
            if (initQuad != 0)
            {
                sharer.removeSharingWith(initQuad);
            }
        }

        //print debug info
        private void printAllQuadrants()
        {
            for (int i = 0; i < myAllProtocols.Length; i++)
            {
                SharingProtocol p = myAllProtocols[i];
                for (int j = 0; j < p.Quadrants; j++)
                {
                    if (j == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("............................."
                            + p.Name + " " + sharedSectorsTranslation[p.InitQuadrant + j] + " "
                            + sharedSectorsProtocolIndex[p.InitQuadrant + j]);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("                                         "
                            + sharedSectorsTranslation[p.InitQuadrant + j] + " "
                            + sharedSectorsProtocolIndex[p.InitQuadrant + j]);
                    }
                }
            }
        }



        public int[] SharedSectorsTranslation
        {
            get
            {
                return sharedSectorsTranslation;
            }

        }
        public int[] SharedSectorsProtocolIndex
        {
            get
            {
                return sharedSectorsProtocolIndex;
            }
        }
        public bool IsSharing
        {
            get
            {
                return isSharing;
            }
        }
        private bool checkValidCombo(QuadrantSelectionDialog.QuadrantSelectionState[] QStates, bool[] QuadrantStatus)
        {
            double[] VialAVolume = { 0, 0, 0, 0 };
            double[] VialBVolume = { 0, 0, 0, 0 };
            double[] VialCVolume = { 0, 0, 0, 0 };
            for (int quad = 0; quad < 4; ++quad)
            {
                //this check makes sure k is initial quadrant of protocol			
                if (QuadrantStatus[quad])
                {
                    SharingProtocol p = myAllProtocols[sharedSectorsProtocolIndex[quad]];
                    for (int i = 0; i < p.Quadrants; i++)
                    {
                        double A = p.Consumables[i, (int)RelativeQuadrantLocation.VialA].Volume.Amount;
                        double B = p.Consumables[i, (int)RelativeQuadrantLocation.VialB].Volume.Amount;
                        double C = p.Consumables[i, (int)RelativeQuadrantLocation.VialC].Volume.Amount;
                        VialAVolume[i] += (A > 0) ? (A - 100) : 0;
                        VialBVolume[i] += (B > 0) ? (B - 100) : 0;
                        VialCVolume[i] += (C > 0) ? (C - 100) : 0;
                    }
                }
            }
            for (int quad = 0; quad < 4; ++quad)
            {
                //ignore dead volume
                if (VialAVolume[quad] > 1000 || VialBVolume[quad] > 1000 || VialCVolume[quad] > 1000)
                    return false;
            }
            return true;
        }
    }
}
