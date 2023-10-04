using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.AI
{
    public class Obstance : MonoBehaviour
    {
        [SerializeField]
        BoxCollider2D Collider2D;
        [SerializeField]
        Transform mTransform;

        List<Vector2> pointExist = new List<Vector2>();

        public List<Vector2> PointExists
        {
            get
            {
                pointExist.Clear();
                Vector2 addVector = Collider2D.size * mTransform.localScale / 2.1f;
                pointExist.Add((Vector2)mTransform.position + Collider2D.offset + addVector);
                pointExist.Add((Vector2)mTransform.position + new Vector2(addVector.x, -addVector.y));
                pointExist.Add((Vector2)mTransform.position + new Vector2(-addVector.x, addVector.y));
                pointExist.Add((Vector2)mTransform.position + new Vector2(-addVector.x, -addVector.y));
                return pointExist;
            }
        }
    }
}