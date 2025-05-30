using UnityEngine;
using System.Collections.Generic;

public class trialmanager : MonoBehaviour
{
        // important stuff for the trial
        public int numTargetPerShape = 28;
        public int numTargetPerPosition = 2;
        public int maxTrials = 56; // maximum trials per round
        public int maxRounds = 8;
        public int practiceTrials = 10;
        public enum ExperimentVersion
        {
                SL, // Sphere long delay
                RL // Rectangle long delay
        }
        public ExperimentVersion version = ExperimentVersion.SL;
        public float longDelay = 0.6f;
        public float shortDelay = 0f;
        public float stimulusTime = 0.4f;

        // Custom class to hold target information (shape and position)
        [System.Serializable] // Makes it visible in the Inspector
        public class TrialInfo
        {
                // target information stuff
                public enum TrialShape
                {
                        Sphere,
                        Rectangle
                }

                public enum TrialPosition
                {
                        Left, Top, Right, Bottom,
                        TopLeft, TopRight, BottomLeft, BottomRight
                }
                public TrialShape Shape;
                public TrialPosition Position;

                public TrialInfo(TrialShape shape, TrialPosition position)
                {
                        Shape = shape;
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
                maxTrials = numTargetPerShape * 2; // 28 spheres and 28 recs
                targetManager = GetComponent<targetmanager>();
                // trackingManager = GetComponent<trackingmanager>();
                fpsCounter = GetComponent<fpscounter>();
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
                // list with 30 left and 30 right orbs (not shuffled)
                trialOrder = new List<TrialInfo>();

                // TODO: shuffle trial order with 50% spheres and 50% rectangles
                // TODO: and a predefined trial per position
                for (int i = 0; i < currentMaxTrials / 2; i++)
                {
                        trialOrder.Add(new TrialInfo(TrialInfo.TrialShape.Sphere, TrialInfo.TrialPosition.Left));
                        trialOrder.Add(new TrialInfo(TrialInfo.TrialShape.Rectangle, TrialInfo.TrialPosition.Right));
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

                OnTrialStarted?.Invoke(currentTargetInfo); // start trial with next side target

        }

        // called in mainmanager after first in in running trial
        // after disappearing of side target (see HideOrbWithDelay())
        public void StopTrial()
        {
                isTrialRunning = false;

                // starts end-trial-event
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
                // return delay based on version and current shape from TrialInfo
                if (version == ExperimentVersion.SL)
                {
                        // Sphere long delay
                        if (activeOrbInfo.Shape == TrialInfo.TrialShape.Sphere)
                        {
                                return longDelay; // long delay for spheres
                        }
                        else
                        {
                                return shortDelay; // short delay for rectangles
                        }
                }
                else if (version == ExperimentVersion.RL)
                {
                        // Rectangle long delay
                        if (activeOrbInfo.Shape == TrialInfo.TrialShape.Rectangle)
                        {
                                return longDelay; // long delay for rectangles
                        }
                        else
                        {
                                return shortDelay; // short delay for spheres
                        }
                }
                return shortDelay; // default short delay

                




                // TODO: change to new versions and no rounds
                //         if (currentRound > 8) return shortDelay; // short delay for all trials after 8 rounds
                //         else if (version == "lld" && activeOrbStr == "left") return longDelay;
                //         else if (version == "rld" && activeOrbStr == "right") return longDelay;
                //         return shortDelay; // short
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