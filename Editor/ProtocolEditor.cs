//----------------------------------------------------------------------------
// FrmProtocolEditor
//
// Invetech Pty Ltd
// Victoria, 3149, Australia
// Phone:   (+61) 3 9211 7700
// Fax:     (+61) 3 9211 7701
//
// The copyright to the computer program(s) herein is the property of 
// Invetech Pty Ltd, Australia.
// The program(s) may be used and/or copied only with the written permission 
// of Invetech Pty Ltd or in accordance with the terms and conditions 
// stipulated in the agreement/contract under which the program(s)
// have been supplied.
// 
// Copyright © 2004. All Rights Reserved.
//
//==============================================================================
// Upgrades:
//
// October 2005 - Print preview removed at customer's request due to numerous bugs.
// To re-add Print Preview, search for "PrintPreview" and uncomment all selections. -IAT
// 
//		03/24/06 - Uniform Multi Sample - RL
//
//----------------------------------------------------------------------------
//
// 2011-09-08 to 2011-09-27 sp various changes
//     - provide support for use in smaller screen displays (support for scrollbar in other files)
//     - align and resize panels for more unify displays  
//     - compatibility issues for upgrading and operating in .NET 2.0 environement
//     - provide support for different volume error checking dialog
//     - implement own overwrite prompt handler as the one supplied hangs in 64 bit mode 
//
//----------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
//using System.Collections;
//using System.ComponentModel;
using System.Windows.Forms;
//using System.Data;
using System.Configuration;
using System.Security.Permissions;  // bdr

using System.Xml;
using System.Xml.Schema;

using Tesla.ProtocolEditorModel;

using System.Runtime.InteropServices;	// Win32 API by CWJ
using Microsoft.Win32;					// Win32 API by CWJ
using System.Threading;					// CWJ 
using TemplateFileDialog;				// CWJ 	
using ProtocolVersionManager;
using Tesla.Common.ResourceManagement;
using System.Collections.Generic;
using Tesla.Common;			// CWJ	

[assembly: PermissionSet(SecurityAction.RequestMinimum, Name = "FullTrust")]
namespace Tesla.ProtocolEditor
{
    /*
    public class OpenNameFileDialog : OpenFileDialog 
    {
        public virtual string Name 
        {
          get 
          { 
            return Path.GetFullPath(FileName); // Normalize and return the clicked file name 
          } 
        }
			
        // Open a read-only filestream from user-selected file name
        // first demand a specific FileDialogPermission to open the file
        // then assert a stronger, general-purpose FileIOPermission  
        [FileDialogPermission(SecurityAction.Demand,Open=true)]
        [FileIOPermission(SecurityAction.Assert,Unrestricted=true)]
        public new Stream OpenFile()
        {
          return new FileStream(
                      Name,
                      FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
    */


    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------
    // ----------------------------------------------------------------------------



    /// <summary>
    /// Summary description for FrmProtocolEditor.
    /// </summary>
    public class FrmProtocolEditor : System.Windows.Forms.Form
    {
        private static ProtocolModel theProtocolModel;
        private FrmProtocolDetails myFrmProtocolDetails = new FrmProtocolDetails();

        private System.Windows.Forms.MainMenu aMainMenu;
        private System.Windows.Forms.MenuItem mniFile;
        private System.Windows.Forms.MenuItem mniFileSave;
        private System.Windows.Forms.MenuItem mniFileExit;
        private System.Windows.Forms.MenuItem mniFileDivider1;
        private System.Windows.Forms.MenuItem mniFileOpen;

        private System.Windows.Forms.MenuItem mniFilePrint;
        //		private System.Windows.Forms.MenuItem mniFilePrintPreview;
        private bool boCanPrintSavedProtocol;

        private OpenFileDialog myOpenFileDialog;
        private SaveFileDialog mySaveFileDialog;
        private System.Windows.Forms.MenuItem mniFileSaveAs;

        private PrintDocument printDoc = new PrintDocument();
        private ProtocolPrintingUtils printingUtils;
        private System.Windows.Forms.MenuItem mniOptions;
        private System.Windows.Forms.MenuItem mniOptionsEnable;
        private System.Windows.Forms.MenuItem mniOptionsCustomize;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem mniAbout;
        private System.Windows.Forms.MenuItem mniAboutThisProtocol;
        private System.Windows.Forms.MenuItem mniOptionsMultiSamples;
        private System.Windows.Forms.MenuItem mniOptionAutofill;

        private string ENABLE_AUTOFILL_LABEL = "&Enable Description AutoFill";
        private string DISABLE_AUTOFILL_LABEL = "&Disable Description AutoFill";
        private System.Windows.Forms.MenuItem mniOptionsResultSelection;

        private bool isThisTemplate = false;
        public VersionManager VerMgr;
        private System.Windows.Forms.MenuItem mniNew;

        private int DisableVersionUpdate = 0;
        private bool isHold = false;
        private System.ComponentModel.IContainer components;
        private MenuItem mniOptionsReagentBarcodes;
        private MenuItem mniConvertTo;
        private MenuItem mniOptionsFeatureSwitch;					//CWJ

        //int[] defaultEndResult = new int[2];                                    // 2011-11-09 sp -- add default location for end-result if not specified

       
        #region Construction/destruction

        public FrmProtocolEditor()
        {
            boCanPrintSavedProtocol = false;

            /*Customize printDoc*/
            printingUtils = new ProtocolPrintingUtils();
            printDoc.DefaultPageSettings.Landscape = true;
            printDoc.PrinterSettings.DefaultPageSettings.Landscape = true;
            printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // 2011-09-16 sp
            // resize window if the screen resolution is smaller than the specify dialog size
            Rectangle screenArea = System.Windows.Forms.Screen.PrimaryScreen.Bounds;      // get size of display
 //           MessageBox.Show("Screen area: " + screenArea.Width + " x " + screenArea.Height + " ; Panel size:" +
 //               this.Size.Width + " x " + this.Size.Height);
            // reset panel to fit within the screen height with some padding
            // No adjustments are made to the display width as horizontal scrolling is not provided
            int padding = 40;
            this.SetBounds(0, 0, this.Size.Width, Math.Min(screenArea.Height - padding, this.Size.Height));

            // Get a local reference to the protocol model. 
            // NOTE: we do this here and not in a static constructor otherwise the 
            // 'model layer' is initialised from the GUI thread and not the main thread.
            // We want the model initialised from the main thread.
            theProtocolModel = ProtocolModel.GetInstance();

            // Initialise the MDI layout and children
            MDI_Layout();

            // Create standard File Open/Save dialogs
            CreateProtocolFileOpenAndSaveDialogs();

            VerMgr = VersionManager.GetInstance();
            VerMgr.StringHostVersion = Application.ProductVersion;
            WM_USER_SHOW = RegisterWindowMessage(keymsgstr);


           }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        // compact debugmsg utility fn	  // bdr
        public void DbgView(string msg)
        { System.Diagnostics.Debug.WriteLine(msg); }


