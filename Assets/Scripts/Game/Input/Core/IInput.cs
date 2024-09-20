using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Input.Core 
{
    public interface IInput 
    {
        UnityEvent<Vector2, float> OnMovement { get; }
        UnityEvent<Vector2, float> OnTurning { get; }
        UnityEvent<bool> OnJumping { get; }
    }
}
