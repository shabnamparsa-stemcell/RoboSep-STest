//----------------------------------------------------------------------------
// ServiceEventSink
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
using Tesla.Common.Separator;
using Tesla.Common.ResourceManagement;

namespace Tesla.InstrumentControl
{
	/// <summary>
	/// Event handlers for IServiceEvents 
	/// </summary>
	/// <remarks>
    /// TCP/IP server.  Created at run-time in response to the initial 
    /// InstrumentControl call (typically a call to the ping() method).
    /// Implicitly a Singleton because the singleton attribute is set when the service
    /// is registered.  
	/// </remarks>
	public class ServiceEventSink : MarshalByRefObject, IServiceEvents
	{
		/// <summary>
		/// 
		/// </summary>
		public ServiceEventSink()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

		#region IServiceEvents Members
		// Refer to the interface definition for method documentation.

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool ping()
		{			
			return true;
		}

        /// <summary>
        /// 
        /// </summary>
        public event RunCompleteDelegate ReportRunComplete;

		/// <summary>
		/// 
		/// </summary>
		public bool runComplete()
		{
            if (ReportRunComplete != null)
            {
                ReportRunComplete();
            }

            return true;
		}

		/// <summary>
		/// 
		/// </summary>
		public event ReportEstimatedTimeOfCompletionDelegate ReportETC;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="estimatedTimeOfBatchRunCompletion"></param>
		/// <returns></returns>
		public bool reportETC(DateTime estimatedTimeOfBatchRunCompletion)
		{
			if (ReportETC != null)
			{
				ReportETC(estimatedTimeOfBatchRunCompletion);
			}
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		public event ReportStatusDelegate ReportStatus;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		/// <param name="statusCode"></param>
		/// <param name="statusMessageValuesCount"></param>
		/// <param name="statusMessageValues"></param>
		/// <returns></returns>
		public bool reportStatus(string state, string statusCode, 
            int statusMessageValuesCount, string[] statusMessageValues)
		{
			bool resultOk = true;

			// Basic integrity check on the supplied data
			if (statusMessageValuesCount != statusMessageValues.GetLength(0))
			{
				// Badly formed message
				resultOk = false;
			}

			// Convert the supplied Instrument Control state string to a corresponding value
			// type.
			InstrumentState instrumentState = InstrumentState.ESTOP;
			try
			{
				if (resultOk)
				{
					instrumentState = (InstrumentState)Enum.Parse(typeof(InstrumentState), state);
				}				
			}
			catch
			{
				resultOk = false;
			}

			if (resultOk && ReportStatus != null)
			{
				string statusMessage;
				try
				{
					// Use the statusCode to lookup a localised string, and use
					// statusMessageValues to populate its run-time parameters (if required)
					statusMessage = SeparatorResourceManager.GetStatusMessageString(statusCode);
					statusMessage = string.Format(statusMessage, statusMessageValues);
				}
					// will trow an exception if can't lookup string
				catch
				{
					statusMessage = "Unknown status message, code " + statusCode;
				}
				if (ReportStatus != null)
					ReportStatus(state, statusMessage);
			}

			return resultOk;
		}

        /// <summary>
        /// 
        /// </summary>
        public event ReportErrorDelegate ReportError;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="severityLevel"></param>
		/// <param name="errorCode"></param>
		/// <param name="errorMessageValuesCount"></param>
		/// <param name="errorMessageValues"></param>
		/// <returns></returns>
		public bool reportError(int severityLevel, string errorCode, 
            int errorMessageValuesCount, string[] errorMessageValues)
		{
            bool result = false;
			if (errorMessageValuesCount != errorMessageValues.GetLength(0))
			{
				result = false;
			}
			else if (ReportError != null)
            {
				string errorMessage;
				try
				{
					// Use the errorCode to lookup a localised string, and use
					// errorMessageValues to populate its run-time parameters (if required)
					errorMessage = SeparatorResourceManager.GetErrorMessageString(errorCode);
                    // will throw an exception if can't lookup string
					errorMessage = string.Format(errorMessage, errorMessageValues);
				}					
				catch
				{
					errorMessage = "Unknown error message, code " + errorCode;
				}
				ReportError(severityLevel, errorMessage);
                result = true;
			}
			return result;
		}

		#endregion
    }
}
