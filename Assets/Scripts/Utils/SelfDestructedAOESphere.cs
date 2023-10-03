using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.BossRoom.Utils
{
    public class SelfDestructedAOESphere : MonoBehaviour
    {
        public void SetDestroyTime(float destroyTime)
        {
            Destroy(gameObject, destroyTime);
        }
    }
}