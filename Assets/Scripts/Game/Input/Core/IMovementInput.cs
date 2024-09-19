using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Input.Core 
{
    public interface IMovementInput
    {
        UnityEvent<Vector3, float> OnMovementInput { get; }
    }
}
