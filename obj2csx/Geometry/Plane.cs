using System;
using System.Collections.Generic;
using System.Text;


namespace obj2csx.Geometry
{
    public class Plane
    {
        public float x;
        public float y;
        public float z;
        private float d;

        void normalize()
        {
            var sqrt = (float)Math.Sqrt((x * x) + (y * y) + (z * z));
            x /= sqrt;
            y /= sqrt;
            z /= sqrt;
        }

        public Plane(Vector3 point1, Vector3 point2)
        {
            this.set(point1, point2);
        }

        public Plane(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            this.set(point1, point2, point3);
        }

        public Plane(float x, float y, float z, float d)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.d = d;
        }

        public virtual void set(Vector3 point1, Vector3 point2)
        {
            this.x = point2.X;
            this.y = point2.Y;
            this.z = point2.Z;
            this.normalize();

            this.d = -(point1.X * this.x + point1.Y * this.y + point1.Z * this.z);
        }

        public void set(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var ax = p1.X - p2.X;
            var ay = p1.Y - p2.Y;
            var az = p1.Z - p2.Z;

            var bx = p3.X - p2.X;
            var by = p3.Y - p2.Y;
            var bz = p3.Z - p2.Z;

            x = ((ay * bz) - (az * by));
            y = ((az * bx) - (ax * bz));
            z = ((ax * by) - (ay * bx));

            normalize();

            d = -((p2.X * x) + (p2.Y * y) + (p2.Z * z));
        }

        public virtual float D
        {
            get
            {
                return this.d;
            }
            set
            {
                this.d = value;
            }
        }


        public virtual float getDistanceToPoint(Vector3 point)
        {
            return (this.x * point.X + this.y * point.Y + this.z * point.Z) + this.d;
        }


        public Plane Inverted()
        {
            return new Plane(-x, -y, -z, -d);
        }

        public override string ToString()
        {
            return x.ToString() + " " + y + " " + z + " " + d;
        }
    }
}
