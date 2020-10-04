using System;


namespace asciiadventure {
    public abstract class GameObject {
        
        public int Row {
            get;
            protected set;
        }
        public int Col {
            get;
            protected set;
        }

        public String Token {
            get;
            protected internal set;
        }

        public Screen Screen {
            get;
            protected set;
        }

        public GameObject(int row, int col, String token, Screen screen){
            Row = row;
            Col = col;
            Token = token;
            Screen = screen;
            Screen[row, col] = this;
        }

        public virtual Boolean IsPassable() {
            return false;
        }

        public override String ToString() {
            return this.Token;
        }

        public void Delete() {
            Screen[Row, Col] = null;
        }

        // for mines
         public Boolean isDefused = false;

        // for treasures
         public Boolean isPresent = true;

        // for mobs
        public Boolean isDead = false;
    }

    public abstract class MovingGameObject : GameObject {

        public MovingGameObject(int row, int col, String token, Screen screen) : base(row, col, token, screen) {}
        
        public string Move(int deltaRow, int deltaCol) {
            int newRow = deltaRow + Row;
            int newCol = deltaCol + Col;
            if (!Screen.IsInBounds(newRow, newCol)) {
                return "";
            }
            GameObject gameObject = Screen[newRow, newCol];
            // Now just make the move
            int originalRow = Row;
            int originalCol = Col;
            // now change the location of the object, if the move was legal
            Row = newRow;
            Col = newCol;
            Screen[originalRow, originalCol] = null;
            Screen[Row, Col] = this;
            return "";
        }
    }

    class Wall : GameObject {
        public Wall(int row, int col, Screen screen) : base(row, col, "=", screen) {}
    }

    class Treasure : GameObject {
        public Treasure(int row, int col, Screen screen) : base(row, col, "T", screen) {}
    }

    class Mine : GameObject {
        public Mine(int row, int col, Screen screen) : base(row, col, "X", screen) {}

        public override bool IsPassable() {
            return true;
        }
    }
}