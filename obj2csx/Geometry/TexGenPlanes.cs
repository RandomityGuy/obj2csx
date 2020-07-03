using System;
using System.Collections.Generic;
using System.Text;

namespace obj2csx.Geometry
{
    public class TexGenPlanes
    {
        public float x1 = 0.5f;
        public float y1;
        public float z1;
        public float d1 = 1;

        public float x2;
        public float y2 = 0.5f;
        public float z2;
        public float d2 = 1;

        public TexGenPlanes(float x1, float y1, float z1, float d1, float x2, float y2, float z2, float d2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.z1 = z1;
            this.d1 = d1;
            this.x2 = x2;
            this.y2 = y2;
            this.z2 = z2;
            this.d2 = d2;
        }

        public TexGenPlanes()
        {

        }
    }
}
