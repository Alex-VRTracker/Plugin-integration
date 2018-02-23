using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace CompleteProject
{
    public class EnemyMovement : NetworkBehaviour
    {
        Transform player;               // Reference to the player's position.
        public PlayerHealth playerHealth;      // Reference to the player's health.
        EnemyHealth enemyHealth;        // Reference to this enemy's health.
        UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.
        public bool isAttacking = false;                        // If the zombie is currently attacking

        [SerializeField] private List<Transform> targets;       // The transform of the zombie's meal
        private float distance;                                 // The distance between the zombie and its target
        private int refreshCount = 0;
        

        void Awake ()
        {
            // Set up the references.
            //player = GameObject.FindGameObjectWithTag ("Player").transform;
            //TODO to update
            player = VRTracker.instance.GetLocalPlayer().transform;

            playerHealth = player.gameObject.GetComponent <PlayerHealth> ();
            Debug.Log("Enemy Movement " + playerHealth);
            enemyHealth = GetComponent <EnemyHealth> ();
            nav = GetComponent <UnityEngine.AI.NavMeshAgent> ();
            //Added

        }

        private void Start()
        {
            if (isServer)
            {
                SetAllTargets();
            }
        }

        void Update ()
        {
            if (isServer)
            {
                // If the enemy and the player have health left...
                if (enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
                {
                    // ... set the destination of the nav mesh agent to the player.
                    // nav.SetDestination (player.position);

                    if (refreshCount == 10)
                    {
                        Transform chosenTarget = FindClosestTarget();
                        if (chosenTarget != null)
                        {
                            player = chosenTarget;
                            Vector3 ChosenPosition = new Vector3(chosenTarget.position.x, 0f, chosenTarget.position.z);
                            RpcSetTarget(ChosenPosition);
                            if (player.parent.gameObject.GetComponent<PlayerHealth>() != null)
                            {
                                playerHealth = player.parent.gameObject.GetComponent<PlayerHealth>();

                            }

                        }
                        refreshCount = 0;
                    }
                    else
                    {
                        refreshCount++;
                    }
                }
                // Otherwise...
                else
                {
                    // ... disable the nav mesh agent.
                    nav.enabled = false;
                }
            }
            
        }
      
        public void SetAllTargets()
        {
            targets.Clear();
            foreach (GameObject player in VRTrackerNetwork.instance.players)
            {
                if (player != null)
                {
                    if (!player.GetComponent<PlayerHealth>().isDead)
                        targets.Add(player.transform.Find("Player"));
                }
            }
        }

        private Transform FindClosestTarget()
        {
            if (targets.Count == 0)
            {
                Debug.Log("No player");
                return null;
            }

            float distanceTemp;
            Transform chosenTransform = targets[0];
            distance = Vector3.Distance(this.transform.position, targets[0].position);
            if (targets.Count > 1)
            {
                for (int i = 1; i < targets.Count; i++)
                {
                    distanceTemp = Vector3.Distance(this.transform.position, targets[i].position);
                    if (distanceTemp < distance)
                    {
                        distance = distanceTemp;
                        chosenTransform = targets[i];
                    }
                }
            }
            Debug.Log("Selected player " + chosenTransform.gameObject);

            return chosenTransform;
        }

        void OnChangeTarget(Vector3 ChosenPosition)
        {
            nav.SetDestination(ChosenPosition);
        }

        [ClientRpc]
        void RpcSetTarget(Vector3 ChosenPosition)
        {
            nav.SetDestination(ChosenPosition);
        }

        void OnTargetChanged(Transform target)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }
}