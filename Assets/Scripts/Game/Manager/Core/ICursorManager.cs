using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager.Core 
{
    public interface ICursorManager
    {
        void RequestCursorMode(GameObject obj, CursorLockMode mode);
    }
}
