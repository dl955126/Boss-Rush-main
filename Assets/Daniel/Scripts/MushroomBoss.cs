using System.Collections.Generic;
using brolive;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Daniel
{
    public class MushroomBoss : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] Transform player;
        [SerializeField] NavMeshAgent agent;
        StateMachine myStateMachine;

        [SerializeField] float bossSpeed;
        Vector3 targetVelocity;
        public int attackIndex { private set; get; }
        public bool inMeleeRange { private set; get; }

        Rigidbody rb;
        NavMeshPath navPath;
        Queue<Vector3> remainingPoints;
        Vector3 currentTargetPoint;

        void Start()
        {
            navPath = new NavMeshPath();
            remainingPoints = new Queue<Vector3>();
            player = FindAnyObjectByType<PlayerLogic>().transform;
            rb = GetComponent<Rigidbody>();

            myStateMachine = new StateMachine(this);
            myStateMachine.ChangeState(new BossIdleState(myStateMachine));
        }

        // Update is called once per frame
        void Update()
        {
            myStateMachine.Update();

        }

        private void FixedUpdate()
        {
            rb.linearVelocity = targetVelocity;
        }

        public void DecideAttack()
        {
            attackIndex = Random.Range(0, 2);
            targetVelocity = Vector3.zero;
        }

        public void StopBoss()
        {
            targetVelocity = Vector3.zero;
        }

        public void SetRunAnimation(bool running)
        {
            animator.SetBool("IsRunning", running);
        }


        public void TurnToPlayer()
        {
            var newForward = (player.position - transform.position).normalized;
            newForward.y = 0;
            transform.forward = newForward;
        }

        public void GetPathToPlayer()
        {
            if(agent.CalculatePath(player.position, navPath))
            {
                remainingPoints.Clear();

                foreach(Vector3 point in navPath.corners)
                {
                    remainingPoints.Enqueue(point);
                }

                if (remainingPoints.Count > 0)
                {
                    currentTargetPoint = remainingPoints.Dequeue();
                }
            }

            targetVelocity = bossSpeed * transform.forward; ;
        }

        public void SetInMeleeRange(bool MeleeRange)
        {
            inMeleeRange = MeleeRange;
            Debug.Log("In Melee range");
        }


        private void OnDrawGizmos()
        {
            if (navPath == null)
                return;

            Gizmos.color = Color.red;
            foreach (Vector3 node in navPath.corners)
            {
                Gizmos.DrawWireSphere(node, 2);
            }
        }

    }



}

