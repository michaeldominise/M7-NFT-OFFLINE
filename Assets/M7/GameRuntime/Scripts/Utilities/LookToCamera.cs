using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class LookToCamera : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
