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
using UnityEngine.Events;
using System.Resources;

public class mainmanager : MonoBehaviour
{
        public static mainmanager Instance { get; private set; }
        public UnityEvent OnExplosionEnd;
        public UnityEvent OnMiddleEnd;

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
        private bool canCastSpell = false; // assure that the player can only cast a spell once per click
        private bool eventTriggered = false;
        private UnityEngine.Vector2 mouseDelta;
        private bool targetAppeared = false;

        private UnityEngine.Vector3 currentHitPoint;

        // public Color sphereColor = new Color(0.9f, 0.0f, 0f, 1f); // color of the sphere target
        // public Color recColor = new Color(0f, 0f, 1f, 1f); // color of the rectangle target
        private bool trialSaved = false;

        void Awake()
        {
                Instance = this;
                DontDestroyOnLoad(gameObject); // set the instance to this
        }

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
                // projectileTest = GetComponent<ProjectileTest>();

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

                if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !trialManager.isRoundFinished) // clicks only counts if no start or pause screen
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
                                                trialSaved = false;
                                                animationManager.StartGlowing();
                                                trialManager.PrepareTrial();
                                                eventTriggered = true;

                                                trackingManager.updateMouseTracking(mouseDelta, trackingmanager.EventTrigger.middleTargetClick);

                                                targetManager.StartMiddleDissolve(); 

                                                soundManager.PlayMiddleSound();

                                                // projectileTest.StartFiring(hit.point); // NOTE: TEST
                                        }
                                        else if ((trialManager.isTrialRunning && hit.collider.name == "red") || (trialManager.isTrialRunning && hit.collider.name == "blue")) // side target hit, trial stops
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
                                        // Debug.Log(hit.collider.name + " hit, but no target");
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
                 // Calculate reaction time
                endRT = Time.time;
                currRT = endRT - startRT;

                // Get trial information
                TrialInfo activeOrb = trialManager.GetCurrentTargetInfo();
                float delay = trialManager.GetDelay(activeOrb);


                // Always provide visual feedback
                if (hitSuccess)
                {
                        animationManager.StopGlowing();
                        HideOrbWithDelay(delay, activeOrb);
                }

                // Only save data if this trial hasn't been saved yet
                if (!trialSaved)
                {
                        // Update tracking data
                        var eventType = hitSuccess ? trackingmanager.EventTrigger.targetClick
                                                  : trackingmanager.EventTrigger.failedClick;

                        trackingManager.updateMouseTracking(mouseDelta, eventType);

                        trackingManager.currentPhase = trackingmanager.TrackingPhase.endClick;
                        trackingManager.isTrackingMouseData = false;

                        // Save trial data
                        dataManager.AddTrialToData(
                            trialManager.currentRound,
                            trialManager.currentTrial,
                            trackingManager.GetMousePosition().x,
                            trackingManager.GetMousePosition().y,
                            activeOrb.Color.ToString(),
                            activeOrb.Position.ToString(),
                            delay,
                            startRT,
                            endRT,
                            currRT,
                            hitSuccess ? 1 : 0);

                        trialSaved = true;
                        // Debug.Log($"Recorded first {(hitSuccess ? "success" : "failure")} for this trial");
                }

                // Always update UI
                uiManager.UpdateInGameUI(true, currRT, trialManager.currentTrial, trialManager.currentMaxTrials);
        }

        public void HideOrbWithDelay(float delay, TrialInfo targetInfo = null)
        {
                canCastSpell = false;

                // targetManager.HideAllOrbs();
                targetManager.HideTargetOrb();
        }

        // called in StopTrial() with first hit after middle target
        public IEnumerator IHideOrbWithDelay(float delay, TrialInfo targetInfo = null)
        {
                canCastSpell = false;

                targetManager.HideTargetOrb();


                trialManager.StopTrial();

                // then wait for stimulus time?
                yield return new WaitForSeconds(trialManager.stimulusTime);

        }


        // gets called with trialmanager.PrepareTrial()
        // happens when middle target is hit

        void HandleTrialStart(TrialInfo activeOrb)
        {
                // NOTE: tracking test
                canCastSpell = false;
                trackingManager.isTrackingMouseData = true; // start mouse tracking

                targetManager.HideMiddleOrb();

                // soundManager.PlayHitSound();
                StartCoroutine(PreTargetStimulusTime(activeOrb));

        }

        public IEnumerator PreTargetStimulusTime(TrialInfo activeOrb)
        {
                trialManager.isWaitingPhase = true;
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

                canCastSpell = true;

                if (trialManager.currentTrial < trialManager.currentMaxTrials)
                {
                        targetManager.ShowMiddleOrb();
                }

        }

        // called when last trial of each round is finished
        // except after last trial of last round => HandleGameEnded()
        private void HandleRoundEnd()
        {
                
                trackingManager.SaveTrackingData();
                uiManager.PauseGame();
                
                dataManager.SaveGameData();

        }

        // called after last trial of the last round
        private void HandleGameEnd()
        {
                FPSController.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                
                trackingManager.SaveTrackingData();
                
                dataManager.SaveGameData(); // save data and start JS-functions

                Invoke("QuitGame", 1.5f);
        }

        void QuitGame()
        {
                
                Application.OpenURL(dataManager.GetQLink());

                Application.Quit();
        }

        public void ResetRound()
        {
                canCastSpell = true;
        }

}