        private void CreateProtocolFileOpenAndSaveDialogs()
        {
            myOpenFileDialog = new OpenFileDialog();
            mySaveFileDialog = new SaveFileDialog();

            string protocolsPath = string.Empty;
            try
            {
                // 2011-09-19
                // replace fixed directory path with the installed path
                protocolsPath = Tesla.Common.Utilities.GetDefaultAppPath() + ConfigurationSettings.AppSettings["ProtocolsPath"];
                //protocolsPath = ConfigurationSettings.AppSettings["ProtocolsPath"];					
            }
            catch
            {
                // protocolsPath = Application.StartupPath;
            }
            finally
            {
                myOpenFileDialog.InitialDirectory = protocolsPath;
                mySaveFileDialog.InitialDirectory = protocolsPath;

                // 2011-09-13 sp : added to allow .NET 2.0 settings to work under Win7 64-bit systems
                myOpenFileDialog.AutoUpgradeEnabled = false;
                mySaveFileDialog.AutoUpgradeEnabled = false;
            }

            myOpenFileDialog.CheckPathExists = true;		// bdr
            myOpenFileDialog.Filter = @"Protocol files (*.xml)|*.xml|All files (*.*)|*.*";
            myOpenFileDialog.FilterIndex = 1;
            myOpenFileDialog.RestoreDirectory = true;

            mySaveFileDialog.CheckPathExists = true;
            mySaveFileDialog.Filter = @"Protocol files (*.xml)|*.xml|All files (*.*)|*.*";
            mySaveFileDialog.FilterIndex = 1;
            mySaveFileDialog.RestoreDirectory = true;
        }


        // replace illegal in file name chars with safe chars
        private string CreateSafeProtocolName(String name)
        {
            string str = name;
            str = str.Replace('\\', '_');
            str = str.Replace('/', '_');
            str = str.Replace(':', '_');
            str = str.Replace('*', '_');
            str = str.Replace('?', '_');
            str = str.Replace('<', '_');
            str = str.Replace('>', '_');
            str = str.Replace('|', '_');

            // no '.' or xmlwriter drops the .xml extension
            str = str.Replace('.', '_');

            // limit name to 212 chars in length to allow for
            // added path characters...max path length < 255
            if (str.Length > 212)
                str = str.Remove(212, str.Length - 212);
            return str;
        }


        #endregion Construction/destruction

        #region Menu event handlers

        private void mniOptionsEnable_Click(object sender, System.EventArgs e)
        {
            if (myFrmProtocolDetails.IsACommand())
            {
                if (mniOptionsEnable.Text == "&Enable \"Extension Time\"")//disable it
                    mniOptionsEnable.Text = "&Disable \"Extension Time\"";
                else//enable it
                    mniOptionsEnable.Text = "&Enable \"Extension Time\"";

                myFrmProtocolDetails.EnableExtensionTime = !myFrmProtocolDetails.EnableExtensionTime;
            }
        }

        private void mniFileExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void mniFileSave_Click(object sender, System.EventArgs e)
        {
            // Note we can only save if there are no validation errors - see  
            // EditModeChangedHandler and SetSaveMenuStateEnabled for details.
            if (myFrmProtocolDetails.WriteBackDetailsFields(DisableVersionUpdate))
            {
                // 2011-11-09 sp
                // if end-result selection has not been defined, save location for the last tube accessed
                //myFrmProtocolDetails.setIfNotCheckedEndResult();
                theProtocolModel.UpdateForReagentCustomVialUse();

                // Save the protocol changes, overwriting the existing protocol file
                if (theProtocolModel.SaveProtocolFile())           // 2012-03-02 sp -- added error checking
                {
                    mniFilePrint.Enabled = true;
                    boCanPrintSavedProtocol = true;

                    VerMgr.SetFileVersion(theProtocolModel.ProtocolFile);		//CWJ ADD
                    this.DisableVersionUpdate = 0;								//CWJ ADD	
                    this.Text = getTitleWithVersion();// "RoboSep Protocol Editor";						//CWJ ADD
                }
            }
        }

        private Dictionary<ProtocolFormat, string> GetAvailConversionList()
        {
            Dictionary<ProtocolFormat, string> results= new Dictionary<ProtocolFormat,string>();
            foreach (ProtocolFormat type in Enum.GetValues(typeof(ProtocolFormat)))
            {
                if (theProtocolModel.IsConversionAvailable(type))
                {
                    results.Add(type, SeparatorResourceManager.ProtocolFormatToUserDisplayString(type));
                }
            }
            return results;
        }

        private void mniConvertTo_Click(object sender, EventArgs e)
        {
            Dictionary<ProtocolFormat, string> conversions = GetAvailConversionList();
            if (conversions.Count <= 0)
            {
                MessageBox.Show("No conversion available for " + SeparatorResourceManager.ProtocolFormatToUserDisplayString(theProtocolModel.ProtocolFormat), "Protocol Convert");
                    
                return;
            }

            List<ProtocolFormat> types = new List<ProtocolFormat>(conversions.Keys);
            List<string> typeNames = new List<string>(conversions.Values);
            using (ListSelect ts = new ListSelect(typeNames,"Please Select Protocol Format","Convert"))
            {
                if (ts.ShowDialog() == DialogResult.OK)
                {
                    ProtocolFormat type = (types[ts.GetSelectedIndex()]);
                    if (!theProtocolModel.IsConversionAvailable(type))
                    {
                        MessageBox.Show("No conversion available for " + theProtocolModel.ProtocolFormat + " -> " + type, "Protocol Convert");
                    }
                    else
                    {
                        string newFilename = FileSaveAsHelper(true, type); 
                        if (newFilename != "")
                        {
                            //do an open to refresh values
                            OpenFilename(newFilename, null);
                        }
                    }
                }
            }


        }

