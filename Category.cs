using System;

namespace Yahtzee {
    public class Category {
        public String Name { get; }
        public int Points { get; set; }
        public int Score { get; set; }
        public Boolean Cashed;

        public Category(String name) {
            this.Name = name;
            this.Points = 0;
            this.Score = 0;
            this.Cashed = false;
        }

        public override String ToString() {
            return Name + new String(' ', (15 - Name.Length)) + Points;
        }

    }
}
