using System;
using UnityEngine;

namespace Game.Input.Core 
{
    public interface IJumpingInput
    {
        void AddJumpingListener(Action<Vector3, float> eventHandler);
        void RemoveJumpingListener(Action<Vector3, float> eventHandler);
    }
}
