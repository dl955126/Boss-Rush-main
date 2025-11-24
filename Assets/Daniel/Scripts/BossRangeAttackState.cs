using UnityEngine;

namespace Daniel
{
    public class BossRangeAttackState : State
    {
        float elapsedInRanged;
        public BossRangeAttackState(StateMachine m) : base(m)
        {
            machine = m;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            machine.myBoss.SetRangedAnimations(true);
            machine.myBoss.SetOrbitPosition();
            Debug.Log("Entered Range Attack State");

        }

        public override void OnUpdate()
        {
            elapsedInRanged += Time.deltaTime;

            base.OnUpdate();
            machine.myBoss.RangeAttackOrbit();
            machine.myBoss.RangedProjectile();
            if (machine.myBoss.isSpinning)
            {
                machine.myBoss.SpinBoss();
            }

            if (elapsedInRanged > 7)
            {
                machine.ChangeState(new BossIdleState(machine));
            }

        }

        public override void OnExit()
        {
            machine.myBoss.SetRangedAnimations(false);
            machine.myBoss.StopSpinning();
            machine.myBoss.ResetProjectileTime();
            base.OnExit();
            Debug.Log("Exited Ranged Attack State");
        }
    }
}


