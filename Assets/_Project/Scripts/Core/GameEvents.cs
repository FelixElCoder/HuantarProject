using System;
using UnityEngine;

namespace Huantar.Core
{
    public static class GameEvents
    {
        public static event Action<Transform> PlayerCaught;
        public static event Action PlayerRespawned;

        public static void RaisePlayerCaught(Transform byWhom) => PlayerCaught?.Invoke(byWhom);
        public static void RaisePlayerRespawned() => PlayerRespawned?.Invoke();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            PlayerCaught = null;
            PlayerRespawned = null;
        }
    }
}