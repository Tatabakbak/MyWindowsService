﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MyWindowsService
{
	public partial class Service1 : ServiceBase
	{

		public enum ServiceState
		{
			SERVICE_STOPPED = 0x00000001,
			SERVICE_START_PENDING = 0x00000002,
			SERVICE_STOP_PENDING = 0x00000003,
			SERVICE_RUNNING = 0x00000004,
			SERVICE_CONTINUE_PENDING = 0x00000005,
			SERVICE_PAUSE_PENDING = 0x00000006,
			SERVICE_PAUSED = 0x00000007,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ServiceStatus
		{
			public long dwServiceType;
			public ServiceState dwCurrentState;
			public long dwControlsAccepted;
			public long dwWin32ExitCode;
			public long dwServiceSpecificExitCode;
			public long dwCheckPoint;
			public long dwWaitHint;
		};

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

		private int eventId;
		public Service1(string[] args)
		{
			eventId = 0;
			InitializeComponent();

			string eventSourceName = "MySource";
			string logName = "MyNewLog";
			if (args.Count() > 0)
			{
				eventSourceName = args[0];
			}
			if (args.Count() > 1)
			{
				logName = args[1];
			}

			eventLog1 = new EventLog();
			if (!EventLog.SourceExists("MySource"))
			{
				EventLog.CreateEventSource("MySource", "MyNewLog");
			}
			eventLog1.Source = eventSourceName;
			eventLog1.Log = logName;
		}

		protected override void OnStart(string[] args)
		{
			

			// Update the service state to Start Pending.  
			ServiceStatus serviceStatus = new ServiceStatus();
			serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
			serviceStatus.dwWaitHint = 100000;
			SetServiceStatus(this.ServiceHandle, ref serviceStatus);

			eventLog1.WriteEntry("In OnStart");
			
			// Set up a timer to trigger every minute.  
			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Interval = 60000; // 60 seconds  
			timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
			timer.Start();

			// Update the service state to Running.  
			serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
			SetServiceStatus(this.ServiceHandle, ref serviceStatus);

		}

		public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
		{
			// TODO: Insert monitoring activities here.  
			eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
		}

		protected override void OnStop()
		{
			// Update the service state to Start Pending.  
			ServiceStatus serviceStatus = new ServiceStatus();
			serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
			serviceStatus.dwWaitHint = 100000;
			SetServiceStatus(this.ServiceHandle, ref serviceStatus);

			eventLog1.WriteEntry("In onStop.");

			// Update the service state to Stopped.  
			serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
			SetServiceStatus(this.ServiceHandle, ref serviceStatus);
		}

		protected override void OnContinue()
		{
			eventLog1.WriteEntry("In OnContinue.");
		}

	}
}
