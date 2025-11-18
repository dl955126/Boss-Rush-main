using UnityEngine;

namespace Daniel
{
    public class BossMeleeState : State
    {

        public BossMeleeState(StateMachine m) : base(m)
        {
            machine = m;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            machine.myBoss.StopBoss();
            Debug.Log("Entered Melee State");
        }

        public override void OnUpdate()
        {

            base.OnUpdate();

        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("Exited Melee State");
        }
    }
}

