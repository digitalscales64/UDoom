using System;
using UnityEngine;

namespace Game.AI.State 
{
    public class RandomPoint : Design.FSM.State
    {
        private readonly float _range;

        #region Base State implementation

        public Action<Vector3> OnPointSelected { get; set; }

        public RandomPoint(GameObject agent, float range) : base(agent) 
        {
            _range = range;
        }

        public override void Enter()
        {
            float x = UnityEngine.Random.Range(-_range, _range);
            float z = UnityEngine.Random.Range(-_range, _range);

            Vector3 point = new Vector3(x, 0.0f, z);
            OnPointSelected?.Invoke(point);
        }

        // No update
        // No exit

        #endregion

        public override string ToString()
        {
            return "RandomPoint";
        }
    }
}
