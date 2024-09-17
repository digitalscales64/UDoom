using System;
using UnityEngine;

namespace Game.Controller.Core 
{
    public interface IInput
    {
        void AddMovementListener(Action<Vector3, float> eventHandler);
        void RemoveMovementListener(Action<Vector3, float> eventHandler);
    }
}
