using UnityEngine;
using PixelH8.Controllers;
using PixelH8.Data;

namespace PixelH8.Controllers.States
{
    public class ActorStateWander : StateBase
    {
        [SerializeField] private Vector3 desiredLocation;
        [SerializeField] private Vector3 desiredRot;
        [SerializeField] private float stopDistance;


        [SerializeField] private bool NavDestSet;
        [SerializeField] private bool obstructed;
        [SerializeField] private bool moving;

        public override void OnEnterState(StateMachine stateMachine)
        {
            var ran = UnityEngine.Random.insideUnitCircle * 10;
            desiredLocation = stateMachine.ActorAI.startPosition + new Vector3(ran.x, 0, ran.y);
            desiredRot = desiredLocation - stateMachine.ActorAI.transform.position;
            //desiredRot = new Vector3(desiredRot.x,0,desiredRot.z);
            desiredRot.Normalize();
            stopDistance = Random.Range(1, 2);
            stateMachine.ActorAI.navMeshAgent.angularSpeed = 0;
        }
        public override void UpdateState(StateMachine stateMachine)
        {
            if (!NavDestSet)
                SetNavDestination(stateMachine);
            else if (NavDestSet)
            {
                if (!obstructed && moving)
                {
                    var hitInfo = CheckForObstruction(stateMachine, 1, ObjectsAndData.Instance.constants.SolidObjects);
                    if (hitInfo.collider != null)
                    {
                        Debug.Log("Obstructed!");
                        obstructed = !obstructed;
                        desiredRot = new Vector3(hitInfo.normal.x, 0, hitInfo.normal.z);
                        desiredRot.Normalize();
                        
                        desiredLocation = stateMachine.ActorAI.navMeshAgent.transform.position;
                        stateMachine.ActorAI.navMeshAgent.SetDestination(desiredLocation);
                        moving = false;
                    }
                }

                //if (rotating)
                LerpYRotation(stateMachine);
                Debug.Log(Vector3.Distance(desiredRot, stateMachine.ActorAI.transform.forward));
                if (CheckDistance(stateMachine))
                {
                    if (moving)
                    {
                        //desiredLocation = stateMachine.ActorAI.navMeshAgent.transform.position;
                        //stateMachine.ActorAI.navMeshAgent.SetDestination(desiredLocation);
                        moving = false;
                    }

                    if (CheckRotation(stateMachine))
                        stateMachine.SetState(stateMachine.Idle);
                }
            }
        }

        public override void OnExitState(StateMachine stateMachine)
        {
            NavDestSet = false;
            obstructed = false;
            moving = false;
            stateMachine.ActorAI.navMeshAgent.angularSpeed = 120;
        }

        void SetNavDestination(StateMachine stateMachine)
        {
            stateMachine.ActorAI.navMeshAgent.SetDestination(desiredLocation);
            NavDestSet = true;
            moving = true;
        }

        void LerpYRotation(StateMachine stateMachine)
        {
            if (CheckRotation(stateMachine))
            {
                stateMachine.ActorAI.transform.forward = desiredRot;
            }
            else
            {
                var lerpRot = Vector3.Slerp(stateMachine.ActorAI.transform.forward, desiredRot, Time.deltaTime * 4);
                stateMachine.ActorAI.transform.forward = lerpRot;
            }
        }

        bool CheckDistance(StateMachine stateMachine)
        {
            return Vector3.Distance(stateMachine.ActorAI.transform.position, desiredLocation) < stopDistance;
        }

        bool CheckRotation(StateMachine stateMachine)
        {
            return Vector3.Distance(desiredRot, stateMachine.ActorAI.transform.forward) < 0.1f;
        }

        RaycastHit CheckForObstruction(StateMachine stateMachine, float Distance, LayerMask mask)
        {
            var pos = stateMachine.ActorAI.transform.position + new Vector3(0, 1, 0);
            var rot = stateMachine.ActorAI.transform.forward;
            if (Physics.Raycast(pos, rot, out RaycastHit hitInfo, Distance, mask))
                return hitInfo;
            else
                return new RaycastHit();
        }
    }
}