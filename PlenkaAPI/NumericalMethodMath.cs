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

    // public struct ConcetrationPerTau
    // {
    //
    //     public double T { get; set; }
    //     public double Concetration { get; set; }
    //
    // }


    public record NumericalParameters
    {
        public int N { get; init; } = 1;
        public double CAin { get; set; } = 1;
        public double CBin { get; set; } = 0;
        public double CCin { get; set; } = 0;
        public double V { get; set; }
        public double CA1 { get; set; }
        public double CB1 { get; set; }
        public double CC1 { get; set; }
        public double G { get; set; }
        public double K { get; set; }
        public double Step { get; set; }
        public double T { get; set; }

        public double Tau
        {
            get
            {
                return V / (N / G);
            }
        }

    }


    public struct NumericalResults
    {
        public NumericalResults()
        {
            ConcetrationPerCell = new Dictionary<int, List<ConcetrationPerTau>>();
        }

        public Dictionary<int, List<ConcetrationPerTau>> ConcetrationPerCell { get; }

    }



    public class NumericalMethodMath // todo как-то красиво переписать все это
    {
        public NumericalMethodMath(NumericalParameters cp)
        {
            Cp = cp;
        }

        public NumericalResults Results { get; private set; }

        private int GetDecimalDigitsCount(double number)
        {
            var str = number.ToString(new NumberFormatInfo
                                          {NumberDecimalSeparator = ".",})
                            .Split('.');

            return str.Length == 2 ? str[1].Length : 0;
        }

        public void Calculate()
        {
            //украдено у антона и минимально переработано
            Results = new NumericalResults();

            // var ConcentrationComponentA = new List<double>();
            // var ConcentrationComponentB = new List<double>();
            // var ConcentrationComponentC = new List<double>();

            // ConcentrationComponentA.Add(Cp.CA1);
            // ConcentrationComponentB.Add(Cp.CB1);
            // ConcentrationComponentC.Add(Cp.CC1);

            Results.ConcetrationPerCell[0] = new List<ConcetrationPerTau>();
            Results.ConcetrationPerCell[1] = new List<ConcetrationPerTau>();
            Results.ConcetrationPerCell[2] = new List<ConcetrationPerTau>();
            
            Results.ConcetrationPerCell[0].Add(new ConcetrationPerTau(){Concetration = Cp.CA1, T = 0,});
            Results.ConcetrationPerCell[1].Add(new ConcetrationPerTau(){Concetration = Cp.CB1, T = 0,});
            Results.ConcetrationPerCell[2].Add(new ConcetrationPerTau(){Concetration = Cp.CC1, T = 0,});
            
            var CA1 = Cp.CA1;
            var CB1 = Cp.CB1;
            var CC1 = Cp.CC1;

            for (var t = Cp.Step; t <= Cp.T; t += Cp.Step)
            {
                
                CA1 += Cp.Step * ((1 / Cp.Tau) * (Cp.CAin - CA1) -
                                  Cp.K * CA1);
                
                CB1 += Cp.Step * (1 / Cp.Tau * (Cp.CBin - CB1) +
                                  3 * Cp.K * CA1);
                
                CC1 += Cp.Step * (1 / Cp.Tau * (Cp.CCin - CC1) +
                                  2 * Cp.K * CA1);

                // ConcentrationComponentA.Add(CA1);
                // ConcentrationComponentB.Add(CB1);
                // ConcentrationComponentC.Add(CC1);

                Results.ConcetrationPerCell[0].Add(new ConcetrationPerTau(){Concetration = CA1, T = t,});
                Results.ConcetrationPerCell[1].Add(new ConcetrationPerTau(){Concetration = CB1, T = t,});
                Results.ConcetrationPerCell[2].Add(new ConcetrationPerTau(){Concetration = CC1, T = t,});
            }

            // ConcentrationA = new ChartValues<decimal>(ConcentrationComponentA);
            // ConcentrationB = new ChartValues<decimal>(ConcentrationComponentB);
            // ConcentrationC = new ChartValues<decimal>(ConcentrationComponentC);
            
        }


    #region Parameters

        public NumericalParameters Cp { get; init; }

    #endregion
    }
}
