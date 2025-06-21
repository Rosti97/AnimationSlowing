using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetProjectiles
{
    public class ProjectileTest : MonoBehaviour
    {
        [SerializeField]
        // public MouseRotation mouseRotation;

        public GameObject fxPrefab;
        // public GameObject areaPrefabs;
        public GameObject StartPoint;
        private int currentPrefabIndex = 0;
        private int currentAreaIndex = 0;
        private float fireRate = 1f;
        private bool isFiring = false;
        private GameObject currentAreaInstance;
        private Vector3 targetPosition = Vector3.zero;
        private GameObject fxInstance;

        void Start()
        {
            if (fxPrefab != null)
            {
                currentPrefabIndex = 0;
                fireRate = fxPrefab.GetComponent<ProjectileMoving>().fireRate;
            }
            else
            {
                Debug.LogWarning("FX Prefabs list is empty or null.");
            }

        }

        void Update()
        {
            if (fxInstance != null && targetPosition != Vector3.zero)
            {
                // Move from the FX's current position towards its calculated destination
                fxInstance.transform.position = Vector3.MoveTowards(
                    fxInstance.transform.position,
                    targetPosition,
                    0.1f
                );

                // // Check if the FX has reached its destination (or is very close)
                // // Or if it has flown past its target
                if (Vector3.Distance(fxInstance.transform.position, StartPoint.transform.position) > 5f)
                {
                    ParticleSystem ps = fxInstance.GetComponent<ParticleSystem>();
                    Destroy(fxInstance, ps.main.startLifetime.constantMax);
                }
            }
        }

        public void StartFiring(Vector3 hitTargetPosition)
        {
            targetPosition = hitTargetPosition;
            Debug.Log("Target Position: " + targetPosition);
            // StartPoint.transform.LookAt(targetPosition);
            // Debug.Log("StartPoint position: " + StartPoint.transform.position);
            SpawnFx();
        }

        // IEnumerator FireContinuously()
        // {

        //     SpawnFx();
        //     yield return new WaitForSeconds(1f / fireRate);

        // }

        IEnumerator DestroyFxAfterDelay()
        {
            yield return new WaitForSeconds(0.4f);
            Destroy(fxInstance);
        }

        void SpawnFx()
        {
            if (StartPoint != null && fxPrefab != null)
            {
                if (fxInstance != null)
                {
                    Destroy(fxInstance);
                }

                fxInstance = Instantiate(fxPrefab, StartPoint.transform.position, Quaternion.identity);
                fxInstance.transform.LookAt(targetPosition);
                fxInstance.SetActive(true);

                StartCoroutine(DestroyFxAfterDelay());
            }
            else
            {
                Debug.Log("StartPoint is null or FX Prefabs list is empty.");
            }
        }

    }
}













