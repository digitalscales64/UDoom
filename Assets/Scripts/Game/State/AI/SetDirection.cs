using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.State
{
    public class SetDirectionArgumentExeption : ArgumentException 
    {
        public SetDirectionArgumentExeption(string message) : base(message) { }
    }

    public class SetDirection : Design.FSM.State
    {
        private const float MIN_DISTANCE_TO_REACH = 0.25f;
        private const float MIN_DISTANCE_TO_STUCK = 0.5f;
        private const float MIN_STUCK_TIME = 5.0f; 

        private readonly float _minDistance;
        
        private int _index;

        private Vector3[] _path;
        public void SetPath(Vector3[] path) 
        {
            _path = path;
        }

        public Action<bool> OnDestinationReached { get; set; }
        public Action<bool> OnStuck { get; set; }

        public Action<Vector3, float> OnMovement { get; set; }

        #region Base State implementation

        public SetDirection(GameObject agent, float stuck = MIN_STUCK_TIME, float min = MIN_DISTANCE_TO_REACH) : base(agent)
        {
            // Min distance should be squared
            _minDistance = min;
        }

        public override void Enter()
        {
            if(_path == null || _path.Length <= 0) 
            {
                string msg = "Path not valid";
                throw new SetDirectionArgumentExeption(msg);
            }

            _index = 0;
        }

        public override void Update()
        {
            if(_index < _path.Length) 
            {
                Vector3 target = _path[_index];
                Vector3 dir = target - Agent.transform.position;

                if(dir.sqrMagnitude <= _minDistance) 
                {
                    _index++;
                }

                OnMovement?.Invoke(dir.normalized, 1.0f);
            } else 
            {
                OnDestinationReached?.Invoke(true);
            }              
        }

        public override void Exit()
        {
            OnDestinationReached?.Invoke(false);
        }

        #endregion

        public override string ToString()
        {
            return "SetDirection";
        }
    }
}
