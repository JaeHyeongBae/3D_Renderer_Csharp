using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace _3D_Projection
{
    public partial class Form1 : Form
    {
        public List<SamDiObject> Scene;

        private Camera cam;
        private Bitmap framebuffer;
        private int width, height;
        private Color[,] image_buffer;
        private double movSpeed = 0.1;
        private BindingList<string> strList;
        int index = 0;

        public Form1()
        {
            InitializeComponent();
            width = 512;
            height = 512;
            Scene = new List<SamDiObject>();
            cam = new Camera(width, height,102);
            framebuffer = new Bitmap(width, height);
            strList = new BindingList<string>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private static SamDiObject makeCube(ref Bitmap texture)
        {
            List<Triangle> triList = new List<Triangle>();
            int t_width = texture.Width;
            int t_height = texture.Height;
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 0, 1, 2 }),
                     DenseVector.OfArray(new double[3] { 0, 0, 2 }),
                     DenseVector.OfArray(new double[3] { 1, 1, 2 }),
                     DenseVector.OfArray(new double[2] { 0, 0 }),
                     DenseVector.OfArray(new double[2] { 0, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, 0 }),
                     ref texture
                )
            );
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 0, 0, 2 }),
                     DenseVector.OfArray(new double[3] { 1, 0, 2 }),
                     DenseVector.OfArray(new double[3] { 1, 1, 2 }),
                     DenseVector.OfArray(new double[2] { 0, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, 0 }),
                     ref texture
                )
            );
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 0, 1, 3 }),
                     DenseVector.OfArray(new double[3] { 0, 0, 3 }),
                     DenseVector.OfArray(new double[3] { 1, 1, 3 }),
                     DenseVector.OfArray(new double[2] { 0, 0 }),
                     DenseVector.OfArray(new double[2] { 0, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, 0 }),
                     ref texture
                )
            );
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 0, 0, 3 }),
                     DenseVector.OfArray(new double[3] { 1, 0, 3 }),
                     DenseVector.OfArray(new double[3] { 1, 1, 3 }),
                     DenseVector.OfArray(new double[2] { 0, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, 0 }),
                     ref texture
                )
            );
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 0, 1, 2 }),
                     DenseVector.OfArray(new double[3] { 0, 0, 3 }),
                     DenseVector.OfArray(new double[3] { 0, 1, 3 }),
                     DenseVector.OfArray(new double[2] { 0, 0 }),
                     DenseVector.OfArray(new double[2] { t_width, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, 0 }),
                     ref texture
                )
            );
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 0, 1, 2 }),
                     DenseVector.OfArray(new double[3] { 0, 0, 2 }),
                     DenseVector.OfArray(new double[3] { 0, 0, 3 }),
                     DenseVector.OfArray(new double[2] { 0, 0 }),
                     DenseVector.OfArray(new double[2] { 0, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, t_height }),
                     ref texture
                )
            );
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 1, 1, 2 }),
                     DenseVector.OfArray(new double[3] { 1, 0, 3 }),
                     DenseVector.OfArray(new double[3] { 1, 1, 3 }),
                     DenseVector.OfArray(new double[2] { 0, 0 }),
                     DenseVector.OfArray(new double[2] { t_width, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, 0 }),
                     ref texture
                )
            );
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 1, 1, 2 }),
                     DenseVector.OfArray(new double[3] { 1, 0, 2 }),
                     DenseVector.OfArray(new double[3] { 1, 0, 3 }),
                     DenseVector.OfArray(new double[2] { 0, 0 }),
                     DenseVector.OfArray(new double[2] { 0, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, t_height }),
                     ref texture
                )
            );
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 0, 0, 2 }),
                     DenseVector.OfArray(new double[3] { 0, 0, 3 }),
                     DenseVector.OfArray(new double[3] { 1, 0, 2 }),
                     DenseVector.OfArray(new double[2] { 0, t_height }),
                     DenseVector.OfArray(new double[2] { 0, 0 }),
                     DenseVector.OfArray(new double[2] { t_width, t_height }),
                     ref texture
                )
            );
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 1, 0, 2 }),
                     DenseVector.OfArray(new double[3] { 1, 0, 3 }),
                     DenseVector.OfArray(new double[3] { 0, 0, 3 }),
                     DenseVector.OfArray(new double[2] { t_width, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, 0 }),
                     DenseVector.OfArray(new double[2] { 0, 0 }),
                     ref texture
                )
            );
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 0, 1, 2 }),
                     DenseVector.OfArray(new double[3] { 0, 1, 3 }),
                     DenseVector.OfArray(new double[3] { 1, 1, 2 }),
                     DenseVector.OfArray(new double[2] { 0, t_height }),
                     DenseVector.OfArray(new double[2] { 0, 0 }),
                     DenseVector.OfArray(new double[2] { t_width, t_height }),
                     ref texture
                )
            );
            triList.Add(
                new Triangle(
                     DenseVector.OfArray(new double[3] { 1, 1, 2 }),
                     DenseVector.OfArray(new double[3] { 1, 1, 3 }),
                     DenseVector.OfArray(new double[3] { 0, 1, 3 }),
                     DenseVector.OfArray(new double[2] { t_width, t_height }),
                     DenseVector.OfArray(new double[2] { t_width, 0 }),
                     DenseVector.OfArray(new double[2] { 0, 0 }),
                     ref texture
                )
            );
            return new SamDiObject(triList, DenseVector.OfArray(new double[3] { 0.5, 0.5, 2.5 }));

        }


        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap[] texture = new Bitmap[4];
         
            texture[0] = (Bitmap)Image.FromFile(@"c:\bmp\img_lights.bmp");
            texture[1] = (Bitmap)Image.FromFile(@"c:\bmp\bricks.bmp");
            texture[2] = (Bitmap)Image.FromFile(@"c:\bmp\image-13.bmp");
            texture[3] = (Bitmap)Image.FromFile(@"c:\bmp\Ultraviolet_image_shows_the_Sun_s_intricate_atmosp.bmp");
            
            addToScene(makeCube(ref texture[index]));
            if (index < 3) index++;
            else index = 0;
            Render();
        }

        public void addToScene(SamDiObject samdiobject)
        {
            Scene.Add(samdiobject);
            strList.Add(samdiobject.Name);
            listBox1.DataSource = strList;
        }



        private void Render()
        {
            cam.Render(Scene);
            cam.getBuffer(ref image_buffer);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    framebuffer.SetPixel(x, height - 1 - y, image_buffer[x, y]);
                }
            }
            pictureBox1.Image = framebuffer;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.W)
            {
                    Scene[listBox1.SelectedIndex].Translate(DenseVector.OfArray(new double[3] { 0, movSpeed, 0 }));
                
            }
            else if (e.KeyCode == Keys.A)
            {

                    Scene[listBox1.SelectedIndex].Translate(DenseVector.OfArray(new double[3] { -movSpeed, 0 , 0 }));
                
            }
            else if (e.KeyCode == Keys.S)
            {

                    Scene[listBox1.SelectedIndex].Translate(DenseVector.OfArray(new double[3] { 0, -movSpeed, 0 }));
                
            }
            else if (e.KeyCode == Keys.D)
            {

                    Scene[listBox1.SelectedIndex].Translate(DenseVector.OfArray(new double[3] { movSpeed, 0, 0 }));
                
            }
            else if (e.KeyCode == Keys.Q)
            {

                    Scene[listBox1.SelectedIndex].Translate(DenseVector.OfArray(new double[3] { 0, 0, -movSpeed }));
                
            }
            else if (e.KeyCode == Keys.E)
            {

                    Scene[listBox1.SelectedIndex].Translate(DenseVector.OfArray(new double[3] { 0, 0, movSpeed }));
                
            }
            else if (e.KeyCode == Keys.Z)
            {
                Scene[listBox1.SelectedIndex].Rotate(DenseVector.OfArray(new double[3] { 0, 0.1, 0 }));
            }
            else if(e.KeyCode == Keys.C)
            {
                Scene[listBox1.SelectedIndex].Rotate(DenseVector.OfArray(new double[3] { 0, -0.1, 0 }));
            }
            Render();
        }

        private void listBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
