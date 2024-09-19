using System;
using Game.Design.FSM;
using Game.Input.Core;
using UnityEngine;

namespace Game.Doom 
{
    public class MarineAI : Machine
    {
        #region Machine Implementation

        protected override void Initialize()
        {

        }

        protected override void Execute()
        {
            base.Execute();
        }

        #endregion

        #region IMovement Implementation

        public void AddMovementListener(Action<Vector3, float> eventHandler)
        {
            
        }

        public void AddRotationListener(Action<Vector3, float> eventHandler)
        {

        }

        #endregion

        #region IRotation Implementation

        public void RemoveMovementListener(Action<Vector3, float> eventHandler)
        {

        }

        public void RemoveRotationListener(Action<Vector3, float> eventHandler)
        {

        }

        #endregion
    }
}
