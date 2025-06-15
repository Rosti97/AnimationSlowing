using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data.Common;
using System.Numerics;
using TrialInfo = trialmanager.TrialInfo;
using AssetProjectiles;
using UnityEngine.Rendering;

public class mainmanager : MonoBehaviour
{
        public Camera playerCamera;
        public GameObject FPSController;

        // all the scripts
        private animationmanager animationManager;
        private soundmanager soundManager;
        private datamanager dataManager;
        private uimanager uiManager;
        private trialmanager trialManager;
        private trackingmanager trackingManager;
        private targetmanager targetManager;
        // NOTE: TEST
        private ProjectileTest projectileTest;

        private GameObject activeOrbObj;
        private float startRT;
        private float endRT;
        private float currRT;
        // private float mouse_x;
        // private float mouse_y;
        private bool canCastSpell = false; // assure that the player can only cast a spell once per click
        private bool eventTriggered = false;
        private UnityEngine.Vector2 mouseDelta;
        private bool targetAppeared = false;

        // TODO: auslagern
        public GameObject shadowsParticleSphere;
        public GameObject explosionParticleSphere;
        
        public GameObject shadowsParticleRec;
        public GameObject explosionParticleRec;
        private GameObject currentShadows; // current shadows particle system
        private GameObject currentExplosion; // current explosion particle system
        private UnityEngine.Vector3 currentHitPoint;
        // private var shadowsParticleSystem;
        // private var explosionParticleSystem;
        public Color sphereColor = new Color(0.9f, 0.0f, 0f, 1f); // color of the sphere target
        public Color recColor = new Color(0f, 0f, 1f, 1f); // color of the rectangle target

        //private UnityEngine.Vector3 hitPosition = new UnityEngine.Vector3(1, 1, 1); // position of the hit target
        void Start()
        {
                // get the scriptmanagers
                animationManager = GetComponent<animationmanager>();
                soundManager = GetComponent<soundmanager>();
                dataManager = GetComponent<datamanager>();
                uiManager = GetComponent<uimanager>();
                trialManager = GetComponent<trialmanager>();
                targetManager = GetComponent<targetmanager>();
                trackingManager = GetComponent<trackingmanager>();

                // NOTE: TEST
                projectileTest = GetComponent<ProjectileTest>();

                currentExplosion = explosionParticleSphere; // set default particle system
                currentShadows = shadowsParticleSphere; // set default particle system


                // register to events
                trialManager.OnTrialStarted += HandleTrialStart;
                trialManager.OnTrialEnded += HandleTrialEnd;
                trialManager.OnRoundEnded += HandleRoundEnd;
                trialManager.OnGameEnded += HandleGameEnd;


                // setup for the game
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                // QualitySettings.vSyncCount = 1; 
                // Application.targetFrameRate = 60;

        }

        // called from the start screen after password check
        public void StartGame()
        {
                FPSController.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                canCastSpell = true;
                trackingManager.SetInitData(dataManager.GetID(), trialManager.version.ToString());
                trialManager.StartPractice();
        }

        void Update()
        {
                if (!FPSController.activeSelf) return; // no FPSController = start screen

                mouseDelta = FPSController.GetComponentInChildren<FirstPersonLook>().updateFPSController(); // rotate camera

                // trackingManager.updateMouseTracking(); // update mouse tracking

                if (Input.GetMouseButtonDown(0) && !trialManager.isRoundFinished) // clicks only counts if no start or pause screen
                {
                        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

                        if (Physics.Raycast(ray, out RaycastHit hit) && canCastSpell)
                        {
                                animationManager.PlayAnimation();
                                soundManager.PlayShootSound();
                                // projectileTest.StartFiring(hit.point); // NOTE: TEST

                                if (hit.collider.CompareTag("orb"))
                                {
                                        if (!trialManager.isTrialRunning && hit.collider.name == "middleOrb") // middle target hit, trial starts
                                        {
                                                // sets up the trial and calls start-event
                                                currentHitPoint = hit.collider.transform.position; // kann wahrscheinlich raus
                                                animationManager.StartGlowing();
                                                trialManager.PrepareTrial();
                                                eventTriggered = true;
                                                trackingManager.updateMouseTracking(mouseDelta, trackingmanager.EventTrigger.middleTargetClick);
                                                // projectileTest.StartFiring(hit.point); // NOTE: TEST
                                        }
                                        else if ((trialManager.isTrialRunning && hit.collider.name == "sphere") || (trialManager.isTrialRunning && hit.collider.name == "rec")) // side target hit, trial stops
                                        {
                                                currentHitPoint = hit.collider.transform.position;
                                                StopTrial(true); // hit success
                                                                 // projectileTest.StartFiring(hit.point); // NOTE: TEST
                                        }
                                }
                                else if (trialManager.isTrialRunning && !hit.collider.CompareTag("orb")) // hit no traget, but only counts if trial is running
                                {
                                        currentHitPoint = hit.point;
                                        StopTrial(false); // hit fail
                                        Debug.Log(hit.collider.name + " hit, but no target");
                                        // projectileTest.StartFiring(hit.point); // NOTE: TEST
                                        // get the x,y,z position of the hit

                                }
                        }
                }
                if (targetAppeared)
                {
                        trackingManager.updateMouseTracking(mouseDelta, trackingmanager.EventTrigger.targetAppeared);
                        targetAppeared = false;
                }
                else if (!eventTriggered)
                {
                        trackingManager.updateMouseTracking(mouseDelta); // update mouse tracking without triggered Event    
                }
                eventTriggered = false;

        }


