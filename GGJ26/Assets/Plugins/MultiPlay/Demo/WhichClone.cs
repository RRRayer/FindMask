using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiPlay.Demo
{
    public class WhichClone : MonoBehaviour
    {
        void Start()
        {
#if !UNITY_EDITOR
            return;
#endif

            int cloneIndex = 0;
            var utilsType = System.Type.GetType("MultiPlay.Utils, MultiPlay") ?? System.Type.GetType("MultiPlay.Utils");
            if (utilsType != null)
            {
                var method = utilsType.GetMethod("GetCurrentCloneIndex", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (method != null)
                {
                    cloneIndex = (int)method.Invoke(null, null);
                }
            }

            if (cloneIndex == 0) Debug.Log("MultiPlay is running on: Main Project/Server");
            else Debug.Log($"MultiPlay is running on Client: {cloneIndex}");
        }
    }
}
