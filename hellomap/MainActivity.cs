using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Nutiteq.Core;
using Nutiteq.Ui;
using Nutiteq.Utils;
using Nutiteq.Layers;
using Nutiteq.DataSources;
using Nutiteq.VectorElements;
using Nutiteq.Projections;
using Nutiteq.Styles;

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

	[Activity (Label = "Hellomap", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Register license
			Nutiteq.Utils.Log.SetShowError (true);
			Nutiteq.Utils.Log.SetShowWarn(true);
			MapView.RegisterLicense("XTUN3Q0ZHRWttdzAzMWErL1g1V2tCdVNJVVF5TGIrTGpBaFExYTFVbHdsL2VxekQvK3ZlNHZDa2k2eGRQbnc9PQoKcHJvZHVjdHM9c2RrLXhhbWFyaW4tYW5kcm9pZC0zLjAuKgpwYWNrYWdlTmFtZT1jb20ubnV0aXRlcS5oZWxsb21hcC54YW1hcmluCndhdGVybWFyaz1udXRpdGVxCnVzZXJLZXk9MmE5ZTlmNzQ2MmNlZjQ4MWJlMmE4YzEyNjFmZTZjYmQK", ApplicationContext);

			// Set our view from the "main" layout resource
			MapView mapView = new MapView (ApplicationContext);
			SetContentView (mapView);

			// Set base projection
			EPSG3857 proj = new EPSG3857();
			mapView.Options.BaseProjection = proj; // note: EPSG3857 is the default, so this is actually not required

			// Set initial location and other parameters, don't animate
			mapView.SetFocusPos(proj.FromWgs84(new MapPos(13.38933, 52.51704)), 0); // Berlin
			mapView.SetZoom(2, 0); // zoom 2, duration 0 seconds (no animation)
			mapView.SetMapRotation(0, 0);
			mapView.SetTilt(90, 0);

			// Create base layer. Use vector style from assets (osmbright.zip)
			VectorTileLayer baseLayer = new NutiteqOnlineVectorTileLayer("osmbright.zip");
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
