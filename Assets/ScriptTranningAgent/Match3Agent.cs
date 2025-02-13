using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace KnightChessTatic
{
    public class Match3Agent : Agent
    {
        private Grid grid;
        private GameManager gameManager;
        private int boardSizeX;
        private int boardSizeY;
        private int lastScore;

        private void Awake()
        {
            grid = FindObjectOfType<Grid>();
            gameManager = FindObjectOfType<GameManager>();
            if (grid == null)
            {
                Debug.LogError("Grid not found");
            }
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found");
            }
            boardSizeX = grid?.xDim ?? 0;
            boardSizeY = grid?.yDim ?? 0;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Encode the 8x8 grid into 448 values
            for (int x = 0; x < boardSizeX; x++)
            {
                for (int y = 0; y < boardSizeY; y++)
                {
                    sensor.AddOneHotObservation((int)grid._pieces[x, y].ItemComponent.Item,
                    System.Enum.GetValues(typeof(ItemPieces.ItemType)).Length);
                }
            }

            sensor.AddObservation(gameManager.CurrentScore);
            sensor.AddObservation(gameManager.RemainingMoves);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            if (actions.DiscreteActions.Length < 4)
            {
                Debug.LogError("Insufficient actions received");
                EndEpisode();
                return;
            }

            int x1 = Mathf.Clamp(actions.DiscreteActions[0], 0, boardSizeX - 1);
            int y1 = Mathf.Clamp(actions.DiscreteActions[1], 0, boardSizeY - 1);
            int x2 = Mathf.Clamp(actions.DiscreteActions[2], 0, boardSizeX - 1);
            int y2 = Mathf.Clamp(actions.DiscreteActions[3], 0, boardSizeY - 1);
            Debug.Log($"Agent action: ({x1},{y1}) => ({x2},{y2})");

            if (x1 < 0 || x1 >= boardSizeX || y1 < 0 || y1 >= boardSizeY || x2 < 0 || x2 >= boardSizeX || y2 < 0 || y2 >= boardSizeY)
            {
                AddReward(-1f);
                EndEpisode();
                return;
            }

            GamePieces piece1 = grid._pieces[x1, y1];
            GamePieces piece2 = grid._pieces[x2, y2];
            if (piece1 == null || piece2 == null)
            {
                Debug.LogError("One of the pieces is null");
                AddReward(-1f);
                EndEpisode();
                return;
            }

            if (Grid.IsAdjacent(piece1, piece2))
            {
                Debug.Log("Swapping pieces");
                grid.SwapPiece(piece1, piece2);
                AddReward(0.1f); // Reward for valid swap

                // Add reward based on match result
                StartCoroutine(EvaluateSwapResult());
            }
            else
            {
                Debug.LogError("Pieces are not adjacent");
                AddReward(-0.2f); // Penalty for invalid swap
                EndEpisode();
            }
        }

        private IEnumerator EvaluateSwapResult()
        {
            yield return new WaitUntil(() => !grid.isFilling);
            float scoreReward = (gameManager.CurrentScore - lastScore) * 0.01f;
            AddReward(scoreReward);
            lastScore = gameManager.CurrentScore;

            Debug.Log("Ending episode");
            EndEpisode();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActions = actionsOut.DiscreteActions;
            if (discreteActions.Length < 4)
            {
                Debug.LogError("Insufficient actions for heuristic");
                return;
            }

            discreteActions[0] = Random.Range(0, boardSizeX);
            discreteActions[1] = Random.Range(0, boardSizeY);
            discreteActions[2] = Random.Range(0, boardSizeX);
            discreteActions[3] = Random.Range(0, boardSizeY);
        }
    }
}