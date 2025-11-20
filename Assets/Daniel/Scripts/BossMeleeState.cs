using UnityEngine;

namespace Daniel
{
    public class BossMeleeState : State
    {
        //bool isAttacking;
        public BossMeleeState(StateMachine m) : base(m)
        {
            machine = m;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            //isAttacking = true;
            machine.myBoss.SlowDownBoss();
            
            Debug.Log("Entered Melee State");
        }

        public override void OnUpdate()
        {

            base.OnUpdate();
            machine.myBoss.SetAttackAnimations();
            if (machine.myBoss.isSpinning) 
            {
                machine.myBoss.SpinBoss();
            }

            if (machine.myBoss.backToIdle)
            {
                machine.ChangeState(new BossIdleState(machine));
            }
            
            //machine.myBoss.TurnToPlayer();

        }

        public override void OnExit()
        {
            base.OnExit();
            //isAttacking = false;
            Debug.Log("Exited Melee State");
        }
    }
}

