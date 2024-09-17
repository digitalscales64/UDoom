using System;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI.State 
{
    public class CalculatePath : Design.FSM.State
    {
        private readonly NavMeshPath _path;

        private Vector3 _destination;
        public void SetDestination(Vector3 destination) 
        {
            _destination = destination;
        }

        public Action<Vector3[]> OnPathCalculated { get; set; }
        public Action<bool> OnUnreachableDestination { get; set; }

        #region Base State implementation

        public CalculatePath(GameObject agent) : base(agent) 
        {
            _path = new NavMeshPath();
        }

        public override void Enter()
        {
            bool success = NavMesh.CalculatePath(Agent.transform.position, _destination, NavMesh.AllAreas, _path);
            if(!success) 
            {
                // Calculate nearest path
                NavMeshHit _hit;
                bool flag = NavMesh.SamplePosition(_destination, out _hit, Vector3.Distance(Agent.transform.position, _destination), NavMesh.AllAreas);
                if(!flag) 
                {
                    OnUnreachableDestination?.Invoke(true);
                    return;
                }

                // Recalculate
                bool reacl = NavMesh.CalculatePath(Agent.transform.position, _hit.position, NavMesh.AllAreas, _path);
                if(!reacl) 
                {
                    OnUnreachableDestination?.Invoke(true);
                    return;
                }
            }

            if(_path.corners.Length <= 0) 
            {
                // Can't reach destination
                OnUnreachableDestination?.Invoke(true);                
            }

            Vector3[] points = _path.corners;
            OnPathCalculated?.Invoke(points);
        }

        // No update

        public override void Exit()
        {
            OnUnreachableDestination?.Invoke(false);
        }

        #endregion

        public override string ToString()
        {
            return "Calculate Path";
        }

    }
}
