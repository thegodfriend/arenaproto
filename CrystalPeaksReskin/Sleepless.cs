using HutongGames.PlayMaker.Actions;
//using ModCommon.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Vasi;

namespace CrystalPeaksReskin
{
    class Sleepless : MonoBehaviour
    {

        private HealthManager _hm;
        private Walker _walker;

        private PlayMakerFSM _control;

        private bool isDunce = false;

        private bool chasing = false;

        private GameObject GSlashEffect;
        private GameObject GSlash1;
        private GameObject GSlash2;
        private GameObject DSlashEffect;
        private GameObject DSlash1;
        private GameObject GSlashSmallEffect;
        private GameObject GSlashSmall1;
        private GameObject farAwayRange;

        private tk2dSpriteAnimator _anim;

        private float singleLungeMult = 4.0f;
        private float tripleLungeMult = 2.0f;
        
        public void Awake()
        {   
            Modding.Logger.Log("In SNoob Awake, placed on " + this.transform.name);

            this.transform.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = CPReskin.Sprites[16].texture;

            _hm = gameObject.GetComponent<HealthManager>();
            _walker = gameObject.GetComponent<Walker>();

            _control = gameObject.LocateMyFSM("ZombieShieldControl");
            _control.CreateBool("shouldTurnInLunge");
            _control.CreateBool("Not Far Away");

            
            farAwayRange = Instantiate(transform.Find("Attack Range").gameObject, this.transform);
            farAwayRange.name = "Far Away Range";
            farAwayRange.transform.localScale *= 3; 
            

            _anim = gameObject.GetComponent<tk2dSpriteAnimator>();

            if (UnityEngine.Random.Range(0f, 100f) < (
                gameObject.scene.name == "Room_Colosseum_Gold"   ? 100 : ( // Dunces appear
                gameObject.scene.name == "Room_Colosseum_Silver" ?   0 : ( // only during the
                gameObject.scene.name == "Room_Colosseum_Bronze" ?   0 : ( // third trial.
                0)))
                )) isDunce = true;
            Modding.Logger.Log("Sleepless isDunce: " + isDunce);

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
                _control.FsmVariables.GetFsmGameObject("Slash Collider").Value.GetComponent<DamageHero>().damageDealt = 2;
                _control.FsmVariables.GetFsmGameObject("Slash Collider 2").Value.GetComponent<DamageHero>().damageDealt = 2;
            } // Aura & extra damage

            {
                GSlashEffect = Instantiate(CPReskin.PreloadedGameObjects["Sly GSlash Effect"], this.transform);
                GSlashEffect.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                GSlash1 = Instantiate(CPReskin.PreloadedGameObjects["Sly GS1"], this.transform);
                GSlash1.transform.localPosition = new Vector3(-0.4f, -2.3f, 0.0f);
                GSlash2 = Instantiate(CPReskin.PreloadedGameObjects["Sly GS2"], this.transform);
                GSlash2.transform.localPosition = new Vector3(-0.4f, -2.3f, 0.0f);

                DSlashEffect = Instantiate(CPReskin.PreloadedGameObjects["Sly DSlash Effect"], this.transform);
                DSlashEffect.transform.localPosition = new Vector3(0.0f, -1.5f, 0.1f);
                DSlash1 = Instantiate(CPReskin.PreloadedGameObjects["Sly DS1"], this.transform);
                DSlash1.transform.localPosition = new Vector3(0.0f, -1.5f, 0.0f);
                if (isDunce)
                {
                    DSlash1.GetComponent<DamageHero>().damageDealt = 2;
                    DSlash1.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
                    DSlashEffect.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
                }
                else {
                    DSlash1.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
                    DSlashEffect.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
                }


                GSlashSmallEffect = Instantiate(CPReskin.PreloadedGameObjects["Sly GSlash Effect"], this.transform);
                GSlashSmallEffect.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                GSlashSmallEffect.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);
                GSlashSmall1 = Instantiate(CPReskin.PreloadedGameObjects["Sly GS1"], this.transform);
                GSlashSmall1.transform.localPosition = new Vector3(-0.4f, -2.3f, 0.0f);
                GSlashSmall1.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);
                GSlashSmall1.GetComponent<DamageHero>().damageDealt = 1;
            } // All the new Slashes' graphics/hitboxes
        }

        public void Start()
        {
            Modding.Logger.Log("In SNoob Start, placed on " + this.transform.name);

            _hm.hp *= (isDunce?8:4); // HP: 65 -> 260 (x4) for Noob, 520 (x8) for Dunce

            Modding.Logger.Log(gameObject.name + " is from the Scene: " + gameObject.scene.name);

            /*
            if (isDunce)
            {

                _hm.hp *= 2; // Double HP (after quadroupling it) for a total of x8 the amount Shielded Fool has. 65 (-> 260) -> 520

            } */

            // Walking is faster
            _control.GetAction<RandomFloat>("Initialise", 6).min = 10f; //6f;
            _control.GetAction<RandomFloat>("Initialise", 6).max = 14f; //10f;

            

            // Evade is twice faster
            _control.GetAction<RandomFloat>("Evade", 2).min = 8f;
            _control.GetAction<RandomFloat>("Evade", 2).max = 12f;
            _control.GetAction<WaitRandom>("Evade", 6).timeMin = 0.2f;
            _control.GetAction<WaitRandom>("Evade", 6).timeMax = 0.4f;

            /*
            _control.GetAction<SendRandomEvent>("Shield Start", 2).weights[0].Value = 30f;
            _control.GetAction<SendRandomEvent>("Shield Start", 2).weights[1].Value = 70f;
            */

            _control.GetAction<RandomInt>("Shield Start", 4).min = (isDunce?5:10);
            _control.GetAction<RandomInt>("Shield Start", 4).max = (isDunce?20:40);

            _control.GetAction<SetFloatValue>("Lunge Speed Set", 0).floatValue =  -16f * (tripleLungeMult * tripleLungeMult);
            _control.GetAction<SetFloatValue>("Lunge Speed Set", 1).floatValue =  -18f * singleLungeMult;
            _control.GetAction<SetFloatValue>("Lunge Speed Set 2", 3).floatValue = 16f * (tripleLungeMult * tripleLungeMult);
            _control.GetAction<SetFloatValue>("Lunge Speed Set 2", 4).floatValue = 18f * singleLungeMult;

            _control.GetAction<SetFloatValue>("Shield Left High", 7).floatValue = -16f * (tripleLungeMult * tripleLungeMult);
            _control.GetAction<SetFloatValue>("Shield Left High", 8).floatValue = -18f * singleLungeMult;
            _control.GetAction<SetFloatValue>("Shield Left Low", 7).floatValue =  -16f * (tripleLungeMult * tripleLungeMult);
            _control.GetAction<SetFloatValue>("Shield Left Low", 8).floatValue =  -18f * singleLungeMult;
            _control.GetAction<SetFloatValue>("Shield Right High", 7).floatValue = 16f * (tripleLungeMult * tripleLungeMult);
            _control.GetAction<SetFloatValue>("Shield Right High", 8).floatValue = 18f * singleLungeMult;
            _control.GetAction<SetFloatValue>("Shield Right Low", 7).floatValue =  16f * (tripleLungeMult * tripleLungeMult);
            _control.GetAction<SetFloatValue>("Shield Right Low", 8).floatValue =  18f * singleLungeMult;

            //_control.GetAction<Wait>("Block Low", 1).time = 0.1f;
            
            // TURN WHILE TRIPLE SLASH
            {

                // ==== FIRST-TO-SECOND LUNGE ====
                _control.GetState("A3 CD 1").InsertMethod(1, () => {

                    float pos = _control.FsmVariables.GetFsmGameObject("Self").Value.transform.position.x;
                    float heroPos = _control.FsmVariables.GetFsmGameObject("Hero").Value.transform.position.x;

                    _control.FsmVariables.GetFsmBool("shouldTurnInLunge").Value = !((pos > heroPos) ^ (_control.FsmVariables.GetFsmFloat("Lunge3 Speed").Value > 0));
                    Modding.Logger.Log("    Should Turn: " + _control.FsmVariables.GetFsmBool("shouldTurnInLunge").Value);
                }); // Detect if turn is needed
                _control.GetState("A3 CD 1").AddAction(new FaceObject()
                {
                    objectA = _control.FsmVariables.GetFsmGameObject("Self"),
                    objectB = _control.FsmVariables.GetFsmGameObject("Hero"),
                    spriteFacesRight = false,
                    playNewAnimation = false,
                    newAnimationClip = "",
                    resetFrame = false,
                    everyFrame = false
                }); // Face Hero
                _control.GetState("A3 CD 1").InsertMethod(2, () => {
                    if (_control.FsmVariables.GetFsmBool("shouldTurnInLunge").Value == true)
                        _control.FsmVariables.GetFsmFloat("Lunge3 Speed").Value *= -1;
                    _control.FsmVariables.GetFsmBool("shouldTurnInLunge").Value = false;
                }); // If turn was needed, turn (flip lunge speed)
                
                // ==== SECOND-TO-THIRD LUNGE ====
                _control.GetState("A3 CD2").InsertMethod(2, () => {

                    float pos = _control.FsmVariables.GetFsmGameObject("Self").Value.transform.position.x;
                    float heroPos = _control.FsmVariables.GetFsmGameObject("Hero").Value.transform.position.x;

                    _control.FsmVariables.GetFsmBool("shouldTurnInLunge").Value = !((pos > heroPos) ^ (_control.FsmVariables.GetFsmFloat("Lunge3 Speed").Value > 0));
                    Modding.Logger.Log("    Should Turn: " + _control.FsmVariables.GetFsmBool("shouldTurnInLunge").Value);
                }); // Detect if turn is needed
                _control.GetState("A3 CD2").AddAction(new FaceObject()
                {
                    objectA = _control.FsmVariables.GetFsmGameObject("Self"),
                    objectB = _control.FsmVariables.GetFsmGameObject("Hero"),
                    spriteFacesRight = false,
                    playNewAnimation = false,
                    newAnimationClip = "",
                    resetFrame = false,
                    everyFrame = false
                });  // Face Hero
                _control.GetState("A3 CD2").InsertMethod(3, () => {
                    if (_control.FsmVariables.GetFsmBool("shouldTurnInLunge").Value == true)
                        _control.FsmVariables.GetFsmFloat("Lunge3 Speed").Value *= -1;
                    _control.FsmVariables.GetFsmBool("shouldTurnInLunge").Value = false;
                }); // If turn was needed, turn (flip lunge speed)

            }

            /*
            transform.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Attack3 A1").fps *= tripleLungeMult;
            transform.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Attack3 L1").fps *= tripleLungeMult;
            transform.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Attack3 S1").fps *= tripleLungeMult;
            transform.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Attack3 L2").fps *= tripleLungeMult;
            transform.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Attack3 L3").fps *= tripleLungeMult;
            transform.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Attack3 S3").fps *= tripleLungeMult;
            */

            transform.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Attack3 CD1").fps *= tripleLungeMult;
            transform.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Attack3 CD2").fps *= tripleLungeMult;
            transform.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Attack3 CD3").fps *= tripleLungeMult;


            _control.ChangeTransition("Block Low", "FINISHED", "Reset");
            //_control.GetState("Block Low").GetAction<Wait>(1).finishEvent = null;
            {
                /*_control.GetState("Block Low").AddAction(new Tk2dPlayAnimation()
                {
                    clipName = "Attack3 L2"
                });*/
                /*_control.GetState("Block Low").AddAction(new Wait()
                {
                    time = 1f,
                    realTime = false
                });*/
                /*
                _control.GetState("Block Low").AddAction(new Tk2dPlayAnimationWithEvents()
                {
                    gameObject = new HutongGames.PlayMaker.FsmOwnerDefault()
                    {
                        OwnerOption = 
                    },
                    clipName = "Attack3 L2",
                    animationCompleteEvent = HutongGames.PlayMaker.FsmEvent.Finished
                });*/
            } // SCRAPPED
            if (isDunce) _control.GetState("Block Low").InsertCoroutine(0, GreatSlash);  // These two lines can, and will, be simplified to one trigger of a GSlash in the polished version.
            else _control.GetState("Block Low").InsertCoroutine(0, GreatSlashSmall);     // That will be done by creating the GSlash GO to fit the Sleepless' type. (See how DSlash did it.)

            _control.ChangeTransition("Block High", "FINISHED", "Reset");                // Block High to mimic Block Low, seems like this is the final choice for how it'll work but unsure
            if (isDunce) _control.GetState("Block High").InsertCoroutine(0, GreatSlash); // See above comments
            else _control.GetState("Block High").InsertCoroutine(0, GreatSlashSmall);    // on these two lines
            
            _control.GetState("Attack1 Slash").InsertCoroutine(0, DashSlash);

            _control.GetState("Evade End").GetAction<SendRandomEvent>(1).weights[0].Value = 0.2f;
            _control.GetState("Evade End").GetAction<SendRandomEvent>(1).weights[1].Value = 0.4f;
            _control.GetState("Evade End").GetAction<SendRandomEvent>(1).weights[2].Value = 0.4f;

            /*
            _control.GetState("Detect").AddAction(new CheckAlertRangeByName()
            {
                storeResult = _control.FsmVariables.GetFsmBool("Not Far Away"),
                childName = "Far Away Range",
                everyFrame = true
            });

            _control.GetState("Detect").AddAction(new BoolNoneTrue
            {
                boolVariables = new HutongGames.PlayMaker.FsmBool[] {
                    _control.FsmVariables.GetFsmBool("Not Far Away") },
                sendEvent = new HutongGames.PlayMaker.FsmEvent("DASH"),
                everyFrame = true
            });

            _control.GetState("Detect").AddTransition("DASH", "Attack 1 Antic");
            */
            

        }

        
        public void Update() {
            if (!chasing && gameObject.GetComponent<LineOfSightDetector>().CanSeeHero && !transform.Find("Far Away Range").gameObject.GetComponent<AlertRange>().IsHeroInRange && _control.ActiveStateName == "Detect") {
                StartCoroutine(ChaseDash());
                
            }
        }

        IEnumerator GreatSlash() {

            yield return new WaitForSeconds(0.35f);

            //_anim.Play("Attack3 L2");
            
            GSlashEffect.SetActive(true);
            GSlash1.SetActive(true);
            GSlash2.SetActive(true);

            yield return new WaitForSeconds(0.015f);

            //_anim.Play("Attack3 CD2");
            GSlash1.SetActive(false);
            GSlash2.SetActive(false);
        }

        IEnumerator DashSlash() {

            //yield return new WaitForSeconds(0.35f);

            //_anim.Play("Attack3 L2");

            DSlashEffect.SetActive(true);
            DSlash1.SetActive(true);

            yield return new WaitForSeconds(0.015f);

            //_anim.Play("Attack3 CD2");
            DSlash1.SetActive(false);
        }
        
        IEnumerator GreatSlashSmall()
        {

            yield return new WaitForSeconds(0.35f);
            
            GSlashSmallEffect.SetActive(true);
            GSlashSmall1.SetActive(true);

            yield return new WaitForSeconds(0.015f);
            
            GSlashSmall1.SetActive(false);
        }
        
        IEnumerator ChaseDash()
        {
            chasing = true;

            yield return new WaitForSeconds(0.2f);

            if (_control.ActiveStateName == "Detect") {
                _walker.Stop(Walker.StopReasons.Controlled);
                yield return new WaitForSeconds(0.1f); //                Base   DSlash modifier                      Direction - To left, -1. To right, 1.                   Chase Modifier
                _control.FsmVariables.GetFsmFloat("Lunge1 Speed").Value = 18f * singleLungeMult * (transform.position.x > HeroController.instance.transform.position.x ? -1 : 1) * 1.3f; 
                _control.SetState("Attack 1 Antic");
            }

            chasing = false;
        }

    }
}
