using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.User
{
    public class CameraAspect : MonoBehaviour
    {
        [SerializeField] Camera cam;
        float Aspect;
        float minimalAspect = 9/16f;
        
        void Update()
        {
            var aspect = Screen.width / (float) Screen.height;
            if (aspect == Aspect) return;
            Aspect = aspect;
            cam.orthographicSize = 5 * Mathf.Clamp((minimalAspect+0.04f) / (float) Aspect, 1, 999f);
        }
    }
}