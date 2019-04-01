//----------------------------------------------------------------------------
// ISeparator
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
// Notes:
//    03/24/05 - added parkarm() and islidclosed() - bdr
// 	  04/11/05 - added parkpump to deenergize pump - bdr
//
//----------------------------------------------------------------------------

#define bdr20
#define bdr21

using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;

using Tesla.Common.Protocol;
using Tesla.Common.Separator;

namespace Tesla.Separator
{
	/// <summary>
	/// Public API for the EasySep separator
	/// </summary>
	/// <remarks>
	/// This interface represents the OperatorConsole-to-Separator direction.
	/// NOTE: This API is intended to be asynchronous in nature -- all methods
	/// should return "immediately" to avoid GUI lockup.  Results, error 
	/// information etc that needs to be returned to the OperatorConsole should be
	/// done via ISeparatorEvents.
	/// </remarks>
	/// <seealso>
	/// ISeparatorEvents
	/// </seealso>
	public interface ISeparator 
	{
		[OneWay]
		void SetSampleVolume(QuadrantId initialQuadrant, FluidVolume volume);

        [OneWay]
        void ConfirmHydraulicFluidRefilled();
		
		[OneWay]
		void SelectProtocol(QuadrantId initialQuadrant, IProtocol protocol);

		[OneWay]
		void DeselectProtocol(QuadrantId initialQuadrant);

		[OneWay] 
		void ScheduleRun(QuadrantId quadrant);

		[OneWay]
		void StartRun(string batchRunUserId, ReagentLotIdentifiers[] protocolReagentLotIds,bool isSharing, int[] sharedSectorsTranslation, int[]sharedSectorsProtocolIndex);

		[OneWay]
		void HaltRun();
		
		#if bdr21
		[OneWay]
		void ParkPump(); // bdr

		[OneWay]
		void ParkArm(); // bdr
		#endif
		
		[OneWay]
		void AbortRun();

		[OneWay]
		void PauseRun();

		[OneWay]
		void ResumeRun();

        [OneWay]
        void Unload();

		[OneWay]
		void Shutdown(IShutdownProtocol protocol);

		[OneWay]
		void Connect(bool reqSubscribe);

		[OneWay]
		void Disconnect(bool isExit);
        
		#if bdr20
	    bool IsLidClosed();	// bdr
		#endif 

		string GetInstrumentState();
		bool isPauseCommand();
		string getPauseCommandCaption();

		void GetCustomNames(string name, out string[] customNames,out int numQuadrants);//CR
		
		void GetResultVialSelection(string name, out bool[] resultChecks, out int numQuadrants);

        int     InitScanVialBarcode();                      //CWJ
        int     ScanVialBarcodeAt(int Quadrant, int Vial);  //CWJ
        string  GetVialBarcodeAt(int Quadrant, int Vial);   //CWJ

        int     SetIgnoreLidSensor(int sw);                       //CWJ
        bool    GetIgnoreLidSensor();

        int     GetCurrentSec();                      //CWJ
        int     GetCurrentSeq();                      //CWJ
        int     GetCurrentID();                      //CWJ

		[OneWay]
		void ReloadProtocols();
	}
}