using UnityEngine;
using PixelH8.Controllers;

namespace PixelH8.Controllers.States
{
    public class ActorStateWander : StateBase
    {
        private Vector3 DesiredLocation;
        private float stopDistance;
        private float desiredRot;

        private bool NavDestSet;

        public override void OnEnterState(StateMachine stateMachine)
        {
            var ran = UnityEngine.Random.insideUnitCircle * 10;
            DesiredLocation = stateMachine.ActorAI.startPosition + new Vector3(ran.x,0,ran.y);
            stopDistance = Random.Range(1,5);
            NavDestSet = false;
        }

        public override void UpdateState(StateMachine stateMachine)
        {
            if(!NavDestSet)
            {
            var rot = stateMachine.ActorAI.transform.position - DesiredLocation;
            stateMachine.ActorAI.transform.rotation = Quaternion.Euler(0,rot.y,0);
            stateMachine.ActorAI.navMeshAgent.SetDestination(DesiredLocation);
            NavDestSet = true;
            }
            else
            {
                if(Vector3.Distance(stateMachine.transform.position,DesiredLocation) < stopDistance)
                {
                    stateMachine.ActorAI.navMeshAgent.SetDestination(stateMachine.ActorAI.navMeshAgent.transform.position);
                    stateMachine.SetState(stateMachine.Idle);
                }
            }
            
        }

        public override void OnExitState(StateMachine stateMachine)
        {

        }
        
    }
}