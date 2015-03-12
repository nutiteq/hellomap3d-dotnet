# Nutiteq Maps SDK for .NET

Includes samples and Nutiteq Maps SDK 3.0 for .NET platform:
* Android (Xamarin)
* iOS (Xamarin)
* Windows Phone (Visual Studio, coming)

**This is currently non-commercial preview version**

#Getting started
## Register license key

Sign up in [developer.nutiteq.com](http://developer.nutiteq.com) and add an application. Select **xamarin-ios** or **xamarin-android** as application type, and make sure you enter same application ID as you have in your app. Finally you should get license code, which is a long string starting with *"XTU..."*. This is needed for your code

If you cover both platforms, register two apps.

## Cross-platform apps #

You can create one Xamarin project (solution) for Android and iOS and share code. These still need to be two apps, as many app aspects (UI, file system etc) are platform-specific. From Nutiteq SDK point of view the API is almost the same and your code can be shared, except some specific API calls which need Android *context* or file system references. For example these calls must be platform specific:

* Register license key: *MapView.RegisterLicense()*
* Create package manager: *new NutiteqPackageManager()*

Almost all of the map API related code: adding layers and objects to map, handling interactions/clicks etc can be shared for iOS and Android!

## Android app#

1) **Add Nutiteq SDK component** to your project

2) **Copy vector style file** (*osmbright.zip*) to your project *Assets* folder. You can take it from samples. This is needed for vector basemap.

3) **Add MapView to your application main layout**

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="fill_parent"
    android:layout_height="fill_parent"
    android:orientation="vertical" >
   <nutiteq.ui.MapView
    android:id="@+id/mapView"
    android:layout_width="fill_parent" 
    android:layout_height="fill_parent" 
    />
</LinearLayout>
```

4) **Create MapView object, add a base layer** 

You you can load layout from axml and load the MapView from Layout, or create it with code. Definition of base layer is enough for minimal map configuration.

```csharp
using Nutiteq.Ui;
using Nutiteq.Layers;
using Nutiteq.DataSources;


[Activity (Label = "Nutiteq.HelloMap", MainLauncher = true)]
public class MainActivity : Activity
{

	protected override void OnCreate ( Bundle bundle )
	{
		base.OnCreate ( bundle );

		// Register license BEFORE creating MapView (done in SetContentView)
		MapView.RegisterLicense("YOUR_LICENSE_KEY", this);

		/// Set our view from the "main" layout resource
		SetContentView ( Resource.Layout.Main );
	
		/// Get our map from the layout resource. 
		var mapView = FindViewById<MapView> ( Resource.Id.mapView );

		/// Online vector base layer
		var baseLayer = new NutiteqOnlineVectorTileLayer("osmbright.zip");

		/// Set online base layer  
		mapView.Layers.Add(baseLayer);
	}
	
```


## iOS app#


1) **Add Nutiteq SDK component** to your project

2) **Copy vector style file** (*osmbright.zip*) to your project. You can take it from samples. This is needed for vector basemap.

3) **Add Map object to app view**. When using Storyboards, use *OpenGL ES View Controller* as a template for the map. Remember to connect *MainViewController* (the name of the class below) as controller class.

4) **Initiate map, set base layer**

Add into MainViewController.cs:

```csharp
using Nutiteq.Ui;
using Nutiteq.Layers;
using Nutiteq.DataSources;

partial class MainViewController : MapViewController
{

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

		// Register license BEFORE creating MapView 
		MapView.RegisterLicense("YOUR_LICENSE_KEY");

		/// Online vector base layer
		var baseLayer = new NutiteqOnlineVectorTileLayer("osmbright.zip");

		/// Note that "this" is the MapViewController itself, it is based on IMapView interface which is shared with Android MapView class.
		/// mapView is used here to keep code same with Android
		var mapView = this;

		/// Set online base layer
		mapView.Layers.Add(baseLayer);

	}
```



## Android and iOS common map code #

3) **Add a marker** to map, to given coordinates. Add following after creating mapView.

You must have *Icon.png* in your Assets folder to set bitmap

```csharp
	// Create overlay layer for markers
	var proj = new EPSG3857();
	var dataSource = new LocalVectorDataSource (proj);
	var overlayLayer = new VectorLayer (dataSource);
	mapView.Layers.Add (overlayLayer);

	// create Marker style
	var markersStyleBuilder = new MarkerStyleBuilder ();
	markersStyleBuilder.SetSize (20);
	UnsignedCharVector iconBytes = AssetUtils.LoadBytes("Icon.png");
	var bitmap = new Bitmap (iconBytes, true);
	markersStyleBuilder.SetBitmap (bitmap);

	// Marker for London
	var marker = new Marker (proj.FromWgs84(new MapPos(-0.8164,51.2383)), markersStyleBuilder.BuildStyle ());
	dataSource.Add (marker);

```

## Other map actions

See sample code how to:

* **Control map view** - set zoom, center, tilt etc
* **Listen events** (MapListener.cs) of clicks to map and map objects
* **Add other objects**: Lines, Polygons, Points, Balloons (callouts). You can even add 3D objects and use customized Balloons.
* **Download offline map packages** for country or smaller region
