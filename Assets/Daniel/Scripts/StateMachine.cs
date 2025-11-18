using UnityEngine;

namespace Daniel
{
    public class StateMachine
    {
        State currentState;
        public MushroomBoss myBoss { private set; get; }

        public StateMachine(MushroomBoss boss)
        {
            myBoss = boss;
        }
        public void Update()
        {
            currentState?.OnUpdate();
        }

        public void ChangeState(State newState)
        {
            currentState?.OnExit();

            currentState = newState;

            currentState?.OnEnter();
        }
    }
}