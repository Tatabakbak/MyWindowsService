using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace MyWindowsService
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : System.Configuration.Install.Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
		}

		// specify the command-line arguments
		//This code modifies the ImagePath registry key, 
		//which typically contains the full path to the executable for the Windows Service, 
		//by adding the default parameter values.
		protected override void OnBeforeInstall(IDictionary savedState)
		{
			string parameter = "MySource1\" \"MyLogFile1";
			Context.Parameters["assemblypath"] = "\"" + Context.Parameters["assemblypath"] + "\" \"" + parameter + "\"";
			base.OnBeforeInstall(savedState);
		}
	}
}