        // called with first hit after middle target was hit
        // hitSuccess = true if the player hit the side target
        void StopTrial(bool hitSuccess)
        {
                endRT = Time.time;
                currRT = endRT - startRT;

                TrialInfo activeOrb = trialManager.GetCurrentTargetInfo();
                Color currentColor = activeOrb.Shape == TrialInfo.TrialShape.Sphere ? sphereColor : recColor;

                float delay = trialManager.GetDelay(activeOrb);

                animationManager.StopGlowing();


                eventTriggered = true;
                if (hitSuccess)
                {
                        trackingManager.updateMouseTracking(mouseDelta, trackingmanager.EventTrigger.targetClick);
                }
                else
                {
                        trackingManager.updateMouseTracking(mouseDelta, trackingmanager.EventTrigger.failedClick);
                }

                trackingManager.currentPhase = trackingmanager.TrackingPhase.endClick; // set tracking phase to end click
                trackingManager.isTrackingMouseData = false; // stop mouse tracking

                dataManager.AddTrialToData(
                        trialManager.currentRound,
                        trialManager.currentTrial,
                        trackingManager.GetMousePosition().x,
                        trackingManager.GetMousePosition().y,
                        activeOrb.Shape.ToString(),
                        activeOrb.Position.ToString(),
                        delay,
                        startRT,
                        endRT,
                        currRT,
                        hitSuccess ? 1 : 0);

                // Debug.Log($"Hit: {hitSuccess}, RT: {currRT}, Delay: {delay}");

                uiManager.UpdateInGameUI(true, currRT, trialManager.currentTrial, trialManager.currentMaxTrials);

                StartCoroutine(HideOrbWithDelay(delay, activeOrb));
        }

        // called in StopTrial() with first hit after middle target
        public IEnumerator HideOrbWithDelay(float delay, TrialInfo targetInfo = null)
        {
                canCastSpell = false;

                targetManager.HideAllOrbs();
                soundManager.PlayHitSound();

                currentShadows = targetInfo.Shape == TrialInfo.TrialShape.Sphere ? shadowsParticleSphere : shadowsParticleRec;
                currentExplosion = targetInfo.Shape == TrialInfo.TrialShape.Sphere ? explosionParticleSphere : explosionParticleRec;

                currentShadows.transform.position = currentHitPoint;
                currentShadows.transform.localScale = UnityEngine.Vector3.zero;
                currentShadows.SetActive(true);
                // // hier größer werden lassen?
                // shadowsParticle.SetActive(true);

                // yield return new WaitForSeconds(delay);

                // shadowsParticle.transform.localScale = UnityEngine.Vector3.zero;
                // shadowsParticle.SetActive(true);

                // Animate growth
                float t = 0f;
                while (t < delay)
                {
                        t += Time.deltaTime;
                        float progress = t / delay;
                        currentShadows.transform.localScale = UnityEngine.Vector3.Lerp(UnityEngine.Vector3.zero, UnityEngine.Vector3.one, progress);
                        yield return null;
                }

                currentExplosion.transform.position = currentHitPoint;

                currentExplosion.transform.position = currentHitPoint;
                currentExplosion.SetActive(true);


                // targetManager.HideAllOrbs();
                // soundManager.PlayHitSound();



                // then wait for stimulus time?
                yield return new WaitForSeconds(trialManager.stimulusTime);

                // shadowsParticle.SetActive(false);
                // explosionParticle.SetActive(false);

                // NOTE: das auslagern auf "ExplosionDone" oder so
                trialManager.StopTrial();

                canCastSpell = true;

                if (trialManager.currentTrial < trialManager.currentMaxTrials)
                {
                        targetManager.ShowMiddleOrb();
                }

        }

        // gets called with trialmanager.PrepareTrial()
        // happens when middle target is hit

        void HandleTrialStart(TrialInfo activeOrb)
        {
                // NOTE: tracking test
                canCastSpell = false;
                trackingManager.isTrackingMouseData = true; // start mouse tracking

                // trackingManager.currentPhase = trackingmanager.TrackingPhase.startClick;
                // trackingManager.updateMouseTracking(); // update mouse tracking

                currentShadows.SetActive(false);
                currentExplosion.SetActive(false);

                
                targetManager.HideAllOrbs();
                soundManager.PlayHitSound();
                StartCoroutine(PreTargetStimulusTime(activeOrb));

        }

        public IEnumerator PreTargetStimulusTime(TrialInfo activeOrb)
        {
                trialManager.isWaitingPhase = true;                // TODO: hier muss maus tracking starten
                // trackingManager.isTrackingMouseData = true;

                trackingManager.currentPhase = trackingmanager.TrackingPhase.waitingForTarget; // set tracking phase to waiting for target

                yield return new WaitForSeconds(trialManager.stimulusTime);


                canCastSpell = true;

                targetManager.ShowTargetOrb(activeOrb);
                trialManager.isWaitingPhase = false;

                targetAppeared = true;
                startRT = Time.time;
        }

        // gets called with hiding of side target
        private void HandleTrialEnd(TrialInfo activeOrb)
        {
                // here happens nothing right now
                // maybe handle mouse tracking here

        }

        // called when last trial of each round is finished
        // except after last trial of last round => HandleGameEnded()
        private void HandleRoundEnd()
        {
                trackingManager.SendDataToJS();
                uiManager.PauseGame();
                if (trialManager.currentRound > 8) // halfway
                {
                        dataManager.SendData(); // send mid-game data to JS
                }
        }

        // called after last trial of the last round
        private void HandleGameEnd()
        {
                FPSController.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                trackingManager.SendDataToJS();
                dataManager.SendSecondData(); // save data and start JS-functions
        }

        // public void ResetRound()
        // {
        //         trialManager.StartNewRound();
        //         uiManager.UpdateInGameUI(false, 0, 0, trialManager.maxTrials);
        // }

        public void ResetRound()
        {
                canCastSpell = true;
        }

}


