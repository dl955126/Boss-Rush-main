using System.Collections;
using System.Collections.Generic;
using brolive;
using TreeEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Daniel
{
    public class MushroomBoss : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Animator animator;
        [SerializeField] Transform player;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] GameObject MeleeDamager;
        StateMachine myStateMachine;
        Rigidbody rb;
        NavMeshPath navPath;
        Queue<Vector3> remainingPoints;
        Vector3 currentTargetPoint;

        [Header("Speed Variables")]
        [SerializeField] float bossSpeed;
        [SerializeField] float jumpForce;
        [SerializeField] float rotationSpeed;
        [SerializeField] float SlamForce;
        public bool isSpinning { private set; get; }
        bool isJumping;
        float slowDownSpeed = 4;
        Vector3 targetVelocity;

        [Header("Melee Variables")]
        public int attackIndex { private set; get; }
        public bool inMeleeRange { private set; get; }
        public bool backToIdle;


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
            if (!isJumping)
            {
                rb.linearVelocity = targetVelocity;
            }
        }

        public void DecideAttack()
        {
            attackIndex = Random.Range(0, 2);
            targetVelocity = Vector3.zero;
        }

        //change boss speed
        public void SlowDownBoss()
        {
            
            targetVelocity = slowDownSpeed * transform.forward;
        }

        public void StopBoss()
        {
            targetVelocity = Vector3.zero;
        }

        //handle animations
        public void SetIdleAnimations(bool isIdle)
        {
            animator.SetBool("CompletedAttack", isIdle);
            animator.ResetTrigger("Attack");
        }
        public void SetRunAnimation(bool running)
        {
            animator.SetBool("IsRunning", running);
        }

        public void SetAttackAnimations()
        {
            animator.SetTrigger("Attack");
        }


        public void TurnToPlayer()
        {
            var newForward = (player.position - transform.position).normalized;
            newForward.y = 0;
            transform.forward = newForward;
        }

        //pursue functions
        public void GetPathToPlayer()
        {
            if(!agent.enabled || !agent.isOnNavMesh) return;

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

        public void ClearPath()
        {
            remainingPoints.Clear();
        }


        //melee functions
        public void SetInMeleeRange(bool MeleeRange)
        {
            inMeleeRange = MeleeRange;
            Debug.Log("In Melee range");
        }

        public void StartCombo()
        {
            StartCoroutine(ComboAttack());
        }

        IEnumerator ComboAttack()
        {
            MeleeDamager.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            MeleeDamager.SetActive(false);

        }

        //animation events
        public void LaunchUp()
        {
            StopBoss();
            isJumping = true;
            agent.enabled = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        public void StopMidAir()
        {
            rb.useGravity = false;
            rb.isKinematic = true;

            StartCoroutine(SlamAttack());
        }

        public void IsSpinning()
        {
            isSpinning = true;
        }

        public void SpinBoss()
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        IEnumerator SlamAttack()
        {
            Debug.Log("Called slam attack");
            yield return new WaitForSeconds(4);
            rb.useGravity = true;
            rb.isKinematic = false;
            isJumping = false;

            var SlamDirection = (player.position - transform.position).normalized;
            SlamDirection.y = 0;

            targetVelocity = (SlamDirection * SlamForce) + (Vector3.down * SlamForce);
            yield return new WaitForSeconds(0.2f);
            
            StartCombo();
            ResetBoss();

        }

        public void ResetBoss()
        {
            agent.enabled = true;
            isSpinning = false;

            animator.SetBool("CompletedAttack", true);
            backToIdle = true;
        }

        //draw path
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

