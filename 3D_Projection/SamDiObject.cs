using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace _3D_Projection
{
    public class SamDiObject
    {
        private List<Triangle> triangles;
        private Vector<double> Position;
        private Vector<double> Rotation;
        public string Name = "삼디오브젝트";


        public SamDiObject(List<Triangle> triangle_list, Vector position, Vector rotation, string name)
        {
            triangles = triangle_list;
            Position = position;
            Rotation = rotation;
            Name = name;
        }
        public SamDiObject(List<Triangle> triangle_list, Vector position, string name)
        {
            triangles = triangle_list;
            Position = position;
            Rotation = DenseVector.OfArray(new double[3] { 0, 0, 0 });
            Name = name;
        }
        public SamDiObject(List<Triangle> triangle_list, Vector position)
        {
            triangles = triangle_list;
            Position = position;
            Rotation = DenseVector.OfArray(new double[3] { 0, 0, 0 });
            Name += position.ToString();
        }

        public List<Triangle> getTriangles()
        {
            return triangles;
        }
        public Vector<double> getPostion()
        {
            return Position;
        }

        public void Translate(Vector displacement)
        {
            
            for (int tri_count=0;tri_count<triangles.Count;tri_count++)
            {
                triangles[tri_count].Translate(displacement);
            }
            Position += displacement;
        }

        public void setPosition(Vector destination)
        {
            for (int tri_count = 0; tri_count < triangles.Count; tri_count++)
            {
                triangles[tri_count].Translate(Position - destination);
            }
            Position = destination;
        }

        public void Rotate(Vector rotation)
        {
            for (int tri_count = 0; tri_count < triangles.Count; tri_count++)
            {
                triangles[tri_count].Rotate(Position, DenseVector.OfArray(new double[3] { 1, 0, 0 }), rotation[0]);
                triangles[tri_count].Rotate(Position, DenseVector.OfArray(new double[3] { 0, 1, 0 }), rotation[1]);
                triangles[tri_count].Rotate(Position, DenseVector.OfArray(new double[3] { 0, 0, 1 }), rotation[2]);
            }
            Rotation += rotation;
        }

        public void setRotation(Vector rotation)
        {
            for (int tri_count = 0; tri_count < triangles.Count; tri_count++)
            {
                triangles[tri_count].Rotate(Position, DenseVector.OfArray(new double[3] { 1, 0, 0 }), rotation[0] - Rotation[0]);
                triangles[tri_count].Rotate(Position, DenseVector.OfArray(new double[3] { 0, 1, 0 }), rotation[1] - Rotation[1]);
                triangles[tri_count].Rotate(Position, DenseVector.OfArray(new double[3] { 0, 0, 1 }), rotation[2] - Rotation[2]);
            }
            Rotation = rotation;
        }
    }
}
