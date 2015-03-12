using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using Nutiteq.Core;
using Nutiteq.Ui;
using Nutiteq.Utils;
using Nutiteq.Layers;
using Nutiteq.DataSources;
using Nutiteq.VectorElements;
using Nutiteq.Projections;
using Nutiteq.Styles;
using Nutiteq.PackageManager;
using Nutiteq.WrappedCommons;
using Nutiteq.VectorTiles;

namespace HelloMap
{
	partial class MainViewController : MapViewController
	{
		public MainViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Register license
			Nutiteq.Utils.Log.ShowError = true;
			Nutiteq.Utils.Log.ShowWarn = true;
			MapViewController.RegisterLicense("XTUMwQ0ZRQzdURnJKck9HYUdhT09VNGFSN3o3Nmg3UWhjQUlVTnV4TStMMk0vemhPMXUwUnBGRlhwbmFtTklFPQoKcHJvZHVjdHM9c2RrLXhhbWFyaW4taW9zLTMuKixzZGsteGFtYXJpbi1hbmRyb2lkLTMuKgpwYWNrYWdlTmFtZT1jb20ubnV0aXRlcS5oZWxsb21hcC54YW1hcmluCmJ1bmRsZUlkZW50aWZpZXI9Y29tLm51dGl0ZXEuaGVsbG9tYXAueGFtYXJpbgp3YXRlcm1hcms9bnV0aXRlcQp2YWxpZFVudGlsPTIwMTUtMDYtMDEKdXNlcktleT0yYTllOWY3NDYyY2VmNDgxYmUyYThjMTI2MWZlNmNiZAo");

			// Create package manager (Platform-specific)
			var paths = NSSearchPath.GetDirectories (NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User);
			var packagesDir = paths [0] + "packages";
			NSFileManager.DefaultManager.CreateDirectory (packagesDir, true, null);
			var packageManager = new NutiteqPackageManager ("nutiteq.mbstreets", packagesDir);

			// Initialize map
			string importPackagePath = AssetUtils.CalculateResourcePath ("world_ntvt_0_4.mbtiles");
			MapSetup.InitializeMapView (packageManager, importPackagePath, this);
		}
	}
}
