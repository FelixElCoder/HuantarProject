using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Huantar.Core
{
    [RequireComponent(typeof(Collider))]
    public class ZoneStreamTrigger : MonoBehaviour
    {
        [SerializeField] string[] scenesToLoad;
        [SerializeField] string[] scenesToUnload;

        void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            StartCoroutine(Process());
        }

        IEnumerator Process()
        {
            foreach (string name in scenesToLoad)
            {
                if (SceneManager.GetSceneByName(name).isLoaded) continue;
                Debug.Log($"[Zonas] Cargando {name}");
                AsyncOperation op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                while (!op.isDone) yield return null;
            }

            foreach (string name in scenesToUnload)
            {
                Scene scene = SceneManager.GetSceneByName(name);
                if (!scene.isLoaded) continue;
                Debug.Log($"[Zonas] Descargando {name}");
                AsyncOperation op = SceneManager.UnloadSceneAsync(scene);
                while (op != null && !op.isDone) yield return null;
            }
        }
    }
}