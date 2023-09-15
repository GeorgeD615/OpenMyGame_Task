using System;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using System.IO;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private readonly string wordsDataPath = Application.dataPath + @"\App\Resources\Fillwords\words_list.txt";
        private readonly string levelsDataPath = Application.dataPath + @"\App\Resources\Fillwords\pack_0.txt";
        public string[] LevelsDescriptions { get; private set; }
        public string[] Vocabulary { get; private set; }
        private void Init()
        {
            if (LevelsDescriptions == null)
                LevelsDescriptions = File.ReadAllLines(levelsDataPath);

            if (Vocabulary == null)
                Vocabulary = File.ReadAllLines(wordsDataPath);
        }

        public GridFillWords LoadModel(int index)
        {
            Init();
            
            GridFillWords gridFillWords = null;

            while(gridFillWords == null && index <= LevelsDescriptions.Length)
            {
                gridFillWords = FindCorrectLevel(index);
                ++index;
            }

            if (gridFillWords == null)
                throw new Exception("There is no correct level");

            return gridFillWords;
        }

        private class WordOnLevel
        {
            public string WordText { get; private set; }
            public int[] LettersPositions { get; private set; }
            public WordOnLevel(string word, string[] lettesPositions)
            {
                if (word.Length != lettesPositions.Length) 
                    throw new Exception("Words from index dictionary are not the same length as indexes from level.");

                WordText = word;
                LettersPositions = new int[lettesPositions.Length];
                for (int i = 0; i < lettesPositions.Length; ++i)
                {
                    LettersPositions[i] = int.Parse(lettesPositions[i]);
                }
            }

        }

        private GridFillWords FindCorrectLevel(int index)
        {
            try
            {
                WordOnLevel[] wordsOnLevel = GetWordsOnLevelData(LevelsDescriptions[index - 1]);

                int gridSize = GetGridSize(wordsOnLevel);

                return CreateGridFillWords(wordsOnLevel, gridSize);
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e.Message);
                return null;
            }
        }

        private WordOnLevel[] GetWordsOnLevelData(string levelInfo)
        {
            string[] wordsData = levelInfo.Split(' ');

            var wordsOnLevel = new WordOnLevel[wordsData.Length / 2];

            for (int i = 0; i < wordsOnLevel.Length; ++i)
                wordsOnLevel[i] = new WordOnLevel(Vocabulary[int.Parse(wordsData[i * 2])], wordsData[i * 2 + 1].Split(';'));

            return wordsOnLevel;
        }
        private int GetGridSize(WordOnLevel[] wordsInfo)
        {
            int maxIndex = -1;
            int indexCounter = 0;
            foreach (var word in wordsInfo)
            {
                for (int i = 0; i < word.LettersPositions.Length; ++i)
                {
                    ++indexCounter;
                    if (word.LettersPositions[i] > maxIndex)
                        maxIndex = word.LettersPositions[i];
                }
            }
            if (indexCounter != maxIndex + 1)
                throw new Exception("There are indices that cannot be at this level.");

            int gridSize = (int)Math.Sqrt(indexCounter);
            if (gridSize * gridSize != indexCounter) 
                throw new Exception("The number of characters in the level cannot be placed in a square grid.");

            return gridSize;
        }
        private GridFillWords CreateGridFillWords(WordOnLevel[] wordsOnLevel, int gridSize)
        {
            GridFillWords gridFillWords = new GridFillWords(new Vector2Int(gridSize, gridSize));

            foreach (var word in wordsOnLevel)
            {
                for (int i = 0; i < word.LettersPositions.Length; ++i)
                {
                    int row = word.LettersPositions[i] / gridSize;
                    int col = word.LettersPositions[i] % gridSize;
                    if (gridFillWords.Get(row, col) != null)
                        throw new Exception("In the level data, there are symbols that refer to the same cell.");

                    gridFillWords.Set(row, col, new CharGridModel(word.WordText[i]));
                }
            }
            return gridFillWords;
        }
    }
}