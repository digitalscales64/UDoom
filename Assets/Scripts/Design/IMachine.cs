using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Design.FSM 
{
    public interface IMachine 
    {
        public UnityAction<IState> OnStateChanged { get; }

        public bool Enabled { get; }

        public IState Current { get; }
        public IState Next { get; }
    }
}
