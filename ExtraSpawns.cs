using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Events;    // Event types (EventRoundStart)
using CounterStrikeSharp.API.Modules.Cvars;     // ConVar
using System.Linq;

// Alias the CS# Timer so we don’t conflict with System.Threading.Timer
using CSTimer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace ExtraSpawns
{
    [MinimumApiVersion(80)]
    public class ExtraSpawns : BasePlugin
    {
        public override string ModuleName => "Extra Spawns";
        public override string ModuleVersion => "1.2.1";
        public override string ModuleAuthor => "Vindict6 (updated)";

        private const int MaxSpawnsPerTeam = 32;
        private static readonly Vector[] Offsets =
        {
            new Vector(2, 0, 0),
            new Vector(1, 0, 0),
            new Vector(0, 2, 0),
            new Vector(0, 1, 0)
        };

        private bool _spawnsCreated = false;
        private CSTimer? _checkTimer;
        private CSTimer? _solidTeammatesTimer;
        private int? _previousSolidTeammates; // store original value if available

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnMapStart>(OnMapStart);

            // Register a proper round-start handler
            RegisterEventHandler<EventRoundStart>(OnRoundStart);

            // Console command to manually generate extra spawns
            AddCommand("css_spawns",
                "Generate extra spawns for T and CT",
                (player, info) =>
                {
                    AddExtraSpawns("info_player_terrorist");
                    AddExtraSpawns("info_player_counterterrorist");
                    info.ReplyToCommand("[ExtraSpawns] Extra spawns generated.");
                });
        }

        public override void Unload(bool hotReload)
        {
            // Clean up timers
            _checkTimer?.Kill();
            _checkTimer = null;

            _solidTeammatesTimer?.Kill();
            _solidTeammatesTimer = null;
        }

        private void OnMapStart(string map)
        {
            _spawnsCreated = false;

            // Start repeating check every 5 seconds to generate extra spawns
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

        private HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info)
        {
            // Kill any outstanding restore timer (safety)
            _solidTeammatesTimer?.Kill();
            _solidTeammatesTimer = null;

            // Attempt to read current value (if ConVar exists) so we can restore it later.
            var cvar = ConVar.Find("mp_solid_teammates");
            _previousSolidTeammates = cvar?.GetPrimitiveValue<int>() ?? 2;

            // Disable teammate collisions at round start
            Server.ExecuteCommand("mp_solid_teammates 0");
            Server.PrintToConsole("[ExtraSpawns] mp_solid_teammates set to 0 at round start.");

            // Restore after 10 seconds (one-shot). Stop if map changes.
            _solidTeammatesTimer = AddTimer(10.0f, () =>
            {
                int restore = _previousSolidTeammates ?? 2;
                Server.ExecuteCommand($"mp_solid_teammates {restore}");
                Server.PrintToConsole($"[ExtraSpawns] mp_solid_teammates restored to {restore}.");
                _solidTeammatesTimer = null;
            }, TimerFlags.STOP_ON_MAPCHANGE);

            return HookResult.Continue;
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
