using System;
using UnityEngine;

namespace Unity.BossRoom.Gameplay.GameplayObjects
{
    public class ManaOwner : MonoBehaviour
    {
        public event Action<int> ManaReceived;
        
        public void ReceiveMP(int MP)
        {
            ManaReceived?.Invoke(MP);
        }
    }
}
