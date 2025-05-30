using System.Collections.Generic;
using UnityEngine;
using TrialInfo = trialmanager.TrialInfo;

public class targetmanager : MonoBehaviour
{
        // public GameObject orb_middle;
        // public GameObject orb_right;
        // public GameObject orb_left;
        public GameObject orb_middle;

        // Neue GameObjects für die 8 Positionen und 2 Formen
        public GameObject sphereTopLeft;
        public GameObject sphereTop;
        public GameObject sphereTopRight;
        public GameObject sphereLeft;
        public GameObject sphereRight;
        public GameObject sphereBottomLeft;
        public GameObject sphereBottom;
        public GameObject sphereBottomRight;

        public GameObject recTopLeft;
        public GameObject recTop;
        public GameObject recTopRight;
        public GameObject recLeft;
        public GameObject recRight;
        public GameObject recBottomLeft;
        public GameObject recBottom;
        public GameObject recBottomRight;

        // Dictionaries for each shape
        private Dictionary<TrialInfo.TrialPosition, GameObject> dictSpheres;
        private Dictionary<TrialInfo.TrialPosition, GameObject> dictRects;


        private GameObject activeTargetObj;
        // private TrialInfo middleTargetInfo = new TrialInfo(TrialInfo.TrialShape.Sphere, TrialInfo.TrialPosition.Top); // Default middle target
        // private string activeTargetStr; // wird wohl eher zu Target[shape, position]

        // public string getactiveTargetString() => activeTargetStr;
        // public GameObject getactiveTargetObject() => activeTargetObj;

        // private trialmanager trialManager;

        void Start()
        {
                // initialize the dictionaries
                dictRects = new Dictionary<TrialInfo.TrialPosition, GameObject>
                {
                        { TrialInfo.TrialPosition.TopLeft, recTopLeft },
                        { TrialInfo.TrialPosition.Top, recTop },
                        { TrialInfo.TrialPosition.TopRight, recTopRight },
                        { TrialInfo.TrialPosition.Left, recLeft },
                        { TrialInfo.TrialPosition.Right, recRight },
                        { TrialInfo.TrialPosition.BottomLeft, recBottomLeft },
                        { TrialInfo.TrialPosition.Bottom, recBottom },
                        { TrialInfo.TrialPosition.BottomRight, recBottomRight }
                };

                dictSpheres = new Dictionary<TrialInfo.TrialPosition, GameObject>
                {
                        { TrialInfo.TrialPosition.TopLeft, sphereTopLeft },
                        { TrialInfo.TrialPosition.Top, sphereTop },
                        { TrialInfo.TrialPosition.TopRight, sphereTopRight },
                        { TrialInfo.TrialPosition.Left, sphereLeft },
                        { TrialInfo.TrialPosition.Right, sphereRight },
                        { TrialInfo.TrialPosition.BottomLeft, sphereBottomLeft },
                        { TrialInfo.TrialPosition.Bottom, sphereBottom },
                        { TrialInfo.TrialPosition.BottomRight, sphereBottomRight }
                };
        }

        public void ShowMiddleOrb()
        {
                orb_middle.SetActive(true);
                activeTargetObj = orb_middle;
                
                // activeTargetStr = "middle";
        }

        public void ShowTargetOrb(TrialInfo activeTarget)
        {
                // show the target based on shape and position
                if (activeTarget.Shape == TrialInfo.TrialShape.Sphere)
                {
                        if (dictSpheres.TryGetValue(activeTarget.Position, out GameObject targetObj))
                        {
                                targetObj.SetActive(true);
                                activeTargetObj = targetObj;
                                // activeTargetStr = activeTarget.Position.ToString();
                        }
                }
                else if (activeTarget.Shape == TrialInfo.TrialShape.Rectangle)
                {
                        if (dictRects.TryGetValue(activeTarget.Position, out GameObject targetObj))
                        {
                                targetObj.SetActive(true);
                                activeTargetObj = targetObj;
                                // activeTargetStr = activeTarget.Position.ToString();
                        }
                }

                // activeTargetStr = side;
                // if (side == "right")
                // {
                //         orb_right.SetActive(true);
                //         activeTargetObj = orb_right;
                // }
                // else if (side == "left")
                // {
                //         orb_left.SetActive(true);
                //         activeTargetObj = orb_left;
                // }
        }

        public void HideAllOrbs()
        {
                orb_middle.SetActive(false);
                // orb_right.SetActive(false);
                // orb_left.SetActive(false);
        }

        void HandleTrialStart(string side)
        {
                // HideMiddleOrb();
                // ShowTargetOrb(side);
        }


}