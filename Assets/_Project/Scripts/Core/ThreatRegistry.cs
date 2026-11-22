using System.Collections.Generic;
using UnityEngine;

namespace Huantar.Core
{
    public static class ThreatRegistry
    {
        static readonly List<Transform> threats = new List<Transform>();
        public static IReadOnlyList<Transform> Threats => threats;

        public static void Register(Transform t)
        {
            if (!threats.Contains(t)) threats.Add(t);
        }

        public static void Unregister(Transform t) => threats.Remove(t);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => threats.Clear();
    }
}