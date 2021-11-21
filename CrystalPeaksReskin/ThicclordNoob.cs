using HutongGames.PlayMaker.Actions;
using ModCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrystalPeaksReskin
{
    class ThicclordNoob : MonoBehaviour
    {

        private HealthManager _hm;

        private PlayMakerFSM _control;

        private bool isDunce = false;

        public void Awake()
        {
            Modding.Logger.Log("In TlNoob Awake, placed on " + this.transform.name);

            this.transform.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = CPReskin.Sprites[16].texture;

            _hm = gameObject.GetComponent<HealthManager>();

            _control = gameObject.LocateMyFSM("Ruins Sentry Fat");

            if (UnityEngine.Random.Range(0f, 100f) < (
                gameObject.scene.name == "Room_Colosseum_Gold"   ? 20 : (
                gameObject.scene.name == "Room_Colosseum_Silver" ? 5  : (
                0))
                )) isDunce = true;
            //Modding.Logger.Log("Thicclord isDunce: " + isDunce);
        }


        public void Start()
        {
            Modding.Logger.Log("In TlNoob Start, placed on " + this.transform.name);

            _hm.hp *= 2; // HP: 90 -> 180

            Modding.Logger.Log(gameObject.name + " is from the Scene: " + gameObject.scene.name);

            
            if (isDunce)
            {
                Modding.Logger.Log("Placing TlDunce on " + this.transform.name);
                gameObject.AddComponent<ThicclordDunce>();
                Modding.Logger.Log("Placed TlDunce on " + this.transform.name);
            }
            
            _control.InsertMethod("Launch", 0, () =>
            {
                _control.FsmVariables.FindFsmFloat("Jump Distance").Value *= 1.25f;
                _control.FsmVariables.FindFsmFloat("Jump Distance").Value -= (3f * gameObject.transform.localScale.x);
            });

            // Faster charge, less charge time, and better decelaration
            _control.FsmVariables.FindFsmFloat("Charge Speed").Value *= 1.5f; // 14f -> 21f
            _control.GetAction<Wait>("Charge", 9).time.Value /= 2f; // 1f -> 0.5f
            _control.GetAction<DecelerateXY>("Charge End", 1).decelerationX.Value /= 1.5f; // 0.85f -> 0.56f
            //                                                                                A reminder that smaller numbers are better deceleration
            // Rapid attacking
            _control.GetAction<WaitRandom>("Attack CD", 0).timeMin = 0f; // Min: 0, Max: 0.5

        }

    }
}
