using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChekersGame
{
    public class GamePiece
    {
        private eGamePieceType m_PieceId;

        public GamePiece(eGamePieceType i_PieceType)
        {
            this.m_PieceId = i_PieceType;
        }

        public eGamePieceType PieceId
        {
            get { return m_PieceId; }
            set { m_PieceId = value; }
        }

        public GamePiece(char i_GamePieceId)
        {
            switch (i_GamePieceId)
            {
                case 'O':
                    m_PieceId = eGamePieceType.Player1;
                    break;
                case 'X':
                    m_PieceId = eGamePieceType.Player2;
                    break;
                case 'U':
                    m_PieceId = eGamePieceType.KingPlayer1;
                    break;
                case 'K':
                    m_PieceId = eGamePieceType.KingPlayer2;
                    break;
                case ' ':
                    m_PieceId = eGamePieceType.Empty;
                    break;
            }
        }
    }
}