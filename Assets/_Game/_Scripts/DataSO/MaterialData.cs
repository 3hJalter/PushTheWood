using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "MaterialData", menuName = "ScriptableObjects/MaterialData", order = 1)]
    public class MaterialData : SerializedScriptableObject
    {
        [SerializeField] private Material transparentMaterial;
        
        public Material GetTransparentMaterial()
        {
            return transparentMaterial;
        }

        [Title("Surface Material")]
        [SerializeField] private readonly Dictionary<MaterialEnum, Material> _surfaceMaterialDic = new();
        
        [Title("Grass Material")]
        [SerializeField] private readonly Dictionary<MaterialEnum, Material> _grassMaterialDic = new();
        
        public int CountSurfaceMaterial => _surfaceMaterialDic.Count;
        
        public int CountGrassMaterial => _grassMaterialDic.Count;
        
        public Material GetSurfaceMaterial(MaterialEnum materialEnum)
        {
            return _surfaceMaterialDic.GetValueOrDefault(materialEnum);
        }
        
        public Material GetGrassMaterial(MaterialEnum materialEnum)
        {
            return _grassMaterialDic.GetValueOrDefault(materialEnum);
        }
    }

    public enum MaterialEnum
    {
        None = -1,
        LightGreen = 0,
        MediumGreen = 1,
        DarkGreen = 2,
    }
}
