using System;                      // Allows to type method names of members of the System namespace without typing the word System every time
using System.Collections.Generic; // Contains interfaces and classes that define generic collections
using System.Data;               // Provides access to classes that represent the ADO.NET architecture.
using System.Drawing;           // Provides access to GDI+ basic graphics functionality
using System.IO;               // It contains types that allow reading and writing to files and data streams
using System.Linq;            // It provides classes and interfaces that support queries that use Language-Integrated Query (LINQ) 
using System.Threading;      // Creates and controls a thread, sets its priority, and gets its status
using System.Windows.Forms; // It contains classes for creating Windows-based applications that take full advantage of the rich user interface  


namespace Assignment5_Sudoku
{
    public partial class Form1 : Form
    {
        //Property Declaration 
        private static int[,] PausedArray = new int[9, 9];      // This array holds the paused puzzle 
        private static int[,] InitalArray = new int[9, 9];     // This array holds the puzzle which is loaded
        private static int[,] SavedArray = new int[9, 9];     // This list holds the textBoxes where the value is given for a new game
        private static int[,] SolutionArray = new int[9, 9]; // This array holds the puzzle which has the solution 
        private static List<string> PuzzlePathList = new List<string>(); // This would set the path where all puzzles are stored
        private static int CurrentTime;                 // Gets the current time
        private static bool IsPuzzleSaved = false;      // Intially the puzzle saved is set to false
        private static bool IsPuzzleCompleted = false; // Intially the puzzle saved is set to false
        private static List<int> PlayedTimings = new List<int>();
        private static String CurrentPuzzleFile = "";
        private static int TotalEmptyCellsCount;       // Keeps a count of the total empty cells
        private static int CurrentEmptyCellsCount;    // Keeps a count of the current empty cells
        private static int TotalIncorrectCellsCount; // Keeps a count of the total incorrect cells
        private static string RootIndexFilePath = @"..\\..\\PuzzlesInput\\directory.txt"; 
        

        //Default method of winform class to initalize all controls on screen
        public Form1()
        {
            InitializeComponent();
        }

        // The Reset Event re-starts the game with the original place 
        private void btnReset_Click(object sender, EventArgs e)
        {
            string tempCurrentPuzzle = CurrentPuzzleFile; // The tempcurrent puzzle has the current puzzle file in init witht the selected puzzle
            CurrentTime = 0;                             // The current time is set to zero
            puzzletimer.Stop();                         // The timer is stopped when reset is clicked
            lblTime.Text = "00:00:00";
            IsPuzzleSaved = false;                     // Ouzzle saved is set to false 
            IsPuzzleCompleted = false;                // Puzzle completed is set to false

            //Depending on the visibilty paramter button's on the form would be visible or invisible
            btnProgress.Enabled = false;
            btnHint.Enabled = false;
            btnReset.Enabled = false;
            btnSave.Enabled = false;
            btnPause.Enabled = false;

            lblTime.Text = "00:00:00";
            lbl_Status.Text = "";

            StartNewPuzzle_Click(sender,e);

            //set current file again 
            CurrentPuzzleFile = tempCurrentPuzzle;

        }
    
        // The events triggers the following methods when a new puzzle is started
        private void StartNewPuzzle_Click(object sender, EventArgs e)
        {
            StartTimer();                      // The timer is started when the puzzle begins
            SetButtonStatus();                // The buttons status are set based on the boolean value they were given
            GetAllPuzzleLocation(sender, e); // It reads the all the puzzles from the file
            ReadPuzzlesFromFile();          // Based on what level is selected it reads the puzzle in that level
            BindTextBox();                 // It binds the text box when they are to be displayed
        }

        // The following actions are taken when the start timer function is called
        private void StartTimer()
        {
            puzzletimer.Interval = 1000;     // The puzzle timer interval is set to 1000
            puzzletimer.Enabled = true;     // The timer is enabled when the puzzle starts
            puzzletimer.Start();           //  It starts the timer
        }

