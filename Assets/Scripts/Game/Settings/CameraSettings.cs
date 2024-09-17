using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Setting 
{
    [CreateAssetMenu(fileName = "new Camera Settings", menuName = "Settings/Camera")]
    public class CameraSettings : ScriptableObject
    {
        [SerializeField] private float _horizontalSensitivity;
        public float HorizontalSensitivity => _horizontalSensitivity;

        [SerializeField] private float _verticalSensitivity;
        public float VerticalSensitivity => _verticalSensitivity;
    }
}
