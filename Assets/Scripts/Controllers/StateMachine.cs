using System.Collections.Generic;
using UnityEngine;
using PixelH8.Controllers.States;

namespace PixelH8.Controllers
{
    public class StateMachine : MonoBehaviour
    {
        public Actors.ActorAI ActorAI;
        public string _currentState;
        public StateBase CurrentState {get; private set;}
        public StateBase DefaultState {get; private set;}
        public StateBase Idle {get; private set;}
        public StateBase Wander {get; private set;}
        void Start()
        {
            Idle = GetComponentInChildren<ActorStateIdle>();
            Wander = GetComponentInChildren<ActorStateWander>();
            DefaultState = Idle;
            SetState(DefaultState);
        }

        void Update()
        {
            if(CurrentState != null)
                CurrentState.UpdateState(this);
        }

        public void SetState(StateBase state)
        {
            if(CurrentState != null)
                CurrentState.OnExitState(this);

            CurrentState = state;
            _currentState = CurrentState.transform.name;
            CurrentState.OnEnterState(this);
        }

        public void SetDefaultState(StateBase state)
        {
            DefaultState = state;
        }
    }
}