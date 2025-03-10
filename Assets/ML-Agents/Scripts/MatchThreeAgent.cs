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

    [Header("ML-Agents Configuration")]
    [SerializeField] private int observationSize;
    [SerializeField] private int[] branchSizes;

    private bool isWaitingForMove = false;
    private float previousEnemyHealth;
    private float previousPlayerHealth;
    private float autoPlayTimer = 0f;
    private const float AUTO_PLAY_INTERVAL = 1f;


    public override void Initialize()
    {
        // Debug.Log("MatchThreeAgent Initializing...");

        // Get references
        if (gameGrid == null)
        {
            // Try to find Grid in scene
            gameGrid = FindObjectOfType<Grid>();
            if (gameGrid == null)
            {
                // Debug.LogError("Failed to find Grid in scene! Please assign it in the inspector.");
                return;
            }
            // Debug.Log("Found Grid in scene");
        }
        // Debug.Log($"Grid reference obtained: xDim={gameGrid.xDim}, yDim={gameGrid.yDim}");

        if (gameManager == null)
        {
            // Try to find GameManager in scene
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                // Debug.LogError("Failed to find GameManager in scene! Please assign it in the inspector.");
                return;
            }
            // Debug.Log("Found GameManager in scene");
        }

        if (enemyCharacter == null || playerCharacter == null)
        {
            // Try to find characters in scene
            if (enemyCharacter == null)
            {
                enemyCharacter = FindObjectOfType<EnemyCharacter>();
                if (enemyCharacter == null)
                {
                    // Debug.LogError("Failed to find EnemyCharacter in scene! Please assign it in the inspector.");
                    return;
                }
                // Debug.Log("Found EnemyCharacter in scene");
            }

            if (playerCharacter == null)
            {
                playerCharacter = FindObjectOfType<HeroCharater>();
                if (playerCharacter == null)
                {
                    // Debug.LogError("Failed to find HeroCharacter in scene! Please assign it in the inspector.");
                    return;
                }
                // Debug.Log("Found HeroCharacter in scene");
            }
        }

        previousEnemyHealth = enemyCharacter.health;
        previousPlayerHealth = playerCharacter.health;

        // Calculate specs
        CalculateSpecs();

        // Debug.Log($"Observation Size: {observationSize}");
        // Debug.Log($"Branch Sizes: [{string.Join(", ", branchSizes)}]");
        // Debug.Log("MatchThreeAgent Initialized");
    }

    private void CalculateSpecs()
    {
        observationSize = (gameGrid.xDim * gameGrid.yDim) + 2;
        branchSizes = new int[4];
        branchSizes[0] = gameGrid.xDim;
        branchSizes[1] = gameGrid.yDim;
        branchSizes[2] = gameGrid.xDim;
        branchSizes[3] = gameGrid.yDim;
    }

    void Update()
    {
        if (!isWaitingForMove)
        {
            autoPlayTimer += Time.deltaTime;
            if (autoPlayTimer >= AUTO_PLAY_INTERVAL)
            {
                autoPlayTimer = 0f;
                // Debug.Log("Requesting new decision...");
                RequestDecision();
                // Debug.Log("Decision requested");
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        // Debug.Log("Episode Begin!");
        if (enemyCharacter != null && playerCharacter != null)
        {
            enemyCharacter.health = enemyCharacter.maxHealth;
            playerCharacter.health = playerCharacter.maxHealth;
            previousEnemyHealth = enemyCharacter.health;
            previousPlayerHealth = playerCharacter.health;
            // Debug.Log($"Health reset - Enemy: {enemyCharacter.health}, Player: {playerCharacter.health}");
        }
        isWaitingForMove = false;
        autoPlayTimer = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        int observationCount = 0;

        for (int x = 0; x < gameGrid.xDim; x++)
        {
            for (int y = 0; y < gameGrid.yDim; y++)
            {
                GamePieces piece = gameGrid._pieces[x, y];
                if (piece != null && piece.IsItemed())
                {
                    float normalizedItemType = (float)piece.ItemComponent.Item /
                        (float)System.Enum.GetValues(typeof(ItemPieces.ItemType)).Length;
                    sensor.AddObservation(normalizedItemType);
                }
                else
                {
                    sensor.AddObservation(0f);
                }
                observationCount++;
            }
        }

        sensor.AddObservation(enemyCharacter.health / enemyCharacter.maxHealth);
        sensor.AddObservation(playerCharacter.health / playerCharacter.maxHealth);
        observationCount += 2;

        if (observationCount != observationSize)
        {
            // Debug.LogWarning($"Observation count mismatch! Expected: {observationSize}, Actual: {observationCount}");
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Debug.Log("OnActionReceived called");

        if (isWaitingForMove)
        {
            // Debug.Log("Still waiting for previous move to complete");
            return;
        }

        // Get discrete actions
        int sourceX = Mathf.Clamp(actions.DiscreteActions[0], 0, gameGrid.xDim - 1);
        int sourceY = Mathf.Clamp(actions.DiscreteActions[1], 0, gameGrid.yDim - 1);
        int targetX = Mathf.Clamp(actions.DiscreteActions[2], 0, gameGrid.xDim - 1);
        int targetY = Mathf.Clamp(actions.DiscreteActions[3], 0, gameGrid.yDim - 1);

        Debug.Log($"Raw actions: [{actions.DiscreteActions[0]}, {actions.DiscreteActions[1]}, {actions.DiscreteActions[2]}, {actions.DiscreteActions[3]}]");
        Debug.Log($"Clamped actions: [{sourceX}, {sourceY}, {targetX}, {targetY}]");

        // Try to perform the swap
        if (IsValidMove(sourceX, sourceY, targetX, targetY))
        {
            GamePieces sourcePiece = gameGrid._pieces[sourceX, sourceY];
            GamePieces targetPiece = gameGrid._pieces[targetX, targetY];

            if (sourcePiece != null && targetPiece != null)
            {
                Debug.Log($"Valid pieces found at source: ({sourceX},{sourceY}) and target: ({targetX},{targetY})");
                isWaitingForMove = true;
                StartCoroutine(PerformMove(sourcePiece, targetPiece));
            }
            else
            {
                // Debug.Log("Pieces are null");
                AddReward(-0.1f);
            }
        }
        else
        {
            // Debug.Log("Invalid move");
            AddReward(-0.1f);
        }
    }

    private bool IsValidMove(int sourceX, int sourceY, int targetX, int targetY)
    {
        if (sourceX < 0 || sourceX >= gameGrid.xDim ||
            sourceY < 0 || sourceY >= gameGrid.yDim ||
            targetX < 0 || targetX >= gameGrid.xDim ||
            targetY < 0 || targetY >= gameGrid.yDim)
        {
            // Debug.Log("Move out of bounds");
            return false;
        }

        GamePieces sourcePiece = gameGrid._pieces[sourceX, sourceY];
        GamePieces targetPiece = gameGrid._pieces[targetX, targetY];

        if (sourcePiece == null || targetPiece == null)
        {
            // Debug.Log("Null pieces");
            return false;
        }

        bool isAdjacent = Grid.IsAdjacent(sourcePiece, targetPiece);
        Debug.Log($"Pieces adjacent: {isAdjacent}");
        return isAdjacent;
    }

    private IEnumerator PerformMove(GamePieces sourcePiece, GamePieces targetPiece)
    {
        // Debug.Log("Starting PerformMove coroutine");

        // Perform the swap
        gameGrid.SwapPiece(sourcePiece, targetPiece);
        // Debug.Log("SwapPiece called");

        // Wait for animations and filling
        while (gameGrid.isFilling)
        {
            // Debug.Log("Waiting for filling to complete...");
            yield return new WaitForSeconds(0.1f);
        }
        // Debug.Log("Filling completed");

        // Calculate rewards
        float enemyHealthDelta = previousEnemyHealth - enemyCharacter.health;
        float playerHealthDelta = previousPlayerHealth - playerCharacter.health;

        // Debug.Log($"Health changes - Enemy: {enemyHealthDelta}, Player: {playerHealthDelta}");

        // Update previous health values
        previousEnemyHealth = enemyCharacter.health;
        previousPlayerHealth = playerCharacter.health;

        // Reward for damaging player
        if (playerHealthDelta > 0)
        {
            float reward = 0.5f * playerHealthDelta / playerCharacter.maxHealth;
            AddReward(reward);
            Debug.Log($"Added reward for damage: {reward}");
        }

        // Penalty for taking damage
        if (enemyHealthDelta > 0)
        {
            float penalty = -0.3f * enemyHealthDelta / enemyCharacter.maxHealth;
            AddReward(penalty);
            Debug.Log($"Added penalty for taking damage: {penalty}");
        }

        // Reward for healing
        if (enemyHealthDelta < 0)
        {
            float reward = 0.2f * -enemyHealthDelta / enemyCharacter.maxHealth;
            AddReward(reward);
            Debug.Log($"Added reward for healing: {reward}");
        }

        // Small penalty for each step
        AddReward(-0.01f);

        // Check for game over
        if (enemyCharacter.health <= 0 || playerCharacter.health <= 0)
        {
            Debug.Log("Game Over - Ending Episode");
            EndEpisode();
        }

        isWaitingForMove = false;
        Debug.Log("Move completed");
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        // For testing, generate random valid moves
        discreteActionsOut[0] = Random.Range(0, gameGrid.xDim);
        discreteActionsOut[1] = Random.Range(0, gameGrid.yDim);

        // Choose a random adjacent cell
        int direction = Random.Range(0, 4); // 0: up, 1: right, 2: down, 3: left
        switch (direction)
        {
            case 0: // up
                discreteActionsOut[2] = discreteActionsOut[0];
                discreteActionsOut[3] = Mathf.Clamp(discreteActionsOut[1] - 1, 0, gameGrid.yDim - 1);
                break;
            case 1: // right
                discreteActionsOut[2] = Mathf.Clamp(discreteActionsOut[0] + 1, 0, gameGrid.xDim - 1);
                discreteActionsOut[3] = discreteActionsOut[1];
                break;
            case 2: // down
                discreteActionsOut[2] = discreteActionsOut[0];
                discreteActionsOut[3] = Mathf.Clamp(discreteActionsOut[1] + 1, 0, gameGrid.yDim - 1);
                break;
            case 3: // left
                discreteActionsOut[2] = Mathf.Clamp(discreteActionsOut[0] - 1, 0, gameGrid.xDim - 1);
                discreteActionsOut[3] = discreteActionsOut[1];
                break;
        }
    }
}