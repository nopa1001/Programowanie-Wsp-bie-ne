using System;
using System.Drawing;
using System.Threading;

namespace Lotniskowiec
{
    internal class Samolot
    {
        int x = -100, y = -100;
        private Statek statek;
        private Random random;

        public Samolot(Statek statek, Random random)
        {
            this.random = random;
            this.statek = statek;

            //tworzenie watku samolotu
            Thread watek = new Thread(this.lec);
            watek.IsBackground = true; //dzialaj w tle
            //start watku
            watek.Start();
        }

        private void lec()
        {
            while (true)
            {
                //watek startuje - czeka na pozwolenie na start
                statek.wystartuj();
                this.x = 70;
                this.y = 215;

                lecDo(300, this.y);

                //zlecial z lotniskowca, zwlania pas dla innych
                statek.zwolnijPas();

                //krazy w kolko parę razy
                int rundy = random.Next(5) + 5;

                for (int i = 0; i < rundy; i++)
                {
                    lecDo(300 + random.Next(700), 50 + random.Next(500));
                }

                //chce wyladowac - czeka na pozwolenie
                statek.wyladuj();

                //podchodzi do lądowania
                lecDo(300, 215);
                lecDo(70, 215);
                this.x = -100;
                this.y = -100;

                //wylądował, zwalnia pas
                statek.zwolnijPas();


                Thread.Sleep(5000 + random.Next(15000));
            }
        }

        private void lecDo(int x, int y)
        {
            while (this.x < x)
            {
                this.x += 5;
                Thread.Sleep(50);
            }

            while (this.x > x)
            {
                this.x -= 5;
                Thread.Sleep(50);
            }

            while (this.y < y)
            {
                this.y += 5;
                Thread.Sleep(50);
            }

            while (this.y > y)
            {
                this.y -= 5;
                Thread.Sleep(50);
            }
        }

        internal void rysuj(Graphics g)
        {
            g.FillEllipse(new SolidBrush(Color.Orange), x, y, 20, 20);
        }
    }
}