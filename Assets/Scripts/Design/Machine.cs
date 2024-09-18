using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Design.FSM 
{
    public class FiniteStateMachineException : ArgumentException 
    {
        public FiniteStateMachineException(string arg) : base(arg) {}
    } 

    public abstract class Machine : MonoBehaviour
    {
        public UnityAction<State> OnStateChanged { get; private set; }

        public State Current { get; private set; }
        public State Next { get; private set; }

        void Start() 
        {
            Initialize();
        }

        void Update() 
        {
            Execute();
        }

        protected abstract void Initialize();

        protected virtual void Execute() 
        {
            // If current state is null, raise an error
            if(Current == null) 
            {
                string msg = $"Null exeption, current state: {Current} is null";
                throw new FiniteStateMachineException(msg);
            }

            Current?.Update();
        }

        public void SetState(State next) 
        {
            // If new state is null, raise an error
            if(next == null) 
            {
                string msg = $"Null exeption, next state: {next} is null";
                throw new FiniteStateMachineException(msg);
            }

            // Exit current state if not null
            Current?.Exit();
            Current = next;

            // Enter new state
            Current?.Enter();
            OnStateChanged?.Invoke(Current);      
        }
    }
}
