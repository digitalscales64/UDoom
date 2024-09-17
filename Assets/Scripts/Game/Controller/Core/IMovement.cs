using System;
using UnityEngine;

namespace Game.Controller.Core 
{
    public interface IMovement
    {
        void AddCollisionListener(Action<GameObject> eventHandler);
        void RemoveCollisionListener(Action<GameObject> eventHander);
        void SetMovement(Vector3 dir, float mag);
    }
}
