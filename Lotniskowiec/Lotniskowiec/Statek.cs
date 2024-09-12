using System;
using System.Drawing;
using System.Threading;

namespace Lotniskowiec
{
    internal class Statek
    {
        /*
         * Statek jest obiektem decyzyjnym, ktory bedzie wybieral komu pozwolic, a komu zabronic korzystac z pasa startowego
         * 
         * Wyglada to tak:
         * Samolot informuje statek, ze chce skorzystac z pasa (startujacyNo, ladujacyNo)
         * Statek jak zobaczy, ze ktos jest chetny to sprawdza aktualne warunki i wydaje pozwolenie (podnosi semafor)
         * Samolot odbiera pozwolenie (opuszcza semafor), korzysta z pasa. Na koniec informuje statek, ze skonczyk (podnosi semafor pasa)
         * Statek opuszcza wskazany semafor (wie, ze pas jest wolny), a nastepnie leci od nowa
         * 
         */ 

        private int wszystkieSamoloty = 0;
        private int samolotyNaPokladzie = 0;
        private int startujacyNo = 0;
        private int ladujacyNo = 0;
        private int k = 0;

        //semafory, blokujace dostep
        SemaphoreSlim ladujacy = new SemaphoreSlim(0);
        SemaphoreSlim startujacy = new SemaphoreSlim(0);
        SemaphoreSlim pasStartowy = new SemaphoreSlim(0);

        public Statek(int k, int wszystkieSamoloty)
        {
            this.wszystkieSamoloty = wszystkieSamoloty;
            this.k = k;

            //rozpocznij watek statku
            Thread thread = new Thread(this.obslugaSamolotow);
            thread.IsBackground = true;
            thread.Start();
        }

        //samolot informuje, ze skonczyl dzialac i pas jest wolny
        internal void zwolnijPas()
        {
            pasStartowy.Release();
        }

        //watek samolotu
        private void obslugaSamolotow()
        {
            while(true)
            {
                //jak na pokladzie jest wiecej samolotow niz k lub gdy nikt nie chce ladowac - pozwol startowac
                if ((samolotyNaPokladzie > k && startujacyNo > 0) || (startujacyNo > 0 && ladujacyNo == 0))
                {
                    ZmienLiczbeStartujacych(-1);
                    startujacy.Release();
                    pasStartowy.Wait();
                }
                //jak odwrotnie - pozwol ladowac
                else if ((samolotyNaPokladzie <= k && ladujacyNo > 0) || (ladujacyNo > 0 && startujacyNo == 0))
                {
                    ZmienLiczbeLadujacych(-1);
                    ladujacy.Release();
                    pasStartowy.Wait();
                }

                Thread.Sleep(50);
            }
        }

        //samolot zglasza chec wyladowania
        internal void wyladuj()
        {
            ZmienLiczbeLadujacych(1); //oznacza, ze czeka na ladowanie
            this.ladujacy.Wait(); //czeka na semaforze
            ZmienLiczbeSamolotowNaPokladzie(1); //zwieksza liczbe samochodow na pokladzie
        }

        public void wystartuj()
        {
            ZmienLiczbeStartujacych(1);
            this.startujacy.Wait();
            ZmienLiczbeSamolotowNaPokladzie(-1);
        }

        public void ZmienLiczbeLadujacych(int no)
        {
            lock (this) //monitor - bo jak wielu naraz sprobuje zmodyfikowac wartosc tej zmiennej, to moga nadpisac wartosci
                //co skonczy sie zlymi wartosciami i zlym dzialaniem symulacji
            {
                ladujacyNo += no;
            }
        }

        public void ZmienLiczbeStartujacych(int no)
        {
            lock (this)
            {
                startujacyNo += no;
            }
        }

        public void ZmienLiczbeSamolotowNaPokladzie(int no)
        {
            lock(this)
            {
                samolotyNaPokladzie += no;
            }
        }

        internal void rysuj(Graphics g)
        {
            string q0 = "Samoloty w powietrzu: " + ( wszystkieSamoloty - samolotyNaPokladzie);
            g.DrawString(q0, new Font("Arial", 12), new SolidBrush(Color.Black), 20, 460);

            string q1 = "Samoloty na pokładzie: " + samolotyNaPokladzie;
            g.DrawString(q1, new Font("Arial", 12), new SolidBrush(Color.Black), 20, 480);

            string q2 = "Chetni do lądowania: " + ladujacyNo;
            g.DrawString(q2, new Font("Arial", 12), new SolidBrush(Color.Black), 20, 500);

            string q3 = "Chetni do startowania: " + startujacyNo;
            g.DrawString(q3, new Font("Arial", 12), new SolidBrush(Color.Black), 20, 520);

            string q4 = "Następne pierwszeństwo: " + (samolotyNaPokladzie > k ? "startujący" : "lądujący");
            g.DrawString(q4, new Font("Arial", 12), new SolidBrush(Color.Black), 20, 540);

            g.FillRectangle(new SolidBrush(Color.Gray), 50, 200, 200, 50);

            for (int i = 1; i < 10; i++)
            {
                g.FillRectangle(new SolidBrush(Color.White), 60 + i * 20, 202, 10, 2);
                g.FillRectangle(new SolidBrush(Color.White), 60 + i * 20, 247, 10, 2);
            }
        }
    }
}