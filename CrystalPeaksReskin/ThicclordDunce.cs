using HutongGames.PlayMaker.Actions;
using ModCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrystalPeaksReskin
{
    class ThicclordDunce : MonoBehaviour

    {
        private HealthManager _hm;

        private PlayMakerFSM _control;

        public void Awake()
        {
            Modding.Logger.Log("In TlDunce Awake, placed on " + this.transform.name);

            // Aura
            GameObject aura = new GameObject();
            aura.AddComponent<SpriteRenderer>();
            aura.GetComponent<SpriteRenderer>().sprite = CPReskin.Sprites[14];
            aura.SetActive(true);
            aura.transform.parent = this.transform;
            aura.transform.localPosition = new Vector3(0, 0, 0);
            aura.transform.localRotation = Quaternion.Euler(0, 0, 0);
            aura.AddComponent<SpinAura>();

            _hm = gameObject.GetComponent<HealthManager>();

            _control = gameObject.LocateMyFSM("Ruins Sentry Fat");
        }

        public void Start()
        {
            Modding.Logger.Log("In TlDunce Start, placed on " + this.transform.name);

            _hm.hp *= 2; // Double HP a second time for a total of x4 the amount Heavy Fool has. 90 (-> 180) -> 360

            // Even faster charge, even less charge time, and even better decelaration
            _control.FsmVariables.FindFsmFloat("Charge Speed").Value *= 1.5f; // 14f (-> 21f) -> 31.5f
            _control.GetAction<Wait>("Charge", 9).time.Value /= 1.5f; // 1f (-> 0.5f) -> 0.33f
            _control.GetAction<DecelerateXY>("Charge End", 1).decelerationX.Value /= 1.5f; // 0.85f (-> 0.56f) -> 0.37f
            //                                                                                A reminder that smaller numbers are better deceleration
            // Rapid attacking
            _control.GetAction<WaitRandom>("Attack CD", 0).timeMax = 0.25f; // Min: 0, Max: 0.25

        }
    }
}
