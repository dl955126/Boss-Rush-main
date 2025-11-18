using UnityEngine;

namespace Daniel
{
    public class BossIdleState : State
    {

        public BossIdleState(StateMachine m) : base(m)
        {
            machine = m;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Entered Idle State");

            //decide what attack to use
            machine.myBoss.DecideAttack();

        }

        public override void OnUpdate()
        {

            base.OnUpdate();
            if(elapsedTime > 2.0f)
            {
                if(machine.myBoss.attackIndex == 0)
                {
                    machine.ChangeState(new BossPursueState(machine));
                }
                if(machine.myBoss.attackIndex == 1)
                {
                    //machine.ChangeState(new BossRangeAttackState(machine));
                    machine.ChangeState(new BossPursueState(machine));
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("Exited Idle State");
        }
    }
}

