using UnityEngine;

namespace SuperScrollView
{
    public class RotateScript : MonoBehaviour
    {
        public float speed = 1f;
        public Vector3 direction;

        // Update is called once per frame
        void Update()
        {
            Vector3 rot = gameObject.transform.localEulerAngles;
            rot.x = rot.x + speed * Time.deltaTime * direction.x;
            rot.y = rot.y + speed * Time.deltaTime * direction.y;
            rot.z = rot.z + speed * Time.deltaTime * direction.z;
            gameObject.transform.localEulerAngles = rot;
        }
    }
}
