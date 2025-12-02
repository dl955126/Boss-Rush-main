using UnityEngine;

namespace Daniel
{
    public class BossDefeatedState : State
    {
        public BossDefeatedState(StateMachine m) : base(m)
        {
            machine = m;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Debug.Log("Entered Defeated State");
        }

        public override void OnUpdate()
        {

            base.OnUpdate();

        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("Exited Defeated State");
        }
    }
}


