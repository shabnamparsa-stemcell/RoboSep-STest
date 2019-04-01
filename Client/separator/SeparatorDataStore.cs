//----------------------------------------------------------------------------
// SeparatorDataStore
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
//  	03/14/06 MRU fix
//	03/29/06 - Short Description for Sample Volume Dialog - RL
//----------------------------------------------------------------------------
using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using Tesla.Common.Protocol;
using Tesla.Common.Separator;

namespace Tesla.Separator
{
	/// <summary>
	/// In-memory database for Separator data.
	/// </summary>
	/// <remarks>
	/// Singleton.
	/// </remarks>
	public class SeparatorDataStore : SeparatorDataSource
	{
        private static string               theProtocolsInfoBaseFilename		= "_mru";
		private static string				theProtocolsInfoFilenameExtenstion	= ".db";	// Note: the content is still XML at this stage & not encrypted or otherwise "hidden".
     
        private static SeparatorDataStore   theDataStore;

        public static SeparatorDataStore GetInstance()
		{
            if (theDataStore == null)
            {
                try
                {
                    theDataStore = new SeparatorDataStore();
                }
                catch
                {
                    theDataStore = null;
                }
            }
            return theDataStore;
        }

        private SeparatorDataStore()
        {
        }

		public ISeparationProtocol[] ListSeparationProtocolsInMRU()
		{
			try
			{
				// Get the Separation protocols in Most-Recently-Used order.
				DataView dv = new DataView(theDataStore.Protocol);
				string labelColumn = theDataStore.Protocol.ProtocolLabelColumn.ColumnName;
				string mruColumn = theDataStore.Protocol.ProtocolUsageTimeColumn.ColumnName;					

				// Note: to change to "Most Frequently Used (Usage Count)" order, use the following instead of mruColumn.
				//string mfuColumn = theDataStore.Protocol.ProtocolUsageCountColumn.ColumnName;

				dv.Sort = string.Format("{0} DESC", mruColumn);	// sort in descending order based on last usage time

				ArrayList protocols = new ArrayList();
				foreach (DataRowView dvr in dv)
				{
					SeparatorDataStore.ProtocolRow pRow = ((SeparatorDataStore.ProtocolRow)dvr.Row);
					SeparationProtocolRow[] spRows	= pRow.GetSeparationProtocolRows();

					Protocol protocol = null;
					if (spRows.GetLength(0) == 1)	// Check we have a valid separation protocol
					{
						try
						{
							// RL - Short Description for Sample Volume Dialog - 03/29/06
							protocol = new SeparationProtocol(pRow.ProtocolId,
                                (ProtocolClass)Enum.Parse(typeof(ProtocolClass), spRows[0].SeparationProtocolClass),
								pRow.ProtocolLabel, 
								pRow.ProtocolDescription,
                                pRow.ProtocolQuadrantCount,
								spRows[0].SeparationProtocolMinimumVolume,
								spRows[0].SeparationProtocolMaximumVolume,
                                pRow.ProtocolUsageTime);
						}
						catch (ArgumentException /*argEx*/)
						{							
							protocol = null;
						}
						protocols.Add(protocol);
					}
					
				}

				return (ISeparationProtocol[])protocols.ToArray(typeof(ISeparationProtocol));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public IMaintenanceProtocol[] ListMaintenanceProtocolsInAlphabetical()
		{
			try
			{
				// Get the Maintenance protocols in Alphabetical (ascending) order.
				DataView dv = new DataView(theDataStore.Protocol);
				string labelColumn = theDataStore.Protocol.ProtocolLabelColumn.ColumnName;
				dv.Sort = string.Format("{0} ASC", labelColumn);	// sort in ascending order based on protocol name

				ArrayList protocols = new ArrayList();
				foreach (DataRowView dvr in dv)
				{
					SeparatorDataStore.ProtocolRow pRow = ((SeparatorDataStore.ProtocolRow)dvr.Row);
					MaintenanceProtocolRow[] mpRows	= pRow.GetMaintenanceProtocolRows();

					Protocol protocol = null;
					if (mpRows.GetLength(0) == 1)	// Check we have a valid maintenance protocol
					{
						try
						{
							protocol = new MaintenanceProtocol(pRow.ProtocolId,
								pRow.ProtocolLabel, pRow.ProtocolQuadrantCount,
								mpRows[0].MaintenanceProtocolTaskId,
								mpRows[0].MaintenanceProtocolTaskDescription);		
						}
						catch (ArgumentException /*argEx*/)
						{
							protocol = null;
						}
						protocols.Add(protocol);
					}					
				}

				return (IMaintenanceProtocol[])protocols.ToArray(typeof(IMaintenanceProtocol));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public IShutdownProtocol[] ListShutdownProtocolsInAlphabetical()
		{
			try
			{
				// Get the Shutdown protocols in Alphabetical (ascending) order.
				DataView dv = new DataView(theDataStore.Protocol);
				string labelColumn = theDataStore.Protocol.ProtocolLabelColumn.ColumnName;
				dv.Sort = string.Format("{0} ASC", labelColumn);	// sort in ascending order based on protocol name

				ArrayList protocols = new ArrayList();
				foreach (DataRowView dvr in dv)
				{
					SeparatorDataStore.ProtocolRow pRow = ((SeparatorDataStore.ProtocolRow)dvr.Row);
					ShutdownProtocolRow[] shutRows	= pRow.GetShutdownProtocolRows();

					Protocol protocol = null;
					if (shutRows.GetLength(0) == 1)	// Check we have a valid shutdown protocol
					{
						try
						{
							protocol = new ShutdownProtocol(pRow.ProtocolId,
								pRow.ProtocolLabel, pRow.ProtocolQuadrantCount,
								shutRows[0].ShutdownProtocolDescription);		
						}
						catch (ArgumentException /*argEx*/)
						{
							protocol = null;
						}
						protocols.Add(protocol);
					}					
				}

				return (IShutdownProtocol[])protocols.ToArray(typeof(IShutdownProtocol));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

        public int GetInstrumentControlProtocolId(int protocolId)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("--------- GetInstrumentControlProtocolId called. protocolId = {0} ----------",  protocolId));
            int ProtoColID = 0;
            try
		    {
                if ((theDataStore != null) && (theDataStore.Protocol != null))
                {
                    ProtocolRow pRow = theDataStore.Protocol.FindByProtocolId(protocolId);
                    if (pRow != null)
                    {
                        ProtoColID = pRow.ProtocolInstrumentControlId;
                        System.Diagnostics.Debug.WriteLine(String.Format("--------- theDataStore.Protocol.FindByProtocolId returns instrument control protocol Id = {0} ----------", ProtoColID));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            return ProtoColID;
        }

		public int GetProtocolQuadrantCount(int protocolId)
		{
            ProtocolRow pRow = theDataStore.Protocol.FindByProtocolId(protocolId);
            int QCount = 0;
            if (pRow != null)
            {
                QCount = pRow.ProtocolQuadrantCount;
            }
            return QCount;
		}

		public void RecordProtocolUsageTime(int protocolId)
		{
			ProtocolRow protocolRow = theDataStore.Protocol.FindByProtocolId(protocolId);
			if (protocolRow != null)
			{
				protocolRow.ProtocolUsageTime = DateTime.Now;
				protocolRow.ProtocolUsageCount = protocolRow.ProtocolUsageCount + 1;
			}
		}

        public void PersistMRU_Statistics()
        {
            string protocolsPath = GetProtocolsPath();
            // Persist the protocols information
            this.WriteXml(protocolsPath + @"\"+theProtocolsInfoBaseFilename+theProtocolsInfoFilenameExtenstion,
                XmlWriteMode.IgnoreSchema);
        }

        public void RetrieveMRU_Statistics()
        {
            // To do this, retrieve persisted data (if any) and update the MRU field
            // of any protocol for which we have a matching protocol ID.
            using (SeparatorDataStore persistedDataStore = new SeparatorDataStore())
            {
                try
                {      
                    string protocolsPath = GetProtocolsPath();					
                    string protocolsXmlFileName = protocolsPath + @"\"+theProtocolsInfoBaseFilename+theProtocolsInfoFilenameExtenstion;
                    if (File.Exists(protocolsXmlFileName))                        
                    {
                        persistedDataStore.EnforceConstraints = false;                        
                        persistedDataStore.ReadXml(protocolsXmlFileName);
                        foreach (ProtocolRow persistRow in persistedDataStore.Protocol)
                        {
							//  03/14/06 MRU fix
							// Create a filter string to lookup protocols based on the Protocol Instrument
							// Control ID (a unique hash code based on the protocol name).
							string filter = string.Format("ProtocolInstrumentControlId={0}",
								persistRow.ProtocolInstrumentControlId);

							// Search for a match between the persisted list of protocols and the 
							// current list of protocols returned from the instrument control layer.
							// We need to do this lookup in case someone has added or deleted protocol
							// files from the protocol directory since last running the application.
							ProtocolRow[] pRows = (ProtocolRow[])theDataStore.Protocol.Select(filter);
							if(pRows.GetLength(0) == 1)
							{
								ProtocolRow pRow = pRows[0];                            	
								if (pRow != null)
								{
									pRow.ProtocolUsageCount = persistRow.ProtocolUsageCount;
									pRow.ProtocolUsageTime	= persistRow.ProtocolUsageTime;
								}
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);                    
                }
            }
        }

        private string GetProtocolsPath()
        {
            // Check the "protocols" directory exists, otherwise create it
            string startupPath = Application.StartupPath;
            int lastBackslashIndex = startupPath.LastIndexOf('\\');  // must escape the '\'
            startupPath = startupPath.Remove(lastBackslashIndex, startupPath.Length - lastBackslashIndex);
            string protocolsPath = startupPath + @"\protocols";
            if ( ! Directory.Exists(protocolsPath))
            {
                // We're not running from the application installation directory,
                // (probably because we're running from development directories)
                // so create a  "protocols" directory.
                Directory.CreateDirectory(protocolsPath);
            }
            return protocolsPath;
        }
	}
}
