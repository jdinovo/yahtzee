using System;
namespace Yahtzee {
    public class Die {
        public int Value { get; set; }
        public int Number { get; }

        public Die(int number) {
            this.Number = number;
            this.Value = 1;
        }

        public int Roll() {
            this.Value = new Random().Next(1,7);

            return this.Value;
        }

        public override String ToString() {
            return "D" + this.Number + ": " + this.Value;
        }

        public static int operator +(Die lhs, Die rhs) {
            return lhs.Value + rhs.Value;
        }

    }
}
