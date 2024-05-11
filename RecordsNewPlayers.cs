using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace RecordsNewPlayers
{
    public class RecordsNewPlayers : BasePlugin
    {
        public override string ModuleName => "RecordsNewPlayers";
        public override string ModuleVersion => "v1.0";
        public override string ModuleAuthor => "E!N";

        private readonly Dictionary<ulong, DateTime> lastLoggedDates = [];
        string? logDirectory;

        public override void Load(bool hotReload)
        {
            logDirectory = GetConfigDirectory();
            EnsureLogDirectory(logDirectory);
        }

        private static string GetConfigDirectory()
        {
            return Path.Combine(Server.GameDirectory, "csgo/addons/counterstrikesharp/logs/RecordsNewPlayers/");
        }

        private static void EnsureLogDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        [GameEventHandler]
        public HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
        {
            if (@event?.Userid?.SteamID == null)
                return HookResult.Continue;

            ulong steamId = @event.Userid.SteamID;

            if (lastLoggedDates.TryGetValue(steamId, out DateTime lastLogged))
            {
                if (lastLogged.Date == DateTime.Today)
                    return HookResult.Continue;
            }

            lastLoggedDates[steamId] = DateTime.Today;
            LogPlayer(@event);
            return HookResult.Continue;
        }

        private void LogPlayer(EventPlayerConnectFull @event)
        {
            if (logDirectory == null)
                return;
            string fileName = Path.Combine(logDirectory, DateTime.Today.ToString("dd-MM-yyyy") + ".txt");
            string logEntry = $"{DateTime.Now:HH:mm:ss} | {@event.Userid?.PlayerName} | {@event.Userid?.SteamID} | {@event.Userid?.IpAddress}\n";

            File.AppendAllText(fileName, logEntry);
        }
    }
}