using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Huantar.Core;
using Huantar.Narrative;

namespace Huantar.Telemetry
{
    [Serializable]
    public class DeathRecord
    {
        public float time;
        public string byWhom;
        public float x, y, z;
        public string lastCheckpoint;
    }

    [Serializable]
    public class TimedRecord
    {
        public float time;
        public string id;
    }

    [Serializable]
    public class SessionData
    {
        public string sessionCode;
        public string startedAt;
        public float totalSeconds;
        public bool completed;
        public int entriesDiscovered;
        public int entriesTotal;
        public List<DeathRecord> deaths = new();
        public List<TimedRecord> checkpoints = new();
        public List<TimedRecord> entries = new();
    }

    public class PlaytestLogger : MonoBehaviour
    {
        public static PlaytestLogger Instance { get; private set; }

        public string SessionCode => data.sessionCode;
        public string LastSavePath { get; private set; }

        readonly SessionData data = new();
        float startTime;
        Transform player;
        string lastCheckpoint = "Inicio";

        float Now => Time.realtimeSinceStartup - startTime;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            startTime = Time.realtimeSinceStartup;
            data.startedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            data.sessionCode = "S" + DateTime.Now.ToString("MMdd-HHmm");
        }

        void OnEnable()
        {
            GameEvents.PlayerCaught += OnCaught;
            GameEvents.CheckpointReached += OnCheckpoint;
            GameEvents.BetaFinished += OnFinished;
        }

        void OnDisable()
        {
            GameEvents.PlayerCaught -= OnCaught;
            GameEvents.CheckpointReached -= OnCheckpoint;
            GameEvents.BetaFinished -= OnFinished;
        }

        void Start()
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;

            if (FieldNotebook.Instance != null)
                FieldNotebook.Instance.EntryAdded += OnEntry;
        }

        void OnDestroy()
        {
            if (FieldNotebook.Instance != null)
                FieldNotebook.Instance.EntryAdded -= OnEntry;
        }

        void OnCaught(Transform byWhom)
        {
            Vector3 pos = player != null ? player.position : Vector3.zero;
            data.deaths.Add(new DeathRecord
            {
                time = Now,
                byWhom = byWhom != null ? byWhom.name : "?",
                x = pos.x, y = pos.y, z = pos.z,
                lastCheckpoint = lastCheckpoint
            });
        }

        void OnCheckpoint(string id)
        {
            lastCheckpoint = id;
            data.checkpoints.Add(new TimedRecord { time = Now, id = id });
        }

        void OnEntry(NotebookEntryData entry)
        {
            data.entries.Add(new TimedRecord { time = Now, id = entry.title });
        }

        void OnFinished()
        {
            data.completed = true;
            Save();
        }

        void OnApplicationQuit()
        {
            Save();
        }

        void Save()
        {
            data.totalSeconds = Now;
            if (FieldNotebook.Instance != null)
            {
                data.entriesDiscovered = FieldNotebook.Instance.CountDiscoveredAll;
                data.entriesTotal = FieldNotebook.Instance.CountTotalAll;
            }

            string dir = Path.Combine(Application.persistentDataPath, "Playtests");
            Directory.CreateDirectory(dir);
            LastSavePath = Path.Combine(dir, $"sesion_{data.sessionCode}.json");
            File.WriteAllText(LastSavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"[Playtest] Sesión guardada en: {LastSavePath}");
        }
    }
}