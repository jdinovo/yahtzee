using System;
using System.Collections.Generic;
using System.Linq;

namespace Yahtzee {
    public class Game {
        Dice dice = new Dice();
        Dictionary<String, Category> categories;

        public Game() {

            categories = new Dictionary<String, Category> {
                { "1s", new Category("1s") },
                { "2s", new Category("2s") },
                { "3s", new Category("3s") },
                { "4s", new Category("4s") },
                { "5s", new Category("5s") },
                { "6s", new Category("6s") },
                { "upper bonus", new Category("Upper Bonus") },
                { "3 of kind", new Category("3 of kind") },
                { "4 of kind", new Category("4 of kind") },
                { "full house", new Category("full house") },
                { "sm straight", new Category("sm straight") },
                { "lg straight", new Category("lg straight") },
                { "yahtzee", new Category("yahtzee") },
                { "chance", new Category("chance") }
            };

        }

        public void Run() {

            for (int round = 1; round <= 13; round++) {
                int numRerolls = 0;
                Boolean[] roll = { true, true, true, true, true };
                Boolean reload = false;
                Boolean invalidInput;
                Console.Clear();
                Console.WriteLine($"--- Round {round} ---\n");


                do {
                    dice.RollDice(ref roll);

                    CalculatePoints();

                    PrintRoundCard(categories);

                    // get user input
                    if (numRerolls < 2) {
                        do {
                            invalidInput = false;
                            Console.WriteLine("Select a category, pick the dice to re-roll (Ex. 1,2,3) or \"show\" for score-card");
                            Console.Write("Choice (<category>/#,#,#/show):");
                            String input = Console.ReadLine();

                            if (input.Equals("show")) {
                                ShowScoreCard(false);
                                reload = true;
                            } else {
                                Boolean canParse = false;
                                String[] inputs = input.Split(',');
                                if (inputs.Length <= 3) {
                                    foreach (String inp in inputs) {
                                        canParse = int.TryParse(inp, out int dieToReroll);
                                        if (canParse) {
                                            reload = true;
                                            if (dieToReroll < 6 && dieToReroll > 0) {
                                                roll[dieToReroll - 1] = true;
                                            } else {
                                                canParse = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (canParse) {
                                    numRerolls++;
                                    Console.Clear();
                                } else {
                                    Boolean found = StoreCat(input);
                                    

                                    if (!found) {
                                        ErrorMessage();
                                        invalidInput = true;
                                    }
                                    reload = false;

                                }
                            }


                        } while (invalidInput);
                    } else {
                        reload = false;
                        bool found;
                        do {
                            Console.Write("Select a category to take the points:");
                            String input = Console.ReadLine();

                            found = StoreCat(input);

                            if (!found) {
                                Console.WriteLine("Invalid category or category already taken");
                            }

                        } while (!found);
                    }
                } while (reload);
            }

            // game over
            ShowScoreCard(true);
        }

        private void ShowScoreCard(Boolean final) {
            // calc upper bonus
            if (categories.Where(cat => cat.Key.Equals("1s") || cat.Key.Equals("2s") || cat.Key.Equals("3s") || cat.Key.Equals("4s") || cat.Key.Equals("5s") || cat.Key.Equals("6s")).Sum(cat => cat.Value.Score) >= 63) {
                categories["upper bonus"].Score = 35;
                categories["upper bonus"].Cashed = true;
            }

            Console.Clear();
            int totScore = 0;

            Console.WriteLine($"{(final? "Game Over\n" : "")}{new String('-', 19)}");
            Console.WriteLine($"|   Score Card    |");
            Console.WriteLine($"{new String('-', 19)}");
            Console.WriteLine("Category:      Points:");

            foreach (var cat in categories) {
                Console.WriteLine($"{cat.Key + new String(' ', (15 - cat.Key.Length)) + (cat.Value.Cashed ? cat.Value.Score + "" : "-")}");
                totScore += cat.Value.Score;
            }

            Console.WriteLine($"{new String('-', 19)}");
            Console.WriteLine($"{(final?"Final":"Total")} Score: {totScore}");

            Console.Write("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        private void ErrorMessage() {
            Console.WriteLine("Invalid category or category already taken or Invalid number format (#,#,#)");
        }

        private Boolean StoreCat(String input) {
            foreach (KeyValuePair<String, Category> cat in categories) {
                if (cat.Key.Equals(input) && !cat.Value.Cashed) {
                    // store points in that category
                    cat.Value.Cashed = true;
                    cat.Value.Score = cat.Value.Points;
                    ShowScoreCard(false);
                    return true;
                }
            }
            return false;
        }

        private void CalculatePoints() {
            // wipe points
            foreach (var cat in categories) {
                cat.Value.Points = 0;
            }

            // sort and group die
            var setOfDice = dice.GroupBy(die => die.Value).Select(die => {
                return new {
                    Value = die.Key,
                    Count = die.Count(),
                    Score = die.Key * die.Count()
                };
            }).OrderBy(die => die.Value);

            // calculate categories
            UpperPointsCalc(setOfDice);
            LowerPointsCalc(setOfDice);
            SequenceCalc(setOfDice.Select(die => {
                return die.Value;
            }));

            
        }

        private void UpperPointsCalc(IEnumerable<dynamic> dieGroup) {
            categories["1s"].Points = dieGroup.Where(die => die.Value == 1).Sum(die => die.Score);
            categories["2s"].Points = dieGroup.Where(die => die.Value == 2).Sum(die => die.Score);
            categories["3s"].Points = dieGroup.Where(die => die.Value == 3).Sum(die => die.Score);
            categories["4s"].Points = dieGroup.Where(die => die.Value == 4).Sum(die => die.Score);
            categories["5s"].Points = dieGroup.Where(die => die.Value == 5).Sum(die => die.Score);
            categories["6s"].Points = dieGroup.Where(die => die.Value == 6).Sum(die => die.Score);
        }

        private void LowerPointsCalc(IEnumerable<dynamic> dieGroup) {
            categories["3 of kind"].Points = dieGroup.Where(die => die.Count >= 3).Sum(die => die.Score);
            categories["4 of kind"].Points = dieGroup.Where(die => die.Count >= 4).Sum(die => die.Score);
            categories["full house"].Points = dieGroup.Any(die => die.Count == 2 && dieGroup.Any(die2 => die2.Count == 3 && die != die2)) ? 25 : 0;
            
            categories["yahtzee"].Points = dieGroup.Any(die => die.Count == 5) ? 50 : 0;
            categories["chance"].Points = dieGroup.Sum(die => die.Score);
        }

        public void SequenceCalc(IEnumerable<int> dieValues) {
            List<int[]> smSeq = new List<int[]> {
                        new int[] { 1, 2, 3, 4 },
                        new int[] { 2, 3, 4, 5 },
                        new int[] { 3, 4, 5, 6 }
                    };
            foreach (var seq in smSeq) {
                if (categories["sm straight"].Points == 0) {
                    categories["sm straight"].Points = dieValues.Intersect(seq).Count() >= 4 ? 30 : 0;
                }
            }
            List<int[]> lgSeq = new List<int[]> {
                        new int[] { 1, 2, 3, 4, 5 },
                        new int[] { 2, 3, 4, 5, 6 }
                    };
            foreach (var seq in lgSeq) {
                if (categories["lg straight"].Points == 0) {
                    categories["lg straight"].Points = dieValues.SequenceEqual(seq) ? 40 : 0;
                }
            }
        }

        private void PrintRoundCard(Dictionary<String, Category> categ) {
            Console.WriteLine($"\n{new String('-', 18)}");
            Console.WriteLine($"|   Round Card   |");
            Console.WriteLine($"{new String('-', 18)}");
            Console.WriteLine("Category:      Points:");

            foreach (KeyValuePair<String, Category> cat in categ) {
                if (!cat.Key.Equals("upper bonus")) {
                    Console.WriteLine(cat.Value);
                }
            }

            Console.WriteLine($"{new String('-', 18)}");
        }

    }
}

/* standard query
    var setOfDice = dice.GroupBy(die => die.Value).Select(die => {
        return new {
            Value = die.Key,
            Count = die.Count(),
            Score = die.Key * die.Count()
        };
    }).OrderBy(die => die.Value);

    foreach(var dieGroups in setOfDice) {
                
    }
*/

/*
    * ANOTHER WAY OF DOING IT
    *  var setOfDie = from die in Dice
    *  group die by die.Value into g
    *  Select new {
    *      Value = g.Value
    *      Count = g.Count()
    *      Score = g.Value * g.Count()
    *      } into g2
    *  Order g2.Value
    *  Select g2;
    *  
 */
