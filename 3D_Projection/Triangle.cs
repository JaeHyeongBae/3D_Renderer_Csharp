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
    public class Triangle
    {
        public Vector<double>[] vertices;
        public Vector<double>[] texture_coordinates;
        public Bitmap texture;

        public Triangle(Vector<double> _p1, Vector<double> _p2, Vector<double> _p3, Vector<double> _pt1, Vector<double> _pt2, Vector<double> _pt3, ref Bitmap texture)
        {
            vertices = new Vector<double>[3];
            texture_coordinates = new Vector<double>[3];
            texture_coordinates[0] = _pt1;
            texture_coordinates[1] = _pt2;
            texture_coordinates[2] = _pt3;
            vertices[0] = _p1;
            vertices[1] = _p2;
            vertices[2] = _p3;
            this.texture = texture;
        }



        override public string ToString()
        {
            return vertices[0].ToString() + " " + vertices[1].ToString() + " " + vertices[2].ToString();
        }

        public void Rotate(Vector<double> origin, Vector<double> axis, double angle )
        {
            double a = origin[0];
            double b = origin[1];
            double c = origin[2];

            double u = axis[0];
            double v = axis[1];
            double w = axis[2];

            double x;
            double y;
            double z;

            double l = u * u + v * v + w * w;

            for (int i = 0; (i < 3) && (l != 0); i++) {
                x = vertices[i][0];
                y = vertices[i][1];
                z = vertices[i][2];

                vertices[i][0] = ((a * (v * v + w * w) - u * (b * v + c * w - u * x - v * y - w * z)) * (1 - Math.Cos(angle)) + l * x * Math.Cos(angle) + Math.Sqrt(l) * ((-1) * c * v + b * w - w * y + v * z) * Math.Sin(angle)) / l;
                vertices[i][1] = ((b * (u * u + w * w) - v * (a * u + c * w - u * x - v * y - w * z)) * (1 - Math.Cos(angle)) + l * y * Math.Cos(angle) + Math.Sqrt(l) * (c * u - a * w + w * x - u * z) * Math.Sin(angle)) / l;
                vertices[i][2] = ((c * (u * u + v * v) - w * (a * u + b * v - u * x - v * y - w * z)) * (1 - Math.Cos(angle)) + l * z * Math.Cos(angle) + Math.Sqrt(l) * ((-1) * b * u + a * v - v * x + u * y) * Math.Sin(angle)) / l;
            }
        }

        public void Translate(Vector<double> displacement)
        {
            for(int i=0;i<3;i++)
            {
                vertices[i] += displacement;
            }
        }

    }
}