        private void mniFileSaveAs_Click(object sender, System.EventArgs e)
        {
            FileSaveAsHelper(false,ProtocolFormat.RoboSepS_1_0);
        }

        //private string FileSaveAsHelper(bool boConvertToRs16)
        private string FileSaveAsHelper(bool boConvert, ProtocolFormat convertToType)
        {
            // Note we can only save if there are no validation errors - see 
            // EditModeChangedHandler and SetSaveMenuStateEnabled for details.

            // Display a standard "file save" dialog to allow the user to choose
            // a location/name to save the protocol....limit to legal xml chars and
            // no more than 212 chars in length
            string fname = myFrmProtocolDetails.getProtocolLabel();
            if (SeparatorResourceManager.isPlatformRS16() || (boConvert && SeparatorResourceManager.isPlatformRS16(convertToType)))
            {
                fname = "RS16-"+fname;
            }
            mySaveFileDialog.FileName = CreateSafeProtocolName(fname);
            //DbgView("safe file name " + mySaveFileDialog.FileName); 

            // 2011-09-27 sp
            // disable the overwrite prompt handler as this hangs on 64bit implementation
            mySaveFileDialog.OverwritePrompt = false;

            if (mySaveFileDialog.ShowDialog() == DialogResult.OK &&
                myFrmProtocolDetails.WriteBackDetailsFields(DisableVersionUpdate))
            {
                bool saveFileAllowed = true;
                // 2011-09-27 sp
                // implement own overwrite prompt handler 
                if (File.Exists(mySaveFileDialog.FileName))
                    if (MessageBox.Show("File already exists. Do you want to overwrite it?", "File Exists", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        saveFileAllowed = false;
                if (!mySaveFileDialog.FileName.EndsWith(".xml"))
                {
                    mySaveFileDialog.FileName += ".xml";
                }
                if (saveFileAllowed)
                {
                    // 2011-11-09 sp
                    // if end-result selection has not been defined, save location for the last tube accessed
                    //myFrmProtocolDetails.setIfNotCheckedEndResult();
                    theProtocolModel.UpdateForReagentCustomVialUse();

                    theProtocolModel.SaveAsProtocolFile(mySaveFileDialog.FileName, boConvert, convertToType,true);
                    myFrmProtocolDetails.IsNewProtocol = false;
                    SetSaveMenuStateEnabled(true);
                    mniFilePrint.Enabled = true;
                    boCanPrintSavedProtocol = true;
                    VerMgr.SetFileVersion(theProtocolModel.ProtocolFile);		//CWJ ADD
                    this.DisableVersionUpdate = 0;								//CWJ ADD	
                    this.Text = getTitleWithVersion();//"RoboSep Protocol Editor";						//CWJ ADD
                    return mySaveFileDialog.FileName;
                }

            }
            return "";
        }

        private void OpenFileHelper(string templateFolder)
        {
            OpenFilename("", templateFolder);
        }

        private void OpenFilename(string filename, string templateFolder)
        {
            DialogResult result = DialogResult.Cancel;
            String openFilename = "";
            int iDisableVersionUpdate = 0;
            bool boOpenTemplate = templateFolder != null;

            mniOptionsEnable.Enabled = true;
            mniOptionsCustomize.Enabled = true;
            mniOptionsReagentBarcodes.Enabled = false;
            mniFilePrint.Enabled = true;
            boCanPrintSavedProtocol = true;
            mniOptionsMultiSamples.Enabled = true;
            mniOptionsResultSelection.Enabled = true;

            // Whatever the outcome of the 'open' cancel 'new' mode if it's active
            myFrmProtocolDetails.IsNewProtocol = false;

            if (!boOpenTemplate)
            {
                if (filename == "")
                {
                    //reg open dialog
                    result = myOpenFileDialog.ShowDialog();
                    openFilename = myOpenFileDialog.FileName;
                }
                else
                {
                    result = DialogResult.OK;
                    openFilename = filename;
                }
            }
            else
            {
                //template dialog
                TemplateDialog TempDlg = new TemplateDialog();
                TempDlg.DirName = templateFolder;
                result = TempDlg.ShowDialog();
                openFilename = TempDlg.TemplateName;
                iDisableVersionUpdate = TempDlg.DisableAutoVersionUpdater;
            }


            // Display a standard "file open" dialog to allow the user to choose
            // an existing protocol to view/edit.			
            if (result == DialogResult.OK)
            {
                InitVolCheck();							//CWJ Add
                // Clear any previous validation errors
                myFrmProtocolValidation.ClearValidationErrors();

                DbgView("Editor - open file: " + openFilename);

                if (!VerMgr.CheckFileVersion(openFilename))			//CWJ ADD
                {																	//CWJ ADD
                    DbgView("!!! -- Doesn't Match Protocol Structure Version!!!");
                    MessageBox.Show("Doesn't match the protocol structure version!!! Please, install new Robosep Client!", "Protocol Error!");
                    //					return;															//CWJ ADD
                }																	//CWJ ADD

                // Open the requested file
                this.Cursor = Cursors.WaitCursor;

                // the file open failed - disable Save/SaveAs menu items
                if (!theProtocolModel.OpenProtocolFile(openFilename))
                {
                    DbgView("!!! -- XML file open failed!!!");
                    SetSaveMenuStateEnabled(false);
                }
                else
                {
                    mniFilePrint.Enabled = true;
                    boCanPrintSavedProtocol = true;
                }
                // disable changes to type for existing protocols
                myFrmProtocolDetails.EnableProtocolType(boOpenTemplate); //Change for enabling protocol type in template open //CWJ ADD
                isThisTemplate = boOpenTemplate;										//CWJ ADD
                if (boOpenTemplate)
                {
                    this.DisableVersionUpdate = iDisableVersionUpdate;			//CWJ ADD

                    if (this.DisableVersionUpdate == 1)										//CWJ ADD
                    {
                        this.Text = getTitleWithVersion()+" - Version Updater Disabled!";	//CWJ ADD
                    }
                    else
                    {
                        this.Text = getTitleWithVersion();//"RoboSep Protocol Editor";								//CWJ ADD					
                    }
                }
                else
                {
                    this.DisableVersionUpdate = 0;								//CWJ ADD	
                    this.Text = getTitleWithVersion();//"RoboSep Protocol Editor";						//CWJ ADD
                }

            }
        }

        private string getTitleWithVersion()
        {
            return "RoboSep Protocol Editor Ver: " +
                   VerMgr.ThisVersions[0].ToString() + "." +
                   VerMgr.ThisVersions[1].ToString() + "." +
                   VerMgr.ThisVersions[2].ToString() + "." +
                   // "x"+ "." +
                   VerMgr.ThisVersions[3].ToString();
        }

        // demand some minimal UI permission to display the dialog
        //		[UIPermission(SecurityAction.Demand,Window=UIPermissionWindow.SafeSubWindows)]
        private void mniFileOpen_Click(object sender, System.EventArgs e)
        {
            OpenFileHelper(null);
        }

        private void mniFilePrint_Click(object sender, System.EventArgs e)
        {
            if (!boCanPrintSavedProtocol)
            {
                MessageBox.Show("Please save the protocol before printing.", 
                    "Protocol Print");
                return;
            }


            /*Bring up the print setup dialog*/
            PrintDialog dlg = new PrintDialog();
            dlg.Document = printDoc;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                printingUtils.pageNumber = 0;
                printDoc.Print();
            }
        }

        //      private void mniFilePrintPreview_Click(Object sender , EventArgs e) 
        //				{
        //					printingUtils.pageNumber = 0;
        //
        //					/*The print preview window*/
        //					PrintPreviewDialog dlg = new PrintPreviewDialog();
        //					dlg.Document = printDoc;
        //					dlg.WindowState = FormWindowState.Maximized;
        //					dlg.AutoScale = false;
        //					dlg.PrintPreviewControl.Zoom = 1.0f;
        //					dlg.Bounds = new Rectangle(250,250,800,700);
        //					dlg.ShowDialog();
        //				}

        private void printDoc_PrintPage(Object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            e.HasMorePages = printingUtils.PrintDocumentPage(g, theProtocolModel, printDoc, e.PageBounds.Height - 60);

        }

        private void SetSaveMenuStateEnabled(bool isEnabled)
        {
            SetSaveMenuStateEnabled(isEnabled, false);
        }
        private void SetSaveMenuStateEnabled(bool isEnabled, bool didLabelChanged)
        {
            mniFileSave.Enabled = isEnabled && (!myFrmProtocolDetails.IsNewProtocol) && theProtocolModel.ValidProtocol;
            mniFileSaveAs.Enabled = isEnabled && theProtocolModel.ValidProtocol;
            mniConvertTo.Enabled = isEnabled && theProtocolModel.ValidProtocol;

            if (isThisTemplate || didLabelChanged)
            {
                mniFileSave.Enabled = false;
            }

            if (!isEnabled)
            {
                mniFilePrint.Enabled = true; // false;
                boCanPrintSavedProtocol = false;
            }
        }

        #endregion Menu event handlers

        #region MDI Behaviour

        private enum MDI_Child
        {
            ProtocolValidation = 0,
            ProtocolDetails,
            NUM_MDI_CHILDREN
        }

        private Rectangle myChildFormBounds;
        private System.Windows.Forms.Form[] myChildForms;
        private MDI_Child myCurrentMdiChild;

        //private FrmProtocolDetails		myFrmProtocolDetails = new FrmProtocolDetails();
        private readonly FrmProtocolValidation myFrmProtocolValidation = new FrmProtocolValidation();

        public void ChangePage(DialogResult currentPageResultCode)
        {
            MDI_Child nextPage = MDI_Child.ProtocolDetails;

            switch (currentPageResultCode)
            {
                case DialogResult.OK:
                    nextPage = (MDI_Child)
                        (((int)myCurrentMdiChild + 1) % (int)MDI_Child.NUM_MDI_CHILDREN);
                    break;
            }
            ShowChildForm(nextPage);
        }

        private void ShowChildForm(MDI_Child childForm)
        {
            lock (this)
            {
                // Record the specified child as the 'current' child
                myCurrentMdiChild = childForm;

                // Trigger the MDI activation (the event will be caught by our handler)
                myChildForms[(int)childForm].Activate();

                // Show the specified child form
                myChildForms[(int)childForm].BringToFront();
                myChildForms[(int)childForm].Show();
            }
        }

        private void MDI_Layout()
        {
            myChildFormBounds = this.ChildFormBounds;
            myChildForms = new System.Windows.Forms.Form[(int)MDI_Child.NUM_MDI_CHILDREN];
            myChildForms[(int)MDI_Child.ProtocolDetails] = myFrmProtocolDetails;
            myChildForms[(int)MDI_Child.ProtocolValidation] = myFrmProtocolValidation;

            for (int i = 0; i < (int)MDI_Child.NUM_MDI_CHILDREN; ++i)
            {
                myChildForms[i].MdiParent = this;
                myChildForms[i].Bounds = myChildFormBounds;
            }
        }

        private Rectangle ChildFormBounds
        {
            get
            {
                // Return the bounding rectangle for the client area, less enough
                // of a gap to ensure that scroll bars are not triggered.
                Rectangle r = this.ClientRectangle;
                r.Inflate(-2, -2);
                r.X = 0;
                r.Y = 0;
                return r;
            }
        }

        private void FrmProtocolEditor_Resize(object sender, System.EventArgs e)
        {
            MDI_Layout();
        }

        /// <summary>
        /// Handler for MDI Child form activation.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// This handler allows us to trigger updates in the child form before it is displayed.
        /// </remarks>
        private void FrmProtocolEditor_MdiChildActivate(object sender, System.EventArgs e)
        {
            // Prevent normal MDI parent/child behaviour kicking in and changing the location!
            myChildForms[(int)myCurrentMdiChild].Bounds = myChildFormBounds;

            switch (myCurrentMdiChild)
            {
                default:
                case MDI_Child.ProtocolValidation:
                case MDI_Child.ProtocolDetails:
                    break;
            }
        }

        #endregion MDI Behaviour

        #region Form events (non-MDI related)

        private void FrmProtocolEditor_Load(object sender, System.EventArgs e)
        {
            // Register for model events
            RegisterForModelEvents();

            // Default to showing the 'details' page
            myCurrentMdiChild = MDI_Child.ProtocolDetails;
            myFrmProtocolDetails.Show();


            this.Text = getTitleWithVersion();//"RoboSep Protocol Editor";	
        }

        // called from FrmProtocolEditor_Load() above
        private void RegisterForModelEvents()
        {
            theProtocolModel.ReportValidationException += new ValidationEventHandler(ValidationExceptionHandler);
            theProtocolModel.ReportDataAvailable += new EventHandler(ProtocolModelDataAvailableHandler);
            myFrmProtocolDetails.ReportEditMode += new Tesla.ProtocolEditor.FrmProtocolDetails.EditModeChangedDelegate(EditModeChangedHandler);
            theProtocolModel.ReportDataVolumeCheck += new EventHandler(ProtocolModelVolumeCheckHandler);
        }

        #endregion Form events (non-MDI related)

        #region Protocol Model event handlers

#if false
        private int myValidationErrorCount;


        public void ValidationHandler(object sender, ValidationEventArgs args)
        {
            ++myValidationErrorCount;
        }
#endif

        public void ValidationExceptionHandler(object sender, ValidationEventArgs args)
        {
            if (myFrmProtocolValidation.InvokeRequired)
            {
                ValidationEventHandler eh = new ValidationEventHandler(this.ValidationExceptionHandler);
                this.Invoke(eh, new object[] { sender, args });
            }
            else
            {
                // Append the error to the validation errors for this protocol.
                myFrmProtocolValidation.DisplayValidationError(args.Message);

                // If not already showing the protocol validation page, change to it
                this.Cursor = Cursors.Default;
                if (myCurrentMdiChild != MDI_Child.ProtocolValidation)
                {
                    ShowChildForm(MDI_Child.ProtocolValidation);
                }
            }
        }

        // called from RegisterForModelEvents()
        private void ProtocolModelDataAvailableHandler(object sender, EventArgs e)
        {
            if (myFrmProtocolDetails.InvokeRequired)
            {
                EventHandler eh = new EventHandler(this.ProtocolModelDataAvailableHandler);
                this.Invoke(eh, new object[] { sender, e });
            }
            else
            {
                // If not already showing the protocol details page, change to it
                this.Cursor = Cursors.Default;
                if (myCurrentMdiChild != MDI_Child.ProtocolDetails)
                {
                    // The file open succeeded -- so enable the 'save' menu and
                    // switch to the 'details' form to display the protocol information.
                    SetSaveMenuStateEnabled(true);
                    mniFilePrint.Enabled = true;
                    boCanPrintSavedProtocol = true;
                    ShowChildForm(MDI_Child.ProtocolDetails);
                }

                // Load the new data into the protocol details page - if in manual descriptor
                // fill mode set autofill menu item to say 'Enable Auto' (its next option) else
                // set menu item to say 'Disable Auto' as next option
                myFrmProtocolDetails.PopulateDetailsFields();
                mniOptionAutofill.Enabled = true;
                if (theProtocolModel.ProtocolDescManualFill)
                    mniOptionAutofill.Text = ENABLE_AUTOFILL_LABEL;
                else mniOptionAutofill.Text = DISABLE_AUTOFILL_LABEL;

                ThingsToDoWhenProtocolFormatChanges();

                //make sure Taylor's vial barcode is initialized
                //theProtocolModel.InitVialBarcodes();
                //theProtocolModel.UpdateForReagentCustomVialUse();

                // Now enable data modification
                myFrmProtocolDetails.Enabled = true;


                mniFilePrint.Enabled = true;
                boCanPrintSavedProtocol = true;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int PostMessage(int hwnd, uint wMsg, uint wParam, uint lParam);	//CWJ

        private void ProtocolModelVolumeCheckHandler(object sender, EventArgs e)		//CWJ	
        {																				//CWJ
            if (myFrmProtocolDetails.InvokeRequired)
            {
                EventHandler eh = new EventHandler(this.ProtocolModelVolumeCheckHandler);
                this.Invoke(eh, new object[] { sender, e });
            }
            else
            {
                if (isThisTemplate)
                {
                    // 2011-09-12 sp
                    // rename from CheckProtocolVolumes to more generic name
                    myFrmProtocolDetails.CheckProtocolCommands(false);
                    myFrmProtocolDetails.SetValidCheckMark(false);
                    SetSaveMenuStateEnabled(false);
                    myFrmProtocolDetails.txtProtocolLabel.Text = "";
                }
                else
                {
                    // 2011-09-12 sp
                    // rename from CheckProtocolVolumes to more generic name
                    bool result = myFrmProtocolDetails.CheckProtocolCommands(false);
                    myFrmProtocolDetails.SetValidCheckMark(result);
                    SetSaveMenuStateEnabled(result);
                }
            }
        }																				//CWJ			

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]			//CWJ 
        static extern uint RegisterWindowMessage(string lpString);						//CWJ 
        private string keymsgstr = "KEYMSGIPC";											//CWJ 
        private uint WM_USER_SHOW;														//CWJ 	

        protected override void WndProc(ref Message m)									//CWJ 
        {
            if (m.Msg == WM_USER_SHOW)
            {
                if (this.isHold == false)
                {
                    this.isHold = true;

                    this.WindowState = FormWindowState.Minimized;
                    this.WindowState = FormWindowState.Normal;
                    this.BringToFront();
                    this.Activate();
                    MessageBox.Show(this, "Protocol Editor is already running!");

                    this.isHold = false;
                }
            }

            base.WndProc(ref m);
        }																				//CWJ 

        private void EditModeChangedHandler(bool isEditMode)
        {
            string currentLabel = myFrmProtocolDetails.GetLabel();
            string fileLabel = theProtocolModel.ProtocolLabel;
            //SetSaveMenuStateEnabled(!isEditMode);
            SetSaveMenuStateEnabled(!isEditMode, fileLabel != currentLabel);
        }

        #endregion Protocol Model event handlers

        // 2011-09-08 to 2011-09-16 sp various changes
        //     - provide support for use in smaller screen displays (support for scrollbar in other files)
        //     - align and resize panels for more unify displays  
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmProtocolEditor));
            this.aMainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.mniFile = new System.Windows.Forms.MenuItem();
            this.mniNew = new System.Windows.Forms.MenuItem();
            this.mniFileOpen = new System.Windows.Forms.MenuItem();
            this.mniFileSave = new System.Windows.Forms.MenuItem();
            this.mniFileSaveAs = new System.Windows.Forms.MenuItem();
            this.mniConvertTo = new System.Windows.Forms.MenuItem();
            this.mniFilePrint = new System.Windows.Forms.MenuItem();
            this.mniFileDivider1 = new System.Windows.Forms.MenuItem();
            this.mniFileExit = new System.Windows.Forms.MenuItem();
            this.mniOptions = new System.Windows.Forms.MenuItem();
            this.mniOptionsEnable = new System.Windows.Forms.MenuItem();
            this.mniOptionsCustomize = new System.Windows.Forms.MenuItem();
            this.mniOptionsMultiSamples = new System.Windows.Forms.MenuItem();
            this.mniOptionAutofill = new System.Windows.Forms.MenuItem();
            this.mniOptionsResultSelection = new System.Windows.Forms.MenuItem();
            this.mniOptionsReagentBarcodes = new System.Windows.Forms.MenuItem();
            this.mniOptionsFeatureSwitch = new System.Windows.Forms.MenuItem();
            this.mniAbout = new System.Windows.Forms.MenuItem();
            this.mniAboutThisProtocol = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // aMainMenu
            // 
            this.aMainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mniFile,
            this.mniOptions,
            this.mniAbout,
            this.menuItem3});
            // 
            // mniFile
            // 
            this.mniFile.Index = 0;
            this.mniFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mniNew,
            this.mniFileOpen,
            this.mniFileSave,
            this.mniFileSaveAs,
            this.mniConvertTo,
            this.mniFilePrint,
            this.mniFileDivider1,
            this.mniFileExit});
            this.mniFile.Text = "&File";
            this.mniFile.Click += new System.EventHandler(this.mniFileOpen_Click);
            // 
            // mniNew
            // 
            this.mniNew.Index = 0;
            this.mniNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.mniNew.Text = "&New";
            this.mniNew.Click += new System.EventHandler(this.mniNew_Click);
            // 
            // mniFileOpen
            // 
            this.mniFileOpen.Index = 1;
            this.mniFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.mniFileOpen.Text = "&Open";
            this.mniFileOpen.Click += new System.EventHandler(this.mniFileOpen_Click);
            // 
            // mniFileSave
            // 
            this.mniFileSave.Enabled = false;
            this.mniFileSave.Index = 2;
            this.mniFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.mniFileSave.Text = "&Save";
            this.mniFileSave.Click += new System.EventHandler(this.mniFileSave_Click);
            // 
            // mniFileSaveAs
            // 
            this.mniFileSaveAs.Enabled = false;
            this.mniFileSaveAs.Index = 3;
            this.mniFileSaveAs.Text = "Save &As";
            this.mniFileSaveAs.Click += new System.EventHandler(this.mniFileSaveAs_Click);
            // 
            // mniConvertTo
            // 
            this.mniConvertTo.Enabled = false;
            this.mniConvertTo.Index = 4;
            this.mniConvertTo.Text = "&Convert To...";
            this.mniConvertTo.Click += new System.EventHandler(this.mniConvertTo_Click);
            // 
            // mniFilePrint
            // 
            this.mniFilePrint.Enabled = false;
            this.mniFilePrint.Index = 5;
            this.mniFilePrint.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
            this.mniFilePrint.Text = "&Print";
            this.mniFilePrint.Click += new System.EventHandler(this.mniFilePrint_Click);
            // 
            // mniFileDivider1
            // 
            this.mniFileDivider1.Index = 6;
            this.mniFileDivider1.Text = "-";
            // 
            // mniFileExit
            // 
            this.mniFileExit.Index = 7;
            this.mniFileExit.Text = "E&xit";
            this.mniFileExit.Click += new System.EventHandler(this.mniFileExit_Click);
            // 
            // mniOptions
            // 
            this.mniOptions.Index = 1;
            this.mniOptions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mniOptionsEnable,
            this.mniOptionsCustomize,
            this.mniOptionsMultiSamples,
            this.mniOptionAutofill,
            this.mniOptionsResultSelection,
            this.mniOptionsReagentBarcodes,
            this.mniOptionsFeatureSwitch});
            this.mniOptions.Text = "&Options";
            // 
            // mniOptionsEnable
            // 
            this.mniOptionsEnable.Enabled = false;
            this.mniOptionsEnable.Index = 0;
            this.mniOptionsEnable.Text = "&Enable \"Extension Time\"";
            this.mniOptionsEnable.Click += new System.EventHandler(this.mniOptionsEnable_Click);
            // 
            // mniOptionsCustomize
            // 
            this.mniOptionsCustomize.Enabled = false;
            this.mniOptionsCustomize.Index = 1;
            this.mniOptionsCustomize.Text = "&Customize Vial Names";
            this.mniOptionsCustomize.Click += new System.EventHandler(this.mniOptionsCustomize_Click);
            // 
            // mniOptionsMultiSamples
            // 
            this.mniOptionsMultiSamples.Enabled = false;
            this.mniOptionsMultiSamples.Index = 2;
            this.mniOptionsMultiSamples.Text = "&Multiple Samples Selection";
            this.mniOptionsMultiSamples.Click += new System.EventHandler(this.mniOptionsMultiSamples_Click);
            // 
            // mniOptionAutofill
            // 
            this.mniOptionAutofill.Enabled = false;
            this.mniOptionAutofill.Index = 3;
            this.mniOptionAutofill.Text = "Enable Description AutoFill";
            this.mniOptionAutofill.Click += new System.EventHandler(this.mniOptionAutofill_Click);
            // 
            // mniOptionsResultSelection
            // 
            this.mniOptionsResultSelection.Enabled = false;
            this.mniOptionsResultSelection.Index = 4;
            this.mniOptionsResultSelection.Text = "&Result Vial Selection";
            this.mniOptionsResultSelection.Click += new System.EventHandler(this.mniOptionsResultSelection_Click);
            // 
            // mniOptionsReagentBarcodes
            // 
            this.mniOptionsReagentBarcodes.Enabled = false;
            this.mniOptionsReagentBarcodes.Index = 5;
            this.mniOptionsReagentBarcodes.Text = "Reagent &Barcodes";
            this.mniOptionsReagentBarcodes.Click += new System.EventHandler(this.mniReagentBarcodes_Click);
            // 
            // mniOptionsFeatureSwitch
            // 
            this.mniOptionsFeatureSwitch.Enabled = false;
            this.mniOptionsFeatureSwitch.Index = 6;
            this.mniOptionsFeatureSwitch.Text = "&Feature Switch";
            this.mniOptionsFeatureSwitch.Click += new System.EventHandler(this.mniOptionsFeatureSwitch_Click);
            // 
            // mniAbout
            // 
            this.mniAbout.Index = 2;
            this.mniAbout.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mniAboutThisProtocol});
            this.mniAbout.Text = "A&bout";
            // 
            // mniAboutThisProtocol
            // 
            this.mniAboutThisProtocol.Index = 0;
            this.mniAboutThisProtocol.Text = "About Current &Protocol";
            this.mniAboutThisProtocol.Click += new System.EventHandler(this.mniAboutThisProtocol_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 3;
            this.menuItem3.Text = "";
            // 
            // FrmProtocolEditor
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(546, 680);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MaximumSize = new System.Drawing.Size(554, 1536);
            this.Menu = this.aMainMenu;
            this.MinimumSize = new System.Drawing.Size(554, 440);
            this.Name = "FrmProtocolEditor";
            this.Text = "RoboSep Protocol Editor";
            this.Load += new System.EventHandler(this.FrmProtocolEditor_Load);
            this.MdiChildActivate += new System.EventHandler(this.FrmProtocolEditor_MdiChildActivate);
            this.Resize += new System.EventHandler(this.FrmProtocolEditor_Resize);
            this.ResumeLayout(false);

        }
        #endregion
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        ///
        [DllImport("user32.dll")]				//CWJ 
        static extern int MessageBeep(uint n);	//CWJ 
        const int HWND_BROADCAST = 0xffff;		//CWJ 


        [STAThread]
        static void Main()
        {
            try
            {
                //CWJ Add for blocking multiple instances
                bool bNew;

                Mutex mutex = new Mutex(true, "PE-running", out bNew);

                if (bNew)
                {
                    ProtocolModel nonUiLayers = ProtocolModel.GetInstance();
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                    Application.EnableVisualStyles();
                    try
                    {
                        Application.Run(new FrmProtocolEditor());
                    }
                    catch (MissingConfigFileException e)
                    {
                        MessageBox.Show(e.Message,"Configuration File Missing Error");
                    }
                    catch (MissingConfigVariableException e)
                    {
                        MessageBox.Show(e.Message, "Configuration Variable Missing Error");
                    }
                    mutex.ReleaseMutex();
                }
                else
                {
                    string keymsgstr = "KEYMSGIPC";
                    uint WM_USER_SHOW;
                    MessageBeep(0x00);
                    WM_USER_SHOW = RegisterWindowMessage(keymsgstr);
                    PostMessage(HWND_BROADCAST, WM_USER_SHOW, 0, 0);
                }

                // Initialise the non-UI layer(s)
                //				ProtocolModel nonUiLayers = ProtocolModel.GetInstance();

                // Define a "if all else fails" exception handler for the UI application
                //				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                // Run the protocol editor application
                //				Application.Run(new FrmProtocolEditor());
            }
            finally
            {
            }
        }

        // Catch any exceptions unhandled by the Protocol Editor application code
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
        }

        private void mniOptionsCustomize_Click(object sender, System.EventArgs e)
        {
            myFrmProtocolDetails.CreateCustomizeWindow();
        }

        private void mniReagentBarcodes_Click(object sender, EventArgs e)
        {
            myFrmProtocolDetails.CreateReagentBarcodesWindow();
        }

        private void mniAboutThisProtocol_Click(object sender, System.EventArgs e)
        {
            string tmpAuthor = theProtocolModel.ProtocolAuthor;
            string tmpRevision = theProtocolModel.ProtocolVersion;
            myFrmProtocolDetails.pasreProtcolNumbers(); ;
            // 2011-09-21 sp
            AboutThisProtocolWindow AboutProWin = new AboutThisProtocolWindow();
            //AboutBox AboutProWin = new AboutBox();
            AboutProWin.ShowDialog(this);

            if (tmpAuthor != theProtocolModel.ProtocolAuthor || tmpRevision != theProtocolModel.ProtocolVersion)
                myFrmProtocolDetails.WhenCommandDetailChanged();
        }

        // RL - Uniform Multi Sample - 03/24/06
        private void mniOptionsMultiSamples_Click(object sender, System.EventArgs e)
        {
            ProtocolMultipleSamples dlg = new ProtocolMultipleSamples();
            dlg.SetupMultipleSamples();
            dlg.ShowDialog(this);
        }


        // the sense of the menu item labels is not clear...it should show the
        // next optional toggle state i.e.
        //        "&Disable desc autofill" when auto fill is currently enabled
        // and 
        //        "&Enable desc autofill"  when autofill is currently disabled
        // is the selected text ID in mniOptionAutofill.Text variable
        // Then the manual fill flag is toggled opposite to the autofill state

        private void mniOptionAutofill_Click(object sender, System.EventArgs e)
        {
            // if menu shows 'auto-enable' label that means auto is currently OFF
            // so toggle the label to say 'auto disabled' and set ProtocolDescManualFill 
            // to false (meaning auto now ON) and grey out the Description edit box since
            // users can't edit while autofill is ON

            if (mniOptionAutofill.Text == ENABLE_AUTOFILL_LABEL)
            {
                // change to auto
                theProtocolModel.ProtocolDescManualFill = false;
                myFrmProtocolDetails.txtProtocolDesc.Enabled = false;
                mniOptionAutofill.Text = DISABLE_AUTOFILL_LABEL;
            }
            else
            {
                // change to manual
                theProtocolModel.ProtocolDescManualFill = true;
                myFrmProtocolDetails.txtProtocolDesc.Enabled = true;
                mniOptionAutofill.Text = ENABLE_AUTOFILL_LABEL;
            }
        }

        private void mniOptionsResultSelection_Click(object sender, System.EventArgs e)
        {
            ProtocolResultVialSelection dlg = new ProtocolResultVialSelection();
            //dlg.SetupMultipleSamples();
            dlg.SetProtocolClass = myFrmProtocolDetails.GetProtocolClass;

            dlg.SetEnabledTotalQuad(myFrmProtocolDetails.AdjustAndCountUsedQuadrants());
            dlg.ShowDialog(this);
        }

        private void InitVolCheck()
        {
            ProtocolReagentVolumeCheck volcheck = ProtocolReagentVolumeCheck.GetInstance();//CWJ Add
            volcheck.ClearAllValues();
            VerMgr.CleanInputVersions();	//CWJ Add	
        }
        //CWJ ADD

        //private void menuItem1_Click(object sender, System.EventArgs e)
        private void newProtocolHelper(ProtocolFormat protocolFormat)
        {
            //Open New protocol

            mniOptionsCustomize.Enabled = true;
            mniOptionsReagentBarcodes.Enabled = true;
            mniOptionsEnable.Enabled = true;
            mniFilePrint.Enabled = true; // false;
            boCanPrintSavedProtocol = false;
            //	mniFilePrintPreview.Enabled = true;
            mniOptionsMultiSamples.Enabled = true;
            mniOptionsResultSelection.Enabled = true;

            // enable the autofill menu item and set autofill to the default
            // of ON, set the menu label to the next toggle state of 'autofill
            // off and since there is no autofill variable set the manualfill
            // to the opposite default state of OFF (confusing!)
            mniOptionAutofill.Enabled = true;
            mniOptionAutofill.Text = DISABLE_AUTOFILL_LABEL;
            theProtocolModel.ProtocolDescManualFill = false;
            myFrmProtocolDetails.IsNewProtocol = true;

            // Clear any previous validation errors
            myFrmProtocolValidation.ClearValidationErrors();

            // Show a blank details form
            myFrmProtocolDetails.ClearForm();
            ShowChildForm(MDI_Child.ProtocolDetails);

            // Now enable data modification
            myFrmProtocolDetails.Enabled = true;
            myFrmProtocolDetails.EnableProtocolType(true);

            theProtocolModel.InitResultVialChecks();

            InitVolCheck();							//CWJ Add
            isThisTemplate = false;					//CWJ Add		

            this.DisableVersionUpdate = 0;
            this.Text = getTitleWithVersion();//"RoboSep Protocol Editor";						

            theProtocolModel.ProtocolFormat = protocolFormat;
            theProtocolModel.ProtocolHackAbsoluteVolumeMultipler = SeparatorResourceManager.isPlatformRS16(protocolFormat)? 5 : 0;
            ThingsToDoWhenProtocolFormatChanges();

            //make sure Taylor's vial barcode is initialized
            theProtocolModel.AddVialBarcodesIfAbsent();
            theProtocolModel.UpdateForReagentCustomVialUse();
        }

        private string definedFeatureSectionName;
        private List<string> definedFeatures;
        private void ThingsToDoWhenProtocolFormatChanges()
        {
            myFrmProtocolDetails.ThingsToDoWhenProtocolFormatChanges();
            mniOptionsReagentBarcodes.Enabled = (theProtocolModel.ProtocolFormat != ProtocolFormat.RoboSepS_1_0);
            mniOptionsFeatureSwitch.Enabled = (theProtocolModel.ProtocolFormat != ProtocolFormat.RoboSepS_1_0 &&
                theProtocolModel.ProtocolFormat != ProtocolFormat.RoboSepS_1_1 &&
                theProtocolModel.ProtocolFormat != ProtocolFormat.RoboSep16);

            //loaded protocol, load feature switch depend on type
            definedFeatureSectionName = SeparatorResourceManager.ProtocolFormatToFeatureSectionName(theProtocolModel.ProtocolFormat); //"FeatureSwitch";  
            definedFeatures = Tesla.Common.Utilities.GetSoftwareConfigAllKeys(definedFeatureSectionName);

        }

        private Dictionary<string, string> GetPlatformList()
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            results.Add("RSS",SeparatorResourceManager.GetSeparatorString(StringId.RoboSepS));
            results.Add("RS16",SeparatorResourceManager.GetSeparatorString(StringId.RoboSep16));
            return results;
        }

        private void mniNew_Click(object sender, System.EventArgs e)
        {
            //OpenFileHelper(true);

            Dictionary<string, string> platforms = GetPlatformList();
            List<string> platformFolders = new List<string>(platforms.Keys);
            List<string> platformNames = new List<string>(platforms.Values);

            using (ListSelect ts = new ListSelect(platformNames,"Please Select Platform","OK"))
            {
                if (ts.ShowDialog() == DialogResult.OK)
                {
                    OpenFileHelper(platformFolders[ts.GetSelectedIndex()]);

                }
            }
             
        }

        private void mniOptionsFeatureSwitch_Click(object sender, EventArgs e)
        {
            ProtocolFeatureSwitch form = new ProtocolFeatureSwitch();
            //update features from ini
            form.SetDefinedFeatureSectionName = definedFeatureSectionName;
            form.SetDefinedFeatures = definedFeatures;
            //update used features from protocol
            form.updateFeaturesFromProtocol(theProtocolModel.getFeatureList());

            form.ShowDialog();
        }



        //END of CWJ ADD


    }
}
