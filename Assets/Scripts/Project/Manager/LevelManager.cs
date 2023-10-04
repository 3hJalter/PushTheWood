using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Utilities.AI;
namespace Project
{
    public class LevelManager : MonoBehaviour
    {
        private static LevelManager inst;
        public static LevelManager Inst => inst;
        [SerializeField]
        private Map map;
        [SerializeField]
        private MovingAgent Agent;
        public Map Map => map;
        void Awake()
        {
            if (inst == null)
            {
                inst = this;
                return;
            }
            Destroy(gameObject);
        }

        private void Start()
        {
            InitLevel();
        }
        private void InitLevel()
        {
            Agent.transform.position = map.MapGrid.GetGridCell(0, 0).WorldPos;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = GridUtilities.GetMouseWorldPosition();
                Agent.SetDestination(mousePos);
            }
        }

    }
}