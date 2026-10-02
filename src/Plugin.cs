using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace FFCSounds
{
    /// <summary>
    /// FFC Sounds: plays your own combat and dice sounds from the "Sounds" folder in the game folder.
    /// Works on its own, or alongside FFC Access.
    /// </summary>
    [BepInPlugin("ffcsounds.soundpacks", "FFC Sounds", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;
        internal static ManualLogSource Log;
        internal static ConfigEntry<bool> Enabled;

        private void Awake()
        {
            Instance = this;
            Log = Logger;
            Enabled = Config.Bind("General", "Enabled", true,
                "Play sounds from the \"Sounds\" folder in the game folder for fights and dice, where there are any.");
            try
            {
                CombatSounds.Patch(new Harmony("ffcsounds.soundpacks"));
            }
            catch (Exception e)
            {
                Log.LogError("Could not hook the game's combat: " + e);
            }
            SoundPacks.Init();
        }
    }
}
