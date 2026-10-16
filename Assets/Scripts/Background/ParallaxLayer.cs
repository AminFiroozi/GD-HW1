using UnityEngine;
using Game.Input;

namespace Game.Background
{
    public class ParallaxLayer : MonoBehaviour
    {
        public float speedMultiplier = 1f;
        public float tileWidth = 24f;

        Transform[] tiles;

        void Awake()
        {
            int count = transform.childCount;
            tiles = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                tiles[i] = transform.GetChild(i);
            }
        }

        void Update()
        {
            float speed = GameSpeed.ScrollSpeed * speedMultiplier;
            float delta = speed * Time.deltaTime;
            float wrapDistance = tileWidth * tiles.Length;

            for (int i = 0; i < tiles.Length; i++)
            {
                Vector3 pos = tiles[i].localPosition;
                pos.x -= delta;
                if (pos.x <= -tileWidth)
                {
                    pos.x += wrapDistance;
                }
                tiles[i].localPosition = pos;
            }
        }
    }
}
