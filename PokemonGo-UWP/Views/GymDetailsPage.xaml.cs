using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PokemonGo_UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GymDetailsPage : Page
    {
        public GymDetailsPage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {

            };
        }

        #region Overrides of Page

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SubscribeToSearchEvents();
        }


        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            UnsubscribeToSearchEvents();
        }

        #endregion

        #region Handlers

        private void SubscribeToSearchEvents()
        {

        }

        private void UnsubscribeToSearchEvents()
        {

        }

        
        #endregion
    }
}