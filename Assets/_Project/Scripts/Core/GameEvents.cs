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

        public static event Action BetaFinished;
        public static void RaiseBetaFinished() => BetaFinished?.Invoke();

        public static event Action<string> CheckpointReached;
        public static void RaiseCheckpointReached(string id) => CheckpointReached?.Invoke(id);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            PlayerCaught = null;
            PlayerRespawned = null;
            BetaFinished = null;
            CheckpointReached = null;
        }
    }
}