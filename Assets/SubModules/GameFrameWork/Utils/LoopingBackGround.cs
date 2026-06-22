using UnityEngine;

namespace GFramework.Utils
{
    public class LoopingBackGround : MonoBehaviour
    {
        public float Speed;
        private float offset;
        public Material mat;

        private void Start()
        {
            mat = GetComponent<Renderer>().material;
        }

        private void Update()
        {
            offset += (Time.smoothDeltaTime * Speed) / 10f;
            mat.SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }
}
