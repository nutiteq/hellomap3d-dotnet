using System;
using Java.IO;
using Nutiteq.Utils;
using Android.App;
using Nutiteq.Layers;

namespace NutiteqSample
{
	[Activity (Label = "Clustered GeoJSON Capitals")]			
	public class ClusteredGeoJSONCaptitals: BaseMapActivity
	{
		protected override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set online base layer
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

