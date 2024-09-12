using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lotniskowiec
{
    public partial class Form1 : Form
    {
        List<Samolot> samoloty = new List<Samolot>();
        Statek statek;
        int k = 10; //liczba przy ktorej zmieni sie pierwszenstwo
        int liczbaSamolotow = 20; //wszystkie samolotu
        Random random = new Random();

        public Form1()
        {
            InitializeComponent();

            statek = new Statek(k, liczbaSamolotow);

            //dodajemy samolotu na statku
            for (int i = 0; i < liczbaSamolotow; i++)
            {
                statek.ZmienLiczbeSamolotowNaPokladzie(1);
                samoloty.Add(new Samolot(statek, random));
            }

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer(); // Popycha symulacje do przodu - szybkość zależy od czasu
            timer.Interval = 20; // przerysowanie co 20 ms
            timer.Tick += new EventHandler(tickZegara);
            timer.Start();
        }

        //funkcja ktora wywoluje zegar
        private void tickZegara(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.FillRectangle(new SolidBrush(Color.Aqua), 0, 0, 1000, 1000);

            statek.rysuj(g);

            for (int i = 0; i < samoloty.Count; i++)
            {
                samoloty[i].rysuj(g);
            }
        }
    }
}
