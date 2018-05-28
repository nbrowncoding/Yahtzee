using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
// ReSharper disable All

namespace Yahtzee
{
    public partial class main_form : Form
    {
        // Initial random generator
        readonly Random rnd = new Random();
        readonly object syncLock = new object();

        // Create an array of dicec containers
        private PictureBox[] diceContainer;

        // Create dice array
        Dice[] dice = new Dice[5];

        // Initial roll counter
        private int rollCount = 0;

        // Total Score
        private int totalScore = 0;

        // Yahtzee Count
        private int numOfYahtzee = 0;

        public main_form()
        {
            InitializeComponent();

            // Create each dice with no value (0 value)
            for (int i = 0; i < 5; i++)
            {
                dice[i] = new Dice();
                dice[i].setValue(0);
                dice[i].setState(Dice.UN_HELD);
            }

            // Create each dice container
            diceContainer = new PictureBox[]{ diceBox0, diceBox1, diceBox2, diceBox3, diceBox4 };

            // Set dice images to null
            for (int i = 0; i < 5; i++)
            {
                setDiceImage(diceContainer[i], dice[i].getValue());
            }
        }

        // FUNCTION CONTROLLERS \\

        /**
         * Sets the image of the correspoding box based on the value of the dice
         */
        void setDiceImage(PictureBox diceContainer, int value)
        {
            // Set image in dice 
            switch (value)
            {
                case 1:
                    diceContainer.Image = Properties.Resources.dice_1;
                    break;

                case 2:
                    diceContainer.Image = Properties.Resources.dice_2;
                    break;

                case 3:
                    diceContainer.Image = Properties.Resources.dice_3;
                    break;

                case 4:
                    diceContainer.Image = Properties.Resources.dice_4;
                    break;

                case 5:
                    diceContainer.Image = Properties.Resources.dice_5;
                    break;

                case 6:
                    diceContainer.Image = Properties.Resources.dice_6;
                    break;

                default:
                    diceContainer.Image = null;
                    break;
            }
        }

        /**
         * Rolls each dice that is not held
         */
        private int rollDice()
        {
            // Generate random number between 1 and 6
            lock (syncLock)
            {
                // synchronize
                return rnd.Next(1, 7);
            }
        }

        /**
         * Toggle hold/unhold dice state
         */
        private String toggleHold(Dice dice)
        {
            // Ensure dice has value before holding
            if (dice.getValue() != 0)
            {
                if (dice.getState() == Dice.UN_HELD)
                {
                    // Hold Dice
                    dice.setState(Dice.HELD);
                    return "DICE_HELD";
                }
                if (dice.getState() == Dice.HELD)
                {
                    // Un Hold Dice
                    dice.setState(Dice.UN_HELD);
                    return "DICE_UNHELD";
                }
                else
                {
                    return "INVALID_DICE_STATE";
                }
            }
            else
            {
                // Return if dice has no value (value == 0)
                return "NO_VALUE";
            }
        }

        /**\
         * Calculate Total Score
         */
        void calcTotalScore()
        {
            int upperScore = 0;
            int lowerScore = 0;
            int bonusScore = 0;

            // Get Upper Score
            upperScore += getScore(aceScore);
            upperScore += getScore(twoScore);
            upperScore += getScore(threeScore);
            upperScore += getScore(fourScore);
            upperScore += getScore(fiveScore);
            upperScore += getScore(sixScore);

            // Display Upper Score
            upperScoreLabel.Text = upperScore.ToString();

            // Get Bonus Score
            if (upperScore >= 63)
            {
                bonusScore = 35;
            }

            // Display Bonus Score
            bonusScoreLabel.Text = bonusScore.ToString();

            // Get Lower Score
            lowerScore += getScore(threeKindScore);
            lowerScore += getScore(fourKindScore);
            lowerScore += getScore(fullHouseScore);
            lowerScore += getScore(smStraightScore);
            lowerScore += getScore(lgStraightScore);
            lowerScore += getScore(chanceScore);
            lowerScore += getScore(yahtzeeScore);

            // Display Bonus Score
            lowerScoreLabel.Text = lowerScore.ToString();

            // Calcualte Total Score
            totalScore = upperScore + bonusScore + lowerScore;

            // Display Total Score
            totalScoreLabel.Text = totalScore.ToString();

        }

        /**
         * Returns the integer score from a label
         */
        int getScore(Label scoreLabel)
        {
            try
            {
                // Return score if able to parse
                return Int32.Parse(scoreLabel.Text);
            }
            catch (Exception e)
            {
                /* If not able to parse, such as in the case
                 * of a field strike, return 0
                 */
                return 0;
            }
        }

        /**
         * Calculates and returns the score for a give field
         */
        int calcFaceScore(int faceValue)
        {
            // Create Array of Scores
            int[] scoreArray = new int[5];

            for (int i = 0; i < 5; i++)
            {
                scoreArray[i] = dice[i].getValue();
            }

            // Count number of dice with face value
            int numOfDice = countDice(scoreArray, faceValue);

            if (numOfDice == 0)
            {
                return 0;
            }

            return numOfDice*faceValue;
        }

        /**
         * Counts the occures of dice face value in score array
         */
        int countDice(Array array, int faceValue)
        {
            // Store the dice count
            int count = 0;

            // Searchs the array for the face value records the frequency 
            foreach (int i in array)
            {
                if (i == faceValue) count++;
            }

            return count;
        }

        // Strike field
        void stikeField(Label field)
        {
            field.Text = "X";
        }

        // Rest Turn
        void resetTurn()
        {
            // Reset Dice and Containers
            for (int i = 0; i < 5; i++)
            {
                dice[i].setValue(0);
                dice[i].setState(Dice.UN_HELD);
                diceContainer[i].BackColor = default(Color);
                setDiceImage(diceContainer[i], 0);
            }

            // Enabel Roll Button
            rollButton.Enabled = true;

            // Reset Roll Count
            rollCount = 0;

            // Reset hold buttons
            holdButton0.Text = "Hold";
            holdButton1.Text = "Hold";
            holdButton2.Text = "Hold";
            holdButton3.Text = "Hold";
            holdButton4.Text = "Hold";
        }

        // LISTINER CONTROLLERS \\

        private void rollButton_Click(object sender, System.EventArgs e)
        {
            // Increment roll counter
            rollCount++;

            if (rollCount == 3)
            {
                // Disable button when roll is 3
                ((Button) sender).Enabled = false;
            }
            // Roll the Dice
            for (int i = 0; i < 5; i++)
            {
                // Check Dice State
                if (dice[i].getState() == Dice.UN_HELD)
                {
                    // If dice is not held, roll dice
                    dice[i].setValue(rollDice());
                    setDiceImage(diceContainer[i], dice[i].getValue());
                }
                // Other wise keep dice value
            }

        }

