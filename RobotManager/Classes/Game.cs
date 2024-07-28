using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotManager.Classes
{
    public class Game
    {
        private DateTime date;
        private string against;
        private Field field;

        public DateTime Date { get => date; set => date = value; }
        public string Against { get => against; set => against = value; }
        public Field Field { get => field; set => field = value; }

    }
    public enum Field
    {
        A,
        B,
        C,
        D
    }
}
