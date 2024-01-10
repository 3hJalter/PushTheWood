using System.Collections.Generic;
using _Game.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game._Scripts.InGame
{
    public class HintLineTrail : HMonoBehaviour
    {
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private ParticleSystem particle;
        [ReadOnly]
        [SerializeField] private Vector3 currentDestination;
        [ReadOnly]
        [SerializeField] private bool isMoving;
        [ReadOnly]
        [SerializeField] private List<Vector3> path = new();
        private int _pathDestinationIndex;

        private void Update()
        {
            if (!isMoving || path.Count <= 0) return;
            // Move to destination
            transform.position = Vector3.MoveTowards(transform.position, currentDestination, moveSpeed * Time.deltaTime);
            // if reach to destination, set new destination
            if (!(Vector3.Distance(Tf.position, currentDestination) < 0.01f)) return;
            if (_pathDestinationIndex < path.Count - 1)
            {
                _pathDestinationIndex++;
                currentDestination = path[_pathDestinationIndex];
            }
            else
            {
                isMoving = false;
            }
        }
        
        public void SetPath(List<Vector3> pathIn)
        {
            if (pathIn.Count == 0) return;
            path = pathIn;
        }
        
        public void OnPlay()
        {
            if (path.Count == 0) return;
            DevLog.Log(DevId.Hoang, "TODO: Fade in particle when play");
            particle.Play();
            isMoving = true;
            Tf.position = path[0];
            _pathDestinationIndex = 1;
            currentDestination = path[_pathDestinationIndex];
        }

        public void OnCancel()
        {
            DevLog.Log(DevId.Hoang, "TODO: Fade out particle when cancel");
            particle.Stop();
            isMoving = false;
        }
    }
}
