using obj2csx.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace obj2csx
{
    class MAPExporter
    {
        public void ConvertToMAP(string path, string dest)
        {

            var obj = new ObjReader();
            obj.Parse(path);

            Console.WriteLine("Loaded Obj");
            var sw = new StreamWriter(File.OpenWrite(dest));

            var facecount = 0;
            foreach (var grp in obj.Groups)
            {
                facecount += grp.Faces.Count;
            }

            sw.WriteLine(@"{
   ""classname"" ""worldspawn""
   ""detail_number"" ""0""
   ""min_pixels"" ""250""
   ""geometry_scale"" ""32""
   ""light_geometry_scale"" ""8""
   ""ambient_color"" ""0 0 0""
   ""emergency_ambient_color"" ""0 0 0""
   ""mapversion"" ""220""");

            var id = 0;
            Console.WriteLine("Writing " + facecount + " brushes,this may take a while");
            for (var i = 0; i < obj.Groups.Count; i++)
            {
                var group = obj.Groups[i];

                for (int k = 0; k < group.Faces.Count; k++)
                {
                    Face face = group.Faces[k];

                    List<Vector3> vertices = new List<Vector3>();
                    vertices.Add(group.Points[face.v1]);
                    vertices.Add(group.Points[face.v2]);
                    vertices.Add(group.Points[face.v3]);
                    //foreach (var index in group.Indices)
                    //{
                    //    vertices.Add(obj.Points[index - 1]);
                    //}

                    var p1 = group.Points[face.v1];
                    var p2 = group.Points[face.v2];
                    var p3 = group.Points[face.v3];

                    var n = ((p2 - p1) * (p2 - p3)).Normalize();
                    n = n.Scalar(-1 / 20f);
                    var p4 = (p1 + p2 + p3).Scalar(1 / 3f) + n;

                    var plane = new Plane(p1, p2, p3);
                    TexGenPlanes texgen;
                    texgen = SolveTexGen(p1, p2, p3, group.UV[face.uv1], group.UV[face.uv2], group.UV[face.uv3]);
                    texgen.d1 = 0;
                    texgen.d2 = 0;
                    var texgenstr = " [ "+texgen.x1 + " " + texgen.y1 + " " + texgen.z1 + " " + texgen.d1 + " ] [" + texgen.x2 + " " + texgen.y2 + " " + texgen.z2 + " " + texgen.d2 + " ] 0 1 1";

                    sw.WriteLine("  {");
                    sw.WriteLine("    ( " + p1.ToString() + " ) ( " + p2.ToString() + " ) ( " + p3.ToString() + " ) " + obj.MtlLib[face.material] + texgenstr);
                    sw.WriteLine("    ( " + p4.ToString() + " ) ( " + p2.ToString() + " ) ( " + p1.ToString() + " ) " + obj.MtlLib[face.material] + texgenstr);
                    sw.WriteLine("    ( " + p1.ToString() + " ) ( " + p3.ToString() + " ) ( " + p4.ToString() + " ) " + obj.MtlLib[face.material] + texgenstr);
                    sw.WriteLine("    ( " + p2.ToString() + " ) ( " + p3.ToString() + " ) ( " + p4.ToString() + " ) " + obj.MtlLib[face.material] + texgenstr);
                    sw.WriteLine("  }");

                    Console.WriteLine("Wrote Brush " + k);
                    id++;
                }

            }
            Console.WriteLine("Finishing up");
            sw.Write(@"}");
            sw.Flush();
            sw.Close();
        }

        bool closeEnough(Vector3 p1, Vector3 p2, float dist = 0.0001f)
        {
            return p1.DistanceTo(p2) < dist;
        }

        bool closeEnough(float p1, float p2, float dist = 0.0001f)
        {
            return Math.Abs(p1 - p2) < dist;
        }

        Vector3 Solve3VarLinearEquation(Matrix3x4 coefficients)
        {
            if (closeEnough(coefficients.a1, 0)) coefficients.a1 = 0;
            if (closeEnough(coefficients.a2, 0)) coefficients.a2 = 0;
            if (closeEnough(coefficients.a3, 0)) coefficients.a3 = 0;
            if (closeEnough(coefficients.a4, 0)) coefficients.a4 = 0;
            if (closeEnough(coefficients.b1, 0)) coefficients.b1 = 0;
            if (closeEnough(coefficients.b2, 0)) coefficients.b2 = 0;
            if (closeEnough(coefficients.b3, 0)) coefficients.b3 = 0;
            if (closeEnough(coefficients.b4, 0)) coefficients.b4 = 0;
            if (closeEnough(coefficients.c1, 0)) coefficients.c1 = 0;
            if (closeEnough(coefficients.c2, 0)) coefficients.c2 = 0;
            if (closeEnough(coefficients.c3, 0)) coefficients.c3 = 0;
            if (closeEnough(coefficients.c4, 0)) coefficients.c4 = 0;

            if (closeEnough(coefficients.a1, 0))
            {
                if (closeEnough(coefficients.b1, 0))
                    coefficients.SwapRows(0, 2);
                else
                    if (closeEnough(coefficients.c1, 0))
                    coefficients.SwapRows(0, 1);
            }

            if (!closeEnough(coefficients.a1, 0))
            {
                if (!closeEnough(coefficients.b1, 0))
                {
                    coefficients.AddRows(0, 1, -coefficients.b1 / coefficients.a1);
                }
                if (!closeEnough(coefficients.c1, 0))
                {
                    coefficients.AddRows(0, 2, -coefficients.c1 / coefficients.a1);
                }
            }

            if (closeEnough(coefficients.b2, 0))
            {
                coefficients.SwapRows(1, 2);
            }

            if (!closeEnough(coefficients.b2, 0))
            {
                if (!closeEnough(coefficients.c2, 0))
                {
                    coefficients.AddRows(1, 2, -coefficients.c2 / coefficients.b2);
                }
            }

            if (!closeEnough(coefficients.a1, 0, 0.00001f))
            {
                coefficients.ScaleRow(0, 1 / coefficients.a1);
            }
            if (!closeEnough(coefficients.b2, 0, 0.00001f))
            {
                coefficients.ScaleRow(1, 1 / coefficients.b2);
            }
            if (!closeEnough(coefficients.c3, 0, 0.00001f))
            {
                coefficients.ScaleRow(2, 1 / coefficients.c3);
            }

            var res = new Vector3();

            res.Z = coefficients.c4;
            res.Y = coefficients.b4 - (res.Z * coefficients.b3);
            res.X = coefficients.a4 - (res.Y * coefficients.a2) - (res.Z * coefficients.a3);

            return res;
        }

        TexGenPlanes SolveTexGen(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            var xmatrix = new Matrix3x4(p1.X, p1.Y, p1.Z, uv1.X, p2.X, p2.Y, p2.Z, uv2.X, p3.X, p3.Y, p3.Z, uv3.X);
            var ymatrix = new Matrix3x4(p1.X, p1.Y, p1.Z, uv1.Y, p2.X, p2.Y, p2.Z, uv2.Y, p3.X, p3.Y, p3.Z, uv3.Y);

            var xsolution = Solve3VarLinearEquation(xmatrix);
            var ysolution = Solve3VarLinearEquation(ymatrix);

            var texgen = new TexGenPlanes(xsolution.X, xsolution.Y, xsolution.Z, 0, ysolution.X, ysolution.Y, ysolution.Z, 0);
            return texgen;
        }

    }
}
