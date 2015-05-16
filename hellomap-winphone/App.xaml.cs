using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

using Nutiteq.Core;
using Nutiteq.Graphics;
using Nutiteq.DataSources;
using Nutiteq.Projections;
using Nutiteq.Layers;
using Nutiteq.Styles;
using Nutiteq.VectorElements;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace HelloMap
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class HelloMapApp : Application
    {
		/// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public HelloMapApp()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
			if (mPage == null)
			{
                // Register Nutiteq app license
				Nutiteq.Ui.MapView.RegisterLicense("XTUN3Q0ZBd2NtcmFxbUJtT1h4QnlIZ2F2ZXR0Mi9TY2JBaFJoZDNtTjUvSjJLay9aNUdSVjdnMnJwVXduQnc9PQoKcHJvZHVjdHM9c2RrLWlvcy0zLiosc2RrLWFuZHJvaWQtMy4qCnBhY2thZ2VOYW1lPWNvbS5udXRpdGVxLioKYnVuZGxlSWRlbnRpZmllcj1jb20ubnV0aXRlcS4qCndhdGVybWFyaz1ldmFsdWF0aW9uCnVzZXJLZXk9MTVjZDkxMzEwNzJkNmRmNjhiOGE1NGZlZGE1YjA0OTYK");

                // Get asset folder for mbtiles file
                var importPackageName = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets\\world_ntvt_0_4.mbtiles");

                // Create folder for packages
                await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("packages", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var packageFolder = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "packages");

                // Create map view and initialize (actual initialization code is shared between Android/iOS/WinPhone platforms)
                mPage = new Nutiteq.Ui.MapView();
                HelloMap.MapSetup.InitializeMapView(packageFolder, importPackageName, mPage);
			}

			// Place the page in the current window and ensure that it is active.
			Windows.UI.Xaml.Window.Current.Content = mPage;
			Windows.UI.Xaml.Window.Current.Activate();
        }

		private Nutiteq.Ui.MapView mPage;
    }
}