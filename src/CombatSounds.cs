using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace FFCSounds
{
    /// <summary>
    /// Where in the game each sound plays. The combat logic shows hits, rounds and results through a few methods,
    /// so small postfixes on those know exactly what just happened. The game's own hit and dice sounds are only
    /// replaced when the current set actually has sounds for them.
    /// </summary>
    internal static class CombatSounds
    {
        public static void Patch(Harmony harmony)
        {
            System.Type t = typeof(CombatSounds);
            harmony.Patch(AccessTools.Method(typeof(EnemySheet), nameof(EnemySheet.TakeHit)),
                postfix: new HarmonyMethod(t, nameof(EnemyTookHit)));
            harmony.Patch(AccessTools.Method(typeof(AdventureSheet), nameof(AdventureSheet.TakeHitAll)),
                postfix: new HarmonyMethod(t, nameof(PlayerTookHit)));
            harmony.Patch(AccessTools.Method(typeof(EnemySheet), nameof(EnemySheet.ShowAttackRoutine)),
                postfix: new HarmonyMethod(t, nameof(AttackRound)));
            harmony.Patch(AccessTools.Method(typeof(CombatController), nameof(CombatController.CheckWin)),
                postfix: new HarmonyMethod(t, nameof(CheckWin)));
            harmony.Patch(AccessTools.Method(typeof(CombatController), nameof(CombatController.CheckLose)),
                postfix: new HarmonyMethod(t, nameof(CheckLose)));
            harmony.Patch(AccessTools.Method(typeof(CombatController), nameof(CombatController.Lucky)),
                postfix: new HarmonyMethod(t, nameof(Lucky)));
            harmony.Patch(AccessTools.Method(typeof(CombatController), nameof(CombatController.Unlucky)),
                postfix: new HarmonyMethod(t, nameof(Unlucky)));
            harmony.Patch(AccessTools.Method(typeof(BBSoundFX), nameof(BBSoundFX.BBPlayCombatHit)),
                prefix: new HarmonyMethod(t, nameof(GameHitSound)));
            harmony.Patch(AccessTools.Method(typeof(BBSoundFX), nameof(BBSoundFX.BBPlayRoll)),
                prefix: new HarmonyMethod(t, nameof(GameDiceSound)));
        }

        private static bool InCombat => CombatController.instance != null && CombatController.instance.inCombat;

        /// <summary>The game keeps two enemy sheets (book and flow styles); react only to the one in use.</summary>
        private static bool IsActiveSheet(EnemySheet sheet)
        {
            return CombatController.instance != null && CombatController.instance.enemySheet == sheet;
        }

        private static void EnemyTookHit(EnemySheet __instance, string target)
        {
            if (target == "enemy" && IsActiveSheet(__instance)) SoundPacks.Play(SoundPacks.PlayerHit);
        }

        private static void PlayerTookHit()
        {
            if (InCombat) SoundPacks.Play(SoundPacks.EnemyHit);
        }

        /// <summary>
        /// A round's dice are shown by a coroutine that writes both attack strengths when done. Its postfix runs when
        /// the round starts, so wait for the result, then play the draw sound if nobody won.
        /// </summary>
        private static void AttackRound(EnemySheet __instance, int[] player_dice, int[] enemy_dice)
        {
            if (player_dice.Length > 0 && enemy_dice.Length > 0 && IsActiveSheet(__instance))
            {
                Plugin.Instance.StartCoroutine(WaitForDraw(__instance));
            }
        }

        private static IEnumerator WaitForDraw(EnemySheet sheet)
        {
            yield return null;
            float deadline = Time.unscaledTime + 6f;
            while (string.IsNullOrEmpty(sheet.EnemyAttackValue.text) && Time.unscaledTime < deadline)
            {
                yield return null;
            }
            if (sheet.player_total == sheet.enemy_total)
            {
                SoundPacks.Play(SoundPacks.Draw);
            }
        }

        private static void CheckWin(bool __result)
        {
            if (__result) SoundPacks.Play(SoundPacks.EnemyDefeated);
        }

        private static void CheckLose(bool __result)
        {
            if (__result) SoundPacks.Play(SoundPacks.PlayerDefeated);
        }

        private static void Lucky()
        {
            SoundPacks.Play(SoundPacks.Lucky);
        }

        private static void Unlucky()
        {
            SoundPacks.Play(SoundPacks.Unlucky);
        }

        /// <summary>
        /// The game plays one generic sound for every hit. If the current set has its own hit sounds, ours play from
        /// the hooks above (which know who was hit), so skip the game's.
        /// </summary>
        private static bool GameHitSound()
        {
            return !(SoundPacks.Has(SoundPacks.PlayerHit) || SoundPacks.Has(SoundPacks.EnemyHit));
        }

        /// <summary>Dice: play the set's own sound if it has one; otherwise let the game play its usual one.</summary>
        private static bool GameDiceSound()
        {
            return !SoundPacks.Play(SoundPacks.Dice);
        }
    }
}
