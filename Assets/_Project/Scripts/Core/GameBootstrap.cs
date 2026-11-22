using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Huantar.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] string[] initialZones = { "Zone_Pueblo" };

        IEnumerator Start()
        {
            Scene persistent = gameObject.scene;

            foreach (string zone in initialZones)
            {
                if (SceneManager.GetSceneByName(zone).isLoaded) continue;
                AsyncOperation op = SceneManager.LoadSceneAsync(zone, LoadSceneMode.Additive);
                while (!op.isDone) yield return null;
            }

            SceneManager.SetActiveScene(persistent);
        }
    }
}