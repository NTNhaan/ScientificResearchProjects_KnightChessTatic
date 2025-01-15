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
        public override void Initialize()
        {
            grid = FindObjectOfType<Grid>();
        }
        public override void OnEpisodeBegin()
        {
            grid.Start();
        }
        public override void CollectObservations(VectorSensor sensor)
        {
            for (int x = 0; x < grid.xDim; x++)
            {
                for (int y = 0; y < grid.yDim; y++)
                {
                    GamePieces piece = grid.GetPieces(x, y);
                    sensor.AddObservation((int)piece.Type);
                    sensor.AddObservation((int)piece.ItemComponent.Item);
                }
            }
        }
        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            // Take actions based on the received actionBuffers
            int index1 = actionBuffers.DiscreteActions[0];
            int index2 = actionBuffers.DiscreteActions[1];
            bool success = grid.AgentSwapPiece(index1, index2);

            if (success)
            {
                AddReward(1.0f);
            }
            else
            {
                AddReward(-0.1f);
            }

            if (grid.IsGameOver())
            {
                EndEpisode();
            }
        }
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            // Define heuristic actions for testing
            var discreteActionsOut = actionsOut.DiscreteActions;
            discreteActionsOut[0] = Random.Range(0, grid.xDim * grid.yDim);
            discreteActionsOut[1] = Random.Range(0, grid.xDim * grid.yDim);
        }
    }
}
