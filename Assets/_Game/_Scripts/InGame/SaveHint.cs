using System;
using System.Collections.Generic;
using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using _Game.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game._Scripts.InGame
{
    public class SaveHint
    {
        [Serializable]
        public record PlayerStep
        {
            public int x;
            public int y;
            public int d; // Direction
            public int i; //  Island ID
        }
        
        [Serializable]
        public class PlayerStepsWrapper
        {
            public List<PlayerStep> pS;
        }
        
        public static List<PlayerStep> ConvertJsonToPlayerSteps(string json)
        {
            PlayerStepsWrapper wrapper = JsonUtility.FromJson<PlayerStepsWrapper>(json);
            return wrapper.pS;
        }
        
        private readonly List<PlayerStep> playerSteps = new();
        
        public void SaveStep(int x, int y, int direction, int islandID)
        {
            playerSteps.Add(new PlayerStep
            {
                x = x,
                y = y,
                d = direction,
                i = islandID
            });
        }

        public void Save()
        {
            Level level = LevelManager.Ins.CurrentLevel;
            // Get type
            LevelType levelType = level.LevelType;
            int levelIndex = level.Index;
            // Save name as H_Lvl_{levelTypePrefix}_{levelIndex}
            // Save to Resources/Levels/Hint
            
            // Get the text asset of the current level
            TextAsset data = DataManager.Ins.GetLevelData(levelType, levelIndex);
            RawLevelData rawLevelData = JsonGridDataHandler.CreateLevelData(levelType, levelIndex);
            
            string dataName = data.name;
            
            string fileName = $"H_{dataName}";
            string path = $"Assets/_Game/Resources/Hint/{fileName}.json";
            // convert list to json
            // ...
            PlayerStepsWrapper wrapper = new() { pS = this.playerSteps };
            string json = JsonUtility.ToJson(wrapper);
            System.IO.File.WriteAllText(path, json);

            //  List to array the wrapper.pS
            PlayerStep[] playerStepsArray = wrapper.pS.ToArray();
            // Set RawLevelData pS to playerStepsArray
            rawLevelData.pS = playerStepsArray;
            
            // Convert rawLevelData to json
            string dataText = JsonUtility.ToJson(rawLevelData);
            
            string levelFolderName = levelType switch
            {
                LevelType.DailyChallenge => "DailyChallenge",
                LevelType.Secret => "Secret",
                _ => "Normal"
            };
            
            path = $"Assets/_Game/Resources/Level/{levelFolderName}/{dataName}.json";
            System.IO.File.WriteAllText(path, dataText);
            
            DevLog.Log(DevId.Hoang, $"Saved hint for {dataName} at {path}");
        }
    }
}
