using UnityEngine;

namespace Daniel
{
    public class BossDefeatedState : State
    {
        bool isDefeated;
        public BossDefeatedState(StateMachine m) : base(m)
        {
            machine = m;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            isDefeated = true;
            machine.myBoss.SetDefeatedAnimations(isDefeated);
            Debug.Log("Entered Defeated State");
        }

        public override void OnUpdate()
        {

            base.OnUpdate();

        }

        public override void OnExit()
        {
            base.OnExit();
            isDefeated = false;
            machine.myBoss.SetDefeatedAnimations(isDefeated);
            Debug.Log("Exited Defeated State");
        }
    }
}


