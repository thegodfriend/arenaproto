using HutongGames.PlayMaker.Actions;
using ModCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrystalPeaksReskin
{
    class AssholeNoob : MonoBehaviour

    {
        private HealthManager _hm;

        private PlayMakerFSM _control;

        private bool isDunce = false;

        public void Awake()
        {
            Modding.Logger.Log("In ANoob Awake, placed on " + this.transform.name);

            this.transform.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = CPReskin.Sprites[16].texture;

            _hm = gameObject.GetComponent<HealthManager>();

            _control = gameObject.LocateMyFSM("Flying Sentry Nail");

            if (UnityEngine.Random.Range(0f, 100f) < (
                gameObject.scene.name == "Room_Colosseum_Gold"   ? 20 : (
                gameObject.scene.name == "Room_Colosseum_Silver" ? 5  : (
                0))
                )) isDunce = true;
            
        }

        public void Start()
        {
            Modding.Logger.Log("In ANoob Start, placed on " + this.transform.name);

            _hm.hp *= 2; // HP: 70 -> 140

            Modding.Logger.Log(gameObject.name + " is from the Scene: " + gameObject.scene.name);


            if (isDunce)
            {
                Modding.Logger.Log("Placing ADunce on " + this.transform.name);
                gameObject.AddComponent<AssholeDunce>();
                Modding.Logger.Log("Placed ADunce on " + this.transform.name);
            }

            _control.GetAction<WaitRandom>("Idle", 1).timeMax = 1f; // Idles less before swinging; rapid attacks. Min wait: 0.5, Max wait: 1 (down from 1.5)

        }
    }
}
