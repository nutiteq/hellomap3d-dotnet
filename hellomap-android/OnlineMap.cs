using System;
using Java.IO;
using Nutiteq.Utils;
using Android.App;
using Nutiteq.Layers;

namespace NutiteqSample
{
	[Activity (Label = "Online Map")]			
	public class OnlineMap: BaseMapActivity
	{
		protected override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			/// Set online base layer
			var baseLayer = new NutiteqOnlineVectorTileLayer("nutibright-v2a.zip");
			mapView.Layers.Add(baseLayer);
		}
	}
}

