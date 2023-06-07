using UnityEngine;

namespace PixelH8.Controllers
{
    public abstract class StateBase : MonoBehaviour
    {
        public virtual void OnEnterState(StateMachine stateMachine){}
        public virtual void OnExitState(StateMachine stateMachine){}
        public virtual void UpdateState(StateMachine stateMachine){}
        public virtual void OnCollisonEnter(StateMachine stateMachine){}
    }
}