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
        StateMachine myStateMachine;
        Rigidbody rb;
        NavMeshPath navPath;
        Queue<Vector3> remainingPoints;
        Vector3 currentTargetPoint;

        [Header("Speed Variables")]
        [SerializeField] float bossSpeed;
        [SerializeField] float jumpForce;
        [SerializeField] float rotationSpeed;
        [SerializeField] float SpinDuration;
        [SerializeField] float minSlamForce;
        [SerializeField] float maxSlamForce;
        public bool isSpinning { private set; get; }
        bool isJumping;
        float slowDownSpeed = 4;
        Vector3 targetVelocity;

        [Header("Melee Variables")]
        [SerializeField] GameObject meleeDamager;
        [SerializeField] GameObject slamDamager;
        public int attackIndex { private set; get; }
        public bool inMeleeRange { private set; get; }

        [Header("Range Variables")]
        [SerializeField] Transform orbitPoint;
        [SerializeField] float orbitSpeed;
        [SerializeField] float projectileSpeed;
        [SerializeField] float timeBetweenProjectiles;
        [SerializeField] GameObject projectilePrefab;
        Vector3 orbitAxis = Vector3.up;
        Vector3 orbitPosition = new Vector3(0, 0, 0);

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

        //---------------------------change boss speed---------------
        public void SlowDownBoss()
        {
            
            targetVelocity = slowDownSpeed * transform.forward;
        }

        public void StopBoss()
        {
            targetVelocity = Vector3.zero;
        }

        //--------------------------handle animations--------------------
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

        public void SetRangedAnimations(bool inRanged)
        {
            animator.SetBool("Spinning", inRanged);
        }


        public void TurnToPlayer()
        {
            var newForward = (player.position - transform.position).normalized;
            newForward.y = 0;
            transform.forward = newForward;
        }

        //----------------------pursue functions--------------------------
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


        //-------------------melee functions-----------------------
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
            meleeDamager.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            meleeDamager.SetActive(false);

        }

        IEnumerator SlamDamager()
        {
            slamDamager.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            slamDamager.SetActive(false);
        }


        IEnumerator SlamAttack()
        {
            Debug.Log("Called slam attack");
            yield return new WaitForSeconds(SpinDuration);
            rb.useGravity = true;
            rb.isKinematic = false;
            isJumping = false;

            var SlamDirection = (player.position - transform.position).normalized;
            SlamDirection.y = 0;

            var distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if(distanceToPlayer < 10)
            {
                Debug.Log("Close to player");
                targetVelocity = SlamDirection * (minSlamForce + distanceToPlayer);
            }
            else
            {
                Debug.Log("Far to player");
                targetVelocity = SlamDirection * (maxSlamForce + distanceToPlayer);
            }
            
            yield return new WaitForSeconds(0.2f);

            StartCoroutine(SlamDamager());
            ResetBoss();

        }
        //---------------------Range attack-----------------------------
        public void RangeAttackOrbit()
        {
            transform.RotateAround(orbitPoint.position, orbitAxis, orbitSpeed * Time.deltaTime);
        }

        public void SetOrbitPosition()
        {
            float RandomZ = Random.Range(-15, 15);
            float RandomX = 0;

            int choosePosition = Random.Range(0, 2);

            if(choosePosition == 0)
            {
                RandomX = -15;
            }
            else if(choosePosition == 1)
            {
                RandomX = 15;
            }
            
            orbitPosition.x = RandomX;
            orbitPosition.z = RandomZ;
            transform.position = orbitPosition;
            
        }

        public void RangedProjectile()
        {
            timeBetweenProjectiles += Time.deltaTime;
            var dirToPlayer = (player.position - transform.position).normalized;

            if (timeBetweenProjectiles >= 2)
            {

                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

                if(projectileRb != null)
                {
                    projectileRb.AddForce(dirToPlayer * projectileSpeed, ForceMode.Impulse);
                }

                timeBetweenProjectiles = 0;
            }
        }

        public void ResetProjectileTime()
        {
            timeBetweenProjectiles = 0;
        }

        //---------------------animation events------------------------
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

        public void StopSpinning()
        {
            isSpinning = false;
        }

        public void SpinBoss()
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }


        public void ResetBoss()
        {
            agent.enabled = true;
            isSpinning = false;

            animator.SetBool("CompletedAttack", true);
            backToIdle = true;
        }

        //------------------------draw path----------------------
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

