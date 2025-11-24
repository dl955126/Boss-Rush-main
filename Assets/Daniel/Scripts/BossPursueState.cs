using UnityEngine;
using UnityEngine.UI;

namespace Daniel
{
    public class BossPursueState : State
    {
        float pursueTimer;
        float pathRecalc;
        bool isRunning;

        public BossPursueState(StateMachine m) : base(m)
        {
            machine = m;
        }

        public override void OnEnter()
        {
            isRunning = true;
            base.OnEnter();
            Debug.Log("Entered Pursue State");
            machine.myBoss.GetPathToPlayer();
            
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            pathRecalc += Time.deltaTime;
            pursueTimer += Time.deltaTime;
            machine.myBoss.TurnToPlayer();

            if (pathRecalc > 1)
            {
                machine.myBoss.GetPathToPlayer();
                pathRecalc = 0;
            }

            machine.myBoss.SetRunAnimation(isRunning);

            if (machine.myBoss.inMeleeRange && pursueTimer > 0.2f)
            {
                machine.ChangeState(new BossMeleeState(machine));
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            isRunning = false;
            machine.myBoss.SetRunAnimation(isRunning);
            machine.myBoss.ClearPath();
            Debug.Log("Exited Pursue State");
        }
    }
}



