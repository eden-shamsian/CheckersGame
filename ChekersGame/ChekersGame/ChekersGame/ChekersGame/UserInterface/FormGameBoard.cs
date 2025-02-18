using ChekersGame;
using System.Drawing;
using System.Windows.Forms;
using System;

public class FormGameBoard : Form
{
    private readonly int r_BoardSize;
    private static Button[,] m_BoardButtons;
    private Button m_SelectedButton;
    private Label m_Player1ScoreLabel;
    private Label m_Player2ScoreLabel;

    public FormGameBoard()
    {
        r_BoardSize = GameBoard.BoardSize;
        m_BoardButtons = new Button[r_BoardSize, r_BoardSize];
        initializeScoreLabels();
        initializeBoard();
    }

    private void initializeScoreLabels()
    {
        m_Player1ScoreLabel = new Label();
        m_Player1ScoreLabel.Text = "Player 1: 0";
        m_Player1ScoreLabel.Location = new Point(10, 10);
        m_Player1ScoreLabel.Font = new Font("Arial", 12, FontStyle.Bold);
        m_Player1ScoreLabel.AutoSize = true;

        m_Player2ScoreLabel = new Label();
        m_Player2ScoreLabel.Text = "Player 2: 0";
        m_Player2ScoreLabel.Location = new Point(150, 10);
        m_Player2ScoreLabel.Font = new Font("Arial", 12, FontStyle.Bold);
        m_Player2ScoreLabel.AutoSize = true;

        this.Controls.Add(m_Player1ScoreLabel);
        this.Controls.Add(m_Player2ScoreLabel);
    }

    private void initializeBoard()
    {
        this.Text = "Damka";
        this.Size = new Size(r_BoardSize * 70 + 20, r_BoardSize * 70 + 150);
        this.StartPosition = FormStartPosition.CenterScreen;
        Panel boardPanelContainer = new Panel();
        boardPanelContainer.Size = new Size(r_BoardSize * 70, r_BoardSize * 70);
        boardPanelContainer.Location = new Point(0, 50);
        TableLayoutPanel boardPanel = new TableLayoutPanel();
        boardPanel.Dock = DockStyle.Fill;
        boardPanel.RowCount = r_BoardSize;
        boardPanel.ColumnCount = r_BoardSize;
        boardPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

        for (int row = 0; row < r_BoardSize; row++)
        {
            for (int col = 0; col < r_BoardSize; col++)
            {
                Button cellButton = new Button();
                cellButton.Size = new Size(58, 58);
                cellButton.FlatStyle = FlatStyle.Flat;
                cellButton.Tag = new Point(row, col);
                cellButton.Click += buttonCell_Click;

                if ((row + col) % 2 == 0)
                {
                    cellButton.BackColor = Color.Brown;
                    cellButton.Enabled = false;
                }
                else
                {
                    cellButton.BackColor = Color.White;
                }

                m_BoardButtons[row, col] = cellButton;
                boardPanel.Controls.Add(cellButton, col, row);
            }
        }

        boardPanelContainer.Controls.Add(boardPanel);
        this.Controls.Add(boardPanelContainer);
    }

    private void buttonCell_Click(object sender, EventArgs e)
    {
        Button clickedButton = sender as Button;

        if (!GameManager.Game.IsGameOver)
        {
            SelectPiece(clickedButton);

            if (m_SelectedButton != null && clickedButton.BackgroundImage == null)
            {
                executeMove(clickedButton);
            }

            eGameState gameState = GameManager.Game.CheckForVictoryOrDraw(GameManager.Player1, GameManager.Player2);
            if (gameState != eGameState.InProgress)
            {
                handleGameEnd(gameState);
                return;
            }

            if (GameManager.Game.CurrentPlayer.GameMode == 2)
            {
                Move computerMove = new Move().GetRandomLegalMove();
                if (computerMove != null)
                {
                    executeComputerMove(computerMove);
                }

                gameState = GameManager.Game.CheckForVictoryOrDraw(GameManager.Player1, GameManager.Player2);
                if (gameState != eGameState.InProgress)
                {
                    handleGameEnd(gameState);
                }
            }
        }
    }

    private void restartGame()
    {
        GameManager.Game = new Game(GameBoard.BoardSize);
        GameManager.Game.CurrentPlayer = GameManager.Player1;
        m_SelectedButton = null;

        for (int row = 0; row < GameBoard.BoardSize; row++)
        {
            for (int col = 0; col < GameBoard.BoardSize; col++)
            {
                GameBoard.Board[row, col] = null;
                m_BoardButtons[row, col].BackgroundImage = null;
                m_BoardButtons[row, col].BackColor = (row + col) % 2 == 0 ? Color.Brown : Color.White;
            }
        }

        GameManager.Game.Board.InitializeBoard();
        printBoard();
        updateScores();
    }

    private void handleGameEnd(eGameState gameState)
    {
        string message = gameState == eGameState.Player1Wins ? "Player 1 Won! Another Round?" :
                         gameState == eGameState.Player2Wins ? "Player 2 Won! Another Round?" :
                         "It's a Tie! Another Round?";
        DialogResult result = MessageBox.Show(message, "Game Over", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            restartGame();
        }
        else
        {
            Close();
        }
    }

    private void executeMove(Button clickedButton)
    {
        Move move = new Move();

        if (GameManager.Game.CheckComputerGameMode(GameManager.Game))
        {
            move = move.GetRandomLegalMove();
            if (move == null)
            {
                MessageBox.Show("No legal moves available for the computer!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GameManager.Game.IsGameOver = true;
            }
            else
            {
                executeComputerMove(move);
            }
        }
        else
        {
            if (m_SelectedButton != null && m_SelectedButton.BackColor == Color.LightBlue)
            {
                Point startPosition = (Point)m_SelectedButton.Tag;
                int startRow = startPosition.X;
                int startCol = startPosition.Y;
                Point endPosition = (Point)clickedButton.Tag;
                int endRow = endPosition.X;
                int endCol = endPosition.Y;
                move.StartRowPos = startRow;
                move.StartColPos = startCol;
                move.EndRowPos = endRow;
                move.EndColPos = endCol;
                bool isValidMove = move.IsValidMove(startRow, startCol, endRow, endCol);

                if (isValidMove)
                {
                    if (GameManager.Game.IsMyTurn(GameManager.Game, GameManager.Player1, GameManager.Player2, move))
                    {
                        clickedButton.BackgroundImage = m_SelectedButton.BackgroundImage;
                        m_SelectedButton.BackgroundImage = null;
                        clickedButton.BackColor = Color.White;
                        m_SelectedButton.BackColor = Color.White;
                        m_SelectedButton = null;
                        printBoard();
                        updateScores();
                        if (!(move.IsJumpOverMove && move.CanJumpOver(move.EndRowPos, move.EndColPos)))
                        {
                            GameManager.Game.CurrentPlayer = (GameManager.Game.CurrentPlayer == GameManager.Player1)
                                ? GameManager.Player2
                                : GameManager.Player1;

                            if (GameManager.Game.CurrentPlayer.GameMode == 2)
                            {
                                Move computerMove = new Move().GetRandomLegalMove();
                                if (computerMove != null)
                                {
                                    executeComputerMove(computerMove);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("It's not your turn!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid move! Please try again.");
                }
            }
        }
    }

    private void executeComputerMove(Move move)
    {
        Button startButton = m_BoardButtons[move.StartRowPos, move.StartColPos];
        Button endButton = m_BoardButtons[move.EndRowPos, move.EndColPos];
        if (GameManager.Game.IsMyTurn(GameManager.Game, GameManager.Player1, GameManager.Player2, move))
        {
            endButton.BackgroundImage = startButton.BackgroundImage;
            startButton.BackgroundImage = null;
            printBoard();
            updateScores();
            if (!(move.IsJumpOverMove && move.CanJumpOver(move.EndRowPos, move.EndColPos)))
            {
                GameManager.Game.CurrentPlayer = GameManager.Player1;
            }
        }
    }


    private void updateScores()
    {
        int player1Count = GameBoard.CountPieces(eGamePieceType.Player1, eGamePieceType.KingPlayer1);
        int player2Count = GameBoard.CountPieces(eGamePieceType.Player2, eGamePieceType.KingPlayer2);

        int score = Math.Abs(player1Count - player2Count);

        if (player1Count > player2Count)
        {
            GameManager.Player1.PlayerScore = score;
            GameManager.Player2.PlayerScore = 0;
        }
        else if (player2Count > player1Count)
        {
            GameManager.Player1.PlayerScore = 0;
            GameManager.Player2.PlayerScore = score;
        }

        m_Player1ScoreLabel.Text = $"{GameManager.Player1.PlayerName}: {GameManager.Player1.PlayerScore}";
        m_Player2ScoreLabel.Text = $"{GameManager.Player2.PlayerName}: {GameManager.Player2.PlayerScore}";
    }

    private void SelectPiece(Button clickedButton)
    {
        if (clickedButton.BackgroundImage != null)
        {
            if (m_SelectedButton == null)
            {
                clickedButton.BackColor = Color.LightBlue;
                m_SelectedButton = clickedButton;
            }
            else if (m_SelectedButton == clickedButton)
            {
                clickedButton.BackColor = Color.White;
                m_SelectedButton.BackColor = Color.White;
                m_SelectedButton = null;
            }
            else
            {
                m_SelectedButton.BackColor = Color.White;
                clickedButton.BackColor = Color.LightBlue;
                m_SelectedButton = clickedButton;
            }
        }
    }

    public static void printBoard()
    {
        for (int row = 0; row < GameBoard.BoardSize; row++)
        {
            for (int col = 0; col < GameBoard.BoardSize; col++)
            {
                Button cellButton = m_BoardButtons[row, col];
                GamePiece gamePiece = GameBoard.Board[row, col];

                if (gamePiece != null)
                {
                    string imagePath = "";
                    switch (gamePiece.PieceId)
                    {
                        case eGamePieceType.Player1:
                            imagePath = @"Images/blackPiece.png";
                            break;
                        case eGamePieceType.Player2:
                            imagePath = @"Images/whitePiece.png";
                            break;
                        case eGamePieceType.KingPlayer1:
                            imagePath = @"Images/blackKing.png";
                            break;
                        case eGamePieceType.KingPlayer2:
                            imagePath = @"Images/whiteKing.png";
                            break;
                        default:
                            imagePath = "";
                            break;
                    }
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        cellButton.BackgroundImage = Image.FromFile(imagePath);
                        cellButton.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else
                    {
                        cellButton.BackgroundImage = null;
                    }
                }
            }
        }
    }

    private void GameBoardForm_Load(object sender, EventArgs e)
    {

    }
}