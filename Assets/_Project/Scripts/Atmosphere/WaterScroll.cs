using UnityEngine;

namespace Huantar.Atmosphere
{
    [RequireComponent(typeof(Renderer))]
    public class WaterScroll : MonoBehaviour
    {
        [SerializeField] Vector2 speed = new Vector2(0.02f, 0.05f);

        Material material;
        Vector2 offset;

        void Awake()
        {
            material = GetComponent<Renderer>().material;
        }

        void Update()
        {
            offset += speed * Time.deltaTime;
            material.SetTextureOffset("_BaseMap", offset);
        }
    }
}