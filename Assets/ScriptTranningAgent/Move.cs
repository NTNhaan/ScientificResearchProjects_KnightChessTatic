using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KnightChessTatic
{
    // public struct Move : MonoBehaviour
    // {
    //     public int Row1;
    //     public int Col1;
    //     public int Row2;
    //     public int Col2;

    //     public Move(int row1, int col1, int row2, int col2)
    //     {
    //         Row1 = row1;
    //         Col1 = col1;
    //         Row2 = row2;
    //         Col2 = col2;
    //     }
    //     public static int NumPotentialMoves(BoardSize maxBoardSize)
    //     {
    //         return (maxBoardSize.Rows * (maxBoardSize.Cols - 1)) + ((maxBoardSize.Rows - 1) * maxBoardSize.Cols);
    //     }
    //     public static Move FromMoveIndex(int moveIndex, BoardSize maxBoardSize)
    //     {
    //         int rows = maxBoardSize.Rows;
    //         int cols = maxBoardSize.Cols;

    //         if (moveIndex < rows * (cols - 1))
    //         {
    //             int row = moveIndex / (cols - 1);
    //             int col = moveIndex % (cols - 1);
    //             return new Move(row, col, row, col + 1);
    //         }
    //         else
    //         {
    //             moveIndex -= rows * (cols - 1);
    //             int row = moveIndex / cols;
    //             int col = moveIndex % cols;
    //             return new Move(row, col, row + 1, col);
    //         }
    //     }
    //     public static Move FromPositionAndDirection(int row, int col, Direction dir, BoardSize maxBoardSize)
    //     {
    //         switch (dir)
    //         {
    //             case Direction.Up:
    //                 return new Move(row, col, row - 1, col);
    //             case Direction.Down:
    //                 return new Move(row, col, row + 1, col);
    //             case Direction.Left:
    //                 return new Move(row, col, row, col - 1);
    //             case Direction.Right:
    //                 return new Move(row, col, row, col + 1);
    //             default:
    //                 throw new System.ArgumentException("Invalid direction");
    //         }
    //     }
    // }
}
// public enum Direction
// {
//     Up,
//     Down,
//     Left,
//     Right
// }
