using ChekersGame;
using System;
using System.Drawing;
using System.Windows.Forms;

public class FormSettings : Form
{
    private RadioButton m_Size6RadioButton;
    private RadioButton m_Size8RadioButton;
    private RadioButton m_Size10RadioButton;
    private TextBox m_Player1TextBox;
    private TextBox m_Player2TextBox;
    private CheckBox m_Player2CheckBox;
    private Button m_DoneButton;
    private Player m_Player1;
    private Player m_Player2;

    public FormSettings()
    {
        initializeGameSettings();
    }

    private void initializeGameSettings()
    {
        const int k_Margin = 20;
        const int k_LabelWidth = 80;
        const int k_TextBoxWidth = 200;
        const int k_ButtonWidth = 100;
        const int k_ButtonHeight = 35;

        this.Text = "Game Settings";
        this.Size = new Size(400, 350);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackgroundImage = Image.FromFile(@"Images/DamkaGame.jpg");
        this.BackgroundImageLayout = ImageLayout.Stretch;

        addBoardSizeControls(k_Margin);
        addPlayerControls(k_Margin, k_LabelWidth, k_TextBoxWidth);
        addDoneButton(k_Margin, k_ButtonWidth, k_ButtonHeight);
    }

    private void addBoardSizeControls(int margin)
    {
        Label boardSizeLabel = new Label();
        boardSizeLabel.Text = "Board Size:";
        boardSizeLabel.Location = new Point(margin, margin);
        boardSizeLabel.AutoSize = true;
        boardSizeLabel.Font = new Font("Times New Roman", 14, FontStyle.Bold);
        this.Controls.Add(boardSizeLabel);

        m_Size6RadioButton = new RadioButton();
        m_Size6RadioButton.Text = "6 x 6";
        m_Size6RadioButton.Location = new Point(margin, boardSizeLabel.Bottom + 10);
        m_Size6RadioButton.AutoSize = true;
        m_Size6RadioButton.Font = new Font("Times New Roman", 14, FontStyle.Bold);
        m_Size6RadioButton.Checked = true;
        this.Controls.Add(m_Size6RadioButton);

        m_Size8RadioButton = new RadioButton();
        m_Size8RadioButton.Text = "8 x 8";
        m_Size8RadioButton.Location = new Point(m_Size6RadioButton.Right + 30, boardSizeLabel.Bottom + 10);
        m_Size8RadioButton.AutoSize = true;
        m_Size8RadioButton.Font = new Font("Times New Roman", 14, FontStyle.Bold);
        this.Controls.Add(m_Size8RadioButton);

        m_Size10RadioButton = new RadioButton();
        m_Size10RadioButton.Text = "10 x 10";
        m_Size10RadioButton.Location = new Point(m_Size8RadioButton.Right + 30, boardSizeLabel.Bottom + 10);
        m_Size10RadioButton.AutoSize = true;
        m_Size10RadioButton.Font = new Font("Times New Roman", 14, FontStyle.Bold);
        this.Controls.Add(m_Size10RadioButton);
    }

    private void addPlayerControls(int margin, int labelWidth, int textBoxWidth)
    {
        Label playersLabel = new Label();
        playersLabel.Text = "Players:";
        playersLabel.Location = new Point(margin, m_Size6RadioButton.Bottom + 20);
        playersLabel.AutoSize = true;
        playersLabel.Font = new Font("Times New Roman", 14, FontStyle.Bold);
        this.Controls.Add(playersLabel);

        Label player1Label = new Label();
        player1Label.Text = "Player 1:";
        player1Label.Location = new Point(margin, playersLabel.Bottom + 10);
        player1Label.AutoSize = true;
        player1Label.Font = new Font("Times New Roman", 14, FontStyle.Bold);
        this.Controls.Add(player1Label);

        m_Player1TextBox = new TextBox();
        m_Player1TextBox.Location = new Point(player1Label.Right + 10, player1Label.Top);
        m_Player1TextBox.Width = textBoxWidth;
        m_Player1TextBox.Height = 25;
        m_Player1TextBox.Font = new Font("Times New Roman", 14, FontStyle.Bold);
        this.Controls.Add(m_Player1TextBox);

        m_Player2CheckBox = new CheckBox();
        m_Player2CheckBox.Text = "Player 2:";
        m_Player2CheckBox.Location = new Point(margin, m_Player1TextBox.Bottom + 10);
        m_Player2CheckBox.AutoSize = true;
        m_Player2CheckBox.Font = new Font("Times New Roman", 14, FontStyle.Bold);
        m_Player2CheckBox.CheckedChanged += player2CheckBox_CheckedChanged;
        this.Controls.Add(m_Player2CheckBox);

        m_Player2TextBox = new TextBox();
        m_Player2TextBox.Location = new Point(player1Label.Right + 20, m_Player1TextBox.Bottom + 10);
        m_Player2TextBox.Width = textBoxWidth;
        m_Player2TextBox.Height = 25;
        m_Player2TextBox.Enabled = false;
        m_Player2TextBox.Text = "[Computer]";
        m_Player2TextBox.Font = new Font("Times New Roman", 14, FontStyle.Bold);
        this.Controls.Add(m_Player2TextBox);
    }

    private void addDoneButton(int margin, int buttonWidth, int buttonHeight)
    {
        m_DoneButton = new Button();
        m_DoneButton.Text = "Done";
        m_DoneButton.Location = new Point((this.ClientSize.Width - buttonWidth) / 2, m_Player2TextBox.Bottom + 20);
        m_DoneButton.Width = buttonWidth;
        m_DoneButton.Height = buttonHeight;
        m_DoneButton.Font = new Font("Times New Roman", 14, FontStyle.Bold);
        m_DoneButton.Click += buttonDone_Click;
        this.Controls.Add(m_DoneButton);
    }

    private void player2CheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_Player2TextBox.Enabled = m_Player2CheckBox.Checked;
        m_Player2TextBox.Text = m_Player2CheckBox.Checked ? "" : "[Computer]";
    }

    private void buttonDone_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(m_Player1TextBox.Text))
        {
            MessageBox.Show("Please enter a name for Player 1.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (m_Player2CheckBox.Checked && string.IsNullOrWhiteSpace(m_Player2TextBox.Text))
        {
            MessageBox.Show("Please enter a name for Player 2.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        this.Close();
        setBoardSize();
        setPlayers();
        FormGameBoard boardForm = new FormGameBoard();
        Game game = new Game(GameBoard.BoardSize);
        GameManager gameManager = new GameManager(game, m_Player1, m_Player2);
        FormGameBoard.printBoard();
        GameManager.Game.CurrentPlayer = GameManager.Player1;
        boardForm.ShowDialog();
    }

    private void setBoardSize()
    {
        if (m_Size6RadioButton.Checked)
        {
            GameBoard.BoardSize = 6;
        }
        else if (m_Size8RadioButton.Checked)
        {
            GameBoard.BoardSize = 8;
        }
        else if (m_Size10RadioButton.Checked)
        {
            GameBoard.BoardSize = 10;
        }
    }

    private void setPlayers()
    {
        m_Player1 = new Player('O');
        m_Player1.PlayerName = m_Player1TextBox.Text;

        m_Player2 = new Player('X');

        if (m_Player2CheckBox.Checked)
        {
            m_Player2.PlayerName = m_Player2TextBox.Text;
        }
        else
        {
            m_Player2.PlayerName = "Computer";
            m_Player2.GameMode = 2;
        }
    }

    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // FormSettings
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "FormSettings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.ResumeLayout(false);

    }

    private void FormSettings_Load(object sender, EventArgs e)
    {

    }
}
