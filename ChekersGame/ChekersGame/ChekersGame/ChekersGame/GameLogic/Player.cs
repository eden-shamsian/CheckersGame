using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChekersGame
{
    public class Player
    {
        private string m_PlayerName;
        private int m_PlayerScore;
        private char m_SymbolPlayer;
        private int m_GameMode;

        public Player()
        {
            m_PlayerName = string.Empty;
            m_PlayerScore = 0;
            m_SymbolPlayer = ' ';
        }

        public Player(string i_Name, char i_Symbol)
        {
            m_PlayerName = i_Name;
            m_PlayerScore = 0;
            m_SymbolPlayer = i_Symbol;
        }

        public Player(char i_Symbol)
        {
            m_PlayerName = string.Empty;
            m_PlayerScore = 0;
            m_SymbolPlayer = i_Symbol;
        }
        public char SymbolPlayer
        {
            get { return m_SymbolPlayer; }
            set { m_SymbolPlayer = value; }
        }

        public string PlayerName
        {
            get { return m_PlayerName; }
            set { m_PlayerName = value; }
        }

        public int PlayerScore
        {
            get { return m_PlayerScore; }
            set { m_PlayerScore = value; }
        }

        public  int GameMode
        {
            get { return m_GameMode; }
            set { m_GameMode = value; }
        }
    }
}
