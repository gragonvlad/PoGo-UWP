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
using PokemonGo_UWP.Views;
using POGOProtos.Networking.Responses;
using Template10.Mvvm;
using Template10.Services.NavigationService;

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
                Busy.SetBusy(true, "Loading Gym");
                CurrentGym = (GymDataWrapper)NavigationHelper.NavigationState[nameof(CurrentGym)];
                this.GymSlots = new object[GypPointsToGymSlotCount(CurrentGym.GymStatus.GymPoints)];
                NavigationHelper.NavigationState.Remove(nameof(CurrentGym));
                Logger.Write($"Loading {CurrentGym.Id}");
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
        private object[] _gymSlots = new object[10];

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

        //TODO: fill members here!?
        public object[] GymSlots
        {
            get { return _gymSlots; }
            set { Set(ref _gymSlots, value); }
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

        #region GameLogic

        int GypPointsToGymSlotCount(long gymPoints)
        {
            var slotCount = 0;

            //Level 1 Gyms start out with 0 XP.A Gym will be at Level 1 until it reaches 500 XP.
            if (gymPoints >= 0 && gymPoints < 499)
                slotCount = 1;
            //Level 2 Gyms require 500 - 999 XP and provide two total slots to for defending Pokemon.
            else if (gymPoints >= 500 && gymPoints < 999)
                slotCount = 2;
            //Level 3 Gyms require 1, 000 XP - 1, 999 XP and provide three total slots for defending Pokemon. Adding a third Pokemon will increase that Gym's XP from 1,000 XP to 1,500 XP.
            //Level 4 Gyms require 2, 000 XP - 2, 999 XP and provide four total slots for defending Pokemon.  Gym level progression continues to work the same way as higher levels are reached
            else if (gymPoints >= 1000)
            {
                //this logic is a little weird maybe, but this way it can scale far up, we need to modify this code again if the ranges change
                slotCount = 2;
                slotCount += (int) gymPoints/1000;
            }

            return slotCount;
        }

        #endregion
}
}
