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
        [SerializeField] private Grid grid;

        public override void OnEpisodeBegin()
        {
            grid.Start();
        }
        public override void CollectObservations(VectorSensor sensor)
        {
            for (int i = 0; i < grid.xDim; i++)
            {
                for (int j = 0; j < grid.yDim; j++)
                {
                    GamePieces piece = grid.GetPieces(i, j);
                    sensor.AddObservation((int)piece.Type);
                    sensor.AddObservation(piece.X);
                    sensor.AddObservation(piece.Y);
                    sensor.AddObservation((int)piece.ItemComponent.Item);
                }
            }
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            int[] act = new int[2];
            act[0] = actions.DiscreteActions[0];
            act[1] = actions.DiscreteActions[1];
            grid.AgentSwapPiece(act[0], act[1]);
        }
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            discreteActions[0] = 0;
            discreteActions[1] = 0;
            if (Input.GetKey(KeyCode.W))
            {
                discreteActions[1] = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                discreteActions[1] = 2;
            }
            if (Input.GetKey(KeyCode.A))
            {
                discreteActions[0] = 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                discreteActions[0] = 2;
            }
        }
    }
}
