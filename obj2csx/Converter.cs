using obj2csx.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace obj2csx
{
    public class Converter
    {
        
        public void ConvertToCSX(string path,string dest,int splitcount = 0,bool flipnorms = false)
        {
            
            var obj = new ObjReader();
            obj.Parse(path);

            Console.WriteLine("Loaded Obj");
            var facecount = 0;
            foreach (var grp in obj.Groups)
            {
                facecount += grp.Faces.Count;
            }

            if (splitcount == 0)
            {
                var sw = new StreamWriter(File.OpenWrite(dest));



                sw.WriteLine(@"<?xml version=""1.0"" encoding=""utf-88"" standalone=""yes""?>
<!--Torque Constructor Scene document http://www.garagegames.com-->
<ConstructorScene version = ""4"" creator = ""Torque Constructor"" date = ""2017/10/30 14:40:36"">
    <Sunlight azimuth = ""180"" elevation = ""35"" color = ""255 255 255"" ambient = ""64 64 64""/>
    <LightingOptions lightingSystem = """"  ineditor_defaultLightmapSize = ""256"" ineditor_maxLightmapSize = ""256"" ineditor_lightingPerformanceHint = ""0"" ineditor_shadowPerformanceHint = ""1"" ineditor_TAPCompatibility = ""0"" ineditor_useSunlight = ""0"" export_defaultLightmapSize = ""256"" export_maxLightmapSize = ""256"" export_lightingPerformanceHint = ""0"" export_shadowPerformanceHint = ""1"" export_TAPCompatibility = ""0"" export_useSunlight = ""0""/>
    <GameTypes>
        <GameType name = ""Constructor""/>
        <GameType name = ""Torque""/>
    </GameTypes>
    <DetailLevels current = ""0"">
        <DetailLevel minPixelSize = ""0"" actionCenter = ""0 0 0"">
            <InteriorMap brushScale = ""32"" lightScale = ""8"" ambientColor = ""0 0 0"" ambientColorEmerg = ""0 0 0"">
                <Entities nextEntityID = ""1"" >
                    <Entity id = ""0"" classname = ""worldspawn"" gametype = ""Torque"" isPointEntity = ""0"">
                        <Properties detail_number = ""0"" min_pixels = ""250"" geometry_scale = ""32"" light_geometry_scale = ""8"" ambient_color = ""0 0 0"" emergency_ambient_color = ""0 0 0"" mapversion = ""220""/>
                    </Entity>
                </Entities>
                <Brushes nextBrushID =""" + (facecount) + "\">");

                var id = 0;
                Console.WriteLine("Writing " + facecount + " brushes,this may take a while");
                for (var i = 0; i < obj.Groups.Count; i++)
                {
                    var group = obj.Groups[i];

                    for (int k = 0; k < group.Faces.Count; k++)
                    {
                        Face face = group.Faces[k];

                        List<Vector3> vertices = new List<Vector3>();
                        var p1 = group.Points[face.v1];
                        var p2 = group.Points[face.v2];
                        var p3 = group.Points[face.v3];

                        var n = ((p2 - p1) * (p2 - p3)).Normalize();

                        if (float.IsNaN(n.X) || float.IsNaN(n.Y) || float.IsNaN(n.Z))
                        {
                            continue; // Don't export 'linear' faces
                        }

                        sw.Write("                      <Brush id=\"" + id + "\" owner=\"0\" type=\"0\" pos=\"0 0 -0.05\" transform=\"1 0 0 0 0 1 0 0 0 0 1 -0.05 0 0 0 1\" group=\"-1\" locked=\"0\" nextFaceID=\"" + "5" + "\" nextVertexID=\"" + "5" + "\">" +
Environment.NewLine + "                       <Vertices>" + Environment.NewLine);

                        n = n.Scalar(-1 / 20f);
                        var p4 = (p1 + p2 + p3).Scalar(1 / 3f) + n;

                        var plane = new Plane(p1, p2, p3);
                        if (flipnorms)
                        {
                            plane = plane.Inverted();
                            p4 = p4 - (n.Scalar(2));
                        }
                        vertices.Add(p1);
                        vertices.Add(p2);
                        vertices.Add(p3);
                        vertices.Add(p4);
                        //foreach (var index in group.Indices)
                        //{
                        //    vertices.Add(obj.Points[index - 1]);
                        //}
                        foreach (var vertex in vertices)
                        {
                            sw.WriteLine("                          <Vertex pos=\"" + vertex.ToString() + "\" />");
                        }
                        sw.WriteLine("                      </Vertices>");


                        TexGenPlanes texgen;
                        texgen = SolveTexGen(p1, p2, p3, group.UV[face.uv1], group.UV[face.uv2], group.UV[face.uv3]);
                        texgen.d1 = 0;
                        texgen.d2 = 0;
                        var texgenstr = texgen.x1 + " " + texgen.y1 + " " + texgen.z1 + " " + texgen.d1 + " " + texgen.x2 + " " + texgen.y2 + " " + texgen.z2 + " " + texgen.d2 + " 0 1 1";
                        if (!flipnorms)
                        {
                            sw.WriteLine("                      <Face id=\"" + 1 + "\" plane=\"" + plane.x + " " + plane.y + " " + plane.z + " " + plane.D + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"0 1 2\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 2 + "\" plane=\"" + new Plane(p4, p2, p1).ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"3 1 0\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 3 + "\" plane=\"" + new Plane(p1, p3, p4).ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"0 2 3\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 4 + "\" plane=\"" + new Plane(p2, p3, p4).ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"1 2 3\"/>");
                            sw.WriteLine("                      </Face>");
                        }
                        else
                        {
                            sw.WriteLine("                      <Face id=\"" + 1 + "\" plane=\"" + plane.x + " " + plane.y + " " + plane.z + " " + plane.D + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"2 1 0\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 2 + "\" plane=\"" + new Plane(p4, p2, p1).Inverted().ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"0 1 3\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 3 + "\" plane=\"" + new Plane(p1, p3, p4).Inverted().ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"3 2 0\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 4 + "\" plane=\"" + new Plane(p2, p3, p4).Inverted().ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"3 2 1\"/>");
                            sw.WriteLine("                      </Face>");
                        }
                        sw.Write("                      </Brush>");
                        Console.WriteLine("Wrote Brush " + k);
                        id++;
                    }

                }
                Console.WriteLine("Finishing up");
                sw.Write(@"              </Brushes>
            </InteriorMap>
        </DetailLevel>
    </DetailLevels>
</ConstructorScene>");
                sw.Flush();
                sw.Close();
            }
            else
            {

                var splitobjsraw = obj.Groups.Select(a => SplitObjects(a,splitcount)).ToList();

                List<Obj> splitobjs = new List<Obj>();

                splitobjsraw.ForEach(a => splitobjs.AddRange(a));

                for (var i = 0; i < splitobjs.Count; i++)
                {
                    var fname = dest.Insert(dest.IndexOf(".csx"), "_" + i);
                    var sw = new StreamWriter(File.OpenWrite(fname));

                    var innerfacecount = 0;
                    foreach (var grp in splitobjs)
                    {
                        innerfacecount += grp.Faces.Count;
                    }

                    sw.WriteLine(@"<?xml version=""1.0"" encoding=""utf-88"" standalone=""yes""?>
<!--Torque Constructor Scene document http://www.garagegames.com-->
<ConstructorScene version = ""4"" creator = ""Torque Constructor"" date = ""2017/10/30 14:40:36"">
    <Sunlight azimuth = ""180"" elevation = ""35"" color = ""255 255 255"" ambient = ""64 64 64""/>
    <LightingOptions lightingSystem = """"  ineditor_defaultLightmapSize = ""256"" ineditor_maxLightmapSize = ""256"" ineditor_lightingPerformanceHint = ""0"" ineditor_shadowPerformanceHint = ""1"" ineditor_TAPCompatibility = ""0"" ineditor_useSunlight = ""0"" export_defaultLightmapSize = ""256"" export_maxLightmapSize = ""256"" export_lightingPerformanceHint = ""0"" export_shadowPerformanceHint = ""1"" export_TAPCompatibility = ""0"" export_useSunlight = ""0""/>
    <GameTypes>
        <GameType name = ""Constructor""/>
        <GameType name = ""Torque""/>
    </GameTypes>
    <DetailLevels current = ""0"">
        <DetailLevel minPixelSize = ""0"" actionCenter = ""0 0 0"">
            <InteriorMap brushScale = ""32"" lightScale = ""8"" ambientColor = ""0 0 0"" ambientColorEmerg = ""0 0 0"">
                <Entities nextEntityID = ""1"" >
                    <Entity id = ""0"" classname = ""worldspawn"" gametype = ""Torque"" isPointEntity = ""0"">
                        <Properties detail_number = ""0"" min_pixels = ""250"" geometry_scale = ""32"" light_geometry_scale = ""8"" ambient_color = ""0 0 0"" emergency_ambient_color = ""0 0 0"" mapversion = ""220""/>
                    </Entity>
                </Entities>
                <Brushes nextBrushID =""" + (innerfacecount) + "\">");

                    var id = 0;
                    Console.WriteLine("Writing " + facecount + " brushes,this may take a while");

                    var group = splitobjs[i];

                   for (int k = 0; k < group.Faces.Count; k++)
                   {
                       Face face = group.Faces[k];
                       sw.Write("                      <Brush id=\"" + id + "\" owner=\"0\" type=\"0\" pos=\"0 0 -0.05\" transform=\"1 0 0 0 0 1 0 0 0 0 1 -0.05 0 0 0 1\" group=\"-1\" locked=\"0\" nextFaceID=\"" + "5" + "\" nextVertexID=\"" + "5" + "\">" +
            Environment.NewLine + "                       <Vertices>" + Environment.NewLine);
                       List<Vector3> vertices = new List<Vector3>();
                       var p1 = group.Points[face.v1];
                       var p2 = group.Points[face.v2];
                       var p3 = group.Points[face.v3];

                       var n = ((p2 - p1) * (p2 - p3)).Normalize();
                       n = n.Scalar(-1 / 20f);
                       var p4 = (p1 + p2 + p3).Scalar(1 / 3f) + n;

                       var plane = new Plane(p1, p2, p3);
                        if (flipnorms)
                        {
                            plane = plane.Inverted();
                            p4 = p4 - (n.Scalar(2));
                        }
                       vertices.Add(p1);
                       vertices.Add(p2);
                       vertices.Add(p3);
                       vertices.Add(p4);
                       //foreach (var index in group.Indices)
                       //{
                       //    vertices.Add(obj.Points[index - 1]);
                       //}
                       foreach (var vertex in vertices)
                           sw.WriteLine("                          <Vertex pos=\"" + vertex.ToString() + "\" />");
                       sw.WriteLine("                      </Vertices>");


                       TexGenPlanes texgen;
                       texgen = SolveTexGen(p1, p2, p3, group.UV[face.uv1], group.UV[face.uv2], group.UV[face.uv3]);
                       texgen.d1 = 0;
                       texgen.d2 = 0;
                       var texgenstr = texgen.x1 + " " + texgen.y1 + " " + texgen.z1 + " " + texgen.d1 + " " + texgen.x2 + " " + texgen.y2 + " " + texgen.z2 + " " + texgen.d2 + " 0 1 1";
                        if (!flipnorms)
                        {
                            sw.WriteLine("                      <Face id=\"" + 1 + "\" plane=\"" + plane.x + " " + plane.y + " " + plane.z + " " + plane.D + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"0 1 2\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 2 + "\" plane=\"" + new Plane(p4, p2, p1).ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"3 1 0\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 3 + "\" plane=\"" + new Plane(p1, p3, p4).ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"0 2 3\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 4 + "\" plane=\"" + new Plane(p2, p3, p4).ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"1 2 3\"/>");
                            sw.WriteLine("                      </Face>");
                        }
                        else
                        {
                            sw.WriteLine("                      <Face id=\"" + 1 + "\" plane=\"" + plane.x + " " + plane.y + " " + plane.z + " " + plane.D + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"2 1 0\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 2 + "\" plane=\"" + new Plane(p4, p2, p1).Inverted().ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"0 1 3\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 3 + "\" plane=\"" + new Plane(p1, p3, p4).Inverted().ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"3 2 0\"/>");
                            sw.WriteLine("                      </Face>");
                            sw.WriteLine("                      <Face id=\"" + 4 + "\" plane=\"" + new Plane(p2, p3, p4).Inverted().ToString() + "\"  album=\"" + "" + "\" material=\"" + obj.MtlLib[face.material] + "\" texgens=\"" + texgenstr + "\" texRot=\"" + "0" + "\" texScale=\"" + "1" + " " + "1" + "\" texDiv=\"" + "32 32" + "\">");
                            sw.WriteLine("                          <Indices indices=\"3 2 1\"/>");
                            sw.WriteLine("                      </Face>");
                        }
                        sw.Write("                      </Brush>");
                       Console.WriteLine("Wrote Brush " + k);
                       id++;
                   }
                    Console.WriteLine("Finishing up");
                    sw.Write(@"              </Brushes>
            </InteriorMap>
        </DetailLevel>
    </DetailLevels>
</ConstructorScene>");
                    sw.Flush();
                    sw.Close();
                }

            }
        }


        List<Obj> SplitObjects(Obj obj,int maxFaces)
        {
            var ret = new List<Obj>();

            var retcount = Math.Ceiling(obj.Faces.Count / (float)maxFaces);

            var groupedFaces = new List<List<Face>>();

            for (var j = 0; j < retcount;j++)
            {
                List<Face> curGroup;

                if (j == retcount-1)
                {
                    curGroup = obj.Faces.GetRange(j * maxFaces, obj.Faces.Count % maxFaces);
                }
                else
                    curGroup = obj.Faces.GetRange(j * maxFaces, maxFaces);

                groupedFaces.Add(curGroup);
            }

            int i = 0;
            foreach (var group in groupedFaces)
            {
                var verts = group.Select(a => obj.Points[a.v1]).Union(group.Select(a => obj.Points[a.v2])).Union(group.Select(a => obj.Points[a.v3])).ToList();
                var uv = group.Select(a => obj.UV[a.uv1]).Union(group.Select(a => obj.UV[a.uv2])).Union(group.Select(a => obj.UV[a.uv3])).ToList();
                var norms = group.Select(a => obj.Normals[a.n1]).Union(group.Select(a => obj.Normals[a.n2])).Union(group.Select(a => obj.Normals[a.n3])).ToList();

                var o = new Obj();
                o.Points = verts;
                o.Normals = norms;
                o.UV = uv;

                o.Faces = new List<Face>();

                foreach (var face in group)
                {
                    var cf = new Face();

                    cf.n1 = norms.IndexOf(obj.Normals[face.n1]);
                    cf.n2 = norms.IndexOf(obj.Normals[face.n2]);
                    cf.n3 = norms.IndexOf(obj.Normals[face.n3]);

                    cf.v1 = verts.IndexOf(obj.Points[face.v1]);
                    cf.v2 = verts.IndexOf(obj.Points[face.v2]);
                    cf.v3 = verts.IndexOf(obj.Points[face.v3]);

                    cf.uv1 = uv.IndexOf(obj.UV[face.uv1]);
                    cf.uv2 = uv.IndexOf(obj.UV[face.uv2]);
                    cf.uv3 = uv.IndexOf(obj.UV[face.uv3]);

                    cf.material = face.material;

                    o.Faces.Add(cf);
                }

                o.name = obj.name + "_" + i;
                i++;

                ret.Add(o);
            }

            return ret;
        }

        bool closeEnough(Vector3 p1,Vector3 p2,float dist = 0.0001f)
        {
            return p1.DistanceTo(p2) < dist;
        }

        bool closeEnough(float p1, float p2, float dist = 0.0001f)
        {
            return Math.Abs(p1-p2) < dist;
        }

        Vector3 Solve3VarLinearEquation(Matrix3x4 coefficients)
        {
            if (closeEnough(coefficients.Determinant3x3(), 0))
            {
                try
                {
                    coefficients = new Matrix3x4(coefficients.a1 + 1, coefficients.a2 + 1, coefficients.a3 + 1, coefficients.a4, coefficients.b1 + 1, coefficients.b2 + 1, coefficients.b3 + 1, coefficients.b4, coefficients.c1 + 1, coefficients.c2 + 1, coefficients.c3 + 1, coefficients.c4);
                    return Solve3VarLinearEquation(coefficients);
                }
                catch (StackOverflowException e)
                {
                    Console.WriteLine("Unable to solve a texgen");
                    return new Vector3(0, 0, 0);
                }
            }

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

            if (closeEnough(coefficients.a1,0))
            {
                if (closeEnough(coefficients.b1, 0))
                    coefficients.SwapRows(0, 2);
                else
                    if (closeEnough(coefficients.c1, 0))
                    coefficients.SwapRows(0, 1);
            }

            if (!closeEnough(coefficients.a1,0))
            {
                if (!closeEnough(coefficients.b1,0))
                {
                    coefficients.AddRows(0, 1, -coefficients.b1 / coefficients.a1);
                }
                if (!closeEnough(coefficients.c1, 0))
                {
                    coefficients.AddRows(0, 2, -coefficients.c1 / coefficients.a1);
                }
            }

            if (closeEnough(coefficients.b2,0))
            {
                coefficients.SwapRows(1, 2);
            }

            if (!closeEnough(coefficients.b2,0))
            {
                if (!closeEnough(coefficients.c2,0))
                {
                    coefficients.AddRows(1, 2, -coefficients.c2 / coefficients.b2);
                }
            }

            if (!closeEnough(coefficients.a1,0,0.00001f))
            {
                coefficients.ScaleRow(0, 1 / coefficients.a1);
            }
            if (!closeEnough(coefficients.b2,0, 0.00001f))
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

        TexGenPlanes SolveTexGen(Vector3 p1, Vector3 p2,Vector3 p3,Vector2 uv1,Vector2 uv2,Vector2 uv3)
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
