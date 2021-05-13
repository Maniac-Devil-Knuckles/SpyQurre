using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandSystem;
using Qurre;
using Qurre.API;
using RemoteAdmin;
using PlyEvents = Qurre.Events.Player;
using RoundEvents = Qurre.Events.Round;
namespace SpyQurre
{
    public static class API
    {
        public static IEnumerable<Player> AllSpy => EventHandlers.Spy.Values;
        public static bool IsSpy(this Player player) => AllSpy.Any(p=>p.Id==player.Id);
        public static void SpawnSpy(Player player) => Plugin.handler.SpawnSpy(player);
        public static void KillSpy(Player player) => Plugin.handler.KillSpy(player);
    }


    public class Plugin : Qurre.Plugin
    {
        public override string Developer => "Maniac Devil Knuckles";
        public override string Name => "SpyQurre";
        public override Version NeededQurreVersion => new Version(1,3,9);
        public override Version Version => new Version(1,0,0);
        public bool IsEnabled
        {
            get
            {
                bool result = true;
                try
                {
                    result = Config.GetBool("spyqurre_enable",true);
                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }

        public bool UseUnitName
        {
            get
            {
                bool result = true;
                try
                {
                    result = Config.GetBool("spyqurre_use_unitname", true);
                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }

        public float SpawnChance
        {
            get
            {
                float chance = 20f;
                try
                {
                    chance = Config.GetFloat("scpqurre_spawnchance", chance);
                }
                catch (Exception ex)
                {

                }
                return chance;
            }
        }

        public string text
        {
            get
            {
                string _text = "You are spy";
                try
                {
                    _text = Config.GetString("scpqurre_spawntext", _text);
                }
                catch (Exception ex)
                {

                }
                return _text;
            }
        }

        public string UnitName
        {
            get
            {
                string _UnitName = "Spy";
                try
                {
                    _UnitName = Config.GetString("scpqurre_unitname", _UnitName);
                }
                catch (Exception ex)
                {

                }
                return _UnitName;
            }
        }

        public ushort time
        {
            get
            {
                ushort _time = 10;
                try
                {
                    _time = Config.GetUShort("scpqurre_time", _time);
                }
                catch (Exception ex)
                {

                }
                return _time;
            }
        }

        public bool ChangeBody
        {
            get
            {
                bool result = false;
                try
                {
                    result = Config.GetBool("spyqurre_changebody_after_kill", result);
                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }
        public Dictionary<Type, Dictionary<Type, ICommand>> Commands { get; } = new Dictionary<Type, Dictionary<Type, ICommand>>()
        {
            { typeof(RemoteAdminCommandHandler), new Dictionary<Type, ICommand>() },
            { typeof(GameConsoleCommandHandler), new Dictionary<Type, ICommand>() },
            { typeof(ClientCommandHandler), new Dictionary<Type, ICommand>() },
        };
        internal static EventHandlers handler { get; set; } = null;
        public override void Enable()
        {
            MEC.Timing.CallDelayed(3f, () =>
            {
                if (!IsEnabled) return;
                handler = new EventHandlers(this);
                PlyEvents.Dead += handler.OnPlayerDead;
                RoundEvents.WaitingForPlayers += handler.OnWaitingForPlayers;
                OnRegisteringCommands();
            });
        }

        public override void Disable()
        {
            PlyEvents.Dead -= handler.OnPlayerDead;
            RoundEvents.WaitingForPlayers -= handler.OnWaitingForPlayers;
            handler = null;
        }
        internal static Assembly Assembly => Assembly.GetCallingAssembly();
        public virtual void OnRegisteringCommands()
        {
            foreach (Type type in Assembly.GetTypes())
            {
                if (type.GetInterface("ICommand") != typeof(ICommand))
                    continue;

                if (!Attribute.IsDefined(type, typeof(CommandHandlerAttribute)))
                    continue;

                foreach (CustomAttributeData customAttributeData in type.CustomAttributes)
                {
                    try
                    {
                        if (customAttributeData.AttributeType != typeof(CommandHandlerAttribute))
                            continue;

                        Type commandType = (Type)customAttributeData.ConstructorArguments?[0].Value;

                        if (!Commands.TryGetValue(commandType, out Dictionary<Type, ICommand> typeCommands))
                            continue;

                        if (!typeCommands.TryGetValue(type, out ICommand command))
                            command = (ICommand)Activator.CreateInstance(type);

                        if (commandType == typeof(RemoteAdminCommandHandler))
                            CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);
                        else if (commandType == typeof(GameConsoleCommandHandler))
                            GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(command);
                        else if (commandType == typeof(ClientCommandHandler))
                            QueryProcessor.DotCommandHandler.RegisterCommand(command);

                        Commands[commandType][type] = command;
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"An error has occurred while registering a command: {exception}");
                    }
                }
            }
        }
    }
}
