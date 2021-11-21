using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Vasi;

namespace CrystalPeaksReskin
{
    class GovernmentDrone : MonoBehaviour 
    {

        private HealthManager _hm;

        private PlayMakerFSM _control1, _control2;

        private GameObject _spineshot;
        private GameObject _honeyspike;

        public void Awake()
        {
            _hm = gameObject.GetComponent<HealthManager>();
            _spineshot = GameObject.Instantiate(CPReskin.PreloadedGameObjects["QG Husk"].LocateMyFSM("Attack").GetAction<FlingObjectsFromGlobalPool>("Fire", 0).gameObject.Value);
            _honeyspike = GameObject.Instantiate(CPReskin.PreloadedGameObjects["Hive Knight Honeyspike"]);

            _control1 = gameObject.LocateMyFSM("FSM");
            _control2 = gameObject.LocateMyFSM("Mozzie2");
        }

        public void Start()
        {
            _hm.hp *= 2;

            _control2.GetState("Pull Out").InsertMethod(0, () =>
            {
                double playerDir;

                Vector3 playerPos = HeroController.instance.transform.position;
                double x = transform.position.x - playerPos.x;
                double y = transform.position.y - playerPos.y;

                playerDir = Math.Atan(y/x)/0.0174532924f + (x>0?180:0);
                
                SpawnHoneySpike(transform.position, (float)playerDir, 20);
                SpawnHoneySpike(transform.position, (float)playerDir + 15f, 20);
                SpawnHoneySpike(transform.position, (float)playerDir - 15f, 20);

            });
            /*
            _control2.GetState("Hit Left").InsertMethod(0, () =>
            {
                GameObject projectile = Instantiate(_spineshot);
                projectile.transform.SetPosition2D(new Vector2(transform.position.x, transform.position.y));
            });
            _control2.GetState("Hit Up").InsertMethod(0, () =>
            {
                GameObject projectile = Instantiate(_spineshot);
                projectile.transform.SetPosition2D(new Vector2(transform.position.x, transform.position.y));
            });
            _control2.GetState("Hit Down").InsertMethod(0, () =>
            {
                GameObject projectile = Instantiate(_spineshot);
                projectile.transform.SetPosition2D(new Vector2(transform.position.x, transform.position.y));
            });
            */
        }

        private GameObject SpawnSpine(Vector3 position, float rotation) {

            GameObject spine = Instantiate(_spineshot);
            spine.SetActive(true);

            spine.transform.localPosition = position;
            spine.transform.localRotation = Quaternion.Euler(0, 0, rotation);
            
            return spine;
        }

        private GameObject SpawnHoneySpike(Vector3 position, float rotation, float speed)
        {
            //Modding.Logger.Log("Spawning Honeyspike");

            GameObject Spike = GameObject.Instantiate(_honeyspike);
            Spike.SetActive(true);

            Spike.transform.localPosition = position;
            Spike.transform.localRotation = Quaternion.Euler(0, 0, rotation);

            
            var hks = Spike.GetComponent<HiveKnightStinger>();
            Destroy(hks);
            //Spike.AddComponent<HoneySpikeMovement>();
            
            Rigidbody2D rb = Spike.GetComponent<Rigidbody2D>();

            float x = speed * Mathf.Cos(rotation * 0.0174532924f);
            float y = speed * Mathf.Sin(rotation * 0.0174532924f);
            Vector2 velocity;
            velocity.x = x;
            velocity.y = y;
            rb.velocity = velocity;

            return Spike;
        }

    }
}
