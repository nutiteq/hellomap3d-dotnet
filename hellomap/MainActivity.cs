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
		TextView _textView;
		Activity _activity;

		public PackageListener(Activity activity, PackageManager packageManager, TextView textView)
		{
			_packageManager = packageManager;
			_textView =  textView;
			_activity = activity;
		}

		public override void OnPackageListUpdated() {
			// called when package list is downloaded
			// now you can start downloading packages
			Android.Util.Log.Debug("Nutiteq", "OnPackageListUpdated");

			// to make sure that package list is updated, full package download is called here
//			if(_packageManager.GetLocalPackage("EE") == null)
//				_packageManager.StartPackageDownload ("EE");
		}

		public override void OnPackageListFailed() {
			Android.Util.Log.Debug("Nutiteq", "OnPackageListFailed");
			// Failed to download package list
		}

		public override void OnPackageStatusChanged(String id, int version, PackageStatus status) {
			// a portion of package is downloaded. Update your progress bar here.
			// Notice that the view and SDK are in different threads, so data copy id needed
			Android.Util.Log.Debug("Nutiteq", "OnPackageStatusChanged "+id+" ver "+version+" progress "+status.Progress.ToString());
			String progress = (String) status.Progress.ToString().Clone();
			_activity.RunOnUiThread(() => {
				_textView.Text = "Downloaded " + id+ " : " + progress + " %";
			});

		}

		public override void OnPackageCancelled(String id, int version) {
			// called when you called cancel package download
			Android.Util.Log.Debug("Nutiteq", "OnPackageCancelled");
		}

		public override void OnPackageUpdated(String id, int version) {
			// called when package is updated
			Android.Util.Log.Debug("Nutiteq", "OnPackageUpdated");
			_activity.RunOnUiThread(() => {
				_textView.Text = "Downloaded " + id + " version " + version;
			});
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
			MapView.RegisterLicense("XTUMwQ0ZBVUo5ZkJjUE1ESEdIY2VNcVFaTUxMY2RuNEdBaFVBbWpsdFlHbTR0Q005ZEE2LzdDNnc5RytHS2pjPQoKcHJvZHVjdHM9c2RrLXhhbWFyaW4tYW5kcm9pZC0zLjAuKgpwYWNrYWdlTmFtZT1jb20ubnV0aXRlcS5oZWxsb21hcC54YW1hcmluCndhdGVybWFyaz1udXRpdGVxCnZhbGlkVW50aWw9MjAxNS0wMy0zMQp1c2VyS2V5PTJhOWU5Zjc0NjJjZWY0ODFiZTJhOGMxMjYxZmU2Y2JkCg==", ApplicationContext);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			MapView mapView = (MapView)FindViewById (Resource.Id.mapView);

			TextView textView = (TextView)FindViewById (Resource.Id.textView);
			textView.Visibility = ViewStates.Visible;

			// Set base projection
			EPSG3857 proj = new EPSG3857();
			mapView.Options.BaseProjection = proj; // note: EPSG3857 is the default, so this is actually not required

			// Set initial location and other parameters, don't animate
			mapView.FocusPos = proj.FromWgs84(new MapPos(-0.8164,51.2383)); // Berlin
			mapView.Zoom = 5;
			mapView.MapRotation = 0;
			mapView.Tilt = 90;

			// offline base layer

			// 1. Create package manager
			File packageFolder = new File (GetExternalFilesDir(null), "packages");
			if (!(packageFolder.Mkdirs() || packageFolder.IsDirectory)) {
				Android.Util.Log.Error("Nutiteq", "Could not create package folder!");
			}
			PackageManager packageManager = new NutiteqPackageManager(this, "nutiteq.mbstreets", packageFolder.AbsolutePath);

			// 2. define listener, definition is in same class above
			packageManager.PackageManagerListener = new PackageListener(this, packageManager, textView);

			// Download new package list only if it is older than 24h
			if (packageManager.ServerPackageListAge > 24 * 60 * 60) {
				packageManager.StartPackageListDownload ();
			}

			// start manager - mandatory
			packageManager.Start ();

			// bounding box download can be done now
			// for country package download see OnPackageListUpdated in PackageListener

			String bbox = "bbox(-0.8164,51.2382,0.6406,51.7401)"; // London (about 30MB)

			if (packageManager.GetLocalPackage(bbox) == null) {
				packageManager.StartPackageDownload (bbox);
			}


			// Now can add vector map as layer
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
			// comment in to use online map. Packagemanager stuff is not needed then
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
