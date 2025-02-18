using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChekersGame
{
    public class GameManager
    {
        private static Game s_Game;
        private static Player s_Player1;
        private static Player s_Player2;

        public GameManager(Game i_Game, Player i_Player1, Player i_Player2)
        {
            s_Game = i_Game;
            s_Player1 = i_Player1;
            s_Player2 = i_Player2;

        }

        public static Game Game
        {
            get { return s_Game; }
            set { s_Game = value; }
        }

        public static Player Player1
        {
            get { return s_Player1; }
        }

        public static Player Player2
        {
            get { return s_Player2; }
        }
    }
}