//----------------------------------------------------------------------------
// CommandExecuter
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
using System.Collections;
using System.Threading;

namespace Tesla.InstrumentControl
{

	#region Delegates
		/// <summary>
		/// Notifies listeners when each command completes
		/// </summary>
		/// <paramref name="commandSucceeded">Did the command successfully complete</paramref>/>
		/// <paramref name="sequenceComplete">True if this command was the last command in the queue</paramref>/>
		/// <paramref name="returnString">If the command succeeded, the return value. Otherwise, an error description.</paramref>/>
	public delegate void ReportCommandCompleteDelegate(bool commandSucceeded, bool sequenceComplete, string returnString);

	/// <summary>
	/// Notifies listeners when a status update request completes
	/// </summary>
	/// <paramref name="obj">The object the method was executed on.</paramref>/>
	/// <paramref name="method">The method executed.</paramref>/>
	/// <paramref name="arguments">The method's arguments.</paramref>/>
	/// <paramref name="returnString">If the command succeeded, the return value. Otherwise, an error description.</paramref>/>
	public delegate void ReportStatusUpdateDelegate(string obj, string method, string arguments, string returnString);

	#endregion Delegates

	/// <summary>
	/// Manages the execution of instrument commands. Uses a worker thread to provide async command execution functionality.
	/// Maintains a queue of outstanding commands to execute in sequence.
	/// </summary>
	public class CommandExecuter: IDisposable
	{
		#region Events

		/// <summary>
		/// Event to notify interested parties when a command completes
		/// </summary>
		public event ReportCommandCompleteDelegate ReportCommandComplete;

		/// <summary>
		/// Event to notify interested parties when a status update completes
		/// </summary>
		public event ReportStatusUpdateDelegate ReportStatusUpdate;

		#endregion Events

		/// <summary>
		/// Struct to store commands
		/// </summary>
		private struct Command
		{
			public string obj, method, args;
			public bool statusCommand;

			public Command(string sObject, string sMethod, string sArgs, bool isStatusCommand)
			{
				obj = sObject;
				method = sMethod;
				args = sArgs;
				statusCommand = isStatusCommand;
			}
		}

		/// <summary>
		/// Queue of commands to be executed
		/// </summary>
		private Queue cmdQueue;

		/// <summary>
		/// Thread to execute the commands, and wait for replies
		/// </summary>
		private Thread execThread;

		/// <summary>
		/// Used to control when the worker thread is executing
		/// </summary>
		private ManualResetEvent workResetEvent = new ManualResetEvent(false);

		/// <summary>
		/// Used to command the worker thread to exit
		/// </summary>
		private ManualResetEvent exitResetEvent = new ManualResetEvent(false);

	
		public CommandExecuter()
		{
			cmdQueue = Queue.Synchronized(new Queue());

            execThread = new Thread(new ThreadStart(Execute));
            execThread.IsBackground = true;
			execThread.Start();
		}

		// Implement IDisposable.
		public void Dispose()
		{
			try
			{
				exitResetEvent.Set();
				if( !execThread.Join(1000) )
				{
					// thread still hasn't closed, try more drastic measures
					execThread.Abort();
					exitResetEvent.Set();
					execThread.Join(2000);
				}
			}
			catch
			{
				// 
			}	
		}

		/// <summary>
		/// Add a command to the queue
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="sMethod"></param>
		/// <param name="sArgs"></param>
		/// <returns></returns>
		public bool AddCommand(string sObject, string sMethod, string sArgs)
		{
			cmdQueue.Enqueue(new Command(sObject,sMethod,sArgs,false));
			if( cmdQueue.Count == 0 )
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Add a status update command to the queue.
		/// The only difference to AddCommand method is that a different
		///  event handler is called upon completion, ReportStatusUpdate.
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="sMethod"></param>
		/// <param name="sArgs"></param>
		/// <returns></returns>
		public bool AddStatusCommand(string sObject, string sMethod, string sArgs)
		{
			cmdQueue.Enqueue(new Command(sObject,sMethod,sArgs,true));
			if( cmdQueue.Count == 0 )
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Start execution of the queued commands
		/// </summary>
		/// <returns></returns>
		public bool Start()
		{
			bool success = false;
			if( cmdQueue.Count == 0 )
				return success;
			success = workResetEvent.Set();
			return success;
		}

		/// <summary>
		/// Pause execution of queued commands. 
		/// This will still allow the currently executing command to complete.
		/// </summary>
		/// <returns></returns>
		public bool Pause()
		{
			// Tell the worker thread to stop
			return workResetEvent.Reset();
		}

		/// <summary>
		/// Cancel execution of queued commands. 
		/// This will still allow the currently executing command to complete.
		/// </summary>
		/// <returns></returns>
		public bool Abort()
		{
			cmdQueue.Clear();
			// Tell the worker thread to stop
			return workResetEvent.Reset();
		}

		/// <summary>
		/// The main processing loop that runs on the worker thread
		/// </summary>
		private void Execute()
		{
			bool sequenceCompleted = false;
			bool commandSucceeded = false;
			string returnString;

			while(true)
			{
				// Process cmd, check if been asked to stop, process next, ....
				try
				{
					int eventIndex = WaitHandle.WaitAny(new WaitHandle[2]{exitResetEvent,workResetEvent});
					// If the exitResetEvent occured, close the worker thread
					if( eventIndex == 0 )
						return;
				}
				catch
				{
					// kill the thread
					return;
				}
				// 
				if( cmdQueue.Count > 0 )
				{
					Command cmd = (Command) cmdQueue.Dequeue();

					//execute cmd
					try
					{
						commandSucceeded = true;
						returnString = XmlRpcServiceInterface.GetInstance().XmlRpcProxy.Execute(
							cmd.obj, cmd.method, cmd.args );
					}
					catch (Exception ex)
					{
						//Kill off the command sequence
						cmdQueue.Clear();
						commandSucceeded = false;
						returnString = "Execute(" + cmd.obj + ", " + cmd.method + ", " + cmd.args + ") threw an exception: " + ex.Message;
						// need to log the error
					}

					//check if queue empty
					if( cmdQueue.Count == 0 )
					{
						// We are finished
						sequenceCompleted = true;

						//Stop the thread until it is signalled again
						workResetEvent.Reset();
					}
					else
					{
						sequenceCompleted = false;
					}

					// fire event to inform of cmd completion
					if( cmd.statusCommand )
					{
						if( commandSucceeded )
							ReportStatusUpdate(cmd.obj, cmd.method, cmd.args, returnString);
					}
					else
						ReportCommandComplete(commandSucceeded,sequenceCompleted,returnString);			
				}
			} //while true
		}
	} // CommandExecuter class
}
