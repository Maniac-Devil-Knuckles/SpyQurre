using CommandSystem;
using MEC;
using System;
using Qurre.API;
namespace SpyQurre
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ChangeBody : ParentCommand
    {
        public ChangeBody()
        {
            LoadGeneratedCommands();
        }

        public override string Command => "changebody";

        public override string[] Aliases => new string[] { };

        public override string Description => "Will change nickname, role";

        public override void LoadGeneratedCommands()
        {
            
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
            Player player = Player.Get(sender as CommandSender);
            if (player.IsSpy())
            {
                RoleType[] roles = new RoleType[] { RoleType.ChaosInsurgency, RoleType.ClassD, RoleType.FacilityGuard, RoleType.NtfCadet, RoleType.NtfCommander, RoleType.NtfLieutenant, RoleType.NtfScientist, RoleType.Scientist, RoleType.Tutorial };
                response = "changebody role nickname\nList: ";
                foreach (RoleType rol in roles) response += $"{rol} , ";
                if (arguments.Count==2)
                {
                    if(Enum.TryParse(arguments.At(0),out RoleType role))
                    {
                        if(role==RoleType.Spectator||GetTeam(role)==Team.SCP||role==RoleType.Spectator)
                        {
                            response = $"You can`t change body to {role}";
                            return false;
                        }
                        response = $"You changed body to {role}";
                        player.ChangeBody(role, false, player.Position, player.Rotation);
                        player.DisplayNickname = arguments.At(1);
                        return true;
                    }
                    return false;
                }
            }
            else response = "You are not spy";
            return true;
        }
        public static Team GetTeam(RoleType roleType)
        {
            switch (roleType)
            {
                case RoleType.ChaosInsurgency:
                    return Team.CHI;
                case RoleType.Scientist:
                    return Team.RSC;
                case RoleType.ClassD:
                    return Team.CDP;
                case RoleType.Scp049:
                case RoleType.Scp93953:
                case RoleType.Scp93989:
                case RoleType.Scp0492:
                case RoleType.Scp079:
                case RoleType.Scp096:
                case RoleType.Scp106:
                case RoleType.Scp173:
                    return Team.SCP;
                case RoleType.Spectator:
                    return Team.RIP;
                case RoleType.FacilityGuard:
                case RoleType.NtfCadet:
                case RoleType.NtfLieutenant:
                case RoleType.NtfCommander:
                case RoleType.NtfScientist:
                    return Team.MTF;
                case RoleType.Tutorial:
                    return Team.TUT;
                default:
                    return Team.RIP;
            }
        }
    }
}
