using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;



namespace Crtanje
{
    public partial class Form1 : Form
    {
        private Point PrvaTocka = new Point(-1, -1);
        private Point DrugaTocka = new Point(-1, -1);
        private int odabir = 0; //0 nista nije odabrano, 1 odabrana je crta, 2 odabran je krug
      
        List<PairPoints> SveCrte= new List<PairPoints>();
        List<Rectangle> SviKrugovi = new List<Rectangle>();
       
        List<PairPoints> TextSveCrte = new List<PairPoints>();
        List<Rectangle> TextSviKrugovi = new List<Rectangle>();

        public Form1()
        {
            InitializeComponent();
            richTextBox1.Visible = false;
            toolStripButton1.Visible = false;
            toolStripButton2.Visible = false;   
        }

        private void obojeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Visible = true;
            toolStripButton1.Visible = true;
            toolStripButton2.Visible = true;
        }

        private void kodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Visible = true;
            toolStripButton1.Visible = false;
            toolStripButton2.Visible = false;
            odabir = 0;

        }

        private void vizualnoUredivanjeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Visible = false;
            toolStripButton1.Visible = true;
            toolStripButton2.Visible = true;
        }
        
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            odabir = 1;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            odabir = 2;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (odabir != 0)
            {
                if (PrvaTocka.Equals(new Point(-1, -1)))
                {
                    PrvaTocka = new Point(e.X, e.Y);
                }

                else
                {
                    DrugaTocka = new Point(e.X, e.Y);
                    var g = CreateGraphics();
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    if (odabir == 1)
                    {
                        g.DrawLine(Pens.IndianRed, PrvaTocka, DrugaTocka);
                        PairPoints nova = new PairPoints(PrvaTocka, DrugaTocka);
                        SveCrte.Add(nova);
                    }
                    else
                    {
                        double radius = Math.Sqrt((PrvaTocka.X - e.X) * (PrvaTocka.X - e.X) + (PrvaTocka.Y - e.Y) * (PrvaTocka.Y - e.Y));
                        Rectangle rect = new Rectangle(PrvaTocka.X - (int)radius, PrvaTocka.Y - (int)radius, 2 * (int)radius, 2 * (int)radius);
                        g.DrawEllipse(Pens.IndianRed, rect);
                        SviKrugovi.Add(rect);
                    }

                    PrvaTocka = new Point(-1, -1);
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            int x1 = 1, y1 = 1, x2 = 1, y2 = 1, r = 1;
            TextSveCrte.Clear();
            TextSviKrugovi.Clear();

            var g = CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.Invalidate();

            Regex rgCrta = new Regex(@"Crta\s*\(\s*[0-9]+\s*,\s*[0-9]+\s*,\s*[0-9]+\s*,\s*[0-9]+\s*\)\s*", RegexOptions.IgnoreCase);
            Regex rgKrug = new Regex(@"Krug\s*\(\s*[0-9]+\s*,\s*[0-9]+\s*,\s*[0-9]+\s*\)\s*", RegexOptions.IgnoreCase);
            //iteracija kroz linije
            foreach (string str in richTextBox1.Lines)
            {
                if (rgCrta.IsMatch(str))
                {
                    string[] brojke = Regex.Split(str, @"\D+");
                    
                    int.TryParse(brojke[1], out x1);
                    int.TryParse(brojke[2], out y1);
                    int.TryParse(brojke[3], out x2);
                    int.TryParse(brojke[4], out y2);

                    Point prva_ = new Point(x1, y1);
                    Point druga_ = new Point(x2, y2);
                    
                    g.DrawLine(Pens.IndianRed, prva_, druga_);
                    PairPoints prva_i_druga = new PairPoints(prva_, druga_);
                    if (!(TextSveCrte.Contains(prva_i_druga))) TextSveCrte.Add(prva_i_druga);
                }

                if (rgKrug.IsMatch(str))
                {
                    string[] brojke = Regex.Split(str, @"\D+");

                    int.TryParse(brojke[1], out x1);
                    int.TryParse(brojke[2], out y1);
                    int.TryParse(brojke[3], out r);

                    Rectangle rect = new Rectangle(x1, y1, r*2, r*2);

                    g.DrawEllipse(Pens.IndianRed, rect);
                    if (!(TextSviKrugovi.Contains(rect))) TextSviKrugovi.Add(rect);
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (PairPoints crta in SveCrte)
            {
                e.Graphics.DrawLine(Pens.IndianRed, crta.prva, crta.druga);
            }
            foreach(Rectangle krug in SviKrugovi)
            {
                e.Graphics.DrawEllipse(Pens.IndianRed, krug);
            }
            foreach (PairPoints crta in TextSveCrte)
            {
                e.Graphics.DrawLine(Pens.IndianRed, crta.prva, crta.druga);
            }
            foreach (Rectangle krug in TextSviKrugovi)
            {
                e.Graphics.DrawEllipse(Pens.IndianRed, krug);
            }
        }
    }
}

class PairPoints
{
    public Point prva;
    public Point druga;
    public PairPoints(Point prva, Point druga)
    {
        this.prva = prva;
        this.druga = druga;
    }
}
