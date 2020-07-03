using System;
using System.Collections.Generic;
using System.Text;

namespace obj2csx.Geometry
{
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x,float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(string nul = "")
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public static Vector3 FromString(string str)
        {
            return new Vector3(Convert.ToSingle(str.Split(' ')[0]), Convert.ToSingle(str.Split(' ')[1]), Convert.ToSingle(str.Split(' ')[2]));
        }

        public override string ToString()
        {
            return X + " " + Y + " " + Z;
        }

        public static Vector3 operator -(Vector3 lhs,Vector3 rhs)
        {
            return new Vector3(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static Vector3 operator *(Vector3 lhs,Vector3 rhs)
        {
            float x = (lhs.Y * rhs.Z) - (lhs.Z * rhs.Y);
            float y = (lhs.Z * rhs.X) - (lhs.X * rhs.Z);
            float z = (lhs.X * rhs.Y) - (lhs.Y * rhs.X);

            return new Vector3(x, y, z);
        }

        public static Vector3 operator +(Vector3 lhs,Vector3 rhs)
        {
            return new Vector3(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        public Vector3 Scalar(float factor)
        {
            return new Vector3(X * factor, Y * factor, Z * factor);
        }
        
        public float DistanceTo(Vector3 other)
        {
            return (float)Math.Sqrt(Math.Pow((this.X - other.X), 2) + Math.Pow((this.Y - other.Y), 2));
        }

        public Vector3 Normalize()
        {
            var l = (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
            return new Vector3(X / l, Y / l, Z / l);
        }

        public bool TouchesCoordAxes()
        {
            return (X == 0 || Y == 0 || Z == 0);
        }
    }
}
