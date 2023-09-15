using System;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;
using System.IO;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        private readonly string levelsDataPath = Application.dataPath + @"\App\Resources\WordSearch\Levels\";

        public LevelInfo LoadLevelData(int levelIndex)
        {
            string jsonInfo;
            using (StreamReader reader = new StreamReader($"{levelsDataPath}{levelIndex}.json"))
            {
                jsonInfo = reader.ReadToEnd();
            }
            return JsonUtility.FromJson<LevelInfo>(jsonInfo);
        }

    }
}