        // It sets the status of the buttons
        private void SetButtonStatus()
        {
            btnEasy.BackColor = SystemColors.Control;    // The easy button color is et using the SystemColors class which is a Color structure that is the color of a Windows display element.
            btnMedium.BackColor = SystemColors.Control; // The medium button color is et using the SystemColors class which is a Color structure that is the color of a Windows display element.
            btnHard.BackColor = SystemColors.Control;  // The hard button color is et using the SystemColors class which is a Color structure that is the color of a Windows display element.

            //Depending on the visibilty paramter button's on the form would be visible or invisible
            btnProgress.Enabled = true;
            btnHint.Enabled = true;
            btnReset.Enabled = true;
            btnSave.Enabled = true;
            btnPause.Enabled = true;

            lblTime.Text = "00:00:00";
            lbl_Status.Text = "";
        }

        // The function gets the location of all puzzles
        private void GetAllPuzzleLocation(object sender, EventArgs e)
        {
            PuzzlePathList.Clear();  // Intially the puzzle path is clear when no level is selected
            CurrentPuzzleFile = ""; // When a level is selcted it stores the path of that level in the current puzzle file
            String fileName = "";  // It stores the file name when the puzzle is selected
            using (StreamReader sr = new StreamReader("..\\..\\PuzzlesInput\\directory.txt"))
            {
                String[] values;
                String line = sr.ReadLine();

                Button diffButton = sender as Button;
                diffButton.BackColor = Color.DarkGray;

                while (line != null)
                {
                    PuzzlePathList.Add(line);
                    values = line.Split('|');
                    fileName = values[0];

                    if (fileName.Contains(diffButton.Text.ToLower()) && CurrentPuzzleFile == "")
                    {
                        CurrentTime = values.Length >= 2 ? Convert.ToInt32(values[1]) : 0; // Since i am appending current time at the end of each file path ,so I am reading from here.
                        CurrentPuzzleFile = fileName;
                        if (values.Length >= 3 && values[2] != "") PlayedTimings = values[2].Split(',').Select(Int32.Parse).ToList();

                        IsPuzzleSaved = false;
                        if (CurrentTime != 0)
                        {
                            IsPuzzleSaved = true;
                        }
                    }
                    line = sr.ReadLine();
                }
            }
        }

        //Read puzzle initial state , saved state & solution state in to arrays defined earlier from puzzle file provided by Professor 
        private void ReadPuzzlesFromFile()
        {
            // Here each puzzle file has 3 set of data (InitalArray, SolutionArray & then saved array. So reading them one by one  )
            if (CurrentPuzzleFile != "")
            {
                using (StreamReader sr1 = new StreamReader("..\\..\\PuzzlesInput\\" + CurrentPuzzleFile))
                {
                    String line = sr1.ReadLine();
                    while (line != null)
                    {
                        //Load data in to inital array
                        for (int i = 0; i < 9; i++)
                        {
                            string[] inp = line.Select(x => x.ToString()).ToArray();

                            for (int j = 0; j < inp.Length; j++)
                            {
                                InitalArray[i, j] = Convert.ToInt32(inp[j]);
                            }
                            line = sr1.ReadLine();
                        }

                        //Adding a new line and reading same in file just for easy readibility 
                        line = sr1.ReadLine();  
                        //Load data in to Solution array
                        for (int i = 0; i < 9; i++)
                        {
                            string[] inp = line.Select(x => x.ToString()).ToArray();

                            for (int j = 0; j < inp.Length; j++)
                            {
                                SolutionArray[i, j] = Convert.ToInt32(inp[j]);
                            }
                            line = sr1.ReadLine();
                        }


                        line = sr1.ReadLine(); 
                        if (line != null)
                        {
                            //Load data in to solution array 
                            for (int i = 0; i < 9; i++)
                            {
                                string[] inp = line.Select(x => x.ToString()).ToArray();

                                for (int j = 0; j < inp.Length; j++)
                                {
                                    SavedArray[i, j] = Convert.ToInt32(inp[j]);
                                }
                                line = sr1.ReadLine();
                            }
                        }
                        else
                        {
                            Array.Clear(SavedArray, 0, 81);
                        }
                    }
                }
            }
        }

        // This method binds values from Inital,Saved & Solution array to all textbox 
        private void BindTextBox()
        {
            // When a user selects a level it checks if a game is saved if not then it performs the following 
            if (!IsPuzzleSaved)
            {
                Array.Copy(InitalArray, SavedArray, InitalArray.Length);    // The array copies the intial,saved and the intial array length
                MessageBox.Show("Please wait!!!! We are loading new game"); // Displays the message to the user
            }
            // If a game is found to be saved then it loads the saved game
            else
            {
                MessageBox.Show("Please wait!!!! We are loading your last saved game");  // Displays the message to the user
            }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    TextBox txtBox = panel1.Controls.Find("textBox" + (i + 1).ToString() + (j + 1).ToString(), true).FirstOrDefault() as TextBox;
                    if (txtBox != null)
                    {
                        if (InitalArray[i, j] != 0)
                        {
                            txtBox.Text = InitalArray[i, j].ToString();
                            txtBox.Font = new Font(txtBox.Font, FontStyle.Bold);
                            txtBox.ReadOnly = true;
                        }
                        else if (SavedArray[i, j] != 0)
                        {
                            txtBox.Text = SavedArray[i, j].ToString();
                            txtBox.Font = new Font(txtBox.Font, FontStyle.Regular);
                            txtBox.ReadOnly = false;
                            TotalEmptyCellsCount += 1;
                        }
                        else
                        {
                            txtBox.Text = "";
                            txtBox.Font = new Font(txtBox.Font, FontStyle.Regular);
                            txtBox.ReadOnly = false;
                            TotalEmptyCellsCount += 1;
                        }
                    }
                }
            }
        }

        // TextBox click event handling method
        private void TextBox_Click(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.ForeColor = Color.Black;
            //allow only character between 1 - 9 in text box 
            if ((e.KeyChar >= '1' && e.KeyChar <= '9') || e.KeyChar == (char)Keys.Back)

            {
                    e.Handled = false; 
            }
            else
            {
                e.Handled = true; 
            }
        }

        // This method will be triggered when user clicks on Progress button and check user progress & show progress & result
        private void btnProgress_Click(object sender, EventArgs e)
        {
            CurrentEmptyCellsCount = 0;
            TotalIncorrectCellsCount = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    TextBox txtBox = panel1.Controls.Find("textBox" + (i + 1).ToString() + (j + 1).ToString(), true).FirstOrDefault() as TextBox;
                    if (txtBox.Text == "")
                    {
                        CurrentEmptyCellsCount += 1;
                    }
                    else if (SolutionArray[i, j].ToString() != txtBox.Text)
                    {
                        TotalIncorrectCellsCount += 1;
                        txtBox.ForeColor = Color.Red;
                    }
                }
            }
            // It checks if we have any incorrect answer if we have any it returns the cell count in red colour
            if (TotalIncorrectCellsCount != 0)
            {
                lbl_Status.Text = "You have " + TotalIncorrectCellsCount + " incorrect cells";
                lbl_Status.ForeColor = Color.Red;
            }
            // If there are any empty cells it displays a message saying the number of cells that are empty in green colour
            else if (CurrentEmptyCellsCount != 0)
            {
                lbl_Status.ForeColor = Color.Green;
                lbl_Status.Text = "You have " + CurrentEmptyCellsCount + " cells to go";
            }
            // If the complete puzzle it done it displays the average,fastest and our time in the message box
            else
            {
                IsPuzzleCompleted = true; // If the puzzle is completed then it is set to true
                puzzletimer.Stop();       // It stops the puzzle timer
                btnSave_Click(sender, e);
                var avg = PlayedTimings.Sum() / PlayedTimings.Count;  // Calulates the avrage time for the level
                var fastest = PlayedTimings.Min();  // Caluculates the fastest time for the current level
                MessageBox.Show("Your time is  " + TimeSpan.FromSeconds(CurrentTime) + "\nAvg time for this difficulity:    " + TimeSpan.FromSeconds(avg) + "\nFastest time for this difficulity:      " + TimeSpan.FromSeconds(fastest));
                FinishCurrentGame(sender, e);
            }
        }

        // Method to finish current game once user has filled all the fields and those are correct 
        private void FinishCurrentGame(object sender, EventArgs e)
        {
            var temp = CurrentPuzzleFile.Split('/');
            var currentGameLevel = string.Format("btn{0}",temp[0]);


            Button btn = (Button)this.Controls.Find(currentGameLevel,true).FirstOrDefault();
            btn.Enabled = false;

            CurrentTime = 0;     // The value of the current timer is set to 0
            puzzletimer.Stop(); // The puzzle timer is stopped when the puzzle is completed

            IsPuzzleSaved = false;
            IsPuzzleCompleted = false;

            SetButtonStatus();

            //Depending on the visibilty paramter button's on the form would be visible or invisible
            btnProgress.Enabled = false;
            btnHint.Enabled = false;
            btnReset.Enabled = false;
            btnSave.Enabled = false;
            btnPause.Enabled = false;

            ClearAllTextBoxes();  // It clears all the text boxes when the level is completed
            // If all the puzzles of the level is completed it displays the following message in the message box
            if (CheckIfAllGameFinished(temp[0]))
            {
                MessageBox.Show(string.Format("You have finished all {0} games , please select another level", temp[0]));
            }
            // If a paticular game is completed it asks the user to select another game
            else
            {
                MessageBox.Show(string.Format("You have finished current game , please select another game", temp[0]));
                btn.Enabled = true;
            }
            

        }

        // This event checks if all the games for given level(ex: easy, medium , hard) are finished.
        private bool CheckIfAllGameFinished(string level)
        {
            List<string> newLst = new List<string>();
            List<string> tempLst = new List<string>();


            PuzzlePathList.Clear();  // Intially the puzzle path is clear when no level is selected
            CurrentPuzzleFile = ""; // When a level is selcted it stores the path of that level in the current puzzle file
            String fileName = "";  // It stores the file name when the puzzle is selected

            using (StreamReader sr = new StreamReader("..\\..\\PuzzlesInput\\directory.txt"))
            {
                String[] values;
                String line = sr.ReadLine();

                //Button diffButton = sender as Button;
                //diffButton.BackColor = Color.DarkGray;

                while (line != null)
                {
                    newLst.Add(line);
                    values = line.Split('|');
                    fileName = values[0];

                    if (fileName.Contains(level) && values.Count() > 1)
                    {
                        tempLst.Add(line);
                    }
                    line = sr.ReadLine();
                }
            }


          
            bool IfAllGameFinished = false;
            if(tempLst.Count == 4)
            {
                IfAllGameFinished = true;
            }
            return IfAllGameFinished;
        }

        //Set all text box to readonly . Called after user finishes thier game.
        private void ClearAllTextBoxes()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    TextBox txtBox = panel1.Controls.Find("textBox" + (i + 1).ToString() + (j + 1).ToString(), true).FirstOrDefault() as TextBox;
                    if (txtBox != null)
                    {
                        txtBox.ReadOnly = true;
                       
                    }
                }
            }
        }

        // It checks if all the cells of the array are filled if it is filled it returns true else it returns false
        private bool IsPuzzleFilled()
        {
            List<string> filledTB = new List<string>();
            for (int m = 0; m < 9; m++)
            {
                for (int n = 0; n < 9; n++)
                {
                    TextBox txtBox = panel1.Controls.Find("textBox" + (m + 1).ToString() + (n + 1).ToString(), true).FirstOrDefault() as TextBox;
                    if(txtBox != null)
                    {
                       if(txtBox.Text != "")
                        {
                            filledTB.Add(txtBox.Text);
                        }
                    }
                }
            }
            if (filledTB.Count == 81)
                return true;
            else
                return false;
        }

        //This method gets called when user clicks on hint  
        //This method looks generate hints in two ways
        //If all of the boxes are filled and user has filled wrong value then hint will be shown in sequence row to columns
        //If all of the boxes are not filled then a random empty box will be shown and hint will be shown and game will be saved
        private void btnHint_Click(object sender, EventArgs e)
        {
            int numberofEmptyTb = 0;
            //GetCurrenttextbox values 
            for (int m = 0; m < 9; m++)
            {
                for (int n = 0; n < 9; n++)
                {
                    TextBox txtBox = panel1.Controls.Find("textBox" + (m + 1).ToString() + (n + 1).ToString(), true).FirstOrDefault() as TextBox;
                    if(txtBox.Text == "0" || txtBox.Text.Trim() == string.Empty )
                    {
                        numberofEmptyTb++;
                    }
                }
            }


            //Stop timer , since we dont want to record time for hint action
            puzzletimer.Stop();
            var puzzleFullFiled = IsPuzzleFilled();
            

            Random rJ = new Random();

            //List all textbox having 0 
            //assign 
            if (puzzleFullFiled)
            {
                //fill hint sequentially when puzzle is already filled 
                for (int m = 0; m < 9; m++)
                {
                    for (int n = 0; n < 9; n++)
                    {
                        if (SavedArray[m, n] != SolutionArray[m, n])
                        {
                            TextBox txtBox = panel1.Controls.Find("textBox" + (m + 1).ToString() + (n + 1).ToString(), true).FirstOrDefault() as TextBox;
                            if (txtBox != null)
                            {
                                txtBox.Text = SolutionArray[m, n].ToString();
                                txtBox.Font = new Font(txtBox.Font, FontStyle.Bold);
                                txtBox.ForeColor = Color.Green;
                                txtBox.ReadOnly = false;
                                btnSave_Click(sender, e);
                                puzzleFullFiled = IsPuzzleFilled();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                //find all textbox which have no values 
                List<string> zeroArr = new List<string>();
                for (int m = 0; m < 9; m++)
                {
                    for (int n = 0; n < 9; n++)
                    {
                        if (SavedArray[m, n] == 0)
                        {
                            zeroArr.Add(string.Format("{0},{1}", m, n));
                        }
                    }
                }

                if(zeroArr.Count() != numberofEmptyTb)
                {
                    lbl_Status.Text = "Please save your game to check the hint";
                    lbl_Status.ForeColor = Color.DarkRed;
                }

                //find random cells where to fill to fill hint
                int x = rJ.Next(0, zeroArr.Count);
                if (zeroArr.Count > 0)
                {
                    var tempTb = zeroArr[x].Split(',');
                    TextBox txtBox = panel1.Controls.Find("textBox" + (Convert.ToInt32(tempTb[0]) + 1).ToString() + (Convert.ToInt32(tempTb[1]) + 1).ToString(), true).FirstOrDefault() as TextBox;
                    if (txtBox != null)
                    {
                        txtBox.Text = SolutionArray[Convert.ToInt32(tempTb[0]), Convert.ToInt32(tempTb[1])].ToString();
                        txtBox.Font = new Font(txtBox.Font, FontStyle.Bold);
                        txtBox.ForeColor = Color.Green;
                        txtBox.ReadOnly = false;
                        btnSave_Click(sender, e);
                    }
                }
            }
        }

        // This event tells the actions that are performed when the pause button is clicked
        private void btnPause_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            // If the pause button is clicked the following actions take place
            if(btn.Text == "Pause")
            {
                
                puzzletimer.Stop();   // The puzzle timer is stoped when the pause is selcetd
                btn.Text = "Resume"; // In place of the pause button we now see the resume button displayed
                //Depending on the visibilty paramter button's on the form would be visible or invisible
                btnProgress.Enabled = false;
                btnHint.Enabled = false;
                btnReset.Enabled = false;
                btnSave.Enabled = false;

                btnEasy.Enabled = false;
                btnMedium.Enabled = false;
                btnHard.Enabled = false;

                HideTextBoxContents(false);  // The text box contets are not hidden when the pause button is not clicked
            }
            else
            {
         
                puzzletimer.Start();  // If the pause button is not selected the puzzle tier continues
                btn.Text = "Pause";   // The button is displayed as pause
                //Depending on the visibilty paramter button's on the form would be visible or invisible
                btnProgress.Enabled = true;
                btnHint.Enabled = true;
                btnReset.Enabled = true;
                btnSave.Enabled = true;
                btnPause.Enabled = true;

                btnEasy.Enabled = true;
                btnMedium.Enabled = true;
                btnHard.Enabled = true;

                HideTextBoxContents(true);     // The text box contets are  hidden when the pause button is  clicked

            }

        }

        // This hide/show the contents of the text box when the pause button is clicked
        private void HideTextBoxContents(bool isShow)
        {
            if (!isShow)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        TextBox txtBox = panel1.Controls.Find("textBox" + (i + 1).ToString() + (j + 1).ToString(), true).FirstOrDefault() as TextBox;
                        if (txtBox != null)
                        {
                            PausedArray[i, j] = txtBox.Text != "" ? Convert.ToInt32(txtBox.Text) : 0;
                            txtBox.Text = "X";  // All the text box are filled with x
                            txtBox.Font = new Font(txtBox.Font, FontStyle.Bold);
                            txtBox.ReadOnly = true;
                            if (InitalArray[i, j] != 0)
                            {
                               txtBox.ReadOnly = true;
                            }
                        }
                    }
                }
            }
            else
            {
                //restore text box content before pause button was clicked 
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        TextBox txtBox = panel1.Controls.Find("textBox" + (i + 1).ToString() + (j + 1).ToString(), true).FirstOrDefault() as TextBox;
                        if (txtBox != null)
                        {
                            if (PausedArray[i, j] != 0)
                            {
                                txtBox.Text = PausedArray[i, j].ToString();
                            }
                            else
                            {
                                txtBox.Text = "";
                            }
                            txtBox.Font = new Font(txtBox.Font, FontStyle.Bold);
                            if (InitalArray[i, j] != 0)
                            {
                                txtBox.ReadOnly = true;
                            }
                            else
                            {
                                txtBox.ReadOnly = false;
                                
                            }
                        }
                    }
                }
            }
        }

        // The event performs the following events when the save button is clicked
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Go through all of the text box and save information to saved array
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    TextBox txtBox = panel1.Controls.Find("textBox" + (i + 1).ToString() + (j + 1).ToString(), true).FirstOrDefault() as TextBox;
                    SavedArray[i, j] = txtBox.Text != "" ? Convert.ToInt32(txtBox.Text) : 0;
                }
            }

            //Record current puzzle state in to all of the arrays in to puzzle file 
            using (StreamWriter strWriteLine = new StreamWriter("..\\..\\PuzzlesInput\\" + CurrentPuzzleFile))
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        strWriteLine.Write(InitalArray[i, j]);
                    }
                    strWriteLine.WriteLine();
                }
                strWriteLine.WriteLine();
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        strWriteLine.Write(SolutionArray[i, j]);
                    }
                    strWriteLine.WriteLine();
                }
                if (!IsPuzzleCompleted)
                {
                    strWriteLine.WriteLine();
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            strWriteLine.Write(SavedArray[i, j]);
                        }
                        strWriteLine.WriteLine();
                    }
                }
            }

            //Record puzzle status , all times, current time in directory file (this info is used for calculating avg time and checking what all games has been completed)
            using (StreamWriter strWriteLine = new StreamWriter(RootIndexFilePath))
            {
                var puzzlePath = PuzzlePathList.FirstOrDefault(x => x.Contains(CurrentPuzzleFile));
                PuzzlePathList.Remove(puzzlePath);

                if (IsPuzzleCompleted)
                {
                    PlayedTimings.Add(CurrentTime);
                    PuzzlePathList.Add(CurrentPuzzleFile + "|0|" + String.Join(",", PlayedTimings.ToArray()));
                }
                else
                {
                    PuzzlePathList.Insert(0, CurrentPuzzleFile + "|" + CurrentTime + "|" + String.Join(",", PlayedTimings.ToArray()));
                }
                for (int i = 0; i < PuzzlePathList.Count; i++)
                {
                    strWriteLine.WriteLine(PuzzlePathList[i]);
                }
            }

                lbl_Status.Text = "Your progress has been saved";  // When all the above conditions are checked it saves our progress
                lbl_Status.ForeColor = Color.Green;               // The text appers in green colour at the bottom of the puzzle
            
        }


        //Method to handle puzzlertimer tick
        private void puzzletimer_Tick(object sender, EventArgs e)
        {
            CurrentTime += 1;
            //show real time timer to label
            lblTime.Text = TimeSpan.FromSeconds(CurrentTime).ToString(@"hh\:mm\:ss");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

