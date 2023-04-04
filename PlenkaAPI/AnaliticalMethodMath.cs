using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;

using PlenkaAPI;

using static System.Math;


namespace PlenkaAPI
{

    public struct ConcetrationPerTau
    {

        public double T { get; set; }
        public double Concetration { get; set; }

    }


    public record CalculationParameters
    {
        public int N { get; init; }
        public double M { get; init; }
        public double ConcetraionIn { get; init; }
        public double V { get; init; }
        public double G { get; init; }

        public double Tau
        {
            get
            {
                return V / G;
            }
        }

        public double TStop
        {
            get
            {
                return M * Tau;
            }
        }

        public double Step { get; init; }
    }


    public struct CalculationResults
    {
        public CalculationResults()
        {
            ConcetrationPerCell = new Dictionary<int, List<ConcetrationPerTau>>();
        }

        public Dictionary<int, List<ConcetrationPerTau>> ConcetrationPerCell { get; }

    }



    public class AnaliticalMethodMath // todo как-то красиво переписать все это
    {
        public AnaliticalMethodMath(CalculationParameters cp)
        {
            Cp = cp;
        }

        public CalculationResults Results { get; private set; }

        private int GetDecimalDigitsCount(double number)
        {
            var str = number.ToString(new NumberFormatInfo
                                          {NumberDecimalSeparator = ".",})
                            .Split('.');

            return str.Length == 2 ? str[1].Length : 0;
        }

        public void Calculate()
        {
            var EPS = 0.1;
            int factrorial(int x)
            {
                if (x == 1)
                {
                    return 1;
                }
                if (x == 0)
                {
                    return 1;
                }

                return x * (factrorial(x - 1));
            }

            double SumTau(double t, double tau, int N)
            {
                var sum = 0.0;

                for (int n = 1; n <= N; n++)
                {
                    sum += (1.0 / factrorial(n - 1)) * Math.Pow(t / tau, n - 1);
                }

                return sum;
            }

            double CANin(double cain, double t, double tau, int N)
            {
                return cain - SumTau(t, tau, N) * Math.Pow(Math.E, -t / tau) * cain;
            }

            double CANout(double cah, double t, double tau, int N)
            {
                return SumTau(t, tau, N) * Math.Pow(Math.E, -t / tau) * cah;
            }

            Results = new CalculationResults();

            for (int n = 1; n <= Cp.N; n++)
            {
                Results.ConcetrationPerCell[n] = new List<ConcetrationPerTau>();
                var t = 0.0;

                //вычисление при входе
                while (t < Cp.TStop)
                {
                    var conc = new ConcetrationPerTau() {Concetration = CANin(Cp.ConcetraionIn, t, Cp.Tau, n), T = t};
                    Results.ConcetrationPerCell[n].Add(conc);
                    t += Cp.Step;
                }

                // при времени остановки
                t = Cp.TStop;
                var concstop = new ConcetrationPerTau() {Concetration = CANin(Cp.ConcetraionIn, t, Cp.Tau, n), T = t};
                Results.ConcetrationPerCell[n].Add(concstop);

                //обратный ход
                var currentConc = concstop.Concetration;
                var t2 = 0.0;
                while (currentConc > EPS)
                {
                    var conc = new ConcetrationPerTau() {Concetration = CANout(concstop.Concetration, t2, Cp.Tau, n), T = t};
                    currentConc = conc.Concetration;
                    Results.ConcetrationPerCell[n].Add(conc);
                    t2 += Cp.Step;
                    t += Cp.Step;
                }

            }
        }


    #region Parameters

        public CalculationParameters Cp { get; init; }

    #endregion
    }
}
