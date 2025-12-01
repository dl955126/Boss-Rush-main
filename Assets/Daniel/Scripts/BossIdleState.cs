using UnityEngine;

namespace Daniel
{
    public class BossIdleState : State
    {
        bool idle;
        public BossIdleState(StateMachine m) : base(m)
        {
            machine = m;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            idle = true;
            machine.myBoss.backToIdle = false;
            Debug.Log("Entered Idle State");
            machine.myBoss.SetIdleAnimations(idle);
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
                    //machine.ChangeState(new BossPursueState(machine));
                    machine.ChangeState(new BossRangeAttackState(machine));
                    //machine.ChangeState(new BossExplosionState(machine));
                }
                if (machine.myBoss.attackIndex == 1)
                {
                    machine.ChangeState(new BossRangeAttackState(machine));
                    //machine.ChangeState(new BossPursueState(machine));
                    //machine.ChangeState(new BossExplosionState(machine));
                }
                if(machine.myBoss.attackIndex == 2)
                {
                    //machine.ChangeState(new BossPursueState(machine));
                    machine.ChangeState(new BossRangeAttackState(machine));
                    //machine.ChangeState(new BossExplosionState(machine));
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            idle = false;
            machine.myBoss.SetIdleAnimations(idle);
            Debug.Log("Exited Idle State");
        }
    }
}

