using System;
using System.Collections.Generic;
using System.Text;

namespace FlipGame
{
	class Flip
	{
		//Játéktér és mérete(nxn).
		private int N;
		private int Meret	{	get	{	return N * N;	}	}
		public int LegrovidebbMegoldas { get; set; }
		public bool[] Kiindulo { get; set; }
		public bool[] JatekTer { get; set; }
		public bool[] Lepesek { get; set; }
		public bool[] MegoldasLepesei { get; set; }
		public bool VanMegolas { get; set; }

		public Flip(int n)
		{
			N = n;
			LegrovidebbMegoldas = Meret;
			VanMegolas = false;
		}

		//Játéktér inicilizálása
		public void Inicializal(bool random)
		{
			//Random értékekkel kezdek
			if (random)
			{
				Random rng = new Random();
				Kiindulo = new bool[Meret];
				for (int i = 0; i < Meret; i++)
				{
					Kiindulo[i] = rng.Next(0, 2) > 0;
				}
			}
			//A menet közbeni játéktér
			JatekTer = new bool[Meret];
			Kiindulo.CopyTo(JatekTer, 0);
			//Melyeket akarom forgatni
			Lepesek = new bool[Meret];
			MegoldasLepesei = new bool[Meret];
		}

		//1 lépés
		public void Forditas(int index)
		{
			//Koordinátává alakítás
			int x = index % N;
			int y = index / N;
			//Választott
			JatekTer[index] = !JatekTer[index];
			//Bal
			if (x > 0)
				JatekTer[index-1] = !JatekTer[index-1];
			//Jobb
			if (x < N-1)
				JatekTer[index+1] = !JatekTer[index+1];
			//Felső
			if (y > 0)
				JatekTer[index-N] = !JatekTer[index-N];
			//Alsó
			if (y < N-1)
				JatekTer[index+N] = !JatekTer[index+N];
		}

		//Kényszer
		public bool IndexFelettiMezo(int index)
		{
			//Ha az első sor, akkor a kényszer nem zár ki megoldást
			if(index < N)
			{
				return true;
			}
			//Ha lentebbi sor, akkor a kényszer kizárja, hogy a felette lévő négyzet fekete legyen.
			//Mert csak akkor lehetne a felette fekete, ha a mostani is az lenne, és most akarnénk átfordítani,
			//de mivel a kényszer teljesülésést lépések után ellenőrizzük, így ennek nem kéne előfordulnia.
			return JatekTer[index-N];
		}

		//Állapottér kiírása
		public void KiirJatekTer()
		{
			StringBuilder sb = new StringBuilder();
			//Számsor
			sb.Append("    ");
			for(int i = 0; i < N; i++)
            {
				sb.Append(" " + i);
            }
			sb.Append("  \n");
			//Elősor
			sb.Append("   /");
			for (int i = 0; i < N; i++)
			{
				sb.Append("--");
			}
			sb.Append("-\\\n");
			//Adatok
			for (int i = 0; i < Meret; i++)
			{
				if (i % N == 0)
				{
					sb.Append(" " + (i / N) + " |");
				}
				sb.Append(" " + (JatekTer[i] ? 1 : 0));
				if (i % N == N-1)
				{
					sb.Append(" |\n");
				}
			}
			//Zárósor

			sb.Append("   \\");
			for (int i = 0; i < N; i++)
			{
				sb.Append("--");
			}
			sb.Append("-/\n");
			//Kiírás
			Console.Write(sb.ToString());
		}

