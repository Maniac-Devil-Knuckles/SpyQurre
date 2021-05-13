using System;
using System.Collections.Generic;
using Qurre.API;
using Qurre.API.Events;

namespace SpyQurre
{
    public class EventHandlers
    {
        internal static Dictionary<int, Player> Spy { get; set; } = new Dictionary<int, Player>();
        private Plugin plugin { get; } = null;
        public EventHandlers(Plugin plugin)
        {
            this.plugin = plugin;
        }
        internal void OnPlayerDead(DeadEvent ev)
        {
            if(ev.Killer.IsSpy())
            {
                if(plugin.ChangeBody) ev.Killer.ChangeBody(ev.Target.Role, false, ev.Target.Position, ev.Target.Rotation, ev.HitInfo.GetDamageType());
            }
            else if(ev.Target.IsSpy())
            {
                KillSpy(ev.Target);
            }
        }

        Random RNG { get; set; } = new Random();
        internal void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if(ev.Allowed && plugin.SpawnChance >= RNG.Next(1,100))
            {
                Player player = ev.Players[RNG.Next(ev.Players.Count)];
                SpawnSpy(player);
            }
        }

        internal void OnWaitingForPlayers()
        {
            if (!plugin.IsEnabled) plugin.Disable();
            if (plugin.UseUnitName)
            {
                Round.AddUnit(Qurre.API.Objects.TeamUnitType.ChaosInsurgency, plugin.UnitName);
                Round.AddUnit(Qurre.API.Objects.TeamUnitType.NineTailedFox, plugin.UnitName);
            }
        }
        internal void KillSpy(Player player)
        {
            if (!player.IsSpy()) return;
            player.Role = RoleType.Spectator;
            if (plugin.UseUnitName)
            {
                player.UnitName = "";
            }
            else
            {
                player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
                player.ReferenceHub.nicknameSync.CustomPlayerInfo = "";
            }
        }

        internal void SpawnSpy(Player player)
        {
            if (player.IsSpy()) return;
            if(plugin.UseUnitName)
            {
                player.UnitName = plugin.UnitName;
            }
            else
            {
                player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;
                player.ReferenceHub.nicknameSync.CustomPlayerInfo = "";
            }
        }
    }
}
