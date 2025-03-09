using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePiece : MonoBehaviour
{
    // quản lý việc di chuyển các mảng trên lưới
    private GamePieces piece;
    private IEnumerator moveCoroutine;
    private void Awake()
    {
        piece = GetComponent<GamePieces>();
    }

    public void Move(int newX, int newY, float time)
    { // di chuyển mảng ghép đến vị trí mới trong mảng trong một khoảng tg nhất định
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    { // thực hiện di chuyển mảng ghép
        piece.X = newX;
        piece.Y = newY;
        Vector3 startPos = transform.position;
        Vector3 endPos = piece.GridRef.GetWorldPosition(newX * 2, newY * 2, 0);
        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            piece.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return null;
        }
        piece.transform.position = endPos;
    }
}
