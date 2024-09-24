using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Design.FSM 
{
    public interface IState
    {
        // Reference to gameobject
        public GameObject Agent { get; }

        void Enter();
        void Update();
        void Exit(); 
    }
}
