using System;

namespace asciiadventure {
    class Player : MovingGameObject {
        public Player(int row, int col, Screen screen, string name) : base(row, col, "@", screen) {
            Name = name;
        }
        public string Name {
            get;
            protected set;
        }
        public override Boolean IsPassable(){
            return true;
        }

        public int numT = 0;
        public String Action(int deltaRow, int deltaCol){
            int newRow = Row + deltaRow;
            int newCol = Col + deltaCol;
            if (!Screen.IsInBounds(newRow, newCol)){
                return "nope";
            }
            GameObject other = Screen[newRow, newCol];
            if (other == null){
                return "negative";
            }
            
            if (other is Treasure){
                other.Delete();
                numT++;
                other.isPresent = false;
                return "Treasure collected! Total Treasures: " + numT;
               
            }
            if (other is Mine){
                other.Delete();
                other.isDefused = true;
                return "Mine defused";
            }
            return "ouch";
        }
    }
}