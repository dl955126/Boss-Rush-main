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
        [SerializeField] GameObject bossClone;
        public int attackIndex { private set; get; }
        public bool inMeleeRange { private set; get; }

        [Header("Range Variables")]
        [SerializeField] Transform orbitPoint;
        [SerializeField] Transform[] orbitPostions;
        [SerializeField] float orbitSpeed;
        [SerializeField] float projectileSpeed;
        [SerializeField] float timeBetweenProjectiles;
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] GameObject phase2Projectile;
        Vector3 orbitAxis = Vector3.up;

        [Header("Explosive Variables")]
        [SerializeField] GameObject explosive;
        [SerializeField] GameObject explosiveDamager;
        [SerializeField] float scaleDuration;
        float elapsedExplosiveTime;
        Vector3 targetScale = new Vector3(1.8f, 1.8f, 1.8f);
        Vector3 initialScale;

        [Header("Ultimate Variables")]
        [SerializeField] GameObject ultimateLaser;
        [SerializeField] float chargeDuration;
        [SerializeField] float SpinSpeed;
        [SerializeField] GameObject ultimateShield;
        float elapsedUltimateTime;
        Vector3 ultimateLaserPosition;
        Vector3 ultimateIntialScale;
        Vector3 ultimateTargetScale = new Vector3(0.4f, 0.4f, 0.5f);
        Vector3 ultimateLaserMidSize = new Vector3(0.4f, 0.4f, 7f);
        Vector3 ultimateLaserMaxSize = new Vector3(0.8f, 0.8f, 7f);
        [SerializeField] float zOffset;
        [SerializeField] float yOffset;
        bool hasUsedUltimate = false;
        public bool isUltimateCharged { private set; get; } = false;

        [Header("Phases Variables")]
        public int currentPhase = 1;
        bool isInPhases2 = false;
        bool isInPhase3 = false;

        [Header("Clone Variables")]
        public bool isCloneMelee { private set; get; }
        public bool isCloneRanged { private set; get; }

        public bool backToIdle;
        public bool ExplosionFinished;


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
            isCloneMelee = false;
            isCloneRanged = false;

            if (currentPhase == 1)
            {
                attackIndex = Random.Range(0, 2);
            }
            else if(currentPhase == 2)
            {
                attackIndex = Random.Range(0, 3);
            }
            else if (currentPhase == 3)
            {
                attackIndex = 3;
            }
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

        public void SetExplodeAnimations(bool isExploding)
        {
            animator.SetBool("Explode", isExploding);
        }

        public void SetUltimateAnimations(bool isUltimate)
        {
            animator.SetBool("Ultimate", isUltimate);
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


        IEnumerator SlamAttack(int phase)
        {
            Debug.Log("Called slam attack");
            yield return new WaitForSeconds(SpinDuration);
            //call slam attack2
            if(phase >= 2)
            {
                SlamAttackPhase2();
            }
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

        public void SlamAttackPhase2()
        {
            isCloneMelee = true;
            Instantiate(bossClone, transform.position, Quaternion.identity);
        }
        //---------------------Range attack-----------------------------
        public void RangeAttackOrbit()
        {
            transform.RotateAround(orbitPoint.position, orbitAxis, orbitSpeed * Time.deltaTime);
        }

        public void SetOrbitPosition(int phase)
        {
            int randomPosition = Random.Range(0, 4);

            transform.position = orbitPostions[randomPosition].position;

            if(phase >= 2)
            {
                RangedAttackPhase2();
                projectilePrefab = phase2Projectile;
            }

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

        public void RangedAttackPhase2()
        {
            isCloneRanged = true;
            GameObject clone = Instantiate(bossClone, transform.position, Quaternion.identity);
            clone.GetComponent<CloneLogic>().projectilePrefab = phase2Projectile;
        }

        //---------------------explosion functions------------------------
        public void SetExplosionPosition()
        {
            explosive.SetActive(true);
            transform.position = orbitPoint.position;
            initialScale = explosive.transform.localScale;
        }

        public void ChargeUpExplosion()
        {
            
            float t = elapsedExplosiveTime / scaleDuration;
            explosive.transform.localScale = Vector3.Lerp(initialScale, targetScale, Mathf.Clamp01(t));
            elapsedExplosiveTime += Time.deltaTime;

            if(t >= 1)
            {
                explosive.SetActive(false);
                StartCoroutine(EnableExplosiveDamager());
            }
        }

        IEnumerator EnableExplosiveDamager()
        {
            explosiveDamager.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            explosiveDamager.SetActive(false);
            ExplosionFinished = true;
        }

        public void ResetExplosive()
        {
            elapsedExplosiveTime = 0;
            explosive.transform.localScale = initialScale;
        }
        //-----------------------Ultimate Attack------------------------
        public void EnterUltimate()
        {
            ultimateShield.SetActive(true);
            ultimateLaser.SetActive(true);
            transform.position = orbitPoint.position;
            ultimateLaserPosition = ultimateLaser.transform.position;
            ultimateIntialScale = ultimateLaser.transform.localScale;

        }
        public void ChargeUpUltimate()
        {
            
            float t = elapsedUltimateTime / chargeDuration;
            ultimateLaser.transform.localScale = Vector3.Lerp(ultimateIntialScale, ultimateTargetScale, Mathf.Clamp01(t));
            elapsedUltimateTime += Time.deltaTime;

            if(t >= 1)
            {
                
                if (!hasUsedUltimate)
                {
                    hasUsedUltimate = true;
                    ultimateLaser.transform.position = transform.position + transform.forward * zOffset + Vector3.up * yOffset;
                }

                isUltimateCharged = true;

            }
        }

        public void UseUltimate()
        {
            ultimateLaser.transform.localScale = ultimateLaserMidSize;
            transform.Rotate(Vector3.up * SpinSpeed * Time.deltaTime);
        }

        public void SpeedUpLaser()
        {
            if(SpinSpeed <= 500)
            {
                SpinSpeed += Time.deltaTime * 10;
            }

            if(SpinSpeed >= 300)
            {
                ultimateLaser.transform.localScale = ultimateLaserMaxSize;
            }
        }

        public void ExitUltimate()
        {
            ultimateLaser.SetActive(false);
            ultimateShield.SetActive(false);
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

            StartCoroutine(SlamAttack(currentPhase));
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

        //-----------------------change phases---------------
        public void ChangePhases(int damage, int currentHealth)
        {
            if(currentHealth <= 50 && !isInPhases2)
            {
                Debug.Log("PHASE 2");
                currentPhase++;
                isInPhases2 = true;
            }

            if(currentHealth <= 20 && !isInPhase3)
            {
                Debug.Log("PHASE 3");
                currentPhase++;
                isInPhase3 = true;
            }
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