		//Megoldás kiírása
		public void KiirLepesek()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < Meret; i++)
			{
				sb.Append(Lepesek[i] ? 1 : 0);
				if (i % 5 == 4) sb.Append('\n');
			}
			Console.Write(sb.ToString());
		}

		//Célállapot ellenőrzés
		public bool Celallapot()
		{
			foreach (var mezo in JatekTer)
			{
				if (!mezo)
				{
					return false;
				}
			}
			return true;
		}

		//Egy megoldás hossza - Ahány igaz mező van a vektorában.
		public int MegoldasHossz()
		{
			int szum = 0;
			foreach (var mezo in Lepesek)
			{
				if (mezo)
				{
					szum++;
				}
			}
			return szum;
		}

		//Rekurzív visszalépéses kényszerkielégítés
		public void Megoldas(int index)
		{
			if(index < Meret)
			{
				//Ha kielégíti a kényszert. - De ez lehet forgatással és anélkül is akár.
				if (index > -1 && Lepesek[index])
				{
					Forditas(index);
				}
				//Az IndexFellettiMező állapottának az ellenőrzése a kényszer ellenőrzésére.
				if (IndexFelettiMezo(index))
				{
					//Ha célállapot, akkor megoldás.
					if (Celallapot())
					{
						VanMegolas = true;
						////--Debug--
						//Console.WriteLine("Megold:");
						//KiirLepesek();
						//Console.WriteLine("___");
						////--Debug--
						int megoldasHossz = MegoldasHossz();
						if(megoldasHossz < LegrovidebbMegoldas)
						{
							Lepesek.CopyTo(MegoldasLepesei, 0);
							LegrovidebbMegoldas = megoldasHossz;
						}
					}
					//Ha nem, akkor lépek tovább
					else
					{
						//Ellenőrzés, hogy a végén vagyok-e.
						if (index >= Meret - 1) return;
						//Ha nem, akkor továbblépek, fordítással és anélkül is.
						Lepesek[index + 1] = true;
						Megoldas(index + 1);
						Forditas(index + 1); //Az előbbi lépés fordított egyet a következőn, így most vissza kell fordítani.
						Lepesek[index + 1] = false;
						Megoldas(index + 1);
					}
				}
				else
				{
					//Amúgy ez nem lehet megoldás, ez a szál nem megy tovább
					return;
				}
			}
		}

	}

	class Futtato
	{
		static void Main(string[] args)
		{
			int n = 5;
			Flip Jatek = new Flip(n);
			Console.WriteLine("Milyen feladatot oldjak meg?\n[random - egy random generált, megadott - a felhasználó ad meg egy feladatot]");
			string parancs = Console.ReadLine();
			if (parancs.ToLower().Equals("random"))
			{
				Jatek.Inicializal(true);
			}
			else
			{

				//Egy adott játéktér beolvasása
				Jatek.Kiindulo = new bool[n * n];
				for (int i = 0; i < n; i++)
				{
					string line = Console.ReadLine();
					for (int j = 0; j < n; j++)
					{
						Jatek.Kiindulo[i * n + j] = (line[j] == '1');
					}
				}
				Jatek.Inicializal(false);
			}
			Console.WriteLine(" - - - - - ");
			Console.WriteLine("Az feladat kezdőállapota:");
			Jatek.KiirJatekTer();
			Jatek.Megoldas(-1);
			Console.WriteLine(" - - - - - ");
			if (Jatek.VanMegolas)
			{
				Console.WriteLine("A feladatnak van megoldása.");
				Console.WriteLine(" - - - - - ");
				Console.WriteLine("A feladat legrövidebb megoldásának lépései: (a bal felső saroktól kezdve)");
				for (int i = 0; i < Jatek.MegoldasLepesei.Length; i++)
				{
					if (Jatek.MegoldasLepesei[i])
					{
						Console.WriteLine(" [" + (i / n) + "," + (i % n) + "]");
					}
				}
                Console.WriteLine(" - - - - - ");
                Console.WriteLine("A feladat megoldva a fentebbi lépésekkel:");
				for(int i = 0; i < Jatek.MegoldasLepesei.Length; i++)
                {
                    if (Jatek.MegoldasLepesei[i])
                    {
						Jatek.Forditas(i);
                    }
                }
				Jatek.KiirJatekTer();
			}
			else
			{
				Console.WriteLine("A feladatnak nincs megoldása.");
			}
			Console.WriteLine(" - - - - - ");
		}
	}
}

/*Tudok behozni kényszert:
 - Mindegyik 1x választható - inkább csak gráfkeresést erősti meg, nem kényszer.
 - A fehérek párosszor, a feketék páratlanszor fordulhatnak, ez a későbbi lépések miatt nehezebb lehet kényszernek venni.
 - Gombok nyomva - ha forgatom, megoldható-e? -> ha nem, akkor rekurzívitás miatt vissza, és nem nyomom meg a gombot
 - Miver sorba megyek 1 megkötésem lehet -> SOSE FORDÍTOK OLYAT, AMI FELETT MÁR ÉG A LÁMPA!!!! (Olyat meg mindig, ami fölött nem)
		Fontos, hogy a lámpa nem létezése is egy külön állapot -> ez esetben próbálkozok.
*/