using UnityEngine;

namespace Daniel
{
    public class BossExplosionState : State
    {
        bool exploding = false;
        public BossExplosionState(StateMachine m) : base(m)
        {
            machine = m;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            machine.myBoss.SetExplosionPosition();
            machine.myBoss.ExplosionFinished = false;
            exploding = true;
            machine.myBoss.SetExplodeAnimations(exploding);
            Debug.Log("Entered EXPLOSION State");

        }

        public override void OnUpdate()
        {
            machine.myBoss.TurnToPlayer();
            machine.myBoss.ChargeUpExplosion();
            if (machine.myBoss.ExplosionFinished)
            {
                machine.ChangeState(new BossIdleState(machine));
            }
            base.OnUpdate();

        }

        public override void OnExit()
        {
            exploding = false;
            machine.myBoss.ResetExplosive();
            machine.myBoss.SetExplodeAnimations(exploding);

        }
    }
}

