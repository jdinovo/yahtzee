using System;
using System.Collections.Generic;
using System.Linq;

namespace Yahtzee {
    public class Dice : List<Die>{
        public Dice() {
            for (int i = 1; i <= 5; i++) {
                this.Add(new Die(i));
            }
        }

        public void RollDice(ref Boolean[] roll) {
            List<String> rerolledDie = new List<String>();
            List<String> dieString = new List<String>();
            for (int i = 0; i < Count; i++) {
                if (roll[i]) {
                    this[i].Roll();
                    rerolledDie.Add("D" + (i + 1));
                }
                roll[i] = false;
                dieString.Add(this[i] + "");

            }
            if (rerolledDie.Count > 0) {
                Console.WriteLine($"Rolling... {rerolledDie.Aggregate((i1, i2) => i1 + ", " + i2)}");
            }
            Console.WriteLine($"You rolled the following: \n{dieString.Aggregate((d1, d2) => d1 + " " + d2)}");
        }
    }
}
