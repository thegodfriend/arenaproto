using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ModCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrystalPeaksReskin
{
    class MethHead : MonoBehaviour
    {

        private HealthManager _hm;

        private PlayMakerFSM _control;
        
        public void Awake() {

            transform.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = CPReskin.Sprites[7].texture;

            _hm = gameObject.GetComponent<HealthManager>();

            _control = gameObject.LocateMyFSM("Beam Miner");
            
        }

        public void Start() {

            Modding.Logger.Log("In MH Start, placed on " + this.transform.name);

            _control.GetAction<Wait>("Beam Antic", 14).time = 0.3f; // Fast beam aim
            _control.GetAction<Wait>("Beam", 5).time = 0.3f; // Quick beam
            _control.GetAction<WaitRandom>("Idle", 1).timeMax = 0.3f; // Rapid attacks

            _control.GetAction<SendRandomEventV3>("Choice", 0).eventMax[0].Value = 5; // Max-in-a-row 5 Laser
            _control.GetAction<SendRandomEventV3>("Choice", 0).eventMax[1].Value = 3; // Max-in-a-row 3 Jump
            _control.GetAction<SendRandomEventV3>("Choice", 0).eventMax[2].Value = 1; // Max-in-a-row 1 Scream (no stacking, so leaves boss vulnerable if more)
            _control.GetAction<SendRandomEventV3>("Choice", 0).missedMax[0].Value = 3; // Max-miss-in-a-row 3 without Laser
            _control.GetAction<SendRandomEventV3>("Choice", 0).missedMax[1].Value = 5; // Max-miss-in-a-row 5 without Jump
            _control.GetAction<SendRandomEventV3>("Choice", 0).missedMax[2].Value = 7; // Max-miss-in-a-row 7 without Scream

            //_control.GetAction<FloatMultiply>("Aim Jump", 3).multiplyBy = 3; // Double size leaps, higher tendency to leap to edges
            _control.InsertMethod("Aim Jump", 4, () => { _control.FsmVariables.FindFsmFloat("Jump Distance").Value = RandomJumpDistance(_control.FsmVariables.FindFsmFloat("Jump Min X").Value, _control.FsmVariables.FindFsmFloat("Jump Max X").Value); });


            // ======== EDITING TRANSITIONS ========
            {// fsm.AddTransition("State1", "New_Event", "State2");
             // fsm.ChangeTransition("State1", "Event", "State2");
             // fsm.RemoveTransition("State", "Event");
            }

            /*
            _control.CopyState("Face Hero", "Face For Charge");
            _control.RemoveTransition("Face For Charge", "LEFT");
            _control.RemoveTransition("Face For Charge", "RIGHT");
            _control.AddTransition("Face For Charge", "FINISHED", "State2");
            */

            //_control.CopyState("Launch", "State2");
            //_control.GetAction<FloatMultiply>("Aim Jump", 3).multiplyBy = 3; // Double size leaps, higher tendency to leap to edges
            //_control.InsertMethod("Launch", 0, () => { _control.FsmVariables.FindFsmFloat("Jump Distance").Value = RandomJumpDistance(_control.FsmVariables.FindFsmFloat("Jump Min X").Value, _control.FsmVariables.FindFsmFloat("Jump Max X").Value); });
            //_control.GetAction<SetVelocity2d>("Launch", 1);
            _control.GetAction<SetVelocity2d>("Launch", 1).y.Value = _control.FsmVariables.FindFsmFloat("Jump Force").Value/20;
            
            //_control.GetAction<Tk2dPlayAnimation>("Beam Antic", 6).clipName.Value = "Run";
        }

        private float RandomJumpDistance(float m1, float m2) {
            
            float baseTarX = UnityEngine.Random.Range(m1, m2);
            float curX = _control.FsmVariables.FindFsmFloat("Self X").Value;
            float distX = baseTarX - curX;

            {
            /*
            if (curX < baseTarX) {
                distX = (curX - m1) * Mathf.Sin((Mathf.PI * (baseTarX - curX)) / (2 * (curX - m1)));
            }
            if (baseTarX < curX)
            {
                distX = (curX - m2) * Mathf.Sin((Mathf.PI * (baseTarX - curX)) / (2 * (curX - m2)));
            }
            */} // Trig Method

            if (curX < baseTarX)
            {
                distX = Mathf.Pow((baseTarX - m1), 4) / Mathf.Pow((curX - m1), 3) + m1 - curX;
            }
            if (baseTarX < curX)
            {
                distX = Mathf.Pow((baseTarX - m2), 4) / Mathf.Pow((curX - m2), 3) + m2 - curX;
            }

            distX *= 1.5f;

            Modding.Logger.Log("Crystal Guardian Jump:\n    Current X     : " + curX + "\n    Base Target X : " + baseTarX + "\n    Base Distance : " + ((baseTarX-curX)*1.5f) + "\n    Distance      : " + distX);
            return distX;
        }

    }
}
