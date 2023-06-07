using UnityEngine;
using PixelH8.Controllers;

namespace PixelH8.Controllers.States
{
    public class ActorStateIdle : StateBase
    {
        private float wanderTime;
        public override void OnEnterState(StateMachine stateMachine)
        {
            wanderTime = Time.time + Random.Range(5,11);
        }

        public override void UpdateState(StateMachine stateMachine)
        {
            if(wanderTime < Time.time)
            {
                OnExitState(stateMachine);
                stateMachine.SetState(stateMachine.Wander);
            }
        }

        public override void OnExitState(StateMachine stateMachine)
        {
        }
    }
}