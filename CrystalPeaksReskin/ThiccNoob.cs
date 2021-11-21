using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Vasi;

namespace CrystalPeaksReskin
{
    class ThiccNoob : MonoBehaviour

    {
        private HealthManager _hm;

        private PlayMakerFSM _control;

        private bool isDunce = false;

        public void Awake()
        {
            Modding.Logger.Log("In TNoob Awake, placed on " + this.transform.name);
            
            this.transform.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = CPReskin.Sprites[15].texture;

            _hm = gameObject.GetComponent<HealthManager>();
            

            _control = gameObject.LocateMyFSM("Zombie Miner");

            
            if (UnityEngine.Random.Range(0f, 100f) < (
                gameObject.scene.name == "Room_Colosseum_Gold"   ? 20 : (
                gameObject.scene.name == "Room_Colosseum_Bronze" ?  50 : (
                0))
                )) isDunce = true;

            if (isDunce)
            {
                GameObject aura = new GameObject();
                aura.AddComponent<SpriteRenderer>();
                aura.GetComponent<SpriteRenderer>().sprite = CPReskin.Sprites[14];
                aura.SetActive(true);
                aura.transform.parent = this.transform;
                aura.transform.localPosition = new Vector3(0, 0, 0);
                aura.transform.localRotation = Quaternion.Euler(0, 0, 0);
                aura.AddComponent<SpinAura>();

                transform.GetComponent<DamageHero>().damageDealt = 2;
                _control.FsmVariables.GetFsmGameObject("Slash").Value.GetComponent<DamageHero>().damageDealt = 2;
            }
        }

        public void Start()
        {


            _hm.hp *= (isDunce?4:2); // HP: 80 -> 160 (x2) for Noob, 320 (x4) for Dunce
            
            _control.FsmVariables.FindFsmFloat("Slash Speed").Value *= (isDunce ? 2.5f : 1.5f);

            _control.GetAction<SetFloatValue>("Evade Start", 3).floatValue.Value *= (isDunce ? 2.5f : 1.5f);
            _control.GetAction<SetFloatValue>("Evade Start", 5).floatValue.Value *= (isDunce ? 2.5f : 1.5f);

        }

    }
}
