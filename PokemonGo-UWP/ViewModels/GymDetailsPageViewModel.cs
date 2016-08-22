using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Google.Protobuf;
using PokemonGo.RocketAPI;
using PokemonGo_UWP.Entities;
using PokemonGo_UWP.Utils;
using POGOProtos.Networking.Responses;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Universal_Authenticator_v2.Views;

namespace PokemonGo_UWP.ViewModels
{
    public class GymDetailsPageViewModel : ViewModelBase
    {

        #region Lifecycle Handlers

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter">MapPokemonWrapper containing the Pokemon that we're trying to capture</param>
        /// <param name="mode"></param>
        /// <param name="suspensionState"></param>
        /// <returns></returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode,
            IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                // Recovering the state
                CurrentGymInfo.MergeFrom(ByteString.FromBase64((string)suspensionState[nameof(CurrentGymInfo)]).CreateCodedInput());
            }
            else
            {
                // Navigating from inventory page so we need to load the pokemon
                Busy.SetBusy(true, "Loading Pokestop");
                CurrentGym = (GymDataWrapper)NavigationHelper.NavigationState[nameof(CurrentGym)];
                NavigationHelper.NavigationState.Remove(nameof(CurrentGym));
                Logger.Write($"Searching {CurrentGym.Id}");
                CurrentGymInfo =
                    await GameClient.GetGym(CurrentGym.Id, CurrentGym.Latitude, CurrentGym.Longitude);
                Busy.SetBusy(false);
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Save state before navigating
        /// </summary>
        /// <param name="suspensionState"></param>
        /// <param name="suspending"></param>
        /// <returns></returns>
        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            await Task.CompletedTask;
        }

        #endregion

        #region Game Management Vars

        /// <summary>
        ///     Pokestop that the user is visiting
        /// </summary>
        private GymDataWrapper _currentGym;

        private GetGymDetailsResponse _currentGymInfo;
        private DelegateCommand _abandonGym;

        #endregion

        #region Bindable Game Vars

        /// <summary>
        ///     Pokestop that the user is visiting
        /// </summary>
        public GymDataWrapper CurrentGym
        {
            get { return _currentGym; }
            set { Set(ref _currentGym, value); }
        }

        //TODO: create a wrapper for this, to bind GymStatus.Memberships
        public GetGymDetailsResponse CurrentGymInfo
        { 
            get { return _currentGymInfo; }
            set { Set(ref _currentGymInfo, value); }
        }

        /// <summary>
        ///     Going back to map page
        /// </summary>
        public DelegateCommand AbandonPokestop => _abandonGym ?? (
            _abandonGym = new DelegateCommand(() =>
            {
                // Re-enable update timer
                GameClient.ToggleUpdateTimer();
                Dispatcher.Dispatch(() => NavigationService.GoBack());
            }, () => true)
        );

        #endregion
    }
}
