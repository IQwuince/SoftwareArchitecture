using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Enemy.Movement
{
	public class EnemyFSM : MonoBehaviour
	{
        public EnemyState CurrentState { get; private set; }
        public string CurrentStateName => CurrentState != null ? CurrentState.StateName : "None";

        public void SetInitialState(EnemyState state)
        {
            CurrentState = state;
            CurrentState?.Enter();
        }

        public void Tick()
        {
            if (CurrentState == null) return;

            CurrentState.Tick();

            EnemyState next = CurrentState.GetNextState();
            if (next != null && next != CurrentState)
                ChangeState(next);
        }

        public void PhysicsTick()
        {
            CurrentState?.PhysicsTick();
        }

        public void ChangeState(EnemyState next)
        {
            if (next == null || next == CurrentState) return;
            CurrentState?.Exit();
            CurrentState = next;
            CurrentState.Enter();
        }

    }
}