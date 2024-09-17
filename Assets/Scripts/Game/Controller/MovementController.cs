using System;
using Game.Attribute;
using Game.Controller.Core;
using UnityEngine;

namespace Game.Controller 
{
    [RequireComponent(typeof(CapsuleCollider), typeof(IInput))]
    public class MovementController : MonoBehaviour, Core.IMovement
    {
        private const int MAX_NEIGHBOURS = 16;
        private const float GRAVITY = 9.8f;
        private const float MIN_DOT_SURFACE = 0.3f;
        private const float EPSILON = 0.3f;

        #region Serialized Fields

        [SerializeField] private Movement _attributes;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private bool _useGravity;

        #endregion

        private Action<GameObject> _onCollision;
        private Action<bool> _onFalling;

        // Desired Movement
        private Vector3 _inputDirection;
        private float _inputMagnitude;

        // Collisions
        private Collider[] _neighbours;

        // Falling
        private bool _falling;
        private float _gravity;

        void Start() 
        {
            IInput input = GetComponent<IInput>();
            input.AddMovementListener(SetMovement);

            if(!_collider) 
            {
                _collider = GetComponent<CapsuleCollider>(); 
                string msg = "Capsule collider not set for MovementController";
                Debug.LogWarning(msg);
            }

            _neighbours = new Collider[MAX_NEIGHBOURS];
        }

        void FixedUpdate() 
        {
            int count = Physics.OverlapCapsuleNonAlloc
            (
                transform.position + Vector3.up * _collider.height/2.0f,
                transform.position - Vector3.up * _collider.height/2.0f,
                _collider.radius + EPSILON, _neighbours

            );

            // Desired Movement
            float impulse = _inputMagnitude * _attributes.MovementSpeed * Time.fixedDeltaTime;
            Vector3 advance = _inputDirection * impulse + Vector3.down * _gravity;

            // Depenetration
            Vector3 resolve = Vector3.zero;

            // Ground checking
            bool groundFlag = false;

            for(int i = 0; i < count; i++) 
            {
                Collider other = _neighbours[i];
                if(_collider == other) continue;

                Vector3 otherPosition = other.gameObject.transform.position;
                Quaternion otherRotation = other.gameObject.transform.rotation; 

                _onCollision?.Invoke(other.gameObject);

                Vector3 direction;
                float distance;

                bool overlapped = Physics.ComputePenetration
                (
                _collider, transform.position + advance, transform.rotation,
                other, otherPosition, otherRotation,
                out direction, out distance
                );

                if(!overlapped) continue;

                RaycastHit hit;
                if(Physics.Raycast(transform.position + Vector3.down * EPSILON, -direction, out hit, 10.0f))
                {
                    float dot = Vector3.Dot(hit.normal, Vector3.up);
                    if(dot >= MIN_DOT_SURFACE) 
                    {
                        groundFlag = true;
                    }   
                }

                resolve += direction * distance;
            }

            _falling = !groundFlag;
            _onFalling?.Invoke(_falling);

            // Check gravity
            if(_useGravity) 
            {
                if(_falling) 
                {
                    _gravity += GRAVITY * Time.fixedDeltaTime * Time.fixedDeltaTime;
                } else 
                {
                    _gravity = 0.0f;
                }
            }

            // Advance is the desired movement, and resolve is a vector stopping the advance
            transform.position += advance + resolve;
        }

        public void AddFallingListener(Action<bool> eventHandler) 
        {
            _onFalling += eventHandler;
        }

        public void RemoveFallingListener(Action<bool> eventHandler) 
        {
            _onFalling -= eventHandler;
        }

        #region IMovement Implementation

        public void AddCollisionListener(Action<GameObject> eventHandler) 
        {
            _onCollision += eventHandler;
        }

        public void RemoveCollisionListener(Action<GameObject> eventHander) 
        {
            _onCollision -= eventHander;
        }

        public void SetMovement(Vector3 dir, float mag)
        {
            _inputDirection = dir;
            _inputMagnitude = mag;
        }

        #endregion
    }
}
