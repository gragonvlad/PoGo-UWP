using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Enums;

namespace PokemonGo_UWP.Utils.Game
{
    /// <summary>
    /// Fort can have 3 visual states: opened, closed, cooldown
    /// </summary>
    [Flags]
    public enum FortDataStatus
    {
        Closed = 0,
        Opened = 1,
        Cooldown = 2,
        Lure = 4
    }

    public struct GymDataStatus
    {
        private readonly PokemonId _guardPokemonId;
        private readonly int _guardPokemonCp;
        private readonly bool _isInBattle;
        private readonly TeamColor _ownedByTeam;
        private readonly long _gymPoints;

        public PokemonId GuardPokemonId => _guardPokemonId;

        public int GuardPokemonCp => _guardPokemonCp;

        public bool IsInBattle => IsInBattle;

        public TeamColor OwnedByTeam => _ownedByTeam;

        public long GymPoints => _gymPoints;

        public GymDataStatus(PokemonId guardPokemonId, int guardPokemonCp, bool isInBattle, TeamColor ownedByTeam, long gymPoints)
        {
            _guardPokemonId = guardPokemonId;
            _guardPokemonCp = guardPokemonCp;
            _isInBattle = isInBattle;
            _ownedByTeam = ownedByTeam;
            _gymPoints = gymPoints;
        }
    }
}
