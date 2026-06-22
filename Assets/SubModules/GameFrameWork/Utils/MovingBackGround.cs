using UnityEngine;

namespace GFramework.Utils
{
    public class MovingBackGround : MonoBehaviour
    {
        public Transform Left;
        public Transform Right;
        public Vector2 Size;
        public float Speed;
        public Vector2 Direction;

        private Vector3 curPos;

        void Start()
        {
            curPos = transform.position;
        }
        private void FixedUpdate()
        {
            curPos += (Time.smoothDeltaTime * Speed * (Vector3)Direction);
                if (Direction.x > 0)
                {
                    if ((curPos.x - Size.x / 2f) > Right.position.x)
                    {
                        curPos.x = Left.position.x - Size.x / 2f;
                    }
                }
                else if (Direction.x < 0)
                {
                    if ((curPos.x + Size.x / 2f) < Left.position.x)
                    {
                        curPos.x = Right.position.x + Size.x / 2f;
                    }
                }
                transform.position = curPos;
        }

        [ContextMenu("GetBound")]
        public void UpdateBoundCollider()
        {
            Size = GetComponent<SpriteRenderer>().bounds.size;
        }
    }
}
