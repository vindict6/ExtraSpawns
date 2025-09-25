ExtraSpawns (CounterStrikeSharp Plugin)

Automatically generates additional spawn points for both Terrorists and Counter-Terrorists, ensuring large servers (up to 64 players) never run out of spawn locations.

✨ Features

✅ Ensures up to 32 spawns per team (info_player_terrorist and info_player_counterterrorist entities).

✅ Places new spawns offset around existing ones, keeping natural flow and orientation.

✅ Runs automatically — checks every 5 seconds after map load until spawns are created, then never runs again.

✅ Manual command (css_spawns) to force regeneration.

✅ Works on any map, no editing required.

⚙️ How It Works

Detect existing spawns: Finds all info_player_terrorist and info_player_counterterrorist entities.

Check requirements: Each side should have 32 spawns. If there are enough already, nothing happens.

Clone with offsets: New spawn points are created by cloning the originals and shifting them slightly:

+64 units X

-64 units X

+64 units Y

-64 units Y

Stop when filled: The process repeats until the team has 32 spawns total.

Visually, each original spawn gets up to four new neighbors in a cross pattern:

   [ ]  
[ ] O [ ]   ← O = original spawn, [ ] = new spawns
   [ ]

📦 Installation

Install CounterStrikeSharp
.

Place the compiled plugin .dll in your server’s addons/counterstrikesharp/plugins/ folder.

Restart the server.

🔧 Commands
Command	Description
css_spawns	Manually generate extra spawns for both teams.

📝 Example Console Output
[ExtraSpawns] Added 12 spawns for info_player_terrorist.
[ExtraSpawns] Added 8 spawns for info_player_counterterrorist.
[ExtraSpawns] Spawns created, stopping checks.

📌 Notes

The plugin does not overwrite existing spawns — it only adds more.

If the map already has 32 spawns per team, no new spawns are added.

Once spawns are successfully generated, the timer stops forever (until next map).

👤 Author

Vindict6 (BONE)
Version: 1.1.0
