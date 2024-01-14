using _Game._Scripts.InGame;
using _Game.Managers;
using UnityEngine;

namespace _Game.GameGrid
{
    public class JsonGridDataHandler
    {
        // Convert json file to LevelData
        public static RawLevelData CreateLevelData(int mapIndex) {
            return JsonUtility.FromJson<RawLevelData>(DataManager.Ins.GetGridTextData(mapIndex).text);
        }
        
    }
}
