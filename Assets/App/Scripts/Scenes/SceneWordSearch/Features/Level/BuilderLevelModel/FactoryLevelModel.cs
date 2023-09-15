using System;
using System.Collections.Generic;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            const int alphabetLenght = 33;
            const int firstSymbNum = 'а';

            var result = new List<char>();
            var mainLettersCount = new int[alphabetLenght];

            foreach (string word in words)
            {
                int[] currLettersCount = new int[33];

                for (int i = 0; i < word.Length; ++i)
                    ++currLettersCount[word[i] - firstSymbNum];

                for (int i = 0; i < mainLettersCount.Length; ++i)
                    if (currLettersCount[i] > mainLettersCount[i])
                        mainLettersCount[i] = currLettersCount[i];
            }

            for(int i = 0; i < mainLettersCount.Length; ++i)
                for(int j = 0; j < mainLettersCount[i]; ++j)
                    result.Add((char)(i + firstSymbNum));

            return result;
        }
    }
}