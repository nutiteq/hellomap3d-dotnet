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
using Nutiteq.WrappedCommons;
using Nutiteq.VectorTiles;

namespace HelloMap
{
	partial class MainViewController : GLKit.GLKViewController
	{
		public MainViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// GLKViewController-specific parameters for smoother animations
			ResumeOnDidBecomeActive = false;
			PreferredFramesPerSecond = 60;

			// Register license
			Nutiteq.Utils.Log.ShowError = true;
			Nutiteq.Utils.Log.ShowWarn = true;
			Nutiteq.Ui.MapView.RegisterLicense("XTUMwQ0ZRQzdURnJKck9HYUdhT09VNGFSN3o3Nmg3UWhjQUlVTnV4TStMMk0vemhPMXUwUnBGRlhwbmFtTklFPQoKcHJvZHVjdHM9c2RrLXhhbWFyaW4taW9zLTMuKixzZGsteGFtYXJpbi1hbmRyb2lkLTMuKgpwYWNrYWdlTmFtZT1jb20ubnV0aXRlcS5oZWxsb21hcC54YW1hcmluCmJ1bmRsZUlkZW50aWZpZXI9Y29tLm51dGl0ZXEuaGVsbG9tYXAueGFtYXJpbgp3YXRlcm1hcms9bnV0aXRlcQp2YWxpZFVudGlsPTIwMTUtMDYtMDEKdXNlcktleT0yYTllOWY3NDYyY2VmNDgxYmUyYThjMTI2MWZlNmNiZAo");

			// Create package manager folder (Platform-specific)
			var paths = NSSearchPath.GetDirectories (NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User);
			var packagesDir = paths [0] + "packages";
			NSFileManager.DefaultManager.CreateDirectory (packagesDir, true, null);

			// Initialize map
			string importPackagePath = AssetUtils.CalculateResourcePath ("world_ntvt_0_4.mbtiles");
			MapSetup.InitializeMapView (packagesDir, importPackagePath, Map);
		}

		public override void ViewWillAppear(bool animated) {
			base.ViewWillAppear (animated);

			// GLKViewController-specific, do on-demand rendering instead of constant redrawing
			// This is VERY IMPORTANT as it stops battery drain when nothing changes on the screen!
			Paused = true;
		}

	}

}
