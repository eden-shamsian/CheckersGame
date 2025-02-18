using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChekersGame
{
    public class GameBoard
    {
        private static GamePiece[,] s_Board;
        private static int s_BoardSize;

        public static int BoardSize
        {
            get { return s_BoardSize; }
            set { s_BoardSize = value; }
        }

        public static GamePiece[,] Board
        {
            get { return s_Board; }
            set { s_Board = value; }
        }

        public GameBoard()
        {
            s_BoardSize = 0;
            s_Board = new GamePiece[0, 0];
        }

        public GameBoard(int i_Size)
        {
            s_BoardSize = i_Size;
            s_Board = new GamePiece[i_Size, i_Size];
            InitializeBoard();
        }

        public static int CountPieces(eGamePieceType i_PieceType, eGamePieceType i_KingType)
        {
            int countOfPieces = 0;

            for (int i = 0; i < GameBoard.s_BoardSize; i++)
            {
                for (int j = 0; j < GameBoard.s_BoardSize; j++)
                {
                    if (GameBoard.s_Board[i, j].PieceId == i_PieceType)
                    {
                        countOfPieces++;
                    }
                    else if (GameBoard.s_Board[i, j].PieceId == i_KingType)
                    {
                        countOfPieces += 4;
                    }
                }
            }

            return countOfPieces;
        }

        public void InitializeBoard()
        {
            int i, j;

            for (i = 0; i < (s_BoardSize / 2) + 1; i++)
            {
                for (j = 0; j < s_BoardSize; j++)
                {
                    if ((i + j) % 2 == 1)
                    {
                        s_Board[i, j] = new GamePiece('O');
                    }
                    else
                    {
                        s_Board[i, j] = new GamePiece(' ');
                    }
                    if (i == (s_BoardSize / 2 - 1) || (i == s_BoardSize / 2))
                    {
                        s_Board[i, j] = new GamePiece(' ');
                    }
                }
            }

            for (i = (s_BoardSize + 2) / 2; i < s_BoardSize; i++)
            {
                for (j = 0; j < s_BoardSize; j++)
                {
                    if ((i + j) % 2 == 1)
                    {
                        s_Board[i, j] = new GamePiece('X');
                    }
                    else
                    {
                        s_Board[i, j] = new GamePiece(' ');
                    }
                }
            }
        }
    }
}