        private void holdButton0_Click(object sender, EventArgs e)
        {
            String toggleHoldMsg = toggleHold(dice[0]);

            // Toggle Button Text
            if (toggleHoldMsg.Equals("DICE_UNHELD"))
            {
                ((Button)sender).Text = "Hold";
                diceContainer[0].BackColor = default(Color);
            }
            else if (toggleHoldMsg.Equals("DICE_HELD"))
            {
                ((Button)sender).Text = "Un hold";
                diceContainer[0].BackColor = Color.DodgerBlue;
            }
            else
            {
                MessageBox.Show(this, "You must roll at least once before holding dice.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void holdButton1_Click(object sender, EventArgs e)
        {
            String toggleHoldMsg = toggleHold(dice[1]);

            // Toggle Button Text
            if (toggleHoldMsg.Equals("DICE_UNHELD"))
            {
                ((Button)sender).Text = "Hold";
                diceContainer[1].BackColor = default(Color);
            }
            else if (toggleHoldMsg.Equals("DICE_HELD"))
            {
                ((Button)sender).Text = "Un hold";
                diceContainer[1].BackColor = Color.DodgerBlue;
            }
            else
            {
                MessageBox.Show(this, "You must roll at least once before holding dice.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void holdButton2_Click(object sender, EventArgs e)
        {
            String toggleHoldMsg = toggleHold(dice[2]);

            // Toggle Button Text
            if (toggleHoldMsg.Equals("DICE_UNHELD"))
            {
                ((Button)sender).Text = "Hold";
                diceContainer[2].BackColor = default(Color);
            }
            else if (toggleHoldMsg.Equals("DICE_HELD"))
            {
                ((Button)sender).Text = "Un hold";
                diceContainer[2].BackColor = Color.DodgerBlue;
            }
            else
            {
                MessageBox.Show(this, "You must roll at least once before holding dice.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void holdButton3_Click(object sender, EventArgs e)
        {
            String toggleHoldMsg = toggleHold(dice[3]);

            // Toggle Button Text
            if (toggleHoldMsg.Equals("DICE_UNHELD"))
            {
                ((Button)sender).Text = "Hold";
                diceContainer[3].BackColor = default(Color);
            }
            else if (toggleHoldMsg.Equals("DICE_HELD"))
            {
                ((Button)sender).Text = "Un hold";
                diceContainer[3].BackColor = Color.DodgerBlue;
            }
            else
            {
                MessageBox.Show(this, "You must roll at least once before holding dice.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void holdButton4_Click(object sender, EventArgs e)
        {
            String toggleHoldMsg = toggleHold(dice[4]);

            // Toggle Button Text
            if (toggleHoldMsg.Equals("DICE_UNHELD"))
            {
                ((Button)sender).Text = "Hold";
                diceContainer[4].BackColor = default(Color);
            }
            else if (toggleHoldMsg.Equals("DICE_HELD"))
            {
                ((Button)sender).Text = "Un hold";
                diceContainer[4].BackColor = Color.DodgerBlue;
            }
            else
            {
                MessageBox.Show(this, "You must roll at least once before holding dice.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void aceScore_Click(object sender, EventArgs e)
        {
            // Check to see if field is already scored
            if (aceScore.Text.Equals(""))
            {
                // Get score for Aces
                int score = calcFaceScore(1);

                // Ask to strike if no dice found
                if (score == 0)
                {
                    if (MessageBox.Show(this, "No Aces were found, do you wish to strike field?", "No Aces found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
                else
                {
                    // Display Score
                    aceScore.Text = score.ToString();

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
            }
        }

        private void twoScore_Click(object sender, EventArgs e)
        {
            // Check to see if field is already scored
            if (twoScore.Text.Equals(""))
            {
                // Get score for Two
                int score = calcFaceScore(2);

                // Ask to strike if no dice found
                if (score == 0)
                {
                    if (MessageBox.Show(this, "No Two's were found, do you wish to strike field?", "No Two's found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
                else
                {
                    // Display Score
                    twoScore.Text = score.ToString();

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
            }
        }

        private void threeScore_Click(object sender, EventArgs e)
        {
            // Check to see if field is already scored
            if (threeScore.Text.Equals(""))
            {
                // Get score for Three
                int score = calcFaceScore(3);

                // Ask to strike if no dice found
                if (score == 0)
                {
                    if (MessageBox.Show(this, "No Three's were found, do you wish to strike field?", "No Three's found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
                else
                {
                    // Display Score
                    threeScore.Text = score.ToString();

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
            }
        }

        private void fourScore_Click(object sender, EventArgs e)
        {
            // Check to see if field is already scored
            if (fourScore.Text.Equals(""))
            {
                // Get score for Four
                int score = calcFaceScore(4);

                // Ask to strike if no dice found
                if (score == 0)
                {
                    if (MessageBox.Show(this, "No Four's were found, do you wish to strike field?", "No Four's found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
                else
                {
                    // Display Score
                    fourScore.Text = score.ToString();

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
            }
        }

        private void fiveScore_Click(object sender, EventArgs e)
        {
            // Check to see if field is already scored
            if (fiveScore.Text.Equals(""))
            {
                // Get score for Aces
                int score = calcFaceScore(5);

                // Ask to strike if no dice found
                if (score == 0)
                {
                    if (MessageBox.Show(this, "No Five's were found, do you wish to strike field?", "No Five's found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
                else
                {
                    // Display Score
                    fiveScore.Text = score.ToString();

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
            }
        }

        private void sixScore_Click(object sender, EventArgs e)
        {
            // Check to see if field is already scored
            if (sixScore.Text.Equals(""))
            {
                // Get score for Six
                int score = calcFaceScore(6);

                // Ask to strike if no dice found
                if (score == 0)
                {
                    if (MessageBox.Show(this, "No Six's were found, do you wish to strike field?", "No Six's found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
                else
                {
                    // Display Score
                    sixScore.Text = score.ToString();

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
            }
        }

        private void chanceScore_Click(object sender, EventArgs e)
        {
            // Check to see if field is already scored
            if (chanceScore.Text.Equals(""))
            {
                // Total of dice for scoring
                int diceTotal = 0;

                // Create Array of Scores
                int[] scoreArray = new int[5];

                for (int i = 0; i < 5; i++)
                {
                    scoreArray[i] = dice[i].getValue();
                    diceTotal += dice[i].getValue();
                }

                chanceScore.Text = diceTotal.ToString();

                calcTotalScore();
                resetTurn();
            }
        }

        private void lgStraightScore_Click(object sender, EventArgs e)
        {

            // Check to see if field is already scored
            if (lgStraightScore.Text.Equals(""))
            {
                // Create Array of Scores
                int[] scoreArray = new int[5];

                for (int i = 0; i < 5; i++)
                {
                    scoreArray[i] = dice[i].getValue();
                }

                // Sort Array of Scores
                Array.Sort(scoreArray);

                // LG Straight Combination 1
                int[] comb1 = new[] { 1, 2, 3, 4, 5 };

                // LG Straight Combination 2
                int[] comb2 = new[] { 2, 3, 4, 5, 6 };

                // Compare Score Array to each Combination
                if (scoreArray.SequenceEqual(comb1))
                {
                    lgStraightScore.Text = "40";

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
                else if (scoreArray.SequenceEqual(comb2))
                {
                    lgStraightScore.Text = "40";

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
                else
                {
                    if (MessageBox.Show(this, "Large Straight not found, do you wish to strike field?", "No Large Straight found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
            }
        }

        private void smStraightScore_Click(object sender, EventArgs e)
        {

            /**
             * Possible Small Straight Combinations
             * [1,2,3,4]
             * [2,3,4,5]
             * [3,4,5,6]
             *
             * Score array must contain one of the combinations above
             * to be a small straight
             */
            
            // Check to see if field is already scored
            if (smStraightScore.Text.Equals(""))
            {
                // Create Array of Scores
                int[] scoreArray = new int[5];

                for (int i = 0; i < 5; i++)
                {
                    scoreArray[i] = dice[i].getValue();
                }

                List<int> scoreList = scoreArray.ToList();

                // Check to see if array has 1, 2, 3 and 4
                if (scoreList.Contains(1) && scoreList.Contains(2) && scoreList.Contains(3) && scoreList.Contains(4))
                {
                    smStraightScore.Text = "30";

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
                // Check to see if array has 2, 3, 4, 5
                else if (scoreList.Contains(2) && scoreList.Contains(3) && scoreList.Contains(4) && scoreList.Contains(5))
                {
                    smStraightScore.Text = "30";

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
                // Check to see if array has 3, 4, 5, 6
                else if (scoreList.Contains(3) && scoreList.Contains(4) && scoreList.Contains(5) && scoreList.Contains(6))
                {
                    smStraightScore.Text = "30";

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
                // Else it cannot be a small straight
                else
                {
                    if (MessageBox.Show(this, "Small Straight not found, do you wish to strike field?", "No Small Straight found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
            }
        }

        private void fullHouseScore_Click(object sender, EventArgs e)
        {
            // Check to see if field is already scored
            if (fullHouseScore.Text.Equals(""))
            {
                // Create Array of Scores
                int[] scoreArray = new int[5];

                for (int i = 0; i < 5; i++)
                {
                    scoreArray[i] = dice[i].getValue();
                }

                // Sort Score Array
                Array.Sort(scoreArray);

                // CHeck for full house
                if (scoreArray[0] == scoreArray[1] && scoreArray[2] == scoreArray[3] && scoreArray[3] == scoreArray[4])
                {
                    fullHouseScore.Text = "25";

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                    ;               }
                else if (scoreArray[0] == scoreArray[1] && scoreArray[1] == scoreArray[2] &&
                         scoreArray[3] == scoreArray[4])
                {
                    fullHouseScore.Text = "25";

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
                else
                {
                    // No Full House
                    if (MessageBox.Show(this, "Fulle House not found, do you wish to strike field?", "No Full House found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
            }
        }

        private void fourKindScore_Click(object sender, EventArgs e)
        {
            // Check to see if field is already scored
            if (fourKindScore.Text.Equals(""))
            {
                // Check for 4 of a kind
                bool fourKind = false;

                // Total of dice for scoring
                int diceTotal = 0;

                // Create Array of Scores
                int[] scoreArray = new int[5];

                for (int i = 0; i < 5; i++)
                {
                    scoreArray[i] = dice[i].getValue();
                    diceTotal += dice[i].getValue();
                }

                // Check to see if face value occurs at least four times
                for (int i = 1; i <= 6; i++)
                {
                    if (countDice(scoreArray, i) >= 4)
                    {
                        fourKind = true;
                        break;
                    }
                }

                // Ask to strike if no dice found
                if (!fourKind)
                {
                    if (MessageBox.Show(this, "4 of a Kind not found, do you wish to strike field?", "No 4 of a Kind found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
                else
                {
                    // Display Score
                    fourKindScore.Text = diceTotal.ToString();

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
            }
        }

        private void threeKindScore_Click(object sender, EventArgs e)
        {
            // Check to see if field is already scored
            if (threeKindScore.Text.Equals(""))
            {
                // Check for 3 of a kind
                bool threeKind = false;

                // Total of dice for scoring
                int diceTotal = 0;

                // Create Array of Scores
                int[] scoreArray = new int[5];

                for (int i = 0; i < 5; i++)
                {
                    scoreArray[i] = dice[i].getValue();
                    diceTotal += dice[i].getValue();
                }

                // Check to see if face value occurs at least three times
                for (int i = 1; i<=6; i++)
                {
                    if(countDice(scoreArray, i)>=3)
                    {
                        threeKind = true;
                        break;
                    }
                }

                // Ask to strike if no dice found
                if (!threeKind)
                {
                    if (MessageBox.Show(this, "3 of a Kind not found, do you wish to strike field?", "No 3 of a Kind found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
                else
                {
                    // Display Score
                    threeKindScore.Text = diceTotal.ToString();

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
            }
        }

        private void yahtzeeScore_Click(object sender, EventArgs e)
        {
            // Check to see if field is stricked
            if (!yahtzeeScore.Text.Equals("X"))
            {
                // Create Array of Scores
                int[] scoreArray = new int[5];

                for (int i = 0; i < 5; i++)
                {
                    scoreArray[i] = dice[i].getValue();
                }

                // Check to see if all the dice are the same
                if (scoreArray.Distinct().Count() == 1)
                {
                    // Incremnt Yahtzee Counter
                    numOfYahtzee++;

                    if (numOfYahtzee == 1)
                    {
                        yahtzeeScore.Text = "50";
                    }
                    else if (numOfYahtzee > 1)
                    {
                        // Increment Yahtzee Score
                        int yahtzeeCurrent = Int32.Parse(yahtzeeScore.Text);
                        yahtzeeCurrent += 100;
                        yahtzeeScore.Text = yahtzeeCurrent.ToString();

                        // Mark off bonus yahtzee field
                        if (yahtzeeBonus1.Text.Equals(""))
                        {
                            yahtzeeBonus1.Text = "X";
                        }
                        else if (yahtzeeBonus2.Text.Equals(""))
                        {
                            yahtzeeBonus2.Text = "X";
                        }
                        else if (yahtzeeBonus3.Text.Equals(""))
                        {
                            yahtzeeBonus3.Text = "X";
                        }
                        else if (yahtzeeBonus4.Text.Equals(""))
                        {
                            yahtzeeBonus4.Text = "X";
                        }
                        // If all fields are marked, just keep track of score and number of yahtzee
                    }

                    // Calculate Score and Reset
                    calcTotalScore();
                    resetTurn();
                }
                else
                {
                    if (MessageBox.Show(this, "Not a Yahtzee, do you wish to strike field?", "No Yahtzee found!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        stikeField((Label)sender);
                        resetTurn();
                    }
                }
            }
        }
    }

    class Dice
    {
        // Dice State
        public const int UN_HELD = 0;
        public const int HELD = 1;

        // Holds the state of the dice
        private int state;

        // Holds the value of the dice
        private int value;

        public void setValue(int value)
        {
            this.value = value;
        }

        public int getValue()
        {
            return value;
        }

        public void setState(int state)
        {
            this.state = state;
        }

        public int getState()
        {
            return state;
        }
    }
}
