using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Il2Cpp;

[assembly: MelonInfo(typeof(Barbaria_bhaptics.Barbaria_bhaptics), "Barbaria_bhaptics", "1.0.0", "Florian Fahrenberger")]
[assembly: MelonGame("Stalwart Games", "Barbaria")]

namespace Barbaria_bhaptics
{
    public class Barbaria_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr = null!;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        [HarmonyPatch(typeof(CombatManager), "KillPlayer", new Type[] { })]
        public class bhaptics_PlayerDies
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
                tactsuitVr.PlaybackHaptics("ExitHero");
            }
        }

        [HarmonyPatch(typeof(CombatManager), "ProcessHit", new Type[] { typeof(CombatHit), typeof(Il2CppECS.CollisionInfo) })]
        public class bhaptics_ProcessHit
        {
            [HarmonyPostfix]
            public static void Postfix(CombatManager __instance, CombatHit hit, Il2CppECS.CollisionInfo collision)
            {
                if (hit.headShot) tactsuitVr.LOG("Headshot");
                if (hit.landed) { tactsuitVr.PlaybackHaptics("RecoilBladeVest_R"); tactsuitVr.LOG("Damage: " + hit.damage.ToString()); }
            }
        }

        [HarmonyPatch(typeof(PossessFx), "PlayHeroFx")]
        public class bhaptics_PossessHero
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.LOG("PlayHeroFx");
                tactsuitVr.PlaybackHaptics("EnterHero");
            }
        }

        [HarmonyPatch(typeof(PossessFx), "PlayImmortalFx")]
        public class bhaptics_PossessImmortal
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.LOG("PlayImmortalFx");
                tactsuitVr.PlaybackHaptics("EnterHero");
            }
        }

        [HarmonyPatch(typeof(MoodMusic), "OnPossessionChange", new Type[] { typeof(bool) })]
        public class bhaptics_PossessionChange
        {
            [HarmonyPostfix]
            public static void Postfix(bool enteringHero)
            {
                if (enteringHero) tactsuitVr.PlaybackHaptics("EnterHero");
                else tactsuitVr.PlaybackHaptics("ExitHero");
            }
        }

        [HarmonyPatch(typeof(CombatEffect), "ApplyEffectDamage", new Type[] { typeof(CombatEffectDef), typeof(int), typeof(int), typeof(UnityEngine.Vector3), typeof(UnityEngine.Vector3), typeof(CombatHit), typeof(PotentialDamage) })]
        public class bhaptics_PlayerDamage
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.LOG("ApplyEffectDamage");
            }
        }

        [HarmonyPatch(typeof(CombatEffect), "ApplyEffectHeal", new Type[] { typeof(CombatEffectDef) })]
        public class bhaptics_PlayerHeal
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.LOG("Heal");
                tactsuitVr.StopHeartBeat();
            }
        }

        [HarmonyPatch(typeof(UIScreenHitEffect), "SetLowHealth", new Type[] { typeof(bool) })]
        public class bhaptics_LowHealth
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.LOG("SetLowHealth");
                tactsuitVr.StartHeartBeat();
            }
        }

    }
}
