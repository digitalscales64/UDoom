using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Attribute 
{
    [CreateAssetMenu(fileName = "new Movement Attributes", menuName = "Attributes/Movement")]
    public class Movement : ScriptableObject
    {
        [SerializeField] private float _movementSpeed;
        public float MovementSpeed => _movementSpeed;
    }
}
