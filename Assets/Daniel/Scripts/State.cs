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
        }

        public virtual void OnExit()
        {
            Debug.Log("Exited State");
        }
    }
}
