﻿using System.Collections.Generic;
using _Game.Data;
using _Game.DesignPattern;
using _Game.Managers;
using UnityEngine;
using VinhLB;

namespace _Game.GameGrid.GridSurface
{
    public class GroundSurface : GridSurface
    {
        [SerializeField]
        private MeshRenderer grassRenderer;
        [SerializeField]
        private MeshRenderer groundMeshRenderer;
        [SerializeField]
        private Transform _flowersParentTF;
        [SerializeField]
        private Transform[] _flowerSpawnPoints;

        public MaterialEnum groundMaterialEnum = MaterialEnum.None;

        private List<Flower> _flowerList = new List<Flower>();

        public override void OnInit(Direction rotateDirection = Direction.Forward,
            MaterialEnum materialEnum = MaterialEnum.None, bool hasUnitInMap = false)
        {
            transform.localRotation = Quaternion.Euler(0, BuildingUnitData.GetRotationAngle(rotateDirection), 0);
            // Change material in mesh renderer
            if (groundMeshRenderer == null) return;
            groundMaterialEnum = materialEnum;
            groundMeshRenderer.material = DataManager.Ins.GetSurfaceMaterial(materialEnum);
            if (groundMaterialEnum is MaterialEnum.None || grassRenderer == null) return;
            grassRenderer.material = DataManager.Ins.GetGrassMaterial(materialEnum);

            // Spawn flowers if it does not has unit in map
            if (!hasUnitInMap)
            {
                if (Random.Range(0, 4) < 2 && _flowersParentTF != null && _flowerSpawnPoints.Length > 1)
                {
                    List<int> indexList = new List<int>();
                    int flowerAmount = Random.Range(1, Mathf.Min(4, _flowerSpawnPoints.Length));
                    while (indexList.Count < flowerAmount)
                    {
                        int randomIndex = Random.Range(0, _flowerSpawnPoints.Length);
                        if (!indexList.Contains(randomIndex))
                        {
                            indexList.Add(randomIndex);
                        }
                    }

                    for (int i = 0; i < indexList.Count; i++)
                    {
                        Transform spawnPoint = _flowerSpawnPoints[indexList[i]];
                        Flower flower = SimplePool.Spawn<Flower>(
                            DataManager.Ins.GetRandomEnvironmentObject(PoolType.Flower),
                            spawnPoint.position,
                            spawnPoint.rotation);
                        flower.Tf.SetParent(_flowersParentTF, true);

                        _flowerList.Add(flower);
                    }
                }
            }
        }

        public override void OnDespawn()
        {
            for (int i = _flowerList.Count - 1; i >= 0; i--)
            {
                _flowerList[i].Despawn();

                _flowerList.RemoveAt(i);
            }

            base.OnDespawn();
        }

        [ContextMenu("Set Material to Ground")]
        public void SetMaterialToGround()
        {
            if (groundMeshRenderer == null) return;
            groundMeshRenderer.material = DataManager.Ins.GetSurfaceMaterial(groundMaterialEnum);
        }
    }
}