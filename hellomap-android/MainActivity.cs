using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.IO;

using Nutiteq.Ui;
using Nutiteq.Utils;

namespace HelloMap
{
	[Activity (Label = "Hellomap", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Register license
			Nutiteq.Utils.Log.ShowError = true;
			Nutiteq.Utils.Log.ShowWarn = true;
			MapView.RegisterLicense("XTUMwQ0ZRQzdURnJKck9HYUdhT09VNGFSN3o3Nmg3UWhjQUlVTnV4TStMMk0vemhPMXUwUnBGRlhwbmFtTklFPQoKcHJvZHVjdHM9c2RrLXhhbWFyaW4taW9zLTMuKixzZGsteGFtYXJpbi1hbmRyb2lkLTMuKgpwYWNrYWdlTmFtZT1jb20ubnV0aXRlcS5oZWxsb21hcC54YW1hcmluCmJ1bmRsZUlkZW50aWZpZXI9Y29tLm51dGl0ZXEuaGVsbG9tYXAueGFtYXJpbgp3YXRlcm1hcms9bnV0aXRlcQp2YWxpZFVudGlsPTIwMTUtMDYtMDEKdXNlcktleT0yYTllOWY3NDYyY2VmNDgxYmUyYThjMTI2MWZlNmNiZAo", ApplicationContext);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			var mapView = (MapView)FindViewById (Resource.Id.mapView);

			// Create package manager folder (Platform-specific)
			var packageFolder = new File (GetExternalFilesDir(null), "packages");
			if (!(packageFolder.Mkdirs() || packageFolder.IsDirectory)) {
				Log.Fatal("Could not create package folder!");
			}

			// Copy bundled tile data to file system, so it can be imported by package manager
			string importPackagePath = new File (GetExternalFilesDir (null), "world_ntvt_0_4.mbtiles").AbsolutePath;
			using (var input = Assets.Open ("world_ntvt_0_4.mbtiles")) {
				using (var output = new System.IO.FileStream (importPackagePath, System.IO.FileMode.Create)) {
					input.CopyTo (output);
				}
			}

			// Initialize map
			MapSetup.InitializeMapView (packageFolder.AbsolutePath, importPackagePath, mapView);
		}
	}
}
