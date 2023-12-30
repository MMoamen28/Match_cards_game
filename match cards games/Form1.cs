using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryMatchGame
{
    public partial class Form1 : Form
    {
        private ArrayList cards = new ArrayList();
        private Button firstCard;
        private Button secondCard;
        private int moves = 0;
        private int matches = 0;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private int seconds = 0;

        private int score = 0;
        private int timeLimit;
        private Stopwatch stopwatch = new Stopwatch();
        private DifficultyLevel difficultyLevel;
        private Label timeLabel;
        private Label scoreLabel;

        public Form1()
        {
            InitializeComponent();
            ShowDifficultySelection();
        }

        private void ShowDifficultySelection()
        {
            using (Form difficultyForm = new Form())
            {
                difficultyForm.Size = new System.Drawing.Size(300, 150);
                difficultyForm.BackColor = System.Drawing.Color.LightBlue;
                difficultyForm.StartPosition = FormStartPosition.CenterScreen;
                difficultyForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                difficultyForm.MaximizeBox = false;
                difficultyForm.MinimizeBox = false;
                difficultyForm.Text = "Select Difficulty";
                difficultyForm.ShowIcon = false;
                difficultyForm.ShowInTaskbar = false;

                Label label = new Label();
                label.Text = "Select the difficulty level:";
                label.Font = new System.Drawing.Font("Segoe UI", 12);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Dock = DockStyle.Top;
                label.ForeColor = System.Drawing.Color.Black;
                difficultyForm.Controls.Add(label);

                Button easyButton = new Button();
                easyButton.Text = "Easy";
                easyButton.Font = new System.Drawing.Font("Segoe UI", 12);
                easyButton.Dock = DockStyle.Left;
                easyButton.Click += (sender, e) =>
                {
                    difficultyLevel = DifficultyLevel.Easy;
                    timeLimit = 40;
                    difficultyForm.Close();
                    InitializeGame();
                };
                difficultyForm.Controls.Add(easyButton);

                Button mediumButton = new Button();
                mediumButton.Text = "Medium";
                mediumButton.Font = new System.Drawing.Font("Segoe UI", 12);
                mediumButton.Dock = DockStyle.Fill;
                mediumButton.Click += (sender, e) =>
                {
                    difficultyLevel = DifficultyLevel.Medium;
                    timeLimit = 30;
                    difficultyForm.Close();
                    InitializeGame();
                };
                difficultyForm.Controls.Add(mediumButton);

                Button hardButton = new Button();
                hardButton.Text = "Hard";
                hardButton.Font = new System.Drawing.Font("Segoe UI", 12);
                hardButton.Dock = DockStyle.Right;
                hardButton.Click += (sender, e) =>
                {
                    difficultyLevel = DifficultyLevel.Hard;
                    timeLimit = 20;
                    difficultyForm.Close();
                    InitializeGame();
                };
                difficultyForm.Controls.Add(hardButton);

                difficultyForm.ShowDialog();
            }
        }

        private void InitializeGame()
        {
            string[] emojis = { "🐱", "🐶", "🐰", "🦊", "🐻", "🐯", "🐸", "🦄" };

            for (int i = 0; i < emojis.Length; i++)
            {
                cards.Add(emojis[i]);
                cards.Add(emojis[i]);
            }

            ShuffleCards();

            timer.Interval = 1000;
            timer.Tick += Timer_Tick;

            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;

            if (difficultyLevel == DifficultyLevel.Medium || difficultyLevel == DifficultyLevel.Hard || difficultyLevel == DifficultyLevel.Easy)
            {
                stopwatch.Start();
                gameTimer.Start();
            }


            AddScoreLabel();


            AddTimeLabel();
        }

        private void AddScoreLabel()
        {
            scoreLabel = new Label();
            scoreLabel.Text = "Score: 0";
            scoreLabel.Font = new System.Drawing.Font("Segoe UI", 12);
            scoreLabel.Dock = DockStyle.Top;
            scoreLabel.ForeColor = System.Drawing.Color.Black;
            Controls.Add(scoreLabel);
        }

        private void AddTimeLabel()
        {
            timeLabel = new Label();
            timeLabel.Text = $"Time: {timeLimit} seconds";
            timeLabel.Font = new System.Drawing.Font("Segoe UI", 12);
            timeLabel.Dock = DockStyle.Bottom;
            timeLabel.ForeColor = System.Drawing.Color.Black;
            Controls.Add(timeLabel);
        }

        private void ShuffleCards()
        {
            Random rand = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                object value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }

            int row = 0, col = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                Button card = new Button();
                card.Size = new System.Drawing.Size(80, 80);
                card.Location = new System.Drawing.Point(col * 90, row * 90);
                card.Text = "?";
                card.Tag = cards[i];
                card.Font = new System.Drawing.Font("Segoe UI", 20);
                card.BackColor = System.Drawing.Color.White;
                card.Click += Card_Click;
                Controls.Add(card);

                col++;
                if (col == 4)
                {
                    col = 0;
                    row++;
                }
            }
        }

        private async void Card_Click(object sender, EventArgs e)
        {
            Button clickedCard = (Button)sender;
            string cardEmoji = (string)clickedCard.Tag;

            if (firstCard == null)
            {
                firstCard = clickedCard;
                firstCard.Text = cardEmoji;
            }
            else if (secondCard == null && clickedCard != firstCard)
            {
                secondCard = clickedCard;
                secondCard.Text = cardEmoji;

                moves++;

                if (firstCard.Text == secondCard.Text)
                {
                    firstCard.Enabled = false;
                    secondCard.Enabled = false;
                    matches++;
                    score += 100;


                    UpdateScoreLabel();

                    if (matches == cards.Count / 2)
                    {
                        gameTimer.Stop();
                        stopwatch.Stop();
                        ShowGameCompletionPopup();
                    }

                    firstCard = null;
                    secondCard = null;
                }
                else
                {
                    timer.Start();
                    await Task.Delay(500);
                    timer.Stop();

                    firstCard.Text = "?";
                    secondCard.Text = "?";
                    firstCard = null;
                    secondCard = null;


                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            firstCard.Text = "?";
            secondCard.Text = "?";
            firstCard = null;
            secondCard = null;
            timer.Stop();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimeLabel();

            if (timeLimit - (int)stopwatch.Elapsed.TotalSeconds <= 0)
            {
                gameTimer.Stop();
                stopwatch.Stop();
                ShowGameOverPopup();
            }
            else if (matches == cards.Count / 2)
            {
                gameTimer.Stop();
                stopwatch.Stop();
                ShowGameCompletionPopup();
            }
        }

        private void ShowGameOverPopup()
        {
            MessageBox.Show("Game Over! Time is out.", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);


            UpdateScoreLabel();
        }

        private void ShowGameCompletionPopup()
        {
            gameTimer.Stop();

            using (Form popupForm = new Form())
            {
                popupForm.Size = new System.Drawing.Size(400, 200);
                popupForm.BackColor = System.Drawing.Color.BlueViolet;
                popupForm.StartPosition = FormStartPosition.CenterParent;
                popupForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                popupForm.Text = "Congratulations!";


                Label label = new Label();
                label.Text = $"You completed the game in {moves} moves and {stopwatch.Elapsed.Seconds} seconds.";
                label.Font = new System.Drawing.Font("Segoe UI", 14);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Dock = DockStyle.Fill;
                label.ForeColor = System.Drawing.Color.White;

                popupForm.Controls.Add(label);

                Button okButton = new Button();
                okButton.Text = "OK";
                okButton.Font = new System.Drawing.Font("Segoe UI", 12);
                okButton.Dock = DockStyle.Bottom;
                okButton.BackColor = System.Drawing.Color.White;
                okButton.ForeColor = System.Drawing.Color.Black;
                okButton.Click += (sender, e) => popupForm.Close();

                popupForm.Controls.Add(okButton);

                popupForm.ShowDialog();
            }
        }

        private void UpdateTimeLabel()
        {
            int remainingSeconds = Math.Max(0, timeLimit - (int)stopwatch.Elapsed.TotalSeconds);
            timeLabel.Text = $"Time: {remainingSeconds} seconds";
        }

        private void UpdateScoreLabel()
        {
            scoreLabel.Text = $"Score: {score}";
        }

        private enum DifficultyLevel
        {
            Easy,
            Medium,
            Hard
        }
    }
}
