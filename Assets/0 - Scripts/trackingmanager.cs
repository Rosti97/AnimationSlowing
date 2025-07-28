using System;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

public class trackingmanager : MonoBehaviour
{
        public LayerMask trackingLayer;
        private float mouseX;
        private float mouseY;

        public TrackingPhase currentPhase = TrackingPhase.startClick;
        private trialmanager trialManager;
        public bool isTrackingMouseData = false;

        private StringBuilder _trackingDataBuilder = new StringBuilder();
        private string _filePath = Application.dataPath + "/data/";
        private string trackingDataHeader = "id, round, trial, version, timestamp, frame, _relativeTime, pointerX, pointerY, mouseDX, mouseDY, phase, event";

        public enum TrackingPhase
        {
                startClick,
                waitingForTarget,
                activeTracking,
                onTarget,
                endClick
        }

        public enum EventTrigger
        {
                noEvent,
                middleTargetClick,
                targetAppeared,
                targetClick,
                failedClick
        }

        private string _pID;
        private string _pVersion;
        private float _startTrialTime = 0f;
        private float _relativeTime = 0f;
        private float _maxCountingTime = 4f; // we exclude trials that take longer than 4 seconds
        // private int roundLog = 0; // for debugging purposes, to log the round number

        void Start()
        {
                trackingLayer = LayerMask.GetMask("Tracker");
                trialManager = GetComponent<trialmanager>();
                ResetTrackingData();

        }

        public void updateMouseTracking(Vector2 mouseDelta, EventTrigger trigger = EventTrigger.noEvent)
        {
                if (!isTrackingMouseData) return;

                if (trigger == EventTrigger.targetAppeared) // start of trial as start of max seconds
                {
                        _startTrialTime = Time.time;
                        _relativeTime = 0f; // reset relative time
                }

                _relativeTime += Time.deltaTime; // update relative time

                if (_relativeTime >= _maxCountingTime) // if trial takes longer than 4s we stop tracking
                {
                        // Debug.Log("STOP");
                        return;
                }

                RaycastHit[] hits;
                hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, 100f, trackingLayer);

                bool orbHit = false;

                foreach (RaycastHit hit in hits)
                {
                        // Debug.Log("Hit: " + hit.collider.name);
                        if (hit.collider.CompareTag("orb"))
                        {
                                orbHit = true;
                                currentPhase = TrackingPhase.onTarget;
                        }
                        else
                        {
                                Vector3 hitPosition = hit.point;
                                mouseX = hitPosition.x;
                                mouseY = hitPosition.y;
                        }
                }

                if (!orbHit && !trialManager.isWaitingPhase)
                {
                        currentPhase = TrackingPhase.activeTracking;
                }
                else if (trialManager.isWaitingPhase)
                {
                        currentPhase = TrackingPhase.waitingForTarget;
                }

                AddTrackingData(trialManager.currentRound, trialManager.currentTrial, mouseDelta, trigger);

        }

        public string GetTrackingData()
        {
                return _trackingDataBuilder.ToString();
        }

        public void ResetTrackingData()
        {
                _trackingDataBuilder.Clear();
                // _trackingDataBuilder.Append(trackingDataHeader);
        }


        private void AddTrackingData(int round, int trial, Vector2 mouseDelta, EventTrigger trigger)
        {
                string lastTrackingEntry =
                       $"{_pID}," +
                       $"{round}," +
                       $"{trial}," +
                       $"{_pVersion}," +
                       $"{DateTime.Now:HH:mm:ss.fff}," +
                       $"{Time.frameCount}," +
                       $"{_relativeTime:F3}," +
                       $"{mouseX:F3}," +
                       $"{mouseY:F3}," +
                       $"{mouseDelta.x:F3}," +
                       $"{mouseDelta.y:F3}," +
                       $"{currentPhase}," +
                       $"{trigger}";

                _trackingDataBuilder.AppendLine(lastTrackingEntry);

        }

        public void SetInitData(string id, string version)
        {
                _pID = id;
                _pVersion = version;
                _filePath = $"{_filePath}{_pID}_trackingData.csv";

                //File.WriteAllText(_filePath, trackingDataHeader); // create file with header
                //File.AppendAllText(_filePath, trackingDataHeader);
                _trackingDataBuilder.AppendLine(trackingDataHeader);
        }

        public Vector2 GetMousePosition()
        {
                return new Vector2(mouseX, mouseY);
        }

        public void SaveTrackingData()
        {
                File.AppendAllText(_filePath, _trackingDataBuilder.ToString());
                _trackingDataBuilder.Clear();
        }

}