using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    static List<int> sayilar = new List<int>();
    static List<int> ciftSayilar = new List<int>();
    static List<int> tekSayilar = new List<int>();
    static List<int> asalSayilar = new List<int>();
    static object lockObj = new object();

    static void Main()
    {
        for (int i = 1; i <= 1000000; i++)
        {
            sayilar.Add(i);
        }

        int parcaSayisi = 4;
        int parcaUzunlugu = sayilar.Count / parcaSayisi;

        List<List<int>> bolunmusListeler = new List<List<int>>();

        for (int i = 0; i < parcaSayisi; i++)
        {
            List<int> parca = sayilar.Skip(i * parcaUzunlugu).Take(parcaUzunlugu).ToList();
            bolunmusListeler.Add(parca);
        }

        Thread t1 = new Thread(() => CiftSayilariBul(bolunmusListeler));
        Thread t2 = new Thread(() => TekSayilariBul(bolunmusListeler));
        Thread t3 = new Thread(() => AsalSayilariBul(bolunmusListeler));
        Thread t4 = new Thread(() => AsalSayilariBul(bolunmusListeler));

        t1.Start();
        t2.Start();
        t3.Start();
        t4.Start();

        t1.Join();
        t2.Join();
        t3.Join();
        t4.Join();

        Console.WriteLine("Tek Sayilar: " + string.Join(", ", tekSayilar.Distinct()));
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Cift Sayilar: " + string.Join(", ", ciftSayilar.Distinct()));
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Asal Sayilar: " + string.Join(", ", asalSayilar.Distinct()));
    }

    static void CiftSayilariBul(List<List<int>> parcaliListeler)
    {
        lock (lockObj)
        {
            foreach (var parca in parcaliListeler)
            {
                foreach (int sayi in parca)
                {
                    if (sayi % 2 == 0)
                    {
                        ciftSayilar.Add(sayi);
                    }
                }
            }
        }
    }

    static void TekSayilariBul(List<List<int>> parcaliListeler)
    {
        lock (lockObj)
        {
            foreach (var parca in parcaliListeler)
            {
                foreach (int sayi in parca)
                {
                    if (sayi % 2 != 0)
                    {
                        tekSayilar.Add(sayi);
                    }
                }
            }
        }
    }

    static void AsalSayilariBul(List<List<int>> parcaliListeler)
    {
        foreach (var parca in parcaliListeler)
        {
            foreach (int sayi in parca)
            {
                int sayac = 0;
                for (int i = 2; i <= Math.Sqrt(sayi); i++)
                {
                    if (sayi % i == 0)
                    {
                        sayac++;
                        break;
                    }
                }

                if (sayac == 0 && sayi > 1)
                {
                    lock (lockObj)
                    {
                        if (!asalSayilar.Contains(sayi))
                        {
                            asalSayilar.Add(sayi);
                        }
                    }
                }
            }
        }
    }
}