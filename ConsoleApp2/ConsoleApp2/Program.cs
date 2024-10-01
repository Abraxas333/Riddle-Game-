using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

class Program
{
    static async Task Main(string[] args)
    {
        bool finished = false;
        int score = 0; // Move score declaration here to accumulate over rounds

        do
        {
            Console.WriteLine("Hello and Welcome to the game of riddles! Please choose either math or logic for playing the game or press any key for quitting:");
            
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            string game = Console.ReadLine();

            if (game.Equals("math", StringComparison.OrdinalIgnoreCase))
            {
                Math riddle = new Math();
                if (await riddle.MathRiddle())
                {
                    score += 1;
                } 
                else { continue; }
            }
            else if (game.Equals("logic", StringComparison.OrdinalIgnoreCase))
            {
                Logic riddle = new Logic();
                if (await riddle.LogicRiddle())
                {
                    score += 1;
                }
                else { finished = false; }
            }
            else
            {
                continue;
            }

            Console.WriteLine($"Your score is {score}");
            await Task.Delay(100);

        } while (!finished);
    }

    public class Math
    {
        private static readonly Random rnd = new Random();
        private int _term1;
        private int _term2;
        private int _operator;
        private int _solution;

        public Math()
        {
            // Generate random numbers for terms
            _term1 = rnd.Next(1, 14);
            _term2 = rnd.Next(1, 14);
            _operator = rnd.Next(1, 5); // Randomly choose an operator

            switch (_operator)
            {
                case 1: // Addition
                    _solution = _term1 + _term2;
                    break;
                case 2: // Subtraction
                    _solution = _term1 - _term2;
                    break;
                case 3: // Multiplication
                    _solution = _term1 * _term2;
                    break;
                case 4: // Division
                    // Ensure we are dividing by a non-zero number
                    if (_term2 == 0)
                    {
                        _term2 = 1; // Avoid division by zero
                    }
                    _solution = _term1 / _term2;
                    break;
            }
        }

        public async Task<bool> MathRiddle()
        {
            switch (_operator)
            {
                case 1:
                    Console.WriteLine($"Please add {_term1} and {_term2}:");
                    break;
                case 2:
                    Console.WriteLine($"Please subtract {_term2} from {_term1}:");
                    break;
                case 3:
                    Console.WriteLine($"Please multiply {_term1} by {_term2}:");
                    break;
                case 4:
                    Console.WriteLine($"Please divide {_term1} by {_term2}:");
                    break;
            }

            using var cts = new CancellationTokenSource();
            var inputTask = Task.Run(() => Console.ReadLine());
            var timerTask = Task.Delay(30000, cts.Token); // 30 seconds timer

            var completedTask = await Task.WhenAny(inputTask, timerTask);

            if (completedTask == inputTask)
            {
                cts.Cancel(); // Cancel the timer

                // Read user input
                string userInput = await inputTask;

                // Attempt to parse the user input as an integer
                bool isValidNumber = int.TryParse(userInput, out int answer);

                if (isValidNumber) // Check if parsing was successful
                {
                    // Compare the parsed answer with the solution
                    if (answer == _solution)
                    {
                        Console.WriteLine("You got that right.");
                        return true; // Correct answer
                    }
                    else
                    {
                        Console.WriteLine($"You got that wrong. The correct answer is {_solution}.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }
            else
            {
                Console.WriteLine("Time's up!"); // Respond when the timer runs out
                cts.Cancel();
                // Wait for the input task to complete to clear any pending reads
                await Task.WhenAny(inputTask, Task.Delay(100));
            }

            return false;
        }
    }

    public class Logic
    {
        private static readonly Random rnd = new Random();
        private Dictionary<string, string> riddles = new Dictionary<string, string>();
        private string _solution;
        private string _riddle;

        public Logic()
        {
            riddles.Add("lillies", "Rose, Iris, and Lilly bought flowers for their mothers: the roses, the irises, and the lilies.\n" +
                                 "\"That's so great!\" said the girl with the roses. \"We bought the roses, the irises, and the lilies, but none of us bought the flowers that sound like our names.\"\n" +
                                 "\"You're so right!\" added Lilly.\n" +
                                 "Which flowers did the girl named Rose buy?");
            riddles.Add("William", "Tin is older than Leo. Leo is younger than Peter and William.\n" +
                                   "William is older than Tin and Mark, and Mark is older than Peter. If Tin and Mark are twins, which boy is the oldest?");

            // Select a random riddle
            int rand = rnd.Next(0, riddles.Count);
            var selectedRiddle = riddles.ElementAt(rand);
            _riddle = selectedRiddle.Value;
            _solution = selectedRiddle.Key;
        }

        public async Task<bool> LogicRiddle()
        {
            Console.WriteLine(_riddle);
            using var cts = new CancellationTokenSource();
            var inputTask = Task.Run(() => Console.ReadLine());
            var timerTask = Task.Delay(30000, cts.Token);

            var completedTask = await Task.WhenAny(inputTask, timerTask);
            if (completedTask == inputTask)
            {
                cts.Cancel();
                string userInput = await inputTask;

                if (string.Equals(userInput, _solution, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("You got that right.");
                    return true;
                }

                else
                {
                    Console.WriteLine($"You got that wrong. The correct answer is {_solution}.");
                }
            }
            else
            {
                Console.WriteLine("Time's up!");
                cts.Cancel();
                // Wait for the input task to complete to clear any pending reads
                await Task.WhenAny(inputTask, Task.Delay(100));
            }
        
        
            return false;
        }
    }
}
