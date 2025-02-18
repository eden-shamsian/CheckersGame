using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChekersGame
{
    public class Game
    {
        private GameBoard m_Board;
        private Player m_CurrentPlayer;
        private bool m_IsGameOver;
        private eGameState m_GameState;

        public Game()
        {
            m_Board = new GameBoard();
            m_CurrentPlayer = null;
            m_IsGameOver = false;
            m_GameState = eGameState.InProgress;
        }

        public Game(int i_SizeOfBoard)
        {
            m_Board = new GameBoard(i_SizeOfBoard);
            m_CurrentPlayer = null;
            m_IsGameOver = false;
            m_GameState = eGameState.InProgress;
        }

        public bool IsGameOver
        {
            get { return m_IsGameOver; }
            set { m_IsGameOver = value; }
        }

        public Player CurrentPlayer
        {
            get { return m_CurrentPlayer; }
            set { m_CurrentPlayer = value; }
        }

        public GameBoard Board
        {
            get { return m_Board; }
            set { m_Board = value; }
        }

        public bool CheckComputerGameMode(Game i_Game)
        {

            return i_Game.CurrentPlayer.GameMode == 2;
        }

        public bool IsMyTurn(Game i_Game, Player i_Player1, Player i_Player2, Move i_Move)
        {
            bool isMyTurn = false;
            if ((i_Game.CurrentPlayer == i_Player1 && isPlayer1Turn(i_Move)) ||
                    (i_Game.CurrentPlayer == i_Player2 && isPlayer2Turn(i_Move)))
            {
                i_Move.UpdateMove();
                i_Move.CheckAndPromoteToKing();
                isMyTurn = true;
            }

            return isMyTurn;
        }

        private bool isPlayer1Turn(Move i_Move)
        {

            return (GameBoard.Board[i_Move.StartRowPos, i_Move.StartColPos].PieceId == eGamePieceType.Player1 || GameBoard.Board[i_Move.StartRowPos, i_Move.StartColPos].PieceId == eGamePieceType.KingPlayer1);
        }

        private bool isPlayer2Turn(Move i_Move)
        {

            return (GameBoard.Board[i_Move.StartRowPos, i_Move.StartColPos].PieceId == eGamePieceType.Player2 || GameBoard.Board[i_Move.StartRowPos, i_Move.StartColPos].PieceId == eGamePieceType.KingPlayer2);
        }

        public eGameState CheckForVictoryOrDraw(Player i_Player1, Player i_Player2)
        {
            bool player1HasMoves = Move.HasLegalMoves(eGamePieceType.Player1, eGamePieceType.KingPlayer1);
            bool player2HasMoves = Move.HasLegalMoves(eGamePieceType.Player2, eGamePieceType.KingPlayer2);
            int player1Pieces = GameBoard.CountPieces(eGamePieceType.Player1, eGamePieceType.KingPlayer1);
            int player2Pieces = GameBoard.CountPieces(eGamePieceType.Player2, eGamePieceType.KingPlayer2);

            if (!player1HasMoves && !player2HasMoves)
            {
                m_GameState = eGameState.Draw;
            }

            if (player1Pieces == 0 || !player1HasMoves)
            {
                i_Player2.PlayerScore = Math.Abs(player2Pieces - player1Pieces);
                m_GameState = eGameState.Player2Wins;
                i_Player2.PlayerScore = Math.Abs(player2Pieces - player1Pieces);
            }

            if (player2Pieces == 0 || !player2HasMoves)
            {
                i_Player1.PlayerScore = Math.Abs(player2Pieces - player1Pieces);
                m_GameState = eGameState.Player1Wins;
                i_Player1.PlayerScore = Math.Abs(player2Pieces - player1Pieces);
            }

            return m_GameState;
        }
    }
}
