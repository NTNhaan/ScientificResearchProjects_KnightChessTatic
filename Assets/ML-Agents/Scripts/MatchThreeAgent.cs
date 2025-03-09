using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;

public class MatchThreeAgent : Agent
{
    [Header("Game References")]
    public Grid gameGrid;
    public GameManager gameManager;
    public EnemyCharacter enemyCharacter;
    public HeroCharater playerCharacter;

    private bool isWaitingForMove = false;
    private float previousEnemyHealth;
    private float previousPlayerHealth;

    public override void Initialize()
    {
        gameGrid = GetComponentInParent<Grid>();
        gameManager = GetComponentInParent<GameManager>();
        previousEnemyHealth = enemyCharacter.health;
        previousPlayerHealth = playerCharacter.health;
    }

    public override void OnEpisodeBegin()
    {
        // Reset game state
        enemyCharacter.health = enemyCharacter.maxHealth;
        playerCharacter.health = playerCharacter.maxHealth;
        previousEnemyHealth = enemyCharacter.health;
        previousPlayerHealth = playerCharacter.health;
        isWaitingForMove = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe grid state
        for (int x = 0; x < gameGrid.xDim; x++)
        {
            for (int y = 0; y < gameGrid.yDim; y++)
            {
                GamePieces piece = gameGrid._pieces[x, y];
                if (piece != null && piece.IsItemed())
                {
                    // Normalize the item type to be between 0 and 1
                    float normalizedItemType = (float)piece.ItemComponent.Item /
                        (float)System.Enum.GetValues(typeof(ItemPieces.ItemType)).Length;
                    sensor.AddObservation(normalizedItemType);
                }
                else
                {
                    sensor.AddObservation(0f);
                }
            }
        }

        // Observe character states
        sensor.AddObservation(enemyCharacter.health / enemyCharacter.maxHealth);
        sensor.AddObservation(playerCharacter.health / playerCharacter.maxHealth);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (isWaitingForMove) return;

        // Get discrete actions
        int sourceX = Mathf.Clamp(actions.DiscreteActions[0], 0, gameGrid.xDim - 1);
        int sourceY = Mathf.Clamp(actions.DiscreteActions[1], 0, gameGrid.yDim - 1);
        int targetX = Mathf.Clamp(actions.DiscreteActions[2], 0, gameGrid.xDim - 1);
        int targetY = Mathf.Clamp(actions.DiscreteActions[3], 0, gameGrid.yDim - 1);

        // Try to perform the swap
        if (IsValidMove(sourceX, sourceY, targetX, targetY))
        {
            GamePieces sourcePiece = gameGrid._pieces[sourceX, sourceY];
            GamePieces targetPiece = gameGrid._pieces[targetX, targetY];

            if (sourcePiece != null && targetPiece != null)
            {
                isWaitingForMove = true;
                StartCoroutine(PerformMove(sourcePiece, targetPiece));
            }
        }
        else
        {
            AddReward(-0.1f); // Penalty for invalid move
        }
    }

    private bool IsValidMove(int sourceX, int sourceY, int targetX, int targetY)
    {
        if (sourceX < 0 || sourceX >= gameGrid.xDim ||
            sourceY < 0 || sourceY >= gameGrid.yDim ||
            targetX < 0 || targetX >= gameGrid.xDim ||
            targetY < 0 || targetY >= gameGrid.yDim)
        {
            return false;
        }

        GamePieces sourcePiece = gameGrid._pieces[sourceX, sourceY];
        GamePieces targetPiece = gameGrid._pieces[targetX, targetY];

        if (sourcePiece == null || targetPiece == null)
            return false;

        return Grid.IsAdjacent(sourcePiece, targetPiece);
    }

    private IEnumerator PerformMove(GamePieces sourcePiece, GamePieces targetPiece)
    {
        // Perform the swap
        gameGrid.SwapPiece(sourcePiece, targetPiece);

        // Wait for animations and filling
        while (gameGrid.isFilling)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Calculate rewards
        float enemyHealthDelta = previousEnemyHealth - enemyCharacter.health;
        float playerHealthDelta = previousPlayerHealth - playerCharacter.health;

        // Update previous health values
        previousEnemyHealth = enemyCharacter.health;
        previousPlayerHealth = playerCharacter.health;

        // Reward for damaging player
        if (playerHealthDelta > 0)
        {
            AddReward(0.5f * playerHealthDelta / playerCharacter.maxHealth);
        }

        // Penalty for taking damage
        if (enemyHealthDelta > 0)
        {
            AddReward(-0.3f * enemyHealthDelta / enemyCharacter.maxHealth);
        }

        // Reward for healing
        if (enemyHealthDelta < 0)
        {
            AddReward(0.2f * -enemyHealthDelta / enemyCharacter.maxHealth);
        }

        // Small penalty for each step
        AddReward(-0.01f);

        // Check for game over
        if (enemyCharacter.health <= 0 || playerCharacter.health <= 0)
        {
            EndEpisode();
        }

        isWaitingForMove = false;
        RequestDecision(); // Request next action
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                GamePieces piece = hit.collider.GetComponent<GamePieces>();
                if (piece != null)
                {
                    discreteActionsOut[0] = piece.X;
                    discreteActionsOut[1] = piece.Y;
                }
            }
        }
    }
}