using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Setting 
{
    [CreateAssetMenu(fileName = "new Input Settings", menuName = "Settings/Input")]
    public class InputSettings : ScriptableObject
    {
        [Header("Interpolation Setting")]

        [Tooltip("Movement smoothing factor")]
        [SerializeField] private float _movementInputSmoothing;
        public float MovementInputSmoothing => _movementInputSmoothing;

        [Tooltip("Turning smoothing factor")]
        [SerializeField] private float _turningInputSmoothing;
        public float TurningInputSmoothing => _turningInputSmoothing;

        [Header("Axis smoothing factor")]
        [SerializeField] private float _axisInputSmoothing;
        public float AxisInputSmoothing => _axisInputSmoothing;

        [Header("Toggleable Actions")]
        
        [Tooltip("Hold or toggle to duck")]
        [SerializeField] private bool _duckToggle;
        public bool DuckToogle => _duckToggle;
    }
}
