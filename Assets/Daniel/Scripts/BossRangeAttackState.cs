using UnityEngine;

namespace Daniel
{
    public class BossRangeAttackState : State
    {

        public BossRangeAttackState(StateMachine m) : base(m)
        {
            machine = m;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Entered Range Attack State");

        }

        public override void OnUpdate()
        {

            base.OnUpdate();
            if (elapsedTime > 2.0f)
            {
                machine.ChangeState(new BossIdleState(machine));
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("Exited Ranged Attack State");
        }
    }
}


