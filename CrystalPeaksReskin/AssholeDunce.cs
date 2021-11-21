using HutongGames.PlayMaker.Actions;
using ModCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrystalPeaksReskin
{
    class AssholeDunce : MonoBehaviour

    {
        private HealthManager _hm;

        private PlayMakerFSM _control;

        public void Awake()
        {
            Modding.Logger.Log("In ADunce Awake, placed on " + this.transform.name);

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

            _control = gameObject.LocateMyFSM("Flying Sentry Nail");
        }

        public void Start()
        {
            Modding.Logger.Log("In ADunce Start, placed on " + this.transform.name);

            _hm.hp *= 2; // Double HP a second time for a total of x4 the amount Winged Fool has. 70 (-> 140) -> 280

            // Minimal to no idling before swinging; incredibly rapid attacking
            _control.GetAction<WaitRandom>("Idle", 3).timeMax = 0.5f; // Fool: 1.5, Noob: 1, Dunce: 0.5
            _control.GetAction<WaitRandom>("Idle", 3).timeMin = 0f; // Down from 0.5


        }
    }
}
