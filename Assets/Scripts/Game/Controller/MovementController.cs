using Game.Attribute;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Controller 
{
    public class MovementController 
    {
        public void OnMovement(InputAction.CallbackContext context) 
        {
            Vector2 input = context.ReadValue<Vector2>();
        }

        public void OnRotation(InputAction.CallbackContext context) 
        {
            bool input = context.performed;
        }
    }
}
