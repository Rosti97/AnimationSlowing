using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class trialmanager : MonoBehaviour
{
        // important stuff for the trial
        public int numTargetPerColor = 7;
        public int numPositions = 4;
        public int numColors = 2; 
        public int maxTrials = 56; // maximum trials per round
        public int maxRounds = 8;
        public int practiceTrials = 10;
        public enum ExperimentVersion
        {
                BL, // Sphere long delay
                RL // Rectangle long delay
        }
        public ExperimentVersion version = ExperimentVersion.BL;
        public float longAnimation = 2f;
        public float shortAnimation = 0.4f;
        public float stimulusTime = 0.4f;

        // Custom class to hold target information (Color and position)
        [System.Serializable] // Makes it visible in the Inspector
        public class TrialInfo
        {
                // target information stuff
                public enum TrialColor
                {
                        Red,
                        Blue
                }

                public enum TrialPosition
                {
                        Left, Top, Right, Bottom
                        //, TopLeft, TopRight, BottomLeft, BottomRight
                }
                public TrialColor Color;
                public TrialPosition Position;

                public TrialInfo(TrialColor color, TrialPosition position)
                {
                        Color = color;
                        Position = position;
                }
        }

        // current state of things of the trial for other scripts to access
        public string currentPhase = "waitingForTarget";
        public bool isTrialRunning = false;
        public bool isWaitingPhase = false;
        public bool isTrackingRunning = false;
        public bool isRoundFinished = false;
        public List<TrialInfo> trialOrder;

        // current count of things
        public int currentTrial = 0;
        public int currentRound = 0;
        public int currentMaxTrials;
        // private string currentTargetStr;
        private TrialInfo currentTargetInfo;

        // event stuff
        public delegate void TrialStartedDelegate(TrialInfo activeOrb);
        public event TrialStartedDelegate OnTrialStarted;
        public delegate void TrialEndedDelegate(TrialInfo activeOrb);
        public event TrialEndedDelegate OnTrialEnded;
        public delegate void RoundEndedDelegate();
        public event RoundEndedDelegate OnRoundEnded;
        public delegate void GameEndedDelegate();
        public event GameEndedDelegate OnGameEnded;

        private targetmanager targetManager;
        // private trackingmanager trackingManager;
        private fpscounter fpsCounter;

        public void Start()
        {
                // get the scriptmanagers
                maxTrials = numColors * numPositions * numTargetPerColor; // with 7 targets per Color and 4 positions, we have 56 trials
                targetManager = GetComponent<targetmanager>();
                // trackingManager = GetComponent<trackingmanager>();
                fpsCounter = GetComponent<fpscounter>();
                mainmanager.Instance.OnExplosionEnd.AddListener(StopTrial);
                mainmanager.Instance.OnMiddleEnd.AddListener(StartTrial);
        }

        // sets trials for practice round
        public void StartPractice()
        {
                currentMaxTrials = practiceTrials;
                GenerateTrialOrder();
                currentTrial = 0;
                currentRound = 0;
                ResetTrialState();
                targetManager.ShowMiddleOrb();
                fpsCounter.StartFPSTest(); // start fps test for practice round for 15 seconds
                // isTrialRunning = false;
                // isWaitingPhase = false;
                // isTrackingRunning = false;
        }

        // sets trials for experiment rounds
        public void StartNewRound()
        {
                currentMaxTrials = maxTrials; // 60 trials per round
                GenerateTrialOrder();
                // isTrialRunning = false;
                currentTrial = 0;
                // currentRound++;
                ResetTrialState();
                targetManager.ShowMiddleOrb();
        }

        private void ResetTrialState()
        {
                isTrialRunning = false;
                isRoundFinished = false;
                // isWaitingPhase = false;
                currentPhase = "waitingForTarget";
        }

        // creates a randomized trial order
        public void GenerateTrialOrder()
        {
                // list
                trialOrder = new List<TrialInfo>();

                for (int i = 0; i < currentMaxTrials / 2; i++)
                {
                        trialOrder.Add(new TrialInfo(TrialInfo.TrialColor.Red, TrialInfo.TrialPosition.Left));
                        trialOrder.Add(new TrialInfo(TrialInfo.TrialColor.Red, TrialInfo.TrialPosition.Right));
                        trialOrder.Add(new TrialInfo(TrialInfo.TrialColor.Red, TrialInfo.TrialPosition.Top));
                        trialOrder.Add(new TrialInfo(TrialInfo.TrialColor.Red, TrialInfo.TrialPosition.Bottom));
                        trialOrder.Add(new TrialInfo(TrialInfo.TrialColor.Blue, TrialInfo.TrialPosition.Left));
                        trialOrder.Add(new TrialInfo(TrialInfo.TrialColor.Blue, TrialInfo.TrialPosition.Right));
                        trialOrder.Add(new TrialInfo(TrialInfo.TrialColor.Blue, TrialInfo.TrialPosition.Top));
                        trialOrder.Add(new TrialInfo(TrialInfo.TrialColor.Blue, TrialInfo.TrialPosition.Bottom));
                }

                // shuffle trial order Fisher-Yates algorithm
                for (int i = trialOrder.Count - 1; i > 0; i--)
                {
                        int randomIndex = UnityEngine.Random.Range(0, i + 1);
                        (trialOrder[randomIndex], trialOrder[i]) = (trialOrder[i], trialOrder[randomIndex]);
                }
        }

        // called in mainmanager with hit of middle target
        public void PrepareTrial()
        {

                if (currentTrial >= currentMaxTrials) return; // maximum trials in round reached

                isTrialRunning = true;
                // isWaitingPhase = false;
                // isTrackingRunning = true;
                // currentPhase = "activeTargeting";

                currentTargetInfo = trialOrder[currentTrial]; // NOTE: nur uebergangsweise
                currentTrial++;

                // should only be called when middle orb is dissolved
                // OnTrialStarted?.Invoke(currentTargetInfo); // start trial with next side target
        }

        private void StartTrial()
        {
                OnTrialStarted?.Invoke(currentTargetInfo);
        }

        // called in mainmanager after first in in running trial
        // after disappearing of side target (see HideOrbWithDelay())
        public void StopTrial()
        {
                // Debug.Log("Stop Trial called");
                isTrialRunning = false;

                StartCoroutine(stimulusTimer()); // start timer for stimulus time

                // starts end-trial-event

        }

        private IEnumerator stimulusTimer()
        {
                yield return new WaitForSeconds(stimulusTime);

                OnTrialEnded?.Invoke(currentTargetInfo);

                // isTrackingRunning = false;
                currentPhase = "waitingForTarget";

                // string activeOrb = trialOrder[currentTrial];


                // check if round and/or game is finished
                if (currentTrial >= currentMaxTrials)
                {
                        isRoundFinished = true;
                        currentRound++;
                        if (currentRound > maxRounds)
                        {
                                OnGameEnded?.Invoke(); // game is finished
                        }
                        else
                        {
                                // currentRound++;
                                OnRoundEnded?.Invoke(); // round is finished
                        }
                }
        }

        // returns delay for the current orb
        public float GetDelay(TrialInfo activeOrbInfo)
        {
                // return delay based on version and current Color from TrialInfo
                if (version == ExperimentVersion.BL)
                {
                        // Sphere long delay
                        if (activeOrbInfo.Color == TrialInfo.TrialColor.Blue)
                        {
                                return longAnimation; // long delay for spheres
                        }
                        else
                        {
                                return shortAnimation; // short delay for rectangles
                        }
                }
                else if (version == ExperimentVersion.RL)
                {
                        // Rectangle long delay
                        if (activeOrbInfo.Color == TrialInfo.TrialColor.Red)
                        {
                                return longAnimation; // long delay for rectangles
                        }
                        else
                        {
                                return shortAnimation; // short delay for spheres
                        }
                }
                return shortAnimation; // default short delay






                // TODO: change to new versions and no rounds
                //         if (currentRound > 8) return shortAnimation; // short delay for all trials after 8 rounds
                //         else if (version == "lld" && activeOrbStr == "left") return longAnimation;
                //         else if (version == "rld" && activeOrbStr == "right") return longAnimation;
                //         return shortAnimation; // short
        }

        public void UpdateTrackingPhase(string phase)
        {
                currentPhase = phase;
        }

        // returns current string of side orb
        public TrialInfo GetCurrentTargetInfo()
        {
                // isWaitingPhase = false;
                // isTrackingRunning = true;
                // currentPhase = "activeTracking";
                return currentTargetInfo;
                // return currentTargetStr;
        }
}