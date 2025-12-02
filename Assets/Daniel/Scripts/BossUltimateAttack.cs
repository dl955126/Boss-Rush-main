using UnityEngine;

namespace Daniel
{
    public class BossUltimateAttack : State
    {
        bool isUltimate;
        float ultimateElapsed;
        
        public BossUltimateAttack(StateMachine m) : base(m)
        {
            machine = m;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            machine.myBoss.EnterUltimate();
            machine.myBoss.TurnToPlayer();
            Debug.Log("Entered Ultimate State");
            isUltimate = true;
            machine.myBoss.SetUltimateAnimations(isUltimate);

        }

        public override void OnUpdate()
        {

            base.OnUpdate();
            ultimateElapsed += Time.deltaTime;
            machine.myBoss.ChargeUpUltimate();

            if (machine.myBoss.isUltimateCharged)
            {
                machine.myBoss.UseUltimate();
                machine.myBoss.SpeedUpLaser();
            }

            if(ultimateElapsed >= 35)
            {
                machine.ChangeState(new BossDefeatedState(machine));
            }

        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("Exited Ultimate State");
            isUltimate = false;
            machine.myBoss.SetUltimateAnimations(isUltimate);
            machine.myBoss.ExitUltimate();

        }
    }
}


