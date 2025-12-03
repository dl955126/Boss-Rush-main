using UnityEngine;

namespace Daniel
{
    public class EmitParticles : MonoBehaviour
    {
        [SerializeField] ParticleSystem spinParticles;

        public void EmitSpinParticles()
        {
            spinParticles.Emit(50);
        }
    }
}