using UnityEngine;

namespace Daniel
{
    public class Projectile : MonoBehaviour
    {
        float elapsedTime;


        // Update is called once per frame
        void Update()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > 1)
            {
                Destroy(gameObject);
            }
        }
    }
}