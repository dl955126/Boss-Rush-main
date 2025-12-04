using UnityEngine;

namespace Daniel
{
    public class EmitParticles : MonoBehaviour
    {
        [SerializeField] ParticleSystem spinParticles;
        [SerializeField] ParticleSystem phase2Particles;
        [SerializeField] ParticleSystem ExplosiveCharge;
        [SerializeField] ParticleSystem EndExplosion;
        float startRadius = 3f;
        float endRadius = 10f;

        private void Start()
        {

        }
        public void PlaySpinParticles()
        {
            spinParticles.Play();
        }

        public void PlayPhase2Particles()
        {
            
            phase2Particles.Play();
        }

        public void PlayExplosiveCharge()
        {
            ExplosiveCharge.Play();
        }

        public void ExplosiveChargeParticles(float time)
        {
            var explosiveShape = ExplosiveCharge.shape;
            explosiveShape.radius = Mathf.Lerp(startRadius, endRadius, time);
        }

        public void PlayEndExplosion()
        {
            EndExplosion.Play();
        }
    }
}