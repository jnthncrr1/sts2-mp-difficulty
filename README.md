# sts2-mp-difficulty

Experimental Slay the Spire 2 mod: custom-run modifiers that make fights harder, aimed at
multiplayer but usable solo. Modifiers are picked by the host in the Custom Run lobby, synced
to every client and saved with the run, so difficulty is chosen per run. Custom runs do not
count toward progression (ascension unlocks, wins, card stats).

`affects_gameplay` is true: every player in the lobby needs the mod at the same version, and
the game refuses to connect players whose gameplay mods differ.

| Modifier | Effect |
|---|---|
| Midas+ | Midas without the doubled gold: no Smithing at Rest Sites |
| Night Terrors+ | Night Terrors without the full heal: resting heals normally and costs 5 Max HP |
| Terminal+ | Terminal without the Plating: lose 1 Max HP on entering each room |
| Hard Labor I–V | Enemies deal 20/40/60/80/100% more damage (to players and Osty) |
| Lasting Consequences I–IV | Player healing reduced by 25/50/75/100%; the multiplayer end-of-combat revive is exempt |
| Convenience Fee I–II | Merchant prices 40/80% higher |
| Calisthenics Program I–II | Enemies have 15/30% more HP, on top of the game's multiplayer scaling |
| Damage Control I–IV | Enemies start combat with 1–4 more Artifact, not scaled by player count |

Each "+" modifier is mutually exclusive with its vanilla counterpart, and the ranks of each
Pact of Punishment condition are mutually exclusive with each other.

## Layout

| Path | Purpose |
|---|---|
| `mod/MpDifficulty/` | .NET 9 class library and manifest |
| `mod/MpDifficulty/src/WorseModifiers.cs` | The "+" modifiers |
| `mod/MpDifficulty/src/PactModifiers.cs` | The Pact of Punishment conditions, one type per rank |
| `mod/MpDifficulty/src/ModifierCatalog.cs` | List order and mutually exclusive groups |
| `mod/MpDifficulty/src/ModifierListPatch.cs` | Adds the modifiers to the Custom Run screen (not to `ModelDb.BadModifiers`, which dailies roll from) and registers the exclusive groups |
| `mod/MpDifficulty/src/LocPatch.cs` | Merges modifier text into the loaded localization tables once ModelDb is initialized (the tables load first) and after language changes; the game only reads modded text from a `.pck` |
| `mod/MpDifficulty/src/EnemyHpPatch.cs`, `HealPatch.cs`, `ArtifactScalingPatch.cs` | Harmony patches for effects with no modifier hook |

The decompiled game source used as reference lives in `~/Projects/sts2-run-stats/decompiled/`.

## Build and install

```sh
cd mod/MpDifficulty
dotnet build -c Release
```

The build copies the mod into the game's `mods/` folder (see `local.props.example` if Steam is
not in the default location). Remove `mods/MpDifficulty/` to play with people who don't have it.
