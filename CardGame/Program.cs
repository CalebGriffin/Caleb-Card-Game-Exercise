using System;
using System.Runtime.CompilerServices;

namespace CardGame
{
    public class Program
    {
        public string UserInput = "";

        public string[] SplitUserInput = new string[0];
        
        private int _score;
        public int Score => _score;

        // Used to calculate the value of face cards
        private Dictionary<string, int> _faceCardValues = new Dictionary<string, int>()
        {
            {"T", 10},
            {"J", 11},
            {"Q", 12},
            {"K", 13},
            {"A", 14},
        };

        // Used to calculate the multiplier for each suit
        private Dictionary<string, int> _suitMultiplierValues = new Dictionary<string, int>()
        {
            {"C", 1},
            {"D", 2},
            {"H", 3},
            {"S", 4},
        };

        private int _noOfJokers = 0;
        public int NoOfJokers => _noOfJokers;

        // Messages to be displayed to the user
        public string WelcomeMessage => "Welcome to the Card Game!";
        public string Prompt => "Please enter a comma-separated list of cards (e.g. AC,4S): ";
        public string InvalidInputFormat => "ERROR: Invalid input format";
        public string InvalidCard => "ERROR: Invalid card found";
        public string TooManyJokers => "ERROR: A hand cannot contain more than 2 jokers";
        public string DuplicateCards => "ERROR: Cards cannot be duplicated";
        public string ScoreMessage => $"Your score is: {Score}";
        public string PlayAgainMessage => "Would you like to play again? (Y/N)";
        public string ThanksForPlaying => "Thanks for playing!";

        public string ResultMessage = ""; // Used for unit testing

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        /// <summary>
        /// Runs the program
        /// </summary>
        public void Run()
        {
            Console.Clear();
            Reset();

            // Get user input
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(WelcomeMessage);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Prompt);
            UserInput = Console.ReadLine() ?? "";

            // Change the text colour to red to display any error messages
            Console.ForegroundColor = ConsoleColor.Red;

            // Run each verification method and display the appropriate message
            if (!VerifyInputFormat())
            {
                ResultMessage = InvalidInputFormat;
                Console.WriteLine(InvalidInputFormat);
            }
            else if (!VerifyAllCards())
            {
                ResultMessage = InvalidCard;
                Console.WriteLine(InvalidCard);
            }
            else if (CheckForDuplicates().found)
            {
                ResultMessage = CheckForDuplicates().message;
                Console.WriteLine(CheckForDuplicates().message);
            }
            else
            {
                CalculateScore();

                ResultMessage = ScoreMessage;

                // Change the text colour to green to display the score
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(ScoreMessage);
            }
        }

        /// <summary>
        /// Verifies that the user input is in the correct format
        /// </summary>
        /// <returns>A boolean value of whether the input is formatted correctly</returns>
        public bool VerifyInputFormat()
        {
            if (UserInput == null || UserInput == "")
            {
                return false;
            }

            if (UserInput != UserInput.ToUpper())
            {
                return false;
            }

            // If the input contains any non-alphanumeric characters, except for commas, return false
            foreach (char c in UserInput)
            {
                if (!char.IsLetterOrDigit(c) && c != ',')
                {
                    return false;
                }
            }

            SplitUserInput = UserInput.Split(',');

            // If any of the values in the array are longer or shorter than 2 characters, return false
            foreach (string s in SplitUserInput)
            {
                if (s.Length != 2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifies that all cards in the user input are valid
        /// </summary>
        /// <returns>True if all the cards are valid or False if any of the cards are invalid</returns>
        public bool VerifyAllCards()
        {
            foreach (string s in SplitUserInput)
            {
                if (!VerifyValidCard(s))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifies that a single card is valid
        /// </summary>
        /// <param name="card">The string value for the card</param>
        /// <returns>True if the card is valid and False if not</returns>
        public bool VerifyValidCard(string card)
        {
            if (card == null || card.Length != 2)
            {
                return false;
            }

            if (card == "JK")
            {
                return true;
            }
            
            // If the first character is not a number between 2 and 9 or a face card, return false
            if (!(char.IsDigit(card[0]) && card[0] != '1' && card[0] != '0') && !_faceCardValues.ContainsKey(card[0].ToString()))
            {
                return false;
            }

            // If the second character is not a suit, return false
            if (!_suitMultiplierValues.ContainsKey(card[1].ToString()))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate cards in the user input
        /// </summary>
        /// <returns>A value tuple of whether duplicates were found and an error message to display if necessary</returns>
        public (bool found, string message) CheckForDuplicates()
        {
            // Dictionary to store the number of times each card appears in the user input
            Dictionary<string, int> duplicates = new Dictionary<string, int>();

            foreach (string s in SplitUserInput)
            {
                if (duplicates.ContainsKey(s))
                    duplicates[s]++;
                else
                    duplicates.Add(s, 1);
                
                // If there are more than 2 jokers or more than 1 of any other card, return true
                if (s == "JK" && duplicates[s] > 2)
                {
                    return (true, TooManyJokers);
                }
                else if (s != "JK" && duplicates[s] > 1)
                {
                    return (true, DuplicateCards);
                }
            }

            // There are no duplicates therefore, set _noOfJokers value to the number of jokers in the user input
            _noOfJokers = duplicates.ContainsKey("JK") ? duplicates["JK"] : 0;

            return (false, "");
        }

        /// <summary>
        /// Calculates the score of the user's hand
        /// </summary>
        public void CalculateScore()
        {
            foreach (string s in SplitUserInput)
            {
                if (s == "JK")
                {
                    continue;
                }
                else
                {
                    // If the first character is a number, use that as the value, otherwise use the face card value
                    int value = 0;
                    if (!int.TryParse(s[0].ToString(), out value))
                    {
                        value = _faceCardValues[s[0].ToString()];
                    }

                    // Multiply the value by the suit multiplier and add it to the score
                    _score += value * _suitMultiplierValues[s[1].ToString()];
                }
            }

            // If there are any jokers, multiply the score by the number of jokers times 2
            if (_noOfJokers > 0)
                _score *= _noOfJokers * 2;
        }

        /// <summary>
        /// Resets all the variables to their default values
        /// </summary>
        public void Reset()
        {
            UserInput = "";
            SplitUserInput = new string[0];
            _score = 0;
            ResultMessage = "";
            _noOfJokers = 0;
        }
    }
}