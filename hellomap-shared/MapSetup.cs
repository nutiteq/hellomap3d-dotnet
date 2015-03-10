using System;
using Nutiteq.Core;
using Nutiteq.Graphics;
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
	public class MapSetup
	{
		public static void InitializeMapView(PackageManager packageManager, string importPackagePath, IMapView mapView)
		{
			// Set base projection
			EPSG3857 proj = new EPSG3857();
			mapView.Options.BaseProjection = proj; // note: EPSG3857 is the default, so this is actually not required

			// Set initial location and other parameters, don't animate
			mapView.FocusPos = proj.FromWgs84(new MapPos(-0.8164,51.2383)); // Berlin
			mapView.Zoom = 2;
			mapView.MapRotation = 0;
			mapView.Tilt = 90;

			// offline base layer

			// 2. define listener, definition is in same class above
			packageManager.PackageManagerListener = new PackageListener(packageManager);

			// Download new package list only if it is older than 24h
			// Note: this is only needed if pre-made packages are used
			//	if (packageManager.ServerPackageListAge > 24 * 60 * 60) {
			//		packageManager.StartPackageListDownload ();
			//	}

			// start manager - mandatory
			packageManager.Start ();

			// Import initial package
			if (packageManager.GetLocalPackage("world0_4") == null) {
				packageManager.StartPackageImport ("world0_4", 1, importPackagePath);
			}

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
			if (styleBytes != null) {
				// Create style set
				MBVectorTileStyleSet vectorTileStyleSet = new MBVectorTileStyleSet (styleBytes);
				vectorTileDecoder = new MBVectorTileDecoder (vectorTileStyleSet);
			} else {
				Log.Error ("Failed to load style data");
			}

			// Create online base layer (no package download needed then). Use vector style from assets (osmbright.zip)
			// comment in to use online map. Packagemanager stuff is not needed then
			//			VectorTileLayer baseLayer = new NutiteqOnlineVectorTileLayer("osmbright.zip");

			var baseLayer = new VectorTileLayer(new PackageManagerTileDataSource(packageManager),vectorTileDecoder);
			mapView.Layers.Add(baseLayer);

			// Create overlay layer for markers
			var dataSource = new LocalVectorDataSource (proj);
			var overlayLayer = new VectorLayer (dataSource);
			mapView.Layers.Add (overlayLayer);

			// Create line style, and line poses
			var lineStyleBuilder = new LineStyleBuilder();
			lineStyleBuilder.SetLineJointType(LineJointType.LineJointTypeRound);
			lineStyleBuilder.SetStretchFactor(2);
			lineStyleBuilder.SetWidth(8);

			var linePoses = new MapPosVector ();
			linePoses.Add(proj.FromWgs84(new MapPos(0, 0)));
			linePoses.Add(proj.FromWgs84(new MapPos(0, 80)));
			linePoses.Add(proj.FromWgs84(new MapPos(45, 45)));

			var line = new Line (linePoses, lineStyleBuilder.BuildStyle ());
			dataSource.Add (line);

			// Create balloon popup
			var balloonPopupStyleBuilder = new BalloonPopupStyleBuilder();
			balloonPopupStyleBuilder.SetCornerRadius(3);
			balloonPopupStyleBuilder.SetStrokeColor(new Color(200, 120, 0, 255));
			balloonPopupStyleBuilder.SetStrokeWidth(1);
			balloonPopupStyleBuilder.SetPlacementPriority(1);
			var popup = new BalloonPopup(
				proj.FromWgs84(new MapPos(0, 20)),
				balloonPopupStyleBuilder.BuildStyle(),
				"Title", "Description");
			dataSource.Add(popup);

			// Create and set map listener
			mapView.MapEventListener = new MapListener (dataSource);
		}
	}
}

