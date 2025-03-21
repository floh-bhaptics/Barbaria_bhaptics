﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Il2Cpp;
using Il2CppECS;
using UnityEngine;
using System.Runtime.ExceptionServices;
using UnityEngine.Playables;

[assembly: MelonInfo(typeof(Barbaria_bhaptics.Barbaria_bhaptics), "Barbaria_bhaptics", "2.0.1", "Florian Fahrenberger")]
[assembly: MelonGame("Stalwart Games", "Barbaria")]

namespace Barbaria_bhaptics
{
    public class Barbaria_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr = null!;
        public static int playerEntityID = 0;
        public static int rightWeapon = 0;
        public static int leftWeapon = 0;

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

        [HarmonyPatch(typeof(PossessFx), "PlayHeroFx")]
        public class bhaptics_PossessHero
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("EnterHero");
            }
        }

        [HarmonyPatch(typeof(PossessFx), "PlayImmortalFx")]
        public class bhaptics_PossessImmortal
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("EnterHero");
            }
        }

        /*
        [HarmonyPatch(typeof(BaseController), "PlayHaptic", new Type[] { typeof(HapticType) })]
        public class bhaptics_PlayHaptic
        {
            [HarmonyPostfix]
            public static void Postfix(BaseController __instance, HapticType type)
            {
                if (type < 0) return;
                //tactsuitVr.LOG("Haptic: " + type.ToString());
                bool isRightHand = true;
                //isRightHand = (__instance.)
                
                switch (type)
                {
                    case HapticType.None:
                        return;
                    case HapticType.Default:
                        //tactsuitVr.Recoil(isRightHand);
                        break;
                    case HapticType.SwordHitSword:
                        tactsuitVr.Recoil(isRightHand);
                        break;
                    case HapticType.SwordHitShield:
                        tactsuitVr.Recoil(isRightHand);
                        break;
                    case HapticType.SwordHitBody:
                        tactsuitVr.Recoil(isRightHand);
                        break;
                    default:
                        return;
                }
                
            }
        }
        */

        [HarmonyPatch(typeof(SysPlayerHitDisplay), "DamageTaken", new Type[] { typeof(int), typeof(CombatHit) })]
        public class bhaptics_PlayerDamage
        {
            [HarmonyPostfix]
            public static void Postfix(SysPlayerHitDisplay __instance, int playerControlledEntityId, CombatHit hit)
            {
                /*
                if (hit.attackFighterEntityId == playerControlledEntityId)
                {
                    //possible attacks go here
                    if (hit.attackEntityId == rightWeapon) tactsuitVr.Recoil(true);
                    if (hit.attackEntityId == leftWeapon) tactsuitVr.Recoil(false);
                    return;
                }
                */
                //tactsuitVr.LOG("HitEntityID: " + hit.hitFighterEntityId.ToString() + " " + hit.attackFighterEntityId.ToString() + " " + playerEntityID.ToString());
                if (hit.hitFighterEntityId != playerEntityID) return;
                string pattern = "HitImpact";
                if (hit.freeze) pattern = "HitFreeze";
                if (hit.headShot) pattern = "HitInTheFace";
                if (hit.shock) pattern = "HitShock";
                if (hit.stab) pattern = "HitStab";
                if (hit.burn) pattern = "HitBurn";
                Vector3 patternOrigin = new Vector3(0f, 0f, 1f);
                Vector3 flattenedHit = new Vector3(hit.direction.x, 0f, hit.direction.z);
                float earlyhitAngle = Vector3.Angle(flattenedHit, patternOrigin);
                Vector3 earlycrossProduct = Vector3.Cross(flattenedHit, patternOrigin);
                if (earlycrossProduct.y > 0f) { earlyhitAngle *= -1f; }
                float myRotation = earlyhitAngle;
                if (earlycrossProduct.y > 0f) { earlyhitAngle *= -1f; }
                //myRotation *= -1f;
                if (myRotation < 0f) { myRotation = 360f + myRotation; }
                tactsuitVr.PlayBackHit(pattern, myRotation, 0.0f);
            }
        }

        [HarmonyPatch(typeof(UIScreenHitEffect), "SetLowHealth", new Type[] { typeof(bool) })]
        public class bhaptics_LowHealth
        {
            [HarmonyPostfix]
            public static void Postfix(bool lowHealth)
            {
                if (lowHealth) tactsuitVr.StartHeartBeat();
                else tactsuitVr.StopHeartBeat();
            }
        }

        [HarmonyPatch(typeof(UIScreenHitEffect), "SetDead", new Type[] { typeof(bool) })]
        public class bhaptics_SetDead
        {
            [HarmonyPostfix]
            public static void Postfix(bool dead)
            {
                if (dead) tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(WorldManager), "GetPlayerControlledEntity", new Type[] {  })]
        public class bhaptics_WorldManagerUpdate
        {
            [HarmonyPostfix]
            public static void Postfix(WorldManager __instance, ref int __result)
            {
                playerEntityID = __result;
            }
        }

    }
}
