using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Vasi;

namespace CrystalPeaksReskin
{
    class Mexican : MonoBehaviour
    {

        private HealthManager _hm;

        private Recoil _recoil;

        private PlayMakerFSM _control;

        //private GameObject shockwaves;

        public void Awake()
        {

            _hm = gameObject.GetComponent<HealthManager>();

            _control = gameObject.LocateMyFSM("Hopper");

            if (transform.name.Contains("Giant Hopper")) {
                this.transform.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = CPReskin.Sprites[18].texture;
            } // Obese Mexican
            else // Regular Mexican
                this.transform.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = CPReskin.Sprites[17].texture;

            _recoil = gameObject.GetComponent<Recoil>();

        }

        public void Start()
        {
            _hm.hp += 30; // 50  -> 80 / 160 -> 260

            _recoil.enabled = false;

            if (transform.name.Contains("Giant Hopper"))
            {
                _control.GetState("Land Anim").InsertMethod(0, () =>
                {
                    // Copied from Traitor God (GroundPound.cs) 
                    
                    Vector3 pos = transform.position;

                    bool[] facingRightBools = { false, true };
                    foreach (bool @bool in facingRightBools)
                    {
                        GameObject shockwave = Instantiate(CPReskin.PreloadedGameObjects["GPZ"].LocateMyFSM("Control").GetAction<SpawnObjectFromGlobalPool>("Land Waves").gameObject.Value);
                        PlayMakerFSM shockFSM = shockwave.LocateMyFSM("shockwave");
                        shockFSM.transform.localScale = new Vector2(1.2f, 1f);
                        shockFSM.FsmVariables.FindFsmBool("Facing Right").Value = @bool;
                        shockFSM.FsmVariables.FindFsmFloat("Speed").Value = 20f;
                        shockwave.AddComponent<DamageHero>().damageDealt = 1;

                        shockwave.SetActive(true);
                        shockwave.transform.SetPosition2D(new Vector2(pos.x, pos.y - 2f));
                    }
                });
            }

        }

    }
}
