using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrystalPeaksReskin
{
    class SpinAura : MonoBehaviour
    {
        private float rot;
        

        public void Start() {
            rot = 0f;
            
        }

        public void Update() {

            rot += 1;

            transform.rotation = Quaternion.Euler(0, 0, rot);

            //transform.localScale = new Vector3(1 + 0.2f * (float)Math.Sin((double)(1.5 * rot * Math.PI / 180)), 1 + 0.2f * (float)Math.Sin((double)(1.5 * rot * Math.PI / 180)), 0);

        }

    }
}
