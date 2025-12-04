using UnityEngine;

namespace Daniel
{
    public abstract class State
    {
        protected StateMachine machine;
        public float elapsedTime { private set; get; }

        public State(StateMachine m)
        {
            machine = m;
        }

        public virtual void OnEnter()
        {
            Debug.Log("Entered State");
        }

        public virtual void OnUpdate()
        {
            elapsedTime += Time.deltaTime;

            if (machine.myBoss.currentPhase == 2 && machine.myBoss.hasEnabledPhase2)
            {
                machine.myBoss.EnablePhase2Particle();
                Debug.Log("EMIT PHASE 2");
                machine.myBoss.hasEnabledPhase2 = false;
            }
        }

        public virtual void OnExit()
        {
            Debug.Log("Exited State");
        }
    }
}
