//----------------------------------------------------------------------------
// IInstrumentControlEvents
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
//----------------------------------------------------------------------------
using System;
using CookComputing.XmlRpc;

using Tesla.Common.Separator;

namespace Tesla.InstrumentControl
{
    #region delegates

    /// <summary>
    /// 
    /// </summary>
    public delegate void RunCompleteDelegate();

	/// <summary>
	/// 
	/// </summary>
	public delegate void ReportEstimatedTimeOfCompletionDelegate(DateTime estimatedTimeOfBatchRunCompletion);

	/// <summary>
	/// 
	/// </summary>
	public delegate void ReportStatusDelegate(InstrumentState state, string statusCode, 
		string[] statusMessageValues);

    /// <summary>
    /// 
    /// </summary>
    public delegate void ReportErrorDelegate(int severityLevel, string errorCode, 
		string[] errorMessageValues);	

    #endregion delegates

	/// <summary>
	/// Public events API for the InstrumentControl subsystem.
	/// </summary>
	/// <seealso>
	/// IInstrumentControl
	/// </seealso>
	/// <comments>
	/// We use Camel-case naming here (rather than the preferred Pascal-case naming) as that was 
	/// the way in which the events API methods were specified.
	/// </comments>
	public interface IInstrumentControlEvents
	{	
        #region events

        /// <summary>
        /// 
        /// </summary>
        event RunCompleteDelegate   					ReportRunComplete;

		/// <summary>
		/// 
		/// </summary>
		event ReportEstimatedTimeOfCompletionDelegate	ReportETC;

		/// <summary>
		/// 
		/// </summary>
		event ReportStatusDelegate						ReportStatus;

        /// <summary>
        /// 
        /// </summary>
        event ReportErrorDelegate   					ReportError;		

        #endregion events

        #region XML-RPC callback methods

		/// <summary>
		/// Echo test (clients may call this method to test connectivity with this XML-RPC event sink).
		/// </summary>
		/// <returns>
		/// True.
		/// </returns>
		[XmlRpcMethod]
		bool ping();

		/// <summary>
		/// 
		/// </summary>
		[XmlRpcMethod]
		bool runComplete();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="estimatedTimeOfBatchRunCompletion"></param>
		/// <returns></returns>
		[XmlRpcMethod]
		bool reportETC(DateTime estimatedTimeOfBatchRunCompletion);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		/// <param name="statusCode"></param>
		/// <param name="statusMessageValuesCount"></param>
		/// <param name="statusMessageValues"></param>
		[XmlRpcMethod]
		bool reportStatus(string state, string statusCode, int statusMessageValuesCount, 
			string[] statusMessageValues);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="severityLevel"></param>
		/// <param name="errorCode"></param>
		/// <param name="errorMessageValuesCount"></param>
		/// <param name="errorMessageValues"></param>
		[XmlRpcMethod]
        bool reportError(int severityLevel, string errorCode, int errorMessageValuesCount,
			string[] errorMessageValues);

        #endregion XML-RPC callback methods
	}
}
