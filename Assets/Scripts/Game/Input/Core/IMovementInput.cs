using System;
using UnityEngine;

namespace Game.Input.Core 
{
    public interface IMovementInput
    {
        void AddMovementListener(Action<Vector3, float> eventHandler);
        void RemoveMovementListener(Action<Vector3, float> eventHandler);
    }
}
