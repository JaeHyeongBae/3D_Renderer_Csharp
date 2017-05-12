using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Drawing;

namespace _3D_Projection
{
    public class Vertex
    {
        public Vertex()
        {

        }
        public Vertex(Vertex v)
        {
            point2d.X = v.point2d.X;
            point2d.Y = v.point2d.Y;
            z = v.z;
            zr = v.zr;
            uz = v.uz;
            vz = v.vz;
            depth = v.depth;
        }
        public Point point2d = new Point();
        public double depth;
        public double z;
        public double zr;
        public double uz;
        public double vz;
    }

    public class Camera
    {
        private int width;
        private int height;
        private double hmwidth;

        private double[,] z_buffer;
        private Color[,] image_buffer;

        public Camera(int _width, int _height, float fov)
        {
            z_buffer = new double[_width, _height];
            image_buffer = new Color[_width, _height];
            width = _width;
            height = _height;
            hmwidth = Math.Tan(Math.PI * (fov / 2) / 180.0);
            for (int i = 0; i < _width; i++)
                for (int j = 0; j < _height; j++)
                {
                    z_buffer[i, j] = Double.PositiveInfinity;
                    image_buffer[i, j] = Color.Black;
                }
        }

        public void ClearBuffers()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    z_buffer[i, j] = Double.PositiveInfinity;
                    image_buffer[i, j] = Color.Black;
                }
        }

        public void getBuffer(ref Color[,] buffer)
        {
            buffer = image_buffer;
        }

        public void Render(List<SamDiObject> scene)
        {
            ClearBuffers();
            foreach (SamDiObject samdiobject in scene)
            {
                foreach (Triangle triangle in samdiobject.getTriangles())
                {
                    Vertex[] vertex = new Vertex[3];
                    for (int i = 0; i < 3; i++) vertex[i] = new Vertex();
                    for (int i = 0; i < 3; i++)
                    {
                        double[] point = { triangle.vertices[i][0] / triangle.vertices[i][2], triangle.vertices[i][1] / triangle.vertices[i][2] };
                        int x_coord = map(point[0], -hmwidth, hmwidth, 0, width - 1);
                        int y_coord = map(point[1], -hmwidth, hmwidth, 0, height - 1);
                        vertex[i].point2d = new Point(x_coord, y_coord);
                        vertex[i].z = triangle.vertices[i][2];
                        vertex[i].depth = Math.Sqrt(Math.Pow(triangle.vertices[i][0], 2) + Math.Pow(triangle.vertices[i][1], 2) + Math.Pow(triangle.vertices[i][2], 2));
                        vertex[i].zr = 1 / triangle.vertices[i][2];
                        vertex[i].uz = triangle.texture_coordinates[i][0] / vertex[i].z;
                        vertex[i].vz = triangle.texture_coordinates[i][1] / vertex[i].z;
                    }


                    Sort(ref vertex);
                    Point v1 = vertex[0].point2d;
                    Point v2 = vertex[1].point2d;
                    Point v3 = vertex[2].point2d;
                    if (v2.Y == v3.Y)
                    {
                        fillBottomFlatTriangle(vertex, ref triangle.texture);
                    }
                    /* check for trivial case of top-flat triangle */
                    else if (v1.Y == v2.Y)
                    {
                        fillTopFlatTriangle(vertex, ref triangle.texture);
                    }
                    else
                    {
                        /* general case - split the triangle in a topflat and bottom-flat one */
                        Vertex vert4 = new Vertex();
                        vert4.point2d = new Point((int)(v1.X + (v2.Y - v1.Y) / (double)(v3.Y - v1.Y) * (v3.X - v1.X)), v2.Y);
                        vert4.depth = vertex[0].depth + ((vertex[2].depth - vertex[0].depth) / (v3.Y - v1.Y)) * (vert4.point2d.Y - v1.Y);
                        vert4.z = vertex[0].z + ((vertex[2].z - vertex[0].z) / (v3.Y - v1.Y)) * (vert4.point2d.Y - v1.Y);
                        vert4.zr = vertex[0].zr + ((vertex[2].zr - vertex[0].zr) / (v3.Y - v1.Y)) * (vert4.point2d.Y - v1.Y);
                        vert4.uz = vertex[0].uz + ((vertex[2].uz - vertex[0].uz) / (v3.Y - v1.Y)) * (vert4.point2d.Y - v1.Y);
                        vert4.vz = vertex[0].vz + ((vertex[2].vz - vertex[0].vz) / (v3.Y - v1.Y)) * (vert4.point2d.Y - v1.Y);
                        Vertex[] newvert = new Vertex[3];
                        for (int i = 0; i < 3; i++)
                        {
                            newvert[i] = new Vertex(vertex[i]);
                        }
                        newvert[2] = new Vertex(vert4);
                        Sort(ref newvert);
                        fillBottomFlatTriangle(newvert, ref triangle.texture);

                        newvert[0] = new Vertex(vertex[1]);
                        newvert[2] = new Vertex(vertex[2]);
                        newvert[1] = new Vertex(vert4);
                        Sort(ref newvert);
                        fillTopFlatTriangle(newvert, ref triangle.texture);
                    }
                }
            }
        }

        public void fillTopFlatTriangle(Vertex[] vertex, ref Bitmap texture)
        {
            Point v1 = vertex[0].point2d;
            Point v2 = vertex[1].point2d;
            Point v3 = vertex[2].point2d;
            double invslope1 = (v3.X - (double)v1.X) / (v3.Y - (double)v1.Y);
            double invslope2 = (v3.X - (double)v2.X) / (v3.Y - (double)v2.Y);
            double depthslope1 = (vertex[2].depth - vertex[0].depth) / (v3.Y - v1.Y);
            double depthslope2 = (vertex[2].depth - vertex[1].depth) / (v3.Y - v2.Y);
            double zslope1 = (vertex[2].z - vertex[0].z) / (v3.Y - v1.Y);
            double zslope2 = (vertex[2].z - vertex[1].z) / (v3.Y - v2.Y);
            double zrslope1 = (vertex[2].zr - vertex[0].zr) / (v3.Y - v1.Y);
            double zrslope2 = (vertex[2].zr - vertex[1].zr) / (v3.Y - v2.Y);
            double uzslope1 = (vertex[2].uz - vertex[0].uz) / (v3.Y - v1.Y);
            double uzslope2 = (vertex[2].uz - vertex[1].uz) / (v3.Y - v2.Y);
            double vzslope1 = (vertex[2].vz - vertex[0].vz) / (v3.Y - v1.Y);
            double vzslope2 = (vertex[2].vz - vertex[1].vz) / (v3.Y - v2.Y);

            double curx1 = v3.X;
            double curx2 = v3.X;

            double curdepth1 = vertex[2].depth;
            double curdepth2 = vertex[2].depth;

            double curz1 = vertex[2].z;
            double curz2 = curz1;

            double curzr1 = vertex[2].zr;
            double curzr2 = curzr1;

            double curuz1 = vertex[2].uz;
            double curuz2 = curuz1;

            double curvz1 = vertex[2].vz;
            double curvz2 = curvz1;

            for (int scanlineY = v3.Y; scanlineY > v1.Y; scanlineY--)
            {
                double hdepthslope = 0, hzslope = 0, hzrslope = 0, huzslope = 0, hvzslope = 0;
                if (Math.Abs(curx1 - curx2) >= 1)
                {
                    hdepthslope = (curdepth2 - curdepth1) / (curx2 - curx1);
                    hzslope = (curz2 - curz1) / (curx2 - curx1);
                    hzrslope = (curzr2 - curzr1) / (curx2 - curx1);
                    huzslope = (curuz2 - curuz1) / (curx2 - curx1);
                    hvzslope = (curvz2 - curvz1) / (curx2 - curx1);
                }
                for (int x = (int)curx1; x <= curx2 && (x >= 0) && (x < width) && (scanlineY >= 0) && (scanlineY < height); x++)
                {
                    double newDepth = curdepth1 + hdepthslope * (x - (int)curx1);
                    double newZ = curz1 + hzslope * (x - curx1);
                    double newZr = curzr1 + hzrslope * (x - curx1);
                    double newUZ = curuz1 + huzslope * (x - curx1);
                    double newVZ = curvz1 + hvzslope * (x - curx1);
                    if (newDepth < z_buffer[x, scanlineY])
                    {
                        z_buffer[x, scanlineY] = newDepth;
                        int u = (int)(newUZ * (1 / newZr));
                        int v = (int)(newVZ * (1 / newZr));
                        if (u > 0 && u < texture.Width && v > 0 && v < texture.Height)
                            image_buffer[x, scanlineY] = texture.GetPixel(u, v);
                    }
                }
                curx1 -= invslope1;
                curx2 -= invslope2;

                curdepth1 -= depthslope1;
                curdepth2 -= depthslope2;

                curz1 -= zslope1;
                curz2 -= zslope2;

                curzr1 -= zrslope1;
                curzr2 -= zrslope2;

                curuz1 -= uzslope1;
                curuz2 -= uzslope2;

                curvz1 -= vzslope1;
                curvz2 -= vzslope2;
            }
        }

        public void fillBottomFlatTriangle(Vertex[] vertex, ref Bitmap texture)
        {
            Point v1 = vertex[0].point2d;
            Point v2 = vertex[1].point2d;
            Point v3 = vertex[2].point2d;
            double invslope1 = (v2.X - v1.X) / (double)(v2.Y - v1.Y);
            double invslope2 = (v3.X - v1.X) / (double)(v3.Y - v1.Y);
            double depthslope1 = (vertex[1].depth - vertex[0].depth) / (v2.Y - v1.Y);//Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2));
            double depthslope2 = (vertex[2].depth - vertex[0].depth) / (v3.Y - v1.Y); //Math.Sqrt(Math.Pow(v3.X - v1.X, 2) + Math.Pow(v3.Y - v1.Y, 2));
            double zslope1 = (vertex[1].z - vertex[0].z) / (v2.Y - v1.Y);
            double zslope2 = (vertex[2].z - vertex[0].z) / (v3.Y - v1.Y);
            double zrslope1 = (vertex[1].zr - vertex[0].zr) / (v2.Y - v1.Y);
            double zrslope2 = (vertex[2].zr - vertex[0].zr) / (v3.Y - v1.Y);
            double uzslope1 = (vertex[1].uz - vertex[0].uz) / (v2.Y - v1.Y);
            double uzslope2 = (vertex[2].uz - vertex[0].uz) / (v3.Y - v1.Y);
            double vzslope1 = (vertex[1].vz - vertex[0].vz) / (v2.Y - v1.Y);
            double vzslope2 = (vertex[2].vz - vertex[0].vz) / (v3.Y - v1.Y);

            double curx1 = v1.X;
            double curx2 = v1.X;

            double curdepth1 = vertex[0].depth;
            double curdepth2 = vertex[0].depth;

            double curz1 = vertex[0].z;
            double curz2 = curz1;

            double curzr1 = vertex[0].zr;
            double curzr2 = curzr1;

            double curuz1 = vertex[0].uz;
            double curuz2 = curuz1;

            double curvz1 = vertex[0].vz;
            double curvz2 = curvz1;

            for (int scanlineY = v1.Y; scanlineY <= v2.Y; scanlineY++)
            {
                double hdepthslope = 0, hzslope = 0, hzrslope = 0, huzslope = 0, hvzslope = 0;
                if (Math.Abs(curx1 - curx2) >= 1)
                {
                    hdepthslope = (curdepth2 - curdepth1) / (curx2 - curx1);
                    hzslope = (curz2 - curz1) / (curx2 - curx1);
                    hzrslope = (curzr2 - curzr1) / (curx2 - curx1);
                    huzslope = (curuz2 - curuz1) / (curx2 - curx1);
                    hvzslope = (curvz2 - curvz1) / (curx2 - curx1);
                }
                for (int x = (int)curx1; (x <= curx2) && (x >= 0) && (x < width) && (scanlineY >= 0) && (scanlineY < height); x++)
                {
                    double newDepth = curdepth1 + hdepthslope * (x - curx1);
                    double newZ = curz1 + hzslope * (x - curx1);
                    double newZr = curzr1 + hzrslope * (x - curx1);
                    double newUZ = curuz1 + huzslope * (x - curx1);
                    double newVZ = curvz1 + hvzslope * (x - curx1);
                    if (newDepth < z_buffer[x, scanlineY])
                    {
                        z_buffer[x, scanlineY] = newDepth;
                        int u = (int)(newUZ * (1 / newZr));
                        int v = (int)(newVZ * (1 / newZr));
                        if (u > 0 && u < texture.Width && v > 0 && v < texture.Height)
                            image_buffer[x, scanlineY] = texture.GetPixel((int)(newUZ * (1 / newZr)), (int)(newVZ * (1 / newZr)));
                    }
                }
                curx1 += invslope1;
                curx2 += invslope2;

                curdepth1 += depthslope1;
                curdepth2 += depthslope2;

                curz1 += zslope1;
                curz2 += zslope2;

                curzr1 += zrslope1;
                curzr2 += zrslope2;

                curuz1 += uzslope1;
                curuz2 += uzslope2;

                curvz1 += vzslope1;
                curvz2 += vzslope2;
            }
        }
        public static Vertex[] Sort(ref Vertex[] vertex)
        {
            Vertex vtemp;
            for (int loop = 0; loop < 2; loop++)
            {
                for (int i = 0; i < 2 - loop; i++)
                {
                    if (vertex[i].point2d.Y > vertex[i + 1].point2d.Y)
                    {
                        vtemp = new Vertex(vertex[i]);
                        vertex[i] = new Vertex(vertex[i + 1]);
                        vertex[i + 1] = new Vertex(vtemp);

                    }
                    else if (vertex[i].point2d.Y == vertex[i + 1].point2d.Y)
                    {
                        if (vertex[i].point2d.X > vertex[i + 1].point2d.X)
                        {
                            vtemp = new Vertex(vertex[i]);
                            vertex[i] = new Vertex(vertex[i + 1]);
                            vertex[i + 1] = new Vertex(vtemp);
                        }
                    }
                }
            }

            return vertex;
        }



        public static int map(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            return (int)(toLow + ((value - fromLow) / (fromHigh - fromLow)) * (toHigh - toLow));
        }

        public static bool PointInTriangle(Point p, Point p0, Point p1, Point p2)
        {
            var s = p0.Y * p2.X - p0.X * p2.Y + (p2.Y - p0.Y) * p.X + (p0.X - p2.X) * p.Y;
            var t = p0.X * p1.Y - p0.Y * p1.X + (p0.Y - p1.Y) * p.X + (p1.X - p0.X) * p.Y;

            if ((s < 0) != (t < 0))
                return false;

            var A = -p1.Y * p2.X + p0.Y * (p2.X - p1.X) + p0.X * (p1.Y - p2.Y) + p1.X * p2.Y;
            if (A < 0.0)
            {
                s = -s;
                t = -t;
                A = -A;
            }
            return s > 0 && t > 0 && (s + t) <= A;
        }

    }
}
