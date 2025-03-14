using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class MatchThreeAgent : Agent
{
    [Header("Game References")]
    public Grid gameGrid;
    public GameManager gameManager;
    public EnemyCharacter enemyCharacter;
    public HeroCharater playerCharacter;
    public TimeBar timeBar;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI episodeText;
    public TextMeshProUGUI averageRewardText;

    [Header("ML-Agents Configuration")]
    [SerializeField] private int observationSize;
    [SerializeField] private int[] branchSizes;

    private bool isWaitingForMove = false;
    private float previousEnemyHealth;
    private float previousPlayerHealth;
    private float autoPlayTimer = 0f;
    private const float AUTO_PLAY_INTERVAL = 1f;

    // Training statistics
    private float totalReward = 0f;
    private int episodeCount = 0;
    private float averageReward = 0f;

    // Thêm các biến theo dõi metrics
    private float episodeReward = 0f;
    private int successfulMoves = 0;
    private int totalMoves = 0;
    private float averageMatchesPerMove = 0f;
    private int totalMatches = 0;
    private Queue<float> recentRewards = new Queue<float>(100); // Lưu 100 episode gần nhất
    private float bestReward = float.MinValue;

    // Training metrics
    private int invalidMovesCount = 0;
    private int validMovesCount = 0;
    private float averageDamagePerMove = 0f;
    private float bestEpisodeReward = float.MinValue;
    private Queue<float> episodeRewards = new Queue<float>(100); // Store last 100 episodes

    // Logging control flags
    private bool enableDetailedLogs = false;  // Set to true to see all logs
    private bool enableMoveLog = false;       // Log for each move
    private bool enableRewardLog = true;      // Log for rewards
    private bool enableEpisodeLog = true;     // Log for episode summary

    // Log buffer
    private Queue<string> logBuffer = new Queue<string>();
    private const int LOG_BUFFER_SIZE = 10;
    private const int LOG_INTERVAL = 5;  // Log every 5 episodes
    private int currentEpisode = 0;

    // Cache WaitForSeconds để tránh tạo mới mỗi lần
    private static readonly WaitForSeconds WaitTime = new WaitForSeconds(0.05f);
    private static readonly WaitForSeconds AutoPlayWait = new WaitForSeconds(AUTO_PLAY_INTERVAL);

    // Add new variables for enemy AI control
    private bool isEnemyTurn = false;
    private bool hasEnemySwapped = false;
    private bool isEnemyThinking = false;

    public override void Initialize()
    {
        // Chỉ tìm references nếu chưa được set
        if (gameGrid == null) gameGrid = GetComponentInParent<Grid>() ?? FindObjectOfType<Grid>();
        if (gameManager == null) gameManager = GetComponentInParent<GameManager>() ?? FindObjectOfType<GameManager>();
        if (enemyCharacter == null) enemyCharacter = GetComponentInParent<EnemyCharacter>() ?? FindObjectOfType<EnemyCharacter>();
        if (playerCharacter == null) playerCharacter = GetComponentInParent<HeroCharater>() ?? FindObjectOfType<HeroCharater>();

        // Validate references
        if (gameGrid == null || gameManager == null || enemyCharacter == null || playerCharacter == null)
        {
            Debug.LogError("Missing required references in MatchThreeAgent");
            return;
        }

        // Initialize values
        previousEnemyHealth = enemyCharacter.health;
        previousPlayerHealth = playerCharacter.health;
        isWaitingForMove = false;
        autoPlayTimer = 0f;

        // Calculate specs
        CalculateSpecs();

        // Debug.Log($"Grid dimensions: {gameGrid.xDim}x{gameGrid.yDim}");
        // Debug.Log($"Observation Size: {observationSize}");
        // Debug.Log($"Branch Sizes: [{string.Join(", ", branchSizes)}]");
        // Debug.Log("MatchThreeAgent Initialized Successfully");
    }

    private void CalculateSpecs()
    {
        // Tính tổng số observation:
        // 1. Piece types và items: gameGrid.xDim * gameGrid.yDim * 2
        // 2. Match potentials: gameGrid.xDim * gameGrid.yDim * 2
        // 3. Character states: 2 (enemy health, player health)
        // 4. Combo multiplier: 1
        observationSize = (gameGrid.xDim * gameGrid.yDim * 4) + 3;

        branchSizes = new int[4];
        branchSizes[0] = gameGrid.xDim;
        branchSizes[1] = gameGrid.yDim;
        branchSizes[2] = gameGrid.xDim;
        branchSizes[3] = gameGrid.yDim;

        // Debug.Log($"Calculated observation size: {observationSize}");
        // Debug.Log($"Grid dimensions: {gameGrid.xDim}x{gameGrid.yDim}");
        // Debug.Log($"Expected observations: {(gameGrid.xDim * gameGrid.yDim * 4) + 3}");
    }

    void Update()
    {
        // Check if it's enemy's turn
        if (timeBar != null && timeBar.role == TimeBar.Role.Demon && !hasEnemySwapped && !isEnemyThinking)
        {
            isEnemyTurn = true;
            StartCoroutine(EnemyTurn());
        }
        else if (timeBar != null && timeBar.role == TimeBar.Role.Player)
        {
            isEnemyTurn = false;
            hasEnemySwapped = false;
        }

        // Original player turn logic
        if (!isWaitingForMove && !isEnemyTurn)
        {
            autoPlayTimer += Time.deltaTime;
            if (autoPlayTimer >= AUTO_PLAY_INTERVAL)
            {
                autoPlayTimer = 0f;
                RequestDecision();
            }
        }
    }

    private IEnumerator EnemyTurn()
    {
        isEnemyThinking = true;
        yield return new WaitForSeconds(0.5f); // Small delay to make it feel more natural

        // Request decision from the trained model
        RequestDecision();

        // Wait for the decision to be processed
        while (isWaitingForMove)
        {
            yield return null;
        }

        hasEnemySwapped = true;
        isEnemyTurn = false;
        isEnemyThinking = false;
    }

    public override void OnEpisodeBegin()
    {
        currentEpisode++;

        // Log detailed metrics every LOG_INTERVAL episodes
        if (enableEpisodeLog && currentEpisode > 1 && currentEpisode % LOG_INTERVAL == 0)
        {
            float moveSuccessRate = totalMoves > 0 ? (float)successfulMoves / totalMoves * 100 : 0;
            float validMoveRate = (validMovesCount + invalidMovesCount) > 0 ?
                (float)validMovesCount / (validMovesCount + invalidMovesCount) * 100 : 0;
            float recentAverageReward = episodeRewards.Count > 0 ? episodeRewards.Average() : 0;
            float recentAverageDamage = episodeRewards.Count > 0 ? averageDamagePerMove : 0;

            BufferedLog($"[Training] Episode {currentEpisode} Summary:\n" +
                       $"Total Reward: {episodeReward:F2}\n" +
                       $"Move Success Rate: {moveSuccessRate:F2}%\n" +
                       $"Valid Move Rate: {validMoveRate:F2}%\n" +
                       $"Matches Per Move: {averageMatchesPerMove:F2}\n" +
                       $"Average Damage: {recentAverageDamage:F2}\n" +
                       $"Recent Average Reward: {recentAverageReward:F2}\n" +
                       $"Best Episode Reward: {bestEpisodeReward:F2}");

            // Update best reward
            if (episodeReward > bestEpisodeReward)
            {
                bestEpisodeReward = episodeReward;
                BufferedLog($"[Training] New Best Episode Reward: {bestEpisodeReward:F2}!");
            }

            // Add to recent rewards
            episodeRewards.Enqueue(episodeReward);
            if (episodeRewards.Count > 100) episodeRewards.Dequeue();
        }

        // Reset metrics for new episode
        episodeReward = 0;
        successfulMoves = 0;
        totalMoves = 0;
        totalMatches = 0;
        averageDamagePerMove = 0f;

        if (enemyCharacter != null && playerCharacter != null)
        {
            enemyCharacter.health = enemyCharacter.maxHealth;
            playerCharacter.health = playerCharacter.maxHealth;
            previousEnemyHealth = enemyCharacter.health;
            previousPlayerHealth = playerCharacter.health;
        }

        isWaitingForMove = false;
        autoPlayTimer = 0f;

        // Reset enemy turn state
        hasEnemySwapped = false;
        isEnemyTurn = false;
        isEnemyThinking = false;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            // scoreText.text = $"Total Reward: {totalReward:F2}";
        }
        if (episodeText != null)
        {
            // episodeText.text = $"Episode: {episodeCount}";
        }
        if (averageRewardText != null)
        {
            // averageRewardText.text = $"Average Reward: {averageReward:F2}";
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        int observationCount = 0;

        // 1. Observe piece types and their positions
        for (int x = 0; x < gameGrid.xDim; x++)
        {
            for (int y = 0; y < gameGrid.yDim; y++)
            {
                GamePieces piece = gameGrid._pieces[x, y];
                if (piece != null)
                {
                    // Normalize piece type
                    float normalizedPieceType = (float)piece.Type / (float)System.Enum.GetValues(typeof(Grid.PieceType)).Length;
                    sensor.AddObservation(normalizedPieceType);

                    // Add item information if piece has an item
                    if (piece.IsItemed())
                    {
                        float normalizedItemType = (float)piece.ItemComponent.Item /
                            (float)System.Enum.GetValues(typeof(ItemPieces.ItemType)).Length;
                        sensor.AddObservation(normalizedItemType);
                    }
                    else
                    {
                        sensor.AddObservation(0f);
                    }
                }
                else
                {
                    // If piece is null, add zeros for both piece type and item
                    sensor.AddObservation(0f);
                    sensor.AddObservation(0f);
                }
                observationCount += 2;
            }
        }

        // 2. Add potential match information
        for (int x = 0; x < gameGrid.xDim; x++)
        {
            for (int y = 0; y < gameGrid.yDim; y++)
            {
                // Check horizontal potential matches
                float horizontalMatchPotential = 0f;
                if (x < gameGrid.xDim - 1)
                {
                    horizontalMatchPotential = CalculateMatchPotential(x, y, x + 1, y);
                }
                sensor.AddObservation(horizontalMatchPotential);

                // Check vertical potential matches
                float verticalMatchPotential = 0f;
                if (y < gameGrid.yDim - 1)
                {
                    verticalMatchPotential = CalculateMatchPotential(x, y, x, y + 1);
                }
                sensor.AddObservation(verticalMatchPotential);

                observationCount += 2;
            }
        }

        // 3. Add character states
        sensor.AddObservation(enemyCharacter.health / enemyCharacter.maxHealth);
        sensor.AddObservation(playerCharacter.health / playerCharacter.maxHealth);
        observationCount += 2;

        // Add combo multiplier
        float comboMultiplier = gameManager.GetComboMultiplier();
        sensor.AddObservation(comboMultiplier);
        observationCount++;

        if (observationCount != observationSize)
        {
            // Debug.LogWarning($"Observation count mismatch! Expected: {observationSize}, Actual: {observationCount}");
            CalculateSpecs(); // Recalculate observation size if needed
        }
    }

    private float CalculateMatchPotential(int x1, int y1, int x2, int y2)
    {
        if (!IsValidMove(x1, y1, x2, y2))
            return 0f;

        GamePieces piece1 = gameGrid._pieces[x1, y1];
        GamePieces piece2 = gameGrid._pieces[x2, y2];

        if (piece1 == null || piece2 == null)
            return 0f;

        // Temporarily swap pieces
        gameGrid._pieces[x1, y1] = piece2;
        gameGrid._pieces[x2, y2] = piece1;

        float potential = 0f;

        // Check for matches after swap
        List<GamePieces> matches = gameGrid.FindMatches();
        if (matches != null && matches.Count > 0)
        {
            // Higher potential for more matches
            potential = Mathf.Min(matches.Count / 3f, 1f);

            // Bonus for special pieces or items
            foreach (GamePieces match in matches)
            {
                if (match.IsItemed())
                    potential += 0.2f;
            }
        }

        // Swap pieces back
        gameGrid._pieces[x1, y1] = piece1;
        gameGrid._pieces[x2, y2] = piece2;

        return potential;
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
            validMovesCount++;
            GamePieces sourcePiece = gameGrid._pieces[sourceX, sourceY];
            GamePieces targetPiece = gameGrid._pieces[targetX, targetY];

            if (sourcePiece != null && targetPiece != null)
            {
                isWaitingForMove = true;
                StartCoroutine(PerformMove(sourcePiece, targetPiece));
            }
            else
            {
                invalidMovesCount++;
                AddReward(-0.1f);
                totalReward -= 0.1f;
                if (enableMoveLog)
                {
                    BufferedLog($"[Training] Invalid Move (Null Pieces) | Reward: -0.1 | Total: {totalReward:F2}");
                }
            }
        }
        else
        {
            invalidMovesCount++;
            AddReward(-0.1f);
            totalReward -= 0.1f;
            if (enableMoveLog)
            {
                BufferedLog($"[Training] Invalid Move | Reward: -0.1 | Total: {totalReward:F2}");
            }
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
        // Debug.Log($"Pieces adjacent: {isAdjacent}");
        return isAdjacent;
    }

    private IEnumerator PerformMove(GamePieces sourcePiece, GamePieces targetPiece)
    {
        totalMoves++;
        int initialScore = gameManager.GetCurrentScore();

        // Cache matches before swap
        List<GamePieces> initialMatches = gameGrid.FindMatches();
        bool hadInitialMatches = initialMatches != null && initialMatches.Count > 0;

        // Perform swap
        gameGrid.SwapPiece(sourcePiece, targetPiece);

        float timeout = 0f;
        const float maxTimeout = 2f; // Giảm thời gian timeout
        const float checkInterval = 0.05f; // Giảm interval

        while (gameGrid.isFilling && timeout < maxTimeout)
        {
            timeout += checkInterval;
            yield return WaitTime; // Sử dụng WaitTime đã cache
        }

        if (timeout >= maxTimeout)
        {
            gameGrid.isFilling = false;
            Debug.LogWarning("Fill operation timed out");
        }

        // Process matches
        ProcessMatches(initialScore, hadInitialMatches);

        // Process combat
        ProcessCombat();

        isWaitingForMove = false;
        UpdateUI();
    }

    private void ProcessMatches(int initialScore, bool hadInitialMatches)
    {
        int finalScore = gameManager.GetCurrentScore();
        int scoreDelta = finalScore - initialScore;
        List<GamePieces> finalMatches = gameGrid.FindMatches();

        if (finalMatches != null && finalMatches.Count > 0)
        {
            successfulMoves++;
            totalMatches += finalMatches.Count;

            // Calculate match reward
            float matchReward = CalculateMatchReward(finalMatches, scoreDelta);

            AddReward(matchReward);
            episodeReward += matchReward;
            totalReward += matchReward;

            if (enableMoveLog)
            {
                BufferedLog($"[Training] Move {totalMoves} | Matches: {finalMatches.Count} | Score: {scoreDelta} | Reward: {matchReward:F2}");
            }
        }
        else if (!hadInitialMatches)
        {
            ApplyNoMatchPenalty();
        }
    }

    private float CalculateMatchReward(List<GamePieces> matches, int scoreDelta)
    {
        float reward = matches.Count * 0.3f;

        if (scoreDelta > 0)
        {
            reward += Mathf.Min(scoreDelta / 1000f, 2.0f);
        }

        // Bonus for special pieces
        reward += matches.Count(match => match.IsItemed()) * 0.5f;

        return reward;
    }

    private void ApplyNoMatchPenalty()
    {
        const float noMatchPenalty = -0.2f;
        AddReward(noMatchPenalty);
        episodeReward += noMatchPenalty;
        totalReward += noMatchPenalty;

        if (enableMoveLog)
        {
            Debug.Log($"[Training] Move {totalMoves} | No Matches | Penalty: {noMatchPenalty:F2} | Episode Reward: {episodeReward:F2}");
        }
    }

    private void ProcessCombat()
    {
        float enemyHealthDelta = previousEnemyHealth - enemyCharacter.health;
        float playerHealthDelta = previousPlayerHealth - playerCharacter.health;

        previousEnemyHealth = enemyCharacter.health;
        previousPlayerHealth = playerCharacter.health;

        // Update average damage
        if (enemyHealthDelta > 0)
        {
            averageDamagePerMove = (averageDamagePerMove * totalMoves + enemyHealthDelta) / (totalMoves + 1);
        }

        // Process combat rewards
        if (enemyHealthDelta > 0)
        {
            ApplyDamageReward(1.5f * enemyHealthDelta / enemyCharacter.maxHealth);
        }

        if (playerHealthDelta > 0)
        {
            ApplyDamageReward(-0.5f * playerHealthDelta / playerCharacter.maxHealth);
        }

        if (enemyHealthDelta < 0)
        {
            ApplyDamageReward(0.8f * -enemyHealthDelta / enemyCharacter.maxHealth);
        }

        CheckGameEnd();
    }

    private void CheckGameEnd()
    {
        if (enemyCharacter.health <= 0 || playerCharacter.health <= 0)
        {
            if (playerCharacter.health <= 0)
            {
                const float winBonus = 5.0f;
                AddReward(winBonus);
                episodeReward += winBonus;
                totalReward += winBonus;

                if (enableRewardLog)
                {
                    Debug.Log($"[Training] Game Won! | Reward: {winBonus:F2} | Final Episode Reward: {episodeReward:F2}");
                }
            }
            else
            {
                const float losePenalty = -3.0f;
                AddReward(losePenalty);
                episodeReward += losePenalty;
                totalReward += losePenalty;

                if (enableRewardLog)
                {
                    Debug.Log($"[Training] Game Lost | Penalty: {losePenalty:F2} | Final Episode Reward: {episodeReward:F2}");
                }
            }

            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // var discreteActionsOut = actionsOut.DiscreteActions;

        // // For testing, generate random valid moves
        // discreteActionsOut[0] = Random.Range(0, gameGrid.xDim);
        // discreteActionsOut[1] = Random.Range(0, gameGrid.yDim);

        // // Choose a random adjacent cell
        // int direction = Random.Range(0, 4); // 0: up, 1: right, 2: down, 3: left
        // switch (direction)
        // {
        //     case 0: // up
        //         discreteActionsOut[2] = discreteActionsOut[0];
        //         discreteActionsOut[3] = Mathf.Clamp(discreteActionsOut[1] - 1, 0, gameGrid.yDim - 1);
        //         break;
        //     case 1: // right
        //         discreteActionsOut[2] = Mathf.Clamp(discreteActionsOut[0] + 1, 0, gameGrid.xDim - 1);
        //         discreteActionsOut[3] = discreteActionsOut[1];
        //         break;
        //     case 2: // down
        //         discreteActionsOut[2] = discreteActionsOut[0];
        //         discreteActionsOut[3] = Mathf.Clamp(discreteActionsOut[1] + 1, 0, gameGrid.yDim - 1);
        //         break;
        //     case 3: // left
        //         discreteActionsOut[2] = Mathf.Clamp(discreteActionsOut[0] - 1, 0, gameGrid.xDim - 1);
        //         discreteActionsOut[3] = discreteActionsOut[1];
        //         break;
        // }
    }

    // Add this method to flush the log buffer
    private void FlushLogBuffer()
    {
        if (logBuffer.Count > 0)
        {
            string combinedLog = string.Join("\n", logBuffer);
            Debug.Log(combinedLog);
            logBuffer.Clear();
        }
    }

    // Add this method for buffered logging
    private void BufferedLog(string message)
    {
        logBuffer.Enqueue(message);
        if (logBuffer.Count >= LOG_BUFFER_SIZE)
        {
            FlushLogBuffer();
        }
    }

    // Update reward logging
    private void LogReward(string eventType, float rewardValue, float totalReward, string additionalInfo = "")
    {
        if (!enableRewardLog) return;

        string message = $"[Training] {eventType} | Reward: {rewardValue:F2} | Total: {totalReward:F2}";
        if (!string.IsNullOrEmpty(additionalInfo))
        {
            message += $" | {additionalInfo}";
        }
        BufferedLog(message);
    }

    // Example usage in your reward methods
    private void ApplyMatchReward(int matchCount)
    {
        float reward = matchCount * 0.3f;
        AddReward(reward);
        episodeReward += reward;

        if (enableRewardLog)
        {
            LogReward("Match", reward, episodeReward, $"Matches: {matchCount}");
        }
    }

    private void ApplyDamageReward(float damage)
    {
        float reward = damage * 1.5f;
        AddReward(reward);
        episodeReward += reward;

        if (enableRewardLog)
        {
            LogReward("Damage", reward, episodeReward, $"Damage: {damage:F2}");
        }
    }

    // Override OnDestroy to ensure remaining logs are flushed
    private void OnDestroy()
    {
        FlushLogBuffer();
    }

    // Method to toggle logging options
    public void SetLoggingOptions(bool detailed, bool moves, bool rewards, bool episodes)
    {
        enableDetailedLogs = detailed;
        enableMoveLog = moves;
        enableRewardLog = rewards;
        enableEpisodeLog = episodes;
    }
}