using System;
using System.Collections.Generic;
using System.Text;

namespace obj2csx.Geometry
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(string nul = "")
        {
            X = 0;
            Y = 0;
        }

        public static Vector2 FromString(string str)
        {
            return new Vector2(Convert.ToSingle(str.Split(' ')[0]), Convert.ToSingle(str.Split(' ')[1]));
        }

    }
}
