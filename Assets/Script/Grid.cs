using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Grid : MonoBehaviour
{
    public enum PieceType
    {
        NORMAL,
        BUBBLE,
        EMPTY,
        COUNT,
    }
    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }
    [System.Serializable]
    public struct PiecePosition
    {
        public PieceType type;
        public int x;
        public int y;
    };

    public int xDim;
    public int yDim;
    public float FillTime;


    public PiecePrefab[] piecePrefabs;   // the array containing the image object is displayed on chessboard
    public GameObject backgroundPrefab;   // backgound for chessboard
    public PiecePosition[] initialPieces;

    private Dictionary<PieceType, GameObject> _piecePrefabDict;
    private GamePieces[,] _pieces;
    private bool _inverse;

    public Vector2[,] backgroundPositions;

    // biến nãy sinh trong quá trình
    private GamePieces pressedPiece;
    private GamePieces enteredPiece;
    // Update is called once per 
    private TimeBar.Role role;
    [SerializeField] private TimeBar timeswap;

    public PieceReward pieceReward;
    public Animator animator;
    public bool isFilling = false;
    public void SetFilling(bool value)
    {
        isFilling = value;
    }
    public void Awake()
    {
        role = TimeBar.Role.Player;
        timeswap = FindObjectOfType<TimeBar>();
    }
    public void Start()
    {
        _piecePrefabDict = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!_piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                _piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject bg = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x * 2, y * 2, 0), Quaternion.identity);
                bg.transform.parent = transform;
            }
        }
        //instantiating pieces
        _pieces = new GamePieces[xDim, yDim];
        for (int i = 0; i < initialPieces.Length; i++)
        {
            if (initialPieces[i].x >= 0 && initialPieces[i].x < xDim && initialPieces[i].y >= 0 && initialPieces[i].y < yDim)
            {
                SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);
            }
        }
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (_pieces[x, y] == null)
                {
                    SpawnNewPiece(x, y, PieceType.EMPTY);
                }
            }
        }
        StartCoroutine(CheckAndFill());
    }
    private IEnumerator CheckAndFill()
    {
        while (true)
        {
            yield return new WaitUntil(() => isFilling);
            yield return StartCoroutine(Fill());
            isFilling = false;
        }
    }
    public IEnumerator Fill()
    {
        bool needRefill = true;
        isFilling = true;
        while (needRefill && isFilling)
        {
            yield return new WaitForSeconds(FillTime);
            while (FillStep())
            {
                //_inverse = !_inverse;
                yield return new WaitForSeconds(FillTime);
            }
            needRefill = ClearAllValidMatches();
        }
    }
    public bool FillStep()
    {
        bool movedPiece = false;
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;
                GamePieces piece = _pieces[x, y];
                piece.name = "Piece(" + x + "," + y + ")" + "[" + piece.X + "," + piece.Y + "]";
                if (piece.IsMoveable())
                {
                    GamePieces pieceBelow = _pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        //Debug.Log("Spawn piece for the second row and fill chessboard:  " + "[" + tmpx + "," + tmpy + "]");
                        piece.MovableComponent.Move(x, y + 1, FillTime);
                        _pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                }
            }
        }
        for (int x = 0; x < xDim; x++)
        {
            GamePieces pieceBelow = _pieces[x, 0];
            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(_piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1, 0), Quaternion.identity);
                newPiece.transform.parent = transform;
                // cập nhật thông tin của gamepiece cho prefab piece mới
                _pieces[x, 0] = newPiece.GetComponent<GamePieces>();
                _pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                _pieces[x, 0].MovableComponent.Move(x, 0, FillTime);
                int randomIndex = UnityEngine.Random.Range(0, _pieces[x, 0].ItemComponent.NumItems);
                _pieces[x, 0].ItemComponent.SetItem((ItemPieces.ItemType)randomIndex);
                movedPiece = true;
            }
        }
        return movedPiece;
    }


    public GamePieces SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = (GameObject)Instantiate(_piecePrefabDict[type], GetWorldPosition(x, y, 0), Quaternion.identity);
        newPiece.transform.parent = transform;
        _pieces[x, y] = newPiece.GetComponent<GamePieces>();
        _pieces[x, y].Init(x, y, this, type);
        return _pieces[x, y];
    }
    private static bool IsAdjacent(GamePieces piece1, GamePieces piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1) ||
        (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);

    }
    public void SwapPiece(GamePieces piece1, GamePieces piece2)
    {
        if (!piece1.IsMoveable() && !piece2.IsMoveable())
        {
            //Debug.Log("Can't swap piece, so piece is null " + piece1 + " " + piece2);
            return;
        }
        _pieces[piece1.X, piece1.Y] = piece2;
        _pieces[piece2.X, piece2.Y] = piece1;
        var match1 = GetMatch(piece1, piece2.X, piece2.Y);
        var match2 = GetMatch(piece2, piece1.X, piece1.Y);
        int piece1X = piece1.X;
        int piece1Y = piece1.Y;
        piece1.MovableComponent.Move(piece2.X, piece2.Y, FillTime);
        piece2.MovableComponent.Move(piece1X, piece1Y, FillTime);
        if (timeswap.role == TimeBar.Role.Player)
        {
            timeswap.Pause();
            // thực hiện animation của piece tạo thành match trước
            timeswap.PlayAnimation("StartTurn");
        }
        else if (timeswap.role == TimeBar.Role.Demon)
        {
            timeswap.Pause();
            timeswap.PlayAnimation("StartTurnBack");
        }
        ClearAllValidMatches();
        StartCoroutine(Fill());
        // if (match1 != null || match2 != null)
        // {
        //     int piece1X = piece1.X;
        //     int piece1Y = piece1.Y;
        //     piece1.MovableComponent.Move(piece2.X, piece2.Y, FillTime);
        //     piece2.MovableComponent.Move(piece1X, piece1Y, FillTime);
        //     if (timeswap.role == TimeBar.Role.Player)
        //     {
        //         timeswap.Pause();
        //         // thực hiện animation của piece tạo thành match trước
        //         timeswap.PlayAnimation("StartTurn");
        //     }
        //     else if (timeswap.role == TimeBar.Role.Demon)
        //     {
        //         timeswap.Pause();
        //         timeswap.PlayAnimation("StartTurnBack");
        //     }
        //     ClearAllValidMatches();
        //     StartCoroutine(Fill());
        // }
        // else
        // {
        //     StartCoroutine(SwapPiecesBack(piece1, piece2, FillTime));
        //     // _pieces[piece1.X, piece1.Y] = piece1;
        //     // _pieces[piece2.X, piece2.Y] = piece2;
        // }
    }
    IEnumerator SwapPiecesBack(GamePieces piece1, GamePieces piece2, float delay)
    {
        yield return new WaitForSeconds(delay);
        _pieces[piece1.X, piece1.Y] = piece2;
        _pieces[piece2.X, piece2.Y] = piece1;
        int piece1X = piece1.X;
        int piece1Y = piece1.Y;
        int piece2X = piece2.X;
        int piece2Y = piece2.Y;
        piece1.MovableComponent.Move(piece2.X, piece2.Y, FillTime);
        piece2.MovableComponent.Move(piece1X, piece1Y, FillTime);

        // Wait for another delay
        yield return new WaitForSeconds(delay);

        // Swap back
        _pieces[piece1.X, piece1.Y] = piece1;
        _pieces[piece2.X, piece2.Y] = piece2;
        piece1.MovableComponent.Move(piece1X, piece1Y, FillTime);
        piece2.MovableComponent.Move(piece2X, piece2Y, FillTime);
    }
    public List<GamePieces> GetMatch(GamePieces piece, int NewX, int NewY)
    {
        if (piece.IsItemed())
        {
            ItemPieces.ItemType type = piece.ItemComponent.Item;
            var horizontalPieces = new List<GamePieces>();
            var verticalPieces = new List<GamePieces>();
            var matchingPieces = new List<GamePieces>();
            horizontalPieces.Add(piece);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;
                    if (dir == 0) x = NewX - xOffset; // left
                    else x = NewX + xOffset; // right
                    if (x < 0 || x >= xDim) break;

                    // piece is the same item type  => add to the list
                    if (_pieces[x, NewY].IsItemed() && _pieces[x, NewY].ItemComponent.Item == type)
                    {
                        horizontalPieces.Add(_pieces[x, NewY]);
                    }
                    else break;
                }
            }
            if (horizontalPieces.Count >= 3)
            {
                matchingPieces.AddRange(horizontalPieces);
                // for (int i = 0; i < horizontalPieces.Count; i++)
                // {
                //     matchingPieces.Add(horizontalPieces[i]);
                //     //Debug.Log("PieceTypeHorizontalList: " + horizontalPieces[i].ItemComponent.Item);
                // }
            }

            // Traverse vertically if we found a match (3 or more pieces)
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;
                            if (dir == 0) y = NewY - yOffset;
                            else y = NewY + yOffset;
                            if (y < 0 || y >= yDim) break;
                            if (_pieces[horizontalPieces[i].X, y].IsItemed() && _pieces[horizontalPieces[i].X, y].ItemComponent.Item == type)
                            {
                                verticalPieces.Add(_pieces[horizontalPieces[i].X, y]);
                            }
                            else break;
                        }
                    }
                    if (verticalPieces.Count < 2)
                        verticalPieces.Clear();
                    else
                    {
                        // for (int j = 0; j < verticalPieces.Count; j++)
                        // {
                        //     matchingPieces.Add(verticalPieces[j]);
                        // }
                        matchingPieces.AddRange(verticalPieces);
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
            horizontalPieces.Clear();
            verticalPieces.Clear();

            //check verticalPiece---m----------------
            verticalPieces.Add(piece);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < xDim; yOffset++)  //(*****************)
                {
                    int y;
                    if (dir == 0) y = NewY - yOffset; // left
                    else y = NewY + yOffset; // right
                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }
                    if (_pieces[NewX, y].IsItemed() && _pieces[NewX, y].ItemComponent.Item == type)
                    {
                        verticalPieces.Add(_pieces[NewX, y]);
                    }
                    else break;
                }
            }
            if (verticalPieces.Count >= 3)
            {
                matchingPieces.AddRange(verticalPieces);
                // for (int i = 0; i < verticalPieces.Count; i++)
                // {
                //     matchingPieces.Add(verticalPieces[i]);
                //     //Debug.Log("PieceTypeVerticalList: " + verticalPieces[i].ItemComponent.Item);
                // }
            }
            // Traverse horizontal if we found a match (3 or more pieces)
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < yDim; xOffset++)
                        {
                            int x;
                            if (dir == 0) x = NewX - xOffset; // left
                            else x = NewX + xOffset; //right
                            if (x < 0 || x >= xDim) break;
                            if (_pieces[x, verticalPieces[i].Y].IsItemed() && _pieces[x, verticalPieces[i].Y].ItemComponent.Item == type)
                            {
                                horizontalPieces.Add(_pieces[x, verticalPieces[i].Y]);
                            }
                            else break;
                        }
                    }
                    if (horizontalPieces.Count < 2)
                        horizontalPieces.Clear();
                    else
                    {
                        // for (int j = 0; j < horizontalPieces.Count; j++)
                        // {
                        //     matchingPieces.Add(horizontalPieces[j]);
                        // }
                        matchingPieces.AddRange(horizontalPieces);
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }
        return null;
    }
    private bool ClearAllValidMatches()
    {
        bool needRefill = false;
        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (_pieces[x, y].IsClearable())
                {
                    List<GamePieces> match = GetMatch(_pieces[x, y], x, y);
                    if (match == null) continue;
                    foreach (var gamePiece in match)
                    {
                        if (gamePiece.ItemComponent.Item == ItemPieces.ItemType.Sword)
                        {
                            animator.SetTrigger("Attack3");
                        }
                        BoxCollider2D boxCollider = gamePiece.GetComponent<BoxCollider2D>();
                        if (boxCollider != null)
                        {
                            boxCollider.enabled = false;
                        }
                        pieceReward.StartCoinMove(gamePiece.transform.position, gamePiece.gameObject);
                        if (!ClearPiece(gamePiece.X, gamePiece.Y)) continue;
                        needRefill = true;
                    }
                }
            }
        }
        return needRefill;
    }
    public bool ClearPiece(int x, int y)
    {
        if (_pieces[x, y].IsClearable() && !_pieces[x, y].ClearableComponent.IsBeingCleared)
        {
            _pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);

            return true;
        }
        return false;
    }

    Vector2 GetPrefabSize(GameObject prefab)
    {
        Renderer prefabRederer = prefab.GetComponent<Renderer>();
        if (prefabRederer != null)
        {
            return new Vector2(prefabRederer.bounds.size.x, prefabRederer.bounds.size.y);
        }
        else
        {
            //Debug.LogError("check prefab Renderer");
            return Vector2.zero;
        }
    }
    public Vector3 GetWorldPosition(float x, float y, float z)
    {
        return new Vector3(
            transform.position.x - xDim / 2.0f + x - 3,
            transform.position.y + yDim / 2.0f - y + 2,
            transform.position.z);
    }
    public void PressPiece(GamePieces piece)
    {
        pressedPiece = piece;
        Debug.Log("location for PressPiece: " + piece.X + " " + piece.Y);
    }
    public void EnterPiece(GamePieces piece)
    {
        enteredPiece = piece;
        Debug.Log("location for EnterPiece: " + piece.X + " " + piece.Y);
    }
    public void ReleasePiece()
    {
        bool Swaped = SwapTurn.Instance.IsSwapping;
        if (!Swaped)
        {
            if (pressedPiece == enteredPiece)
            {
                Debug.Log("Overlapping piece =((");
                return;
            }
            else
            {
                if (IsAdjacent(pressedPiece, enteredPiece))
                {
                    Debug.Log("IsAdjacent is true =))");
                    SwapPiece(pressedPiece, enteredPiece);
                }
            }
        }
    }
}

