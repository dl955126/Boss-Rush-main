using System.Collections;
using UnityEngine;

namespace Daniel
{
    public class CloneLogic : MonoBehaviour
    {
        [SerializeField] Animator anim;
        [SerializeField] Transform player;
        [SerializeField] GameObject damager;
        [SerializeField] Transform orbitPoint;
        [SerializeField] ParticleSystem cloneParticle;
        public GameObject projectilePrefab;
        MushroomBoss myBoss;
        Rigidbody rb;

        float rotationSpeed = 1000;
        float minSlamForce = 30;
        float maxSlamForce = 60;
        float orbitSpeed = -150;
        float projectileSpeed = 100;
        float timeBetweenProjectiles;
        float lifetime;
        Vector3 targetVelocity;
        Vector3 orbitAxis = Vector3.up;

        private void Awake()
        {
            myBoss = FindAnyObjectByType<MushroomBoss>();
            
        }
        private void Start()
        {
            anim = GetComponent<Animator>();
            player = FindAnyObjectByType<PlayerLogic>().transform;
            rb = GetComponent<Rigidbody>();
            orbitPoint = FindAnyObjectByType<OrbitPoint>().transform;
            cloneParticle.Play();

            if (myBoss.isCloneMelee)
            {
                StartCoroutine(SlamAttack());
                
            }
            
        }
        void Update()
        {
            SpinClone();

            if (myBoss.isCloneRanged)
            {
                RangeAttackOrbit();
                RangedProjectile();
            }

            lifetime += Time.deltaTime;
            if(lifetime > 7)
            {
                Destroy(gameObject);
            }
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = targetVelocity;
        }

        public void SpinClone()
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        IEnumerator SlamAttack()
        {
            cloneParticle.Play();
            yield return new WaitForSeconds(1f);

            var SlamDirection = (player.position - transform.position).normalized;
            //SlamDirection.y = 0;

            var distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < 10)
            {
                
                targetVelocity = SlamDirection * (minSlamForce + distanceToPlayer);
            }
            else
            {
                
                targetVelocity = SlamDirection * (maxSlamForce + distanceToPlayer);
            }

            yield return new WaitForSeconds(0.2f);
            damager.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            damager.SetActive(false);
            Destroy(gameObject);
        }
        public void RangeAttackOrbit()
        {
            transform.RotateAround(orbitPoint.position, orbitAxis, orbitSpeed * Time.deltaTime);
        }
        public void RangedProjectile()
        {
            timeBetweenProjectiles += Time.deltaTime;
            var dirToPlayer = (player.position - transform.position).normalized;

            if (timeBetweenProjectiles >= 2)
            {

                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

                if (projectileRb != null)
                {
                   projectileRb.AddForce(dirToPlayer * projectileSpeed, ForceMode.Impulse);
                }

                timeBetweenProjectiles = 0;
            }
        }

    }
}