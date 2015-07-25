using Nutiteq.Core;
using Nutiteq.Ui;
using Nutiteq.DataSources;
using Nutiteq.VectorElements;
using Nutiteq.Styles;
using Nutiteq.WrappedCommons;
using Nutiteq.Utils;
using Nutiteq.Graphics;

namespace NutiteqSample
{

	public class MapListener : MapEventListener
	{
		private LocalVectorDataSource _dataSource;

		public MapListener(LocalVectorDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		public override void OnMapClicked (MapClickInfo mapClickInfo)
		{
			// Add default marker to the click location
			var styleBuilder = new MarkerStyleBuilder ();
			styleBuilder.Size = 10;
			var marker = new Marker (mapClickInfo.ClickPos, styleBuilder.BuildStyle());
			_dataSource.Add (marker);
		}

		public override void OnMapMoved()
		{
		}

		public override void OnVectorElementClicked(VectorElementsClickInfo vectorElementsClickInfo)
		{
			// A note about iOS: DISABLE 'Optimize PNG files for iOS' option in iOS build settings,
			// otherwise icons can not be loaded using AssetUtils/Bitmap constructor as Xamarin converts
			// PNGs to unsupported custom format.
			var bitmap = BitmapUtils.LoadBitmapFromAssets("Icon.png", true);

			var styleBuilder = new MarkerStyleBuilder ();
			styleBuilder.Size = 20;
			styleBuilder.Bitmap = bitmap;
			styleBuilder.Color = new Nutiteq.Graphics.Color (200, 0, 200, 200);
			var marker = new Marker (vectorElementsClickInfo.VectorElementClickInfos[0].ClickPos, styleBuilder.BuildStyle());
			_dataSource.Add (marker);
		}
	}
}
