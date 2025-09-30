ExtraSpawns (CounterStrikeSharp Plugin)

Automatically generates additional spawn points for both Terrorists and Counter-Terrorists, ensuring large servers (up to 64 players) never run out of spawn locations.

✨ Features

✅ Ensures up to 32 spawns per team (info_player_terrorist and info_player_counterterrorist entities).

✅ Places new spawns offset around existing ones, keeping natural flow and orientation.

✅ Runs automatically — Once every round start.

✅ Optional command (css_spawns) to manually force spawn generation.

✅ Works on any map, no editing required.

⚙️ How It Works

Detect existing spawns: Finds all info_player_terrorist and info_player_counterterrorist entities.

Check requirements: Each side should have 32 spawns. If there are enough already, nothing happens.

Clone with offsets: New spawn points are created by cloning the originals and shifting them slightly:

+/-1 to +/-2 units X and Y

Stop when filled: The process repeats until the team has 32 spawns total.

Visually, each original spawn gets up to four new neighbors in a tight cross pattern:

<img width="256" height="256" alt="image" src="https://github.com/user-attachments/assets/6c15cf4c-6e76-4862-8e26-0b0b3d5b1f86" />

The new player spawns will appear right on top of the existing ones when beginning a round.

***The mp_solid_teammates cvar is set to 0 for ten seconds, and then back to 2 at the beginning of every round.***


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
Version: 1.2.0
