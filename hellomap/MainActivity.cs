using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.IO;

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
using System.Collections;


namespace HelloMap
{

	public class Listener : MapEventListener
	{
		LocalVectorDataSource _dataSource;

		public Listener(LocalVectorDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		public override void OnMapClicked (MapClickInfo mapClickInfo)
		{
			var styleBuilder = new MarkerStyleBuilder ();
			styleBuilder.SetSize (10);
			var marker = new Marker (mapClickInfo.ClickPos, styleBuilder.BuildStyle());
			_dataSource.Add (marker);
		}

		public override void OnMapMoved()
		{
		}

		public override void OnVectorElementClicked(VectorElementsClickInfo vectorElementsClickInfo)
		{
			var styleBuilder = new MarkerStyleBuilder ();
			styleBuilder.SetSize (20);
			styleBuilder.SetColor (new Nutiteq.Graphics.Color (200, 0, 200, 200));
			var marker = new Marker (vectorElementsClickInfo.VectorElementClickInfos[0].ClickPos, styleBuilder.BuildStyle());
			_dataSource.Add (marker);
		}
	}

	public class PackageListener : PackageManagerListener
	{
		PackageManager _packageManager;

		public PackageListener(PackageManager packageManager)
		{
			_packageManager = packageManager;
		}

		public override void OnPackageListUpdated() {
			// called when package list is downloaded
			// now you can start downloading packages
			Android.Util.Log.Debug("Nutiteq", "OnPackageListUpdated");

			// you have to download full package when list is downloaded
			if(_packageManager.GetLocalPackage("JE") == null)
				_packageManager.StartPackageDownload ("JE");
		}

		public override void OnPackageListFailed() {
			Android.Util.Log.Debug("Nutiteq", "OnPackageListFailed");
			// Failed to download package list
		}

		public override void OnPackageStatusChanged(String id, int version, PackageStatus status) {
			// a portion of package is downloaded. Update your progress bar here.
//			Android.Util.Log.Debug("Nutiteq", "OnPackageStatusChanged "+id+" ver "+version+" progress "+status.Progress.ToString());
			Android.Util.Log.Debug("Nutiteq", "OnPackageStatusChanged "+ id +" ver "+ version);
		}

		public override void OnPackageCancelled(String id, int version) {
			// called when you called cancel package download
			Android.Util.Log.Debug("Nutiteq", "OnPackageCancelled");
		}

		public override void OnPackageUpdated(String id, int version) {
			// called when package is updated
			Android.Util.Log.Debug("Nutiteq", "OnPackageUpdated");
		}

		public override void OnPackageFailed(String id, int version) {
			// Failed to download package " + id + "/" + version
			Android.Util.Log.Debug("Nutiteq", "OnPackageFailed");
		}

	}


	[Activity (Label = "Hellomap", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Register license
			Nutiteq.Utils.Log.SetShowError (true);
			Nutiteq.Utils.Log.SetShowWarn (true);
			MapView.RegisterLicense("XTUN3Q0ZHRWttdzAzMWErL1g1V2tCdVNJVVF5TGIrTGpBaFExYTFVbHdsL2VxekQvK3ZlNHZDa2k2eGRQbnc9PQoKcHJvZHVjdHM9c2RrLXhhbWFyaW4tYW5kcm9pZC0zLjAuKgpwYWNrYWdlTmFtZT1jb20ubnV0aXRlcS5oZWxsb21hcC54YW1hcmluCndhdGVybWFyaz1udXRpdGVxCnVzZXJLZXk9MmE5ZTlmNzQ2MmNlZjQ4MWJlMmE4YzEyNjFmZTZjYmQK", ApplicationContext);

			// Set our view from the "main" layout resource
			MapView mapView = new MapView (ApplicationContext);
			SetContentView (mapView);

			// Set base projection
			EPSG3857 proj = new EPSG3857();
			mapView.Options.BaseProjection = proj; // note: EPSG3857 is the default, so this is actually not required

			// Set initial location and other parameters, don't animate
			mapView.FocusPos = proj.FromWgs84(new MapPos(-0.8164,51.2383)); // Berlin
			mapView.Zoom = 5;
			mapView.MapRotation = 0;
			mapView.Tilt = 90;


			// offline base layer
			// 1. download map metadata

			// Create package manager
			File packageFolder = new File (GetExternalFilesDir(null), "packages");
			if (!(packageFolder.Mkdirs() || packageFolder.IsDirectory)) {
				Android.Util.Log.Error("Nutiteq", "Could not create package folder!");
			}
			PackageManager packageManager = new NutiteqPackageManager(this, "nutiteq.mbstreets", packageFolder.AbsolutePath);

			packageManager.PackageManagerListener = new PackageListener(packageManager);

			// Download new package list only if it is older than 24h
			if (packageManager.ServerPackageListAge > 24 * 60 * 60) {
				packageManager.StartPackageListDownload ();
			}
			packageManager.Start ();

			// bbox download can be done right away, no need to wait for package download
			String bbox = "bbox(51.2383,-0.8164,51.7402,0.6406)"; // London (about 30MB)

			if (packageManager.GetLocalPackage(bbox) == null) {
				packageManager.StartPackageDownload (bbox);
			}


			// define styling for vector map
			UnsignedCharVector styleBytes = AssetUtils.LoadBytes("osmbright.zip");
			MBVectorTileDecoder vectorTileDecoder = null;
			if (styleBytes != null){

				// Create style set
				MBVectorTileStyleSet vectorTileStyleSet = new MBVectorTileStyleSet(styleBytes);
				vectorTileDecoder = new MBVectorTileDecoder(vectorTileStyleSet);
			}

			VectorTileLayer baseLayer = new VectorTileLayer(new PackageManagerTileDataSource(packageManager),vectorTileDecoder);

			// Create online base layer (no package download needed then). Use vector style from assets (osmbright.zip)
//			VectorTileLayer baseLayer = new NutiteqOnlineVectorTileLayer("osmbright.zip");


			mapView.Layers.Add(baseLayer);

			// Create overlay layer for markers
			LocalVectorDataSource dataSource = new LocalVectorDataSource (proj);
			VectorLayer overlayLayer = new VectorLayer (dataSource);
			mapView.Layers.Add (overlayLayer);

			// Create and set map listener
			mapView.MapEventListener = new Listener (dataSource);
		}
	}
}
