using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using ModCommon.Util;

namespace CrystalPeaksReskin
{
    internal class Methamphetaminer : MonoBehaviour
    {

        private PlayMakerFSM _control;

        bool isAngry = false;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Zombie Miner");
            
        }

        private void Start ()
        {

            _control.InsertMethod("Startled?", 0, () => {
                if (!isAngry)
                {
                    isAngry = true;
                    StartCoroutine(ReskinAngry());
                }
            });

        }

        private IEnumerator ReskinAngry() {
            yield return null;
            gameObject.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = CPReskin.Sprites[2].texture;

        }

    }
}
