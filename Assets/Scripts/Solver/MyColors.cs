using UnityEngine;

namespace Solver
{
    public class MyColors
    {
        public Color Color;
        public int Amount = 0;

        public MyColors(Color color)
        {
            this.Color = color;
            this.Amount = 0;
        }

        public bool MoreThan4()
        {
            return Amount >= 4;
        }
    }
}
