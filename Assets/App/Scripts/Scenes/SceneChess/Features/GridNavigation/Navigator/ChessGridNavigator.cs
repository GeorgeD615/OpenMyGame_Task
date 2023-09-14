using System;
using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            switch (unit){
                case ChessUnitType.Bishop:
                    return BishopMove(from, to, grid);
                case ChessUnitType.King:
                    return null;
                case ChessUnitType.Knight:
                    return null;
                case ChessUnitType.Pon:
                    return null;
                case ChessUnitType.Queen:
                    return null;
                case ChessUnitType.Rook:
                    return null;
                default:
                    return null;
            }
        }

        private class ChessFigureStep
        {
            public Vector2Int Position { get; set; }
            public ChessFigureStep PrevStep { get; set; }
        }

        private const int _maxPossibleDistance = 7;

        private List<Vector2Int> BishopMove(Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            if (!CheckTheSameColor(from, to))
                return null;

            var queueSteps = new Queue<ChessFigureStep>();
            queueSteps.Enqueue(new ChessFigureStep() { Position = from, PrevStep = null });

            int[] xSteps = new int[] { 1, -1, 1, -1 };
            int[] ySteps = new int[] { 1, 1, -1, -1 };

            return ContinuousChessMovement(queueSteps, xSteps, ySteps, to, grid);
        }

        private bool CheckTheSameColor(Vector2Int firstPosition, Vector2Int secondPosition)
        {
            return ((firstPosition.x + firstPosition.y) % 2 == 0 && (secondPosition.x + secondPosition.y) % 2 == 0) ||
                ((firstPosition.x + firstPosition.y) % 2 == 1 && (secondPosition.x + secondPosition.y) % 2 == 1);
        }
        private bool IsPositionAvailableToMove(Vector2Int pos, ChessGrid grid)
        {
            return IsPositionAvailableToMove(pos.y, pos.x, grid);
        }
        private bool IsPositionAvailableToMove(int i, int j, ChessGrid grid)
        {
            return grid.Get(i, j) == null;
        }

        private List<Vector2Int> ContinuousChessMovement(Queue<ChessFigureStep> queueSteps, int[] xSteps, int[] ySteps, Vector2Int to, ChessGrid grid)
        {
            while (queueSteps.Count != 0)
            {
                for (int i = 0; i < xSteps.Length; ++i)
                {
                    for (int j = 1; j <= _maxPossibleDistance; ++j)
                    {
                        int x = queueSteps.Peek().Position.x + xSteps[i] * j;
                        int y = queueSteps.Peek().Position.y + ySteps[i] * j;
                        if (x > 7 || x < 0 || y > 7 || y < 0 || !IsPositionAvailableToMove(y, x, grid))
                            break;
                        var nextMove = new ChessFigureStep()
                        {
                            Position = new Vector2Int(x, y),
                            PrevStep = queueSteps.Peek()
                        };
                        queueSteps.Enqueue(nextMove);

                        //TODO: Check contains???
                        Debug.Log("check" + x + " " + y);

                        if (nextMove.Position == to)
                        {
                            return GetStepsSequence(nextMove);
                        }
                    }
                }
                queueSteps.Dequeue();
            }
            return null;
        }
        private List<Vector2Int> GetStepsSequence(ChessFigureStep move)
        {
            var result = new List<Vector2Int>();
            ChessFigureStep lastMove = move;
            while (lastMove.PrevStep != null)
            {
                result.Insert(0, lastMove.Position);
                lastMove = lastMove.PrevStep;
            }
            return result;
        }

    }
}