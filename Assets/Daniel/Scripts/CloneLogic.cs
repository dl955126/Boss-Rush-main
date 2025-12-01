using System.Collections;
using UnityEngine;

namespace Daniel
{
    public class CloneLogic : MonoBehaviour
    {
        [SerializeField] Animator anim;
        [SerializeField] Transform player;
        [SerializeField] GameObject damager;
        Rigidbody rb;

        float rotationSpeed = 1000;
        float minSlamForce = 30;
        float maxSlamForce = 60;
        Vector3 targetVelocity;
        private void Start()
        {
            anim = GetComponent<Animator>();
            player = FindAnyObjectByType<PlayerLogic>().transform;
            rb = GetComponent<Rigidbody>();

            StartCoroutine(SlamAttack());
        }
        void Update()
        {
            SpinClone();
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
    }
}