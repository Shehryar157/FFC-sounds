# FFC Sounds

Custom combat and dice sounds for **Fighting Fantasy Classics** (Steam, Windows), as a BepInEx 5 plugin.
It works on its own or alongside [FFC Access](https://github.com/Shehryar157/FFC-access).

Put sound files (.ogg, .wav or .mp3) in the `Sounds` folder in the game folder. Each kind of moment has its own folder
(you hit, you're hit, a draw, victory, defeat, Luck tests, dice), and sounds come in sets per kind of book
(fantasy, vehicles, sci-fi, superhero, horror) or per individual book. One sound is picked at random each time, never
the same twice in a row, with a slightly varied pitch. Where a folder is empty, the game's own sound plays as usual.
`How to add sounds.txt` in that folder explains the details.

## How it works

- At start-up it creates any missing folders, reads `book sets.txt` (which book uses which set) and loads the sound files
  in the background.
- Harmony postfixes on the game's combat methods (`EnemySheet.TakeHit`, `AdventureSheet.TakeHitAll`,
  `CombatController.CheckWin`, `CheckLose`, `Lucky`, `Unlucky`, and the round display) play the matching sound.
- Prefixes on the game's own `BBPlayCombatHit` and `BBPlayRoll` skip the game's sound only when a set has its own.
- Car and ship fights use the game's own car and ship combat flags to pick the vehicles and sci-fi sets.
- Sounds play through the game's sound player, so its Sound FX volume and on/off switch apply.

Turn it off with `Enabled = false` in `BepInEx\config\ffcsounds.soundpacks.cfg`.

## Building

Needs the .NET SDK and the game installed. Set `GameDir` in `FFCSounds.csproj` if needed, then `dotnet build -c Release`;
the build copies the plugin into the game's `BepInEx\plugins\FFCSounds` folder.
