using Game.Attribute;
using Game.Input;
using Game.State;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Controller 
{
    public class MovementController : Design.FSM.Machine
    {
        [SerializeField] private MovementAttributes _movementAttributes;
        [SerializeField] private ProcessedInput _input;

        private Walking _walking;

        protected override void Initialize()
        {
            GameObject self = this.gameObject; 

            // Walking state
            _walking = new Walking(self, _movementAttributes);
            // Callable methods
            _input.OnMovement.AddListener(_walking.SetMovement);
            _input.OnTurning.AddListener(_walking.SetRotation);

            //Initial state
            SetState(_walking);
        }

        protected override void Execute()
        {
            base.Execute();
            
        }
    }
}
