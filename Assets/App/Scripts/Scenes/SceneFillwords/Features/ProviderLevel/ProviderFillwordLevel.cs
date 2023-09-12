using System;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using System.IO;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private string wordsDataPath = Application.dataPath + @"\App\Resources\Fillwords\words_list.txt";
        private string levelsDataPath = Application.dataPath + @"\App\Resources\Fillwords\pack_0.txt";

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

            WordOnLevel[] wordsOnLevel = GetWordsOnLevelData(LevelsDescriptions[index - 1]);

            int gridSize = GetGridSize(wordsOnLevel);

            return CreateGridFillWords(wordsOnLevel, gridSize);
        }

        private class WordOnLevel
        {
            public string WordText { get; private set; }
            public int[] LettersPositions { get; private set; }

            public WordOnLevel(string word, string[] lettesPositions)
            {
                if (word.Length != lettesPositions.Length) 
                    throw new Exception("слова из словаря по индексу не совпадает по длине с индексами из уровня");

                WordText = word;
                LettersPositions = new int[lettesPositions.Length];
                for (int i = 0; i < lettesPositions.Length; ++i)
                {
                    LettersPositions[i] = int.Parse(lettesPositions[i]);
                }
            }

        }
        private WordOnLevel[] GetWordsOnLevelData(string levelInfo)
        {
            string[] wordsData = levelInfo.Split(' ');

            var wordsOnLevel = new WordOnLevel[wordsData.Length / 2];
            for (int i = 0; i < wordsOnLevel.Length; ++i)
            {
                wordsOnLevel[i] = new WordOnLevel(Vocabulary[int.Parse(wordsData[i * 2])], wordsData[i * 2 + 1].Split(';'));
            }

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
                throw new Exception("есть индексы которых не может быть на данном уровне");

            int gridSize = (int)Math.Sqrt(indexCounter);
            if (gridSize * gridSize != indexCounter) 
                throw new Exception("кол-во символов в уровне не возможно уложить в квадратную сетку так, чтобы не было ни пустых и лишних символов");

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
                        throw new Exception("в данных уровня в есть символы которые ссылаются на одну и ту же клетку");

                    gridFillWords.Set(row, col, new CharGridModel(word.WordText[i]));
                }
            }
            return gridFillWords;
        }
    }
}