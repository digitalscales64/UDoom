using System;
using UnityEngine;

namespace Game.Input.Core 
{
    public interface IRotationInput
    {
        void AddRotationListener(Action<Vector3, float> eventHandler);
        void RemoveRotationListener(Action<Vector3, float> eventHandler);
    }
}
