using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Design.FSM 
{
    public abstract class State
    {
        // Reference to gameobject
        public GameObject Agent { get; private set; }

        public State(GameObject agent) 
        {
            Agent = agent;
        }

        public virtual void Enter() {}
        public virtual void Update() {}
        public virtual void Exit() {} 
    }
}
