using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChekersGame
{
    public class Move
    {
        private int m_StartRowPos;
        private int m_EndRowPos;
        private int m_StartColPos;
        private int m_EndColPos;
        private bool m_IsJumpOverMove;
        private eJumpDirection m_JumpMoveDirection;

        public Move()
        {
            m_StartRowPos = 0;
            m_EndRowPos = 0;
            m_StartColPos = 0;
            m_EndColPos = 0;
            m_IsJumpOverMove = false;
            m_JumpMoveDirection = eJumpDirection.Empty;
        }

        public Move(int startRow, int startCol, int endRow, int endCol)
        {
            m_StartRowPos = startRow;
            m_StartColPos = startCol;
            m_EndRowPos = endRow;
            m_EndColPos = endCol;
            m_JumpMoveDirection = eJumpDirection.Empty;
        }

        public int StartRowPos
        {
            get { return m_StartRowPos; }
            set { m_StartRowPos = value; }
        }

        public int EndRowPos
        {
            get { return m_EndRowPos; }
            set { m_EndRowPos = value; }
        }

        public int StartColPos
        {
            get { return m_StartColPos; }
            set { m_StartColPos = value; }
        }

        public int EndColPos
        {
            get { return m_EndColPos; }
            set { m_EndColPos = value; }
        }

        public bool IsJumpOverMove
        {
            get { return m_IsJumpOverMove; }
            set { m_IsJumpOverMove = value; }
        }

        public eJumpDirection JumpMoveDirection
        {
            get { return m_JumpMoveDirection; }
            set { m_JumpMoveDirection = value; }
        }

        public static bool HasLegalMoves(eGamePieceType i_PieceType, eGamePieceType i_KingType)
        {
            bool hasLegalMoves = false;

            for (int i = 0; i < GameBoard.BoardSize; i++)
            {
                for (int j = 0; j < GameBoard.BoardSize; j++)
                {
                    GamePiece gamePiece = GameBoard.Board[i, j];
                    if (gamePiece == null || (gamePiece.PieceId != i_PieceType && gamePiece.PieceId != i_KingType))
                    {
                        continue; 
                    }

                    for (int dRow = -2; dRow <= 2; dRow += 4)
                    {
                        for (int dCol = -2; dCol <= 2; dCol += 4)
                        {
                            int endRow = i + dRow;
                            int endCol = j + dCol;

                            if (IsWithinBounds(endRow, endCol))
                            {
                                Move move = new Move(i, j, endRow, endCol);
                                if (move.IsValidMove(i, j, endRow, endCol) && move.IsJumpOverMove)
                                {
                                    hasLegalMoves = true;
                                    break;
                                }
                            }
                        }
                    }

                    for (int dRow = -1; dRow <= 1; dRow += 2)
                    {
                        for (int dCol = -1; dCol <= 1; dCol += 2)
                        {
                            int endRow = i + dRow;
                            int endCol = j + dCol;

                            if (IsWithinBounds(endRow, endCol))
                            {
                                Move move = new Move(i, j, endRow, endCol);
                                if (move.IsValidMove(i, j, endRow, endCol))
                                {
                                    hasLegalMoves = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return hasLegalMoves;
        }

        private static bool IsWithinBounds(int i_Row, int i_Col)
        {
            return i_Row >= 0 && i_Row < GameBoard.BoardSize && i_Col >= 0 && i_Col < GameBoard.BoardSize;
        }

        public Move GetRandomLegalMove()
        {
            int startRow, startCol;
            Random random = new Random();
            Move move = null;

            while (HasLegalMoves(eGamePieceType.Player2, eGamePieceType.KingPlayer2))
            {
                startRow = random.Next(0, GameBoard.BoardSize);
                startCol = random.Next(0, GameBoard.BoardSize);
                if (GameBoard.Board[startRow, startCol].PieceId == eGamePieceType.Player2 ||
                    GameBoard.Board[startRow, startCol].PieceId == eGamePieceType.KingPlayer2)
                {
                    int endRow = random.Next(0, GameBoard.BoardSize);
                    int endCol = random.Next(0, GameBoard.BoardSize);
                    move = new Move(startRow, startCol, endRow, endCol);
                    if (move.IsValidMove(startRow, startCol, endRow, endCol))
                    {
                        break;
                    }
                }
            }

            return move;
        }

        private bool hasMandatoryJump(eGamePieceType playerPiece, eGamePieceType kingPiece)
        {
            int i, j;
            bool hasMandatoryJump = false;

            for (i = 0; i < GameBoard.BoardSize; i++)
            {
                for (j = 0; j < GameBoard.BoardSize; j++)
                {
                    if (GameBoard.Board[i, j].PieceId == playerPiece || GameBoard.Board[i, j].PieceId == kingPiece)
                    {
                        if (CanJumpOver(i, j))
                        {
                            hasMandatoryJump = true;
                            break;
                        }
                    }
                }
            }

            return hasMandatoryJump;
        }

        public bool IsValidMove(int startRow, int startCol, int endRow, int endCol)
        {
            eGamePieceType playerPiece = eGamePieceType.Empty;
            eGamePieceType kingPiece = eGamePieceType.Empty;
            bool canJump = false;
            bool isValidMove = false;

            if (GameBoard.Board[startRow, startCol].PieceId == eGamePieceType.Player1 || GameBoard.Board[startRow, startCol].PieceId == eGamePieceType.KingPlayer1)
            {
                playerPiece = eGamePieceType.Player1;
                kingPiece = eGamePieceType.KingPlayer1;
            }

            if (GameBoard.Board[startRow, startCol].PieceId == eGamePieceType.Player2 || GameBoard.Board[startRow, startCol].PieceId == eGamePieceType.KingPlayer2)
            {
                playerPiece = eGamePieceType.Player2;
                kingPiece = eGamePieceType.KingPlayer2;
            }

            canJump = CanJumpOver(startRow, startCol);

            if (!IsJumpOverMove && canJump)
            {
                isValidMove = false;
            }
            else if (hasMandatoryJump(playerPiece, kingPiece) && !IsJumpOverMove)
            {
                isValidMove = false;
            }
            else
            {
                isValidMove = isDiagonalMoveValid(startRow, startCol, endRow, endCol) || canJump;
            }

            return isValidMove;
        }

        private bool isDiagonalMoveValid(int i_StartRow, int i_StartCol, int i_EndRow, int i_EndCol)
        {
            char player;
            bool isDiagonalMoveValid = false;
        
                if (GameBoard.Board[i_EndRow, i_EndCol].PieceId == eGamePieceType.Empty)
                {
                    if (i_EndRow >= 0 && i_EndRow < GameBoard.BoardSize && i_EndCol >= 0 && i_EndCol < GameBoard.BoardSize)
                    {
                        player = whichPlayer(i_StartRow, i_StartCol);
                        if (player == 'O' && (i_StartRow + 1 == i_EndRow) && (Math.Abs(i_StartCol - i_EndCol) == 1))
                        {
                            isDiagonalMoveValid = true;
                        }
                        else if (player == 'X' && (i_StartRow - 1 == i_EndRow) && (Math.Abs(i_StartCol - i_EndCol) == 1))
                        {
                            isDiagonalMoveValid = true;
                        }
                        else if ((player == 'U' || player == 'K') && (Math.Abs(i_StartCol - i_EndCol) == 1) && (Math.Abs(i_StartRow - i_EndRow) == 1))
                        {
                            isDiagonalMoveValid = true;
                        }
                    }
                }

            return isDiagonalMoveValid;
        }

        private char whichPlayer(int i_Row, int i_Col)
        {
            char playerSymbol = ' ';

            switch (GameBoard.Board[i_Row, i_Col].PieceId)
            {
                case eGamePieceType.Player1:
                    playerSymbol = 'O';
                    break;
                case eGamePieceType.Player2:
                    playerSymbol = 'X';
                    break;
                case eGamePieceType.KingPlayer1:
                    playerSymbol = 'U';
                    break;
                case eGamePieceType.KingPlayer2:
                    playerSymbol = 'K';
                    break;
                case eGamePieceType.Empty:
                    playerSymbol = ' ';
                    break;
            }

            return playerSymbol;
        }

        private bool isJumpOverFromK(int i_StartRow, int i_StartCol)
        {
            bool isJumpOver = false;

            if (i_StartCol > 1 && i_StartCol < GameBoard.BoardSize - 2)
            {
                if (isLeftBackJumpMoveFromK(i_StartRow, i_StartCol))
                {
                    JumpMoveDirection = eJumpDirection.LeftDown;
                    isJumpOver = true;
                }
                else if (isRightBackJumpMoveFromK(i_StartRow, i_StartCol))
                {
                    JumpMoveDirection = eJumpDirection.RightDown;
                    isJumpOver = true;
                }
                else if (IsLeftJumpOverFromX(i_StartRow, i_StartCol))
                {
                    JumpMoveDirection = eJumpDirection.LeftUp;
                    isJumpOver = true;
                }
                else if (isRightJumpOverFromX(i_StartRow, i_StartCol))
                {
                    JumpMoveDirection = eJumpDirection.RightUp;
                    isJumpOver = true;
                }
            }
            else
            {
                if (i_StartCol < GameBoard.BoardSize - 2)
                {
                    if (isRightBackJumpMoveFromK(i_StartRow, i_StartCol))
                    {
                        JumpMoveDirection = eJumpDirection.RightDown;
                        isJumpOver = true;
                    }

                    else if (isRightJumpOverFromX(i_StartRow, i_StartCol))
                    {
                        JumpMoveDirection = eJumpDirection.RightUp;
                        isJumpOver = true;
                    }
                }

                if (i_StartCol > 1)
                {
                    if (isLeftBackJumpMoveFromK(i_StartRow, i_StartCol))
                    {
                        JumpMoveDirection = eJumpDirection.LeftDown;
                        isJumpOver = true;
                    }

                    else if (IsLeftJumpOverFromX(i_StartRow, i_StartCol))
                    {
                        JumpMoveDirection = eJumpDirection.LeftUp;
                        isJumpOver = true;
                    }
                }
            }

            return isJumpOver;
        }

        private bool isJumpOverFromU(int i_StartRow, int i_StartCol)
        {
            bool isJumpOver = false;

            if (i_StartCol > 1 && i_StartCol < GameBoard.BoardSize - 2)
            {
                if (IsLeftBackJumpMoveFromU(i_StartRow, i_StartCol))
                {
                    JumpMoveDirection = eJumpDirection.LeftUp;
                    isJumpOver = true;
                }
                else if (IsRightBackJumpMoveFromU(i_StartRow, i_StartCol))
                {
                    JumpMoveDirection = eJumpDirection.RightUp;
                    isJumpOver = true;
                }
                else if (IsRightJumpOverFromO(i_StartRow, i_StartCol))
                {
                    JumpMoveDirection = eJumpDirection.RightDown;
                    isJumpOver = true;
                }
                else if (IsLeftJumpOverFromO(i_StartRow, i_StartCol))
                {
                    JumpMoveDirection = eJumpDirection.LeftDown;
                    isJumpOver = true;
                }
            }
            else
            {
                if (i_StartCol < GameBoard.BoardSize - 2)
                {
                    if (IsRightBackJumpMoveFromU(i_StartRow, i_StartCol))
                    {
                        JumpMoveDirection = eJumpDirection.RightUp;
                        isJumpOver = true;
                    }
                    else if (IsRightJumpOverFromO(i_StartRow, i_StartCol))
                    {
                        JumpMoveDirection = eJumpDirection.RightDown;
                        isJumpOver = true;
                    }
                }

                if (i_StartCol > 1)
                {
                    if (IsLeftBackJumpMoveFromU(i_StartRow, i_StartCol))
                    {
                        JumpMoveDirection = eJumpDirection.LeftUp;
                        isJumpOver = true;
                    }
                    else if (IsLeftJumpOverFromO(i_StartRow, i_StartCol))
                    {
                        JumpMoveDirection = eJumpDirection.LeftDown;
                        isJumpOver = true;
                    }
                }
            }

            return isJumpOver;
        }

        private bool isRightJumpOverFromX(int i_StartRow, int i_StartCol)
        {
            bool isRightJumpOverFromX = false;
            int endRowPos = EndRowPos;
            int endColPos = EndColPos;

            if (i_StartRow > 1)
            {
                if (i_StartCol < GameBoard.BoardSize - 2)
                {
                    if (whichPlayer(i_StartRow - 2, i_StartCol + 2) == ' ')
                    {
                        if (whichPlayer(i_StartRow - 1, i_StartCol + 1) == 'O' || whichPlayer(i_StartRow - 1, i_StartCol + 1) == 'U')
                        {
                            if (i_StartRow - 2 == endRowPos && endColPos == i_StartCol + 2)
                            {
                                IsJumpOverMove = true;
                                JumpMoveDirection = eJumpDirection.RightUp;
                            }

                            isRightJumpOverFromX = true;
                        }
                    }
                }
            }

            return isRightJumpOverFromX;
        }

        public bool IsLeftJumpOverFromX(int i_StartRow, int i_StartCol)
        {
            bool isLeftJumpOverFromX = false;
            int endRowPos = EndRowPos;
            int endColPos = EndColPos;

            if (i_StartRow > 1)
            {
                if (i_StartCol > 1)
                {
                    if (whichPlayer(i_StartRow - 2, i_StartCol - 2) == ' ')
                    {
                        if (whichPlayer(i_StartRow - 1, i_StartCol - 1) == 'O' || whichPlayer(i_StartRow - 1, i_StartCol - 1) == 'U')
                        {
                            if (endRowPos == i_StartRow - 2 && endColPos == i_StartCol - 2)
                            {
                                IsJumpOverMove = true;
                                JumpMoveDirection = eJumpDirection.LeftUp;
                            }

                            isLeftJumpOverFromX = true;
                        }
                    }
                }
            }

            return isLeftJumpOverFromX;
        }

        public bool IsRightJumpOverFromO(int i_StartRow, int i_StartCol)
        {
            bool isRightJumpOverFromO = false;
            int endRowPos = EndRowPos;
            int endColPos = EndColPos;

            if (i_StartCol < GameBoard.BoardSize - 2)
            {
                if (i_StartRow < GameBoard.BoardSize - 2)
                {
                    if (whichPlayer(i_StartRow + 2, i_StartCol + 2) == ' ')
                    {
                        if (whichPlayer(i_StartRow + 1, i_StartCol + 1) == 'X' || whichPlayer(i_StartRow + 1, i_StartCol + 1) == 'K')
                        {
                            if (endRowPos == i_StartRow + 2 && endColPos == i_StartCol + 2)
                            {
                                IsJumpOverMove = true;
                                JumpMoveDirection = eJumpDirection.RightDown;
                            }

                            isRightJumpOverFromO = true;
                        }
                    }
                }
            }

            return isRightJumpOverFromO;
        }

        public bool IsLeftJumpOverFromO(int i_StartRow, int i_StartCol)
        {
            bool isLeftJumpOverFromO = false;
            int endRowPos = EndRowPos;
            int endColPos = EndColPos;

            if (i_StartRow < GameBoard.BoardSize - 2)
            {
                if (i_StartCol > 1)
                {
                    if (whichPlayer(i_StartRow + 2, i_StartCol - 2) == ' ')
                    {
                        if (whichPlayer(i_StartRow + 1, i_StartCol - 1) == 'X' || whichPlayer(i_StartRow + 1, i_StartCol - 1) == 'K')
                        {
                            if (endRowPos == i_StartRow + 2 && endColPos == i_StartCol - 2)
                            {
                                IsJumpOverMove = true;
                                JumpMoveDirection = eJumpDirection.LeftDown;
                            }

                            isLeftJumpOverFromO = true;
                        }
                    }
                }
            }

            return isLeftJumpOverFromO;
        }

        public bool IsRightBackJumpMoveFromU(int i_StartRow, int i_StartCol)
        {
            bool isRightBackJumpMoveFromU = false;
            int endRowPos = EndRowPos;
            int endColPos = EndColPos;

            if (i_StartRow > 1)
            {
                if (i_StartCol < GameBoard.BoardSize - 2)
                {
                    if (whichPlayer(i_StartRow - 2, i_StartCol + 2) == ' ')
                    {
                        if (whichPlayer(i_StartRow - 1, i_StartCol + 1) == 'X' || whichPlayer(i_StartRow - 1, i_StartCol + 1) == 'K')
                        {
                            if (endRowPos == i_StartRow - 2 && endColPos == i_StartCol + 2)
                            {
                                IsJumpOverMove = true;
                                JumpMoveDirection = eJumpDirection.RightUp;
                            }

                            isRightBackJumpMoveFromU = true;
                        }
                    }
                }
            }

            return isRightBackJumpMoveFromU;
        }

        public bool IsLeftBackJumpMoveFromU(int i_StartRow, int i_StartCol)
        {
            bool isLeftBackJumpMoveFromU = false;
            int endRowPos = EndRowPos;
            int endColPos = EndColPos;

            if (i_StartRow > 1)
            {
                if (i_StartCol > 1)
                {
                    if (whichPlayer(i_StartRow - 2, i_StartCol - 2) == ' ')
                    {
                        if (whichPlayer(i_StartRow - 1, i_StartCol - 1) == 'X' || whichPlayer(i_StartRow - 1, i_StartCol - 1) == 'K')
                        {
                            if (endRowPos == i_StartRow - 2 && endColPos == i_StartCol - 2)
                            {
                                IsJumpOverMove = true;
                                JumpMoveDirection = eJumpDirection.LeftUp;
                            }

                            isLeftBackJumpMoveFromU = true;
                        }
                    }
                }
            }

            return isLeftBackJumpMoveFromU;
        }

        private bool isRightBackJumpMoveFromK(int i_StartRow, int i_StartCol)
        {
            bool isRightBackJumpMoveFromK = false;
            int endRowPos = EndRowPos;
            int endColPos = EndColPos;

            if (i_StartRow < GameBoard.BoardSize - 2)
            {
                if (i_StartCol < GameBoard.BoardSize - 2)
                {
                    if (whichPlayer(i_StartRow + 2, i_StartCol + 2) == ' ')
                    {
                        if (whichPlayer(i_StartRow + 1, i_StartCol + 1) == 'O' || whichPlayer(i_StartRow + 1, i_StartCol + 1) == 'U')
                        {
                            if (i_StartRow + 2 == endRowPos && i_StartCol + 2 == endColPos)
                            {
                                IsJumpOverMove = true;
                                JumpMoveDirection = eJumpDirection.RightDown;
                            }

                            isRightBackJumpMoveFromK = true;
                        }
                    }
                }
            }

            return isRightBackJumpMoveFromK;
        }

        private bool isLeftBackJumpMoveFromK(int i_StartRow, int i_StartCol)
        {
            bool isLeftBackJumpMoveFromK = false;
            int endRowPos = EndRowPos;
            int endColPos = EndColPos;

            if (i_StartRow < GameBoard.BoardSize - 2)
            {
                if (i_StartCol > 1)
                {
                    if (whichPlayer(i_StartRow + 2, i_StartCol - 2) == ' ')
                    {
                        if (whichPlayer(i_StartRow + 1, i_StartCol - 1) == 'O' || whichPlayer(i_StartRow + 1, i_StartCol - 1) == 'U')
                        {
                            if (i_StartRow + 2 == endRowPos && i_StartCol - 2 == endColPos)
                            {
                                IsJumpOverMove = true;
                                JumpMoveDirection = eJumpDirection.LeftDown;
                            }

                            isLeftBackJumpMoveFromK = true;
                        }
                    }
                }
            }

            return isLeftBackJumpMoveFromK;
        }

        public bool CanJumpOver(int i_StartRow, int i_StartCol)
        {
            bool canJumpOver = false;

            if (whichPlayer(i_StartRow, i_StartCol) == 'X')
            {
                if (i_StartRow > 1)
                {
                    if (i_StartCol > 1 && i_StartCol < GameBoard.BoardSize - 2)
                    {
                        canJumpOver = isRightJumpOverFromX(i_StartRow, i_StartCol) || IsLeftJumpOverFromX(i_StartRow, i_StartCol);
                    }

                    else if (i_StartCol > 1)
                    {
                        canJumpOver = IsLeftJumpOverFromX(i_StartRow, i_StartCol);
                    }

                    else if (i_StartCol < GameBoard.BoardSize - 2)
                    {
                        canJumpOver = isRightJumpOverFromX(i_StartRow, i_StartCol);
                    }
                }
            }
            else if (whichPlayer(i_StartRow, i_StartCol) == 'O')
            {
                if (i_StartRow < GameBoard.BoardSize - 2)
                {
                    if (i_StartCol > 1 && i_StartCol < GameBoard.BoardSize - 2)
                    {
                        canJumpOver = IsRightJumpOverFromO(i_StartRow, i_StartCol) || IsLeftJumpOverFromO(i_StartRow, i_StartCol);
                    }

                    else if (i_StartCol < GameBoard.BoardSize - 2)
                    {
                        canJumpOver = IsRightJumpOverFromO(i_StartRow, i_StartCol);
                    }

                    else if (i_StartCol > 1)
                    {
                        canJumpOver = IsLeftJumpOverFromO(i_StartRow, i_StartCol);
                    }
                }
            }
            else if (whichPlayer(i_StartRow, i_StartCol) == 'K')
            {
                canJumpOver = isJumpOverFromK(i_StartRow, i_StartCol);
            }
            else if (whichPlayer(i_StartRow, i_StartCol) == 'U')
            {
                canJumpOver = isJumpOverFromU(i_StartRow, i_StartCol);
            }

            return canJumpOver;
        }

        private void updateJumpOver(int i_StartRow, int i_StartCol, int i_EndRow, int i_EndCol)
        {
            GameBoard.Board[i_EndRow, i_EndCol] = new GamePiece(GameBoard.Board[i_StartRow, i_StartCol].PieceId);
            GameBoard.Board[i_StartRow, i_StartCol].PieceId = eGamePieceType.Empty;

            CheckAndPromoteToKing();
            if (JumpMoveDirection == eJumpDirection.RightUp && IsWithinBounds(i_StartRow - 1, i_StartCol + 1))
            {
                GameBoard.Board[i_StartRow - 1, i_StartCol + 1].PieceId = eGamePieceType.Empty;
            }

            if (JumpMoveDirection == eJumpDirection.LeftUp && IsWithinBounds(i_StartRow - 1, i_StartCol - 1))
            {
                GameBoard.Board[i_StartRow - 1, i_StartCol - 1].PieceId = eGamePieceType.Empty;
            }

            if (JumpMoveDirection == eJumpDirection.RightDown && IsWithinBounds(i_StartRow + 1, i_StartCol + 1))
            {
                GameBoard.Board[i_StartRow + 1, i_StartCol + 1].PieceId = eGamePieceType.Empty;
            }

            if (JumpMoveDirection == eJumpDirection.LeftDown && IsWithinBounds(i_StartRow + 1, i_StartCol - 1))
            {
                GameBoard.Board[i_StartRow + 1, i_StartCol - 1].PieceId = eGamePieceType.Empty;
            }
        }

        public void UpdateMove()
        {
            int startRowPos = StartRowPos;
            int endRowPos = EndRowPos;
            int startColPos = StartColPos;
            int endColPos = EndColPos;

            if (IsJumpOverMove)
            {
                updateJumpOver(startRowPos, startColPos, endRowPos, endColPos);
            }
            else if (isDiagonalMoveValid(startRowPos, startColPos, endRowPos, endColPos))
            {
                GameBoard.Board[endRowPos, endColPos] = new GamePiece(GameBoard.Board[startRowPos, startColPos].PieceId);
                GameBoard.Board[startRowPos, startColPos].PieceId = eGamePieceType.Empty;
            }
        }

        public void CheckAndPromoteToKing()
        {
            int endRowPos = EndRowPos;
            int endColPos = EndColPos;

            if (GameBoard.Board[endRowPos, endColPos].PieceId == eGamePieceType.Player1 && endRowPos == GameBoard.Board.GetLength(0) - 1)
            {
                GameBoard.Board[endRowPos, endColPos] = new GamePiece(eGamePieceType.KingPlayer1);
            }

            else if (GameBoard.Board[endRowPos, endColPos].PieceId == eGamePieceType.Player2 && endRowPos == 0)
            {
                GameBoard.Board[endRowPos, endColPos] = new GamePiece(eGamePieceType.KingPlayer2);
            }
        }
    }
}