using System;
using UnityEngine;

namespace Game.State
{
    public class TimedIdle : Design.FSM.State
    {
        private const float DEFAULT_RANGE = 1.0f; // Default range

        private readonly float _randomRange; // Random deviation
        private readonly float _timeToReach; // Time to reach to fire event

        private float _randomTimeToReach; // Time to reach plus Random deviation
        private float _lastTimeStored; // Last  time in system

        #region Base State implementation

        public Action<bool> OnTimerReached { get; set; }

        public TimedIdle(GameObject agent, float reach, float range = DEFAULT_RANGE) : base(agent) 
        {
            _timeToReach = reach;
            _randomRange = range;
        }

        public override void Enter()
        {
            _randomTimeToReach = _timeToReach + UnityEngine.Random.Range(_randomRange, _randomRange);
            _lastTimeStored = Time.time;
        }

        public override void Update()
        {
            float difference = Time.time - _lastTimeStored;
            if(difference >= _randomTimeToReach) 
            {
                OnTimerReached?.Invoke(true);
            }
        }

        public override void Exit()
        {
            OnTimerReached?.Invoke(false);
        }

        #endregion

        public override string ToString()
        {
            return "TimedIdle";
        }

    }
}
