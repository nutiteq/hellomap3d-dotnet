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
using Nutiteq.Layers;

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
			MapView.RegisterLicense("XTUMwQ0ZEeFpTU0piNCs0M05lcHM2eVdKSFd3SWNQdHBBaFVBbzk0K1VuUjNMWG9vc1JsOGthbHNBYjJ5RnBVPQoKcHJvZHVjdHM9c2RrLXhhbWFyaW4tYW5kcm9pZC0zLioKcGFja2FnZU5hbWU9Y29tLm51dGl0ZXEuaGVsbG9tYXAueGFtYXJpbgp3YXRlcm1hcms9bnV0aXRlcQp1c2VyS2V5PTE1Y2Q5MTMxMDcyZDZkZjY4YjhhNTRmZWRhNWIwNDk2Cg==", ApplicationContext);

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
			//MapSetup.InitializeMapView (packageFolder.AbsolutePath, importPackagePath, mapView);

		
			/// Set online base layer
			var baseLayer = new NutiteqOnlineVectorTileLayer("osmbright.zip");
			mapView.Layers.Add(baseLayer);

			// read json from assets and add to map
			string json;
			using (System.IO.StreamReader sr = new System.IO.StreamReader (Assets.Open ("capitals_3857.geojson")))
			{
				json = sr.ReadToEnd ();
			}

			MapSetup.addJosnLayer (mapView, json);
		}
	}
}
