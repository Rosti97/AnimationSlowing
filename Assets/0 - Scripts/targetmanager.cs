using System.Collections.Generic;
using UnityEngine;
using TrialInfo = trialmanager.TrialInfo;

public class targetmanager : MonoBehaviour
{
        // Für 8 Positionen
        // public GameObject redTop;Left;
        // public GameObject redTop;Right;
        // public GameObject redBottomLeft;
        // public GameObject redBottomRight;
        // public GameObject blueTopLeft;
        // public GameObject blueBottomLeft;
        // public GameObject blueBottomRight;
        // public GameObject blueTopRight;    

        public GameObject orb_middle;

        public GameObject redTop;
        public GameObject redLeft;
        public GameObject redRight;
        public GameObject redBottom;

        public GameObject blueTop;
        public GameObject blueLeft;
        public GameObject blueRight;
        public GameObject blueBottom;

        public DissolveEffect currentDissolveEffect;
        public DissolveEffect middleDissolveEffect;
        private MiddleAnimation middleAnimation;


        // Dictionaries for each shape
        private Dictionary<TrialInfo.TrialPosition, GameObject> dictSpheres;
        private Dictionary<TrialInfo.TrialPosition, GameObject> dictRects;


        private GameObject activeTargetObj;

        void Start()
        {
                // initialize the dictionaries
                dictRects = new Dictionary<TrialInfo.TrialPosition, GameObject>
                {
                        { TrialInfo.TrialPosition.Top, blueTop },
                        { TrialInfo.TrialPosition.Left, blueLeft },
                        { TrialInfo.TrialPosition.Right, blueRight },
                        { TrialInfo.TrialPosition.Bottom, blueBottom },
                };

                dictSpheres = new Dictionary<TrialInfo.TrialPosition, GameObject>
                {
                        { TrialInfo.TrialPosition.Top, redTop },
                        { TrialInfo.TrialPosition.Left, redLeft },
                        { TrialInfo.TrialPosition.Right, redRight },
                        { TrialInfo.TrialPosition.Bottom, redBottom },
                };

                middleAnimation = orb_middle.GetComponent<MiddleAnimation>();
        }

        // TODO: searching for occurrence
        public void ShowMiddleOrb()
        {
                if (middleDissolveEffect == null)
                {
                        middleDissolveEffect = orb_middle.GetComponent<DissolveEffect>();
                }
                orb_middle.SetActive(true);
                activeTargetObj = orb_middle;
        }

        // could be more cleaned up, but this is a quick solution
        public void ShowTargetOrb(TrialInfo activeTarget)
        {
                // show the target based on shape and position
                if (activeTarget.Color == TrialInfo.TrialColor.Red)
                {
                        if (dictSpheres.TryGetValue(activeTarget.Position, out GameObject targetObj))
                        {
                                targetObj.SetActive(true);
                                activeTargetObj = targetObj;

                                currentDissolveEffect = targetObj.GetComponent<DissolveEffect>();
                        }
                }
                else if (activeTarget.Color == TrialInfo.TrialColor.Blue)
                {
                        if (dictRects.TryGetValue(activeTarget.Position, out GameObject targetObj))
                        {
                                targetObj.SetActive(true);
                                activeTargetObj = targetObj;

                                currentDissolveEffect = targetObj.GetComponent<DissolveEffect>();
                        }
                }
        }

        public void HideTargetOrb()
        {
                currentDissolveEffect.StartDissolving();
                activeTargetObj = null;
        }

        public void HideMiddleOrb()
        {
                // orb_middle.SetActive(false);
                middleDissolveEffect.StartDissolving();
        }

        public void StartMiddleDissolve()
        {
                middleAnimation.StartDissolving();
        }
        void HandleTrialStart(string side)
        {

        }


}