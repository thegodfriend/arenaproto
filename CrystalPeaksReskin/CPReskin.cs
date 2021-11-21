using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using System.Reflection;
using System.IO;
using System.Collections;
using On;
using ModCommon.Util;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;

namespace CrystalPeaksReskin
{
    /*
     * 000 - Meth Hunter                    : Crystal Hunter ("Crystal Flyer")
     * 001 - Meth Slitherer                 : Crystal Crawler ("Crystallised Lazer Bug")
     * 002 - Methamphetaminer               : Husk Miner ("Zombie Miner 1")
     * 003 - Methamphetaminer, alternate version
     * 004 - Crackback                      : Glimback ("Crystal Crawler")
     * 005 - Methamphetamite                : Shardmite ("Mines Crawler")
     * 006 - Methminded Cadaver             : Crystallised Husk ("Zombie Beam Miner")
     * 007 - Meth Head (Pineapple)          : Crystal Guardian ("Mega Zombie Beam Miner")
     *                                        Enraged Guardian ("Zombie Beam Miner Rematch")
     * 008 - Breakable Crystal 1
     * 009 - Breakable Crystal 2
     * 010 - Breakable Crystal 3
     * 011 - Breakable Crystal 4
     * 
     * 012
     * 
     * 013 - Demon Primal Aspid
     * 014 - Primal Aspid Aura
     * 
     * 015 - Thicc Dunce                    : Sturdy Fool ("Colosseum_Miner")
     * 016 - Other Dunces                   : Shielded Fool ("Colosseum_Shield_Zombie")
     *                                        Winged Fool ("Colosseum_Flying_Sentry")
     *                                        Heavy Fool ("Colosseum_Worm")
     * 017 - Mexican                        : Hopper
     * 018 - Obese Mexican                  : Great Hopper
    */

    public class CPReskin : Mod
    {

        public CPReskin() : base("CPReskin") { }// : Meth Head Testing") { }



        public static readonly List<Sprite> Sprites = new List<Sprite>();

        public static readonly Dictionary<string, GameObject> PreloadedGameObjects = new Dictionary<string, GameObject>();

        public override string GetVersion()
        {
            return "Colosseum Testing : Sturdy Fools : 1.0.2.2"; //"0.0.2.15 : Testing states : 12";
        }

        
        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("GG_Grey_Prince_Zote", "Grey Prince"),
                ("GG_Sly", "Battle Scene/Sly Boss/GSlash Effect"),
                ("GG_Sly", "Battle Scene/Sly Boss/GS1"),
                ("GG_Sly", "Battle Scene/Sly Boss/GS2"),
                ("GG_Sly", "Battle Scene/Sly Boss/DSlash Effect"),
                ("GG_Sly", "Battle Scene/Sly Boss/DS1"),
                ("Fungus3_34", "Garden Zombie"),
                ("GG_Hive_Knight", "Battle Scene/Globs/Hive Knight Glob/Stingers/Stinger"),
                ("Room_Colosseum_Gold","Colosseum Manager/Waves/Wave 50/Colosseum Cage Large"),
                ("Room_Final_Boss_Core", "Boss Control/Title"),
                ("GG_Hollow_Knight", "Battle Scene/Title")
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Storing GameObjects");
            PreloadedGameObjects.Add("Sly GSlash Effect", preloadedObjects["GG_Sly"]["Battle Scene/Sly Boss/GSlash Effect"]);
            PreloadedGameObjects.Add("Sly GS1", preloadedObjects["GG_Sly"]["Battle Scene/Sly Boss/GS1"]);
            PreloadedGameObjects.Add("Sly GS2", preloadedObjects["GG_Sly"]["Battle Scene/Sly Boss/GS2"]);
            PreloadedGameObjects.Add("Sly DSlash Effect", preloadedObjects["GG_Sly"]["Battle Scene/Sly Boss/DSlash Effect"]);
            PreloadedGameObjects.Add("Sly DS1", preloadedObjects["GG_Sly"]["Battle Scene/Sly Boss/DS1"]);
            PreloadedGameObjects.Add("GPZ", preloadedObjects["GG_Grey_Prince_Zote"]["Grey Prince"]);
            PreloadedGameObjects.Add("QG Husk", preloadedObjects["Fungus3_34"]["Garden Zombie"]);
            PreloadedGameObjects.Add("Hive Knight Honeyspike", preloadedObjects["GG_Hive_Knight"]["Battle Scene/Globs/Hive Knight Glob/Stingers/Stinger"]);
            PreloadedGameObjects.Add("Sturdy Fool Cage", preloadedObjects["Room_Colosseum_Gold"]["Colosseum Manager/Waves/Wave 50/Colosseum Cage Large"]);
            PreloadedGameObjects.Add("THK Title", preloadedObjects["Room_Final_Boss_Core"]["Boss Control/Title"]);
            PreloadedGameObjects.Add("PV Title", preloadedObjects["GG_Hollow_Knight"]["Battle Scene/Title"]);

            Log("Initializing...");
            

            USceneManager.activeSceneChanged += SceneChanged;
            //ModHooks.Instance.SceneChanged += SceneLoaded;
            ModHooks.Instance.LanguageGetHook += LanguageGet;
            ModHooks.Instance.OnEnableEnemyHook += EnemyEnabled;
            On.HeroController.Start += HeroControllerStart;
            On.EnemyDreamnailReaction.RecieveDreamImpact += EnemyDreamnailHit;
            On.InfectedEnemyEffects.RecieveHitEffect += EnemyHitInfectedEffects;
            On.EnemyDeathEffects.EmitInfectedEffects += EnemyDeathInfectedEffects;
            On.PlayMakerFSM.OnEnable += OnFsmEnable;



            // Based off Traitor God, who in turn is based off Pale Champion, and Transtrans. Mainly Traitor God. Like, very very much. A lot. Almost character-for-character.
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (string resource in assembly.GetManifestResourceNames())
            {
                if (!resource.EndsWith(".png")) continue;

                using (Stream stream = assembly.GetManifestResourceStream(resource))
                {
                    if (stream == null) continue;

                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    stream.Dispose();

                    var texture = new Texture2D(1, 1);
                    texture.LoadImage(buffer, true);

                    Sprites.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), (200/3f)));

                }

            }

            /*GameObject sturdyFool = PreloadedGameObjects["Sturdy Fool Cage"].LocateMyFSM("Spawn").FsmVariables.GetFsmGameObject("Enemy").Value;
            GameObject sturdyFoolProjectile = sturdyFool.LocateMyFSM("Zombie Miner").GetAction<SpawnObjectFromGlobalPool>("Spawn Bullet L", 1).gameObject.Value;*/

            //sturdyFoolProjectile.LocateMyFSM("collide_with_wall").RemoveAction("Nail Hit", 1);
            //sturdyFoolProjectile.LocateMyFSM("collide_with_wall").RemoveTransition("In Air", "NAIL");

            Log("Initialized.");

        }

        private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            if (self.FsmName == "collide_with_wall")
            {
                Log("SHOULD BE REMOVING TRANSITION RIGHT HERE. >:/");
                self.RemoveTransition("In Air", "Nail Hit");
            }
        }

        private void EnemyDeathInfectedEffects(On.EnemyDeathEffects.orig_EmitInfectedEffects orig, EnemyDeathEffects self)
        {
            GameObject enemy = self.gameObject;
            if (enemy.name.Contains("Hopper")) {
                return;
            } // Mexicans

            orig(self);
        }

        private void EnemyHitInfectedEffects(On.InfectedEnemyEffects.orig_RecieveHitEffect orig, InfectedEnemyEffects self, float attackDirection)
        {
            GameObject enemy = self.gameObject;
            if (enemy.name.Contains("Hopper"))
            {

                //self.impactAudio.SpawnAndPlayOneShot(self.audioSourcePrefab, self.transform.position);
                
                switch (DirectionUtils.GetCardinalDirection(attackDirection))
                {
                    case 0: // Right
                        GlobalPrefabDefaults.Instance.SpawnBlood(enemy.transform.position , 3, 4, 10f, 15f, 120f, 150f, new Color(0.66f, 0.93f, 0.34f));
                        GlobalPrefabDefaults.Instance.SpawnBlood(enemy.transform.position , 8, 15, 10f, 25f, 30f, 60f, new Color(0.66f, 0.93f, 0.34f));
                        
                        //this.hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(0f, 90f, 270f));
                        break;

                    case 1: // Up
                        GlobalPrefabDefaults.Instance.SpawnBlood(enemy.transform.position , 8, 10, 20f, 30f, 80f, 100f, new Color(0.66f, 0.93f, 0.34f));
                        
                        //this.hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(270f, 90f, 270f));
                        break;

                    case 2: // Left
                        GlobalPrefabDefaults.Instance.SpawnBlood(enemy.transform.position , 3, 4, 10f, 15f, 30f, 60f, new Color(0.66f, 0.93f, 0.34f));
                        GlobalPrefabDefaults.Instance.SpawnBlood(enemy.transform.position , 8, 10, 15f, 25f, 120f, 150f, new Color(0.66f, 0.93f, 0.34f));
                        
                        //this.hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(180f, 90f, 270f));
                        break;

                    case 3: // Down
                        GlobalPrefabDefaults.Instance.SpawnBlood(enemy.transform.position , 4, 5, 15f, 25f, 140f, 180f, new Color(0.66f, 0.93f, 0.34f));
                        GlobalPrefabDefaults.Instance.SpawnBlood(enemy.transform.position , 4, 5, 15f, 25f, 360f, 400f, new Color(0.66f, 0.93f, 0.34f));
                        
                        //this.hitPuffPrefab.Spawn(base.transform.position, Quaternion.Euler(-72.5f, -180f, -180f));
                        break;
                }

                return;
            } // Mexicans

            orig(self, attackDirection);
        }

        private void EnemyDreamnailHit(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
        {

            GameObject enemy = self.transform.gameObject;
            //Modding.Logger.Log("Dreamnailed a " + enemy.name);

            if (enemy.name.Contains("Colosseum_Shield_Zombie")) return; // Shielded Fools/Sleepless Noobs, Dunces cannot be Dream Nailed

            orig(self);
            //Modding.Logger.Log("Completed normal enemy Dreamnail reaction sequence");
        }

        private void HeroControllerStart(On.HeroController.orig_Start orig, HeroController self)
        {
            PlayMakerFSM _spellControl = HeroController.instance.gameObject.LocateMyFSM("Spell Control");

            //_spellControl.GetAction<SetIntValue>("Set HP Amount", 2).intValue = 1;
            _spellControl.InsertMethod("Set HP Amount", 2, () => {
                GameManager.instance.StartCoroutine(DeepFocusTimescale());
            });
        }

        private bool EnemyEnabled(GameObject enemy, bool isAlreadyDead)
        {
            if (enemy.name.Contains("Colosseum_Miner"))
            {
                enemy.AddComponent<ThiccNoob>();
            }
            if (enemy.name.Contains("Colosseum_Flying_Sentry"))
            {
                enemy.AddComponent<AssholeNoob>();
            }
            if (enemy.name.Contains("Colosseum_Shield_Zombie"))
            {
                //enemy.AddComponent<Sleepless>();
            }
            if (enemy.name.Contains("Colosseum_Worm"))
            {
                enemy.AddComponent<ThicclordNoob>();
            }
            if (enemy.name.Contains("Hopper"))
            {
                enemy.AddComponent<Mexican>();
            }
            if (enemy.name.Contains("Colosseum_Armoured_Mosquito"))
            {
                enemy.AddComponent<GovernmentDrone>();
            }

            return isAlreadyDead;
        }

        private string LanguageGet(string key, string sheet)
        {

            string text = Language.Language.GetInternal(key, sheet);
            //Log("Key: " + key);
            //Log("Text: " + text);
            //return text;

            //text = key;

            switch (key)
            {
                case "GLAD_DUNGEON":
                    if (PlayerData.instance.colosseumGoldCompleted)
                        text = "Know this, Dunce. You now access my thoughts solely because I am resting. But in the Arena, your little Sword will not affect the minds of my fellow Sleepless and I.";
                    else text = "Know this, aspiring Dunce. You now access my thoughts solely because I am resting. But in the Arena, your little Sword will not affect the minds of my fellow Sleepless and I.";
                    break;
                case "NAME_COL_SHIELD":
                    text = "Sleepless Noob";
                    break;
                case "DESC_COL_SHIELD":
                    text = "Gamer fighting for glory in the Arena of Dunces. Does not sleep, and dedicates every waking hour to playing; as a result, skilled, but tired and weak.";
                    break;
                case "NOTE_COL_SHIELD":
                    text = "To practice nonstop...these are truly determined fighters. Many are weak and tired as a result...but those who overcome their bodies' limits are gods among their peers. Be wary of those.";
                    break;
                case "CRYSTAL_GUARDIAN_SUPER":
                    text = "Reskinned";
                    break;
                case "CRYSTAL_GUARDIAN_MAIN":
                    text = "Crystal Guardian";
                    break;
                case "CRYSTAL_GUARDIAN_SUB":
                    text = "(Not Meth Head)";
                    break;
                case "ENRAGED_GUARDIAN_SUPER":
                    text = "Reskinned";
                    break;
                case "ENRAGED_GUARDIAN_MAIN":
                    text = "Enraged Guardian";
                    break;
                case "ENRAGED_GUARDIAN_SUB":
                    text = "(Not Sober Meth Head)";
                    break;
                case "NAME_FLUKE_MOTHER":
                    text = "L =-(1/2) Tr[F^(μν)F-(μν)]+Ψ_L iγ^μD_μΨ_L+tr[(D_μΦ)^†D_μΦ]+μ^2Φ^†Φ-(1/2)λ(Φ^†Φ)^2+[(1/2) Ψ_L^TChΦΨ_L + h.c.]";
                    break;
                case "FLUKEMARM_SUPER":
                    text = "The";
                    break;
                case "FLUKEMARM_MAIN":
                    text = "MINDFUCK";
                    break;
                /*case "FLUKEMARM_SUB":
                    text = "L =-(1/2) Tr[F^(μν)F-(μν)]+Ψ_L iγ^μD_μΨ_L+tr[(D_μΦ)^†D_μΦ]+μ^2Φ^†Φ-(1/2)λ(Φ^†Φ)^2+[(1/2) Ψ_L^TChΦΨ_L + h.c.]";
                    break;*/
                case "GG_S_FLUKEMUM":
                    text = "L =-(1/2) Tr[F^(μν)F-(μν)]+Ψ_L iγ^μD_μΨ_L+tr[(D_μΦ)^†D_μΦ]+μ^2Φ^†Φ-(1/2)λ(Φ^†Φ)^2+[(1/2) Ψ_L^TChΦΨ_L + h.c.]";
                    break;
                case "DESC_FLUKE_MOTHER":
                    text = "L =-(1/2) Tr[F^(μν)F-(μν)]+Ψ_L iγ^μD_μΨ_L+tr[(D_μΦ)^†D_μΦ]+μ^2Φ^†Φ-(1/2)λ(Φ^†Φ)^2+[(1/2) Ψ_L^TChΦΨ_L + h.c.]";
                    break;
                case "NOTE_FLUKE_MOTHER":
                    text = "L =-(1/2) Tr[F^(μν)F-(μν)]+Ψ_L iγ^μD_μΨ_L+tr[(D_μΦ)^†D_μΦ]+μ^2Φ^†Φ-(1/2)λ(Φ^†Φ)^2+[(1/2) Ψ_L^TChΦΨ_L + h.c.]";
                    break;
            }
            return text;
        }
        
        private void SceneChanged(Scene arg0, Scene arg1)
        {
            //GameManager.instance.StartCoroutine(SetSprites());
            GameManager.instance.StartCoroutine(DemonizeAspids());

            if (arg1.name == "GG_Flukemarm") {
                GameObject flukemarm = GameObject.Find("Fluke Mother");
                PlayMakerFSM flukemarmControl = flukemarm.LocateMyFSM("Fluke Mother");
                //flukemarmControl.GetAction<ActivateGameObject>("Roar Start", 9).activate = false;
                //flukemarmControl.GetAction<SetFsmString>("Roar Start", 11).setValue = "MINDFUCK";
                flukemarmControl.GetAction<SetFsmBool>("Roar Start", 10).setValue = false;
                flukemarmControl.GetAction<Wait>("Roar Start", 5).time = 5f;
                flukemarmControl.InsertAction("Roar Start", new SendEventByName()
                {
                    eventTarget = new FsmEventTarget()
                    {
                        target = FsmEventTarget.EventTarget.GameObject,
                        gameObject = new FsmOwnerDefault()
                        {
                            OwnerOption = OwnerDefaultOption.SpecifyGameObject,
                            GameObject = HeroController.instance.gameObject
                        }
                    },
                    sendEvent = "ROAR ENTER",
                    delay = 0f,
                    everyFrame = false
                }, 4);
                flukemarmControl.InsertAction("Roar End", new SendEventByName()
                {
                    eventTarget = new FsmEventTarget()
                    {
                        target = FsmEventTarget.EventTarget.GameObject,
                        gameObject = new FsmOwnerDefault()
                        {
                            OwnerOption = OwnerDefaultOption.SpecifyGameObject,
                            GameObject = HeroController.instance.gameObject
                        }
                    },
                    sendEvent = "ROAR EXIT",
                    delay = 0f,
                    everyFrame = false
                }, 1);
                

            }
        }

        IEnumerator SetSprites()
        {
            yield return null;


            foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<HealthManager>().Select(hm => hm.gameObject))
            {
                if (go.name.Contains("Crystal Flyer"))
                    go.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = Sprites[0].texture;
                if (go.name.Contains("Crystallised Lazer Bug"))
                    go.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = Sprites[1].texture;
                if (go.name.Contains("Zombie Miner 1"))
                    go.AddComponent<Methamphetaminer>();
                if (go.name.Contains("Crystal Crawler"))
                    go.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = Sprites[4].texture;
                if (go.name.Contains("Mines Crawler"))
                    go.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = Sprites[5].texture;
                if (go.name.Contains("Zombie Beam Miner"))
                    if (go.name.Contains("Mega Zombie Beam Miner"))
                        go.AddComponent<MethHead>();
                    else if (go.name.Contains("Zombie Beam Miner Rematch"))
                        go.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = CPReskin.Sprites[7].texture;
                    else go.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = Sprites[6].texture;
                
            }
            
            /*
            foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<SpriteRenderer>().Select(sr => sr.gameObject))
            {
                if (go.name.Contains("brk_Crystal1"))
                    go.GetComponent<SpriteRenderer>().sprite = Sprites[8];
                if (go.name.Contains("brk_Crystal2"))
                    go.GetComponent<SpriteRenderer>().sprite = Sprites[9];
                if (go.name.Contains("brk_Crystal3"))
                    go.GetComponent<SpriteRenderer>().sprite = Sprites[10];
                if (go.name.Contains("brk_Crystal4"))
                    go.GetComponent<SpriteRenderer>().sprite = Sprites[11];

                if (go.name.Contains("crystal_spike_short"))
                    go.GetComponent<SpriteRenderer>().sprite = Sprites[12];

                if (go.name.Contains("Mines_Layered"))
                    GameObject.Destroy(go);
                    //go.GetComponent<SpriteRenderer>().sprite = Sprites[12];

            }*/ 
        }

        IEnumerator DemonizeAspids() {
            yield return null;

            foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<HealthManager>().Select(hm => hm.gameObject)) {
                if (go.name.Contains("Super Spitter")) {
                    go.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.mainTexture = Sprites[13].texture;
                    
                    GameObject aura = new GameObject();
                    aura.AddComponent<SpriteRenderer>();
                    aura.GetComponent<SpriteRenderer>().sprite = Sprites[14];
                    aura.SetActive(true);
                    aura.transform.parent = go.transform;
                    aura.transform.localPosition = new Vector3(0, 0, 0);
                    aura.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    aura.AddComponent<SpinAura>();
                    
                    {
                        /*
                        GameObject aura = new GameObject();
                        aura.SetActive(true);
                        aura.transform.parent = go.transform;
                        aura.transform.localPosition = new Vector3(0, 0, 0);
                        aura.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        aura.AddComponent<SpinAura>();

                        GameObject subaura1 = new GameObject();
                        subaura1.AddComponent<SpriteRenderer>();
                        subaura1.GetComponent<SpriteRenderer>().sprite = Sprites[14];
                        subaura1.SetActive(true);
                        subaura1.transform.parent = aura.transform;
                        subaura1.transform.localPosition = new Vector3(0, 5f, 0);
                        subaura1.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        subaura1.AddComponent<SpinAura>();

                        GameObject subaura2 = new GameObject();
                        subaura2.AddComponent<SpriteRenderer>();
                        subaura2.GetComponent<SpriteRenderer>().sprite = Sprites[14];
                        subaura2.SetActive(true);
                        subaura2.transform.parent = aura.transform;
                        subaura2.transform.localPosition = new Vector3(0, -5f, 0);
                        subaura2.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        subaura2.AddComponent<SpinAura>();*/
                    }
                }
            }

        }

        IEnumerator DeepFocusTimescale()
        {
            
            Time.timeScale = 0.25f;
            while (Time.timeScale < 1f)
            {
                yield return new WaitForSeconds(0.2f* Time.timeScale);
                Time.timeScale += 0.075f;
            }
            Time.timeScale = 1f;
        }

    }
}
