using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using System.Linq;

namespace ExtraSpawns
{
    [MinimumApiVersion(80)]
    public class ExtraSpawns : BasePlugin
    {
        public override string ModuleName => "Extra Spawns";
        public override string ModuleVersion => "1.1.0";
        public override string ModuleAuthor => "Vindict6";

        private const int MaxSpawnsPerTeam = 32;

        private static readonly Vector[] Offsets =
        {
            new(64, 0, 0),
            new(-64, 0, 0),
            new(0, 64, 0),
            new(0, -64, 0)
        };

        private bool _spawnsCreated = false;
        private CounterStrikeSharp.API.Modules.Timers.Timer? _checkTimer;

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnMapStart>(OnMapStart);

            // Console command: css_spawns
            AddCommand("css_spawns",
                "Generate extra spawns for T and CT",
                (player, info) =>
                {
                    AddExtraSpawns("info_player_terrorist");
                    AddExtraSpawns("info_player_counterterrorist");
                    info.ReplyToCommand("[ExtraSpawns] Extra spawns generated.");
                });
        }

        private void OnMapStart(string map)
        {
            _spawnsCreated = false;

            // Start repeating check every 5 seconds
            _checkTimer = AddTimer(5.0f, () =>
            {
                if (_spawnsCreated)
                {
                    _checkTimer?.Kill();
                    _checkTimer = null;
                    return;
                }

                bool madeT = AddExtraSpawns("info_player_terrorist");
                bool madeCT = AddExtraSpawns("info_player_counterterrorist");

                if (madeT || madeCT)
                {
                    _spawnsCreated = true;
                    Server.PrintToConsole("[ExtraSpawns] Spawns created, stopping checks.");
                    _checkTimer?.Kill();
                    _checkTimer = null;
                }
            }, TimerFlags.REPEAT);
        }

        private static bool AddExtraSpawns(string className)
        {
            var spawns = Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(className).ToList();
            if (spawns.Count == 0)
                return false;

            int needed = MaxSpawnsPerTeam - spawns.Count;
            if (needed <= 0)
                return false;

            int created = 0;

            foreach (var spawn in spawns)
            {
                if (created >= needed)
                    break;

                var origin = spawn.AbsOrigin;
                var angles = spawn.AbsRotation;

                if (origin == null || angles == null)
                    continue;

                foreach (var offset in Offsets)
                {
                    if (created >= needed)
                        break;

                    var newPos = origin + offset;

                    var newSpawn = Utilities.CreateEntityByName<CBaseEntity>(className);
                    if (newSpawn != null)
                    {
                        newSpawn.Teleport(newPos, angles, null);
                        newSpawn.DispatchSpawn();
                        created++;
                    }
                }
            }

            if (created > 0)
                Server.PrintToConsole($"[ExtraSpawns] Added {created} spawns for {className}.");

            return created > 0;
        }
    }
}
