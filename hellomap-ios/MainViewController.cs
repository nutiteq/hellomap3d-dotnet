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
using Nutiteq.Graphics;
using Nutiteq.Geometry;

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
			PreferredFramesPerSecond = 45;

			// Register license
			Nutiteq.Utils.Log.ShowError = true;
			Nutiteq.Utils.Log.ShowWarn = true;
			Nutiteq.Utils.Log.ShowDebug = true;
			Nutiteq.Ui.MapView.RegisterLicense("XTUM0Q0ZRQytoU2xnd0MrZ2kvV0VhUmlWVGYxK0xsbDJNd0lWQUt6Z245R1F5NW95RXJ5WndoejBNcGpDdGpSZgoKcHJvZHVjdHM9c2RrLXhhbWFyaW4taW9zLTMuKgpidW5kbGVJZGVudGlmaWVyPWNvbS5udXRpdGVxLmhlbGxvbWFwLnhhbWFyaW4Kd2F0ZXJtYXJrPW51dGl0ZXEKdXNlcktleT0yYTllOWY3NDYyY2VmNDgxYmUyYThjMTI2MWZlNmNiZAo=");

			// Create package manager folder (Platform-specific)
			var paths = NSSearchPath.GetDirectories (NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User);
			var packagesDir = paths [0] + "packages";
			NSFileManager.DefaultManager.CreateDirectory (packagesDir, true, null);


			// Initialize map
			string importPackagePath = AssetUtils.CalculateResourcePath ("world_ntvt_0_4.mbtiles");
			MapSetup.InitializeMapView (packagesDir, importPackagePath, Map);

			/// Online vector base layer
//			var baseLayer = new NutiteqOnlineVectorTileLayer("osmbright.zip");

			/// Set online base layer
//			Map.Layers.Add(baseLayer);

			var json = System.IO.File.ReadAllText(AssetUtils.CalculateResourcePath("capitals_3857.geojson"));

			MapSetup.addJosnLayer (Map, json);
		}

		public override void ViewWillAppear(bool animated) {
			base.ViewWillAppear (animated);

			// GLKViewController-specific, do on-demand rendering instead of constant redrawing
			// This is VERY IMPORTANT as it stops battery drain when nothing changes on the screen!
			Paused = true;
		}

		}
}
