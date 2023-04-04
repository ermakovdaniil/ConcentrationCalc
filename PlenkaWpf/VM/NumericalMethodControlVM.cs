using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

using PlenkaAPI;

using PlenkaWpf.Utils;


namespace PlenkaWpf.VM
{

    internal class TableRow
    {
        public double ConcA { get; set; }
        public double ConcB { get; set; }
        public double ConcC { get; set; }
        public double T { get; set; }
    }
    /// <summary>
    ///     VM для окна исследователя
    /// </summary>
    internal class NumericalMethodControlVM : ViewModelBase

    {
        public List<int> ErrorList { get; set; }


    #region Functions

    #region Constructors

        public NumericalMethodControlVM()
        {

            // TempLineSerie = new LineSeries
            //     {Title = "Температура, °С",};
            //
            // TempLineSerie.Fill = Brushes.Transparent;
            //
            // TempSeries = new SeriesCollection
            //     {TempLineSerie,};
            //
            // NLineSerie = new LineSeries
            //     {Title = "Вязкость, Па·с",};
            //
            // NLineSerie.Fill = Brushes.Transparent;
            //
            // ConcetraionSeries = new SeriesCollection
            //     {NLineSerie,};

            IsCalculated = false;
        }

    #endregion
        
        private LineSeries buildLineSeries(string name, List<double> x, List<double> y)
        {
            if (x.Count != y.Count)
            {
                throw new ArgumentException("Количество значений x не совпадает с количеством значений y");
            }

            var newValues = new ChartValues<ObservablePoint>();

            for (var i = 0; i < x.Count; i++)
            {
                newValues.Add(new ObservablePoint(x[i], y[i]));
            }

            var ls = new LineSeries()
            {
                Values = newValues,
                Title = name
            };

            return ls;
        }

        //как же это плохо
        private List<TableRow> buildTable(NumericalResults res)
        {
            int findCountOfRows()
            {
                var max = 0;

                foreach (var c in res.ConcetrationPerCell)
                {
                    if (c.Value.Count>max)
                    {
                        max = c.Value.Count;
                    }
                }

                return max;
            }

            var max = findCountOfRows();
            var rows = new List<TableRow>();
            
            for (int i = 0; i < max; i++)
            {
                var row = new TableRow();
                row.T = res.ConcetrationPerCell.First().Value[i].T;
                row.ConcA = res.ConcetrationPerCell[0][i].Concetration;
                row.ConcB = res.ConcetrationPerCell[1][i].Concetration;
                row.ConcC = res.ConcetrationPerCell[2][i].Concetration;
                rows.Add(row);
            }

            return rows;
        }
    #endregion


    #region Properties

    #region input

        public int N { get; set; } = 3;
        public double Cain { get; set; } = 5;
        public double V { get; set; } = 10;
        public double Ca1 { get; set; } = 5;
        public double Cb1 { get; set; } = 6;
        public double Cc1 { get; set; } = 7;
        public double G { get; set; } = 5;
        public double K { get; set; } = 2;
        public double Step { get; set; } = 0.1;
        public double T { get; set; } = 20;

    #endregion

    #region Graphics
        
        /// <summary>
        ///     Серия точек концетрации
        /// </summary>
        private List<LineSeries> ConcetrationLineSeries { get; set; }

        public SeriesCollection ConcetraionSeries { get; set; }

    #endregion


        public List<TableRow> TableRows { get; set; }
        public NumericalMethodMath NumericalMethodMath { get; set; }

        private void UpdateInterfaceElelemts()
        {
            ConcetrationLineSeries = new List<LineSeries>();
            foreach (var concPercell in NumericalMethodMath.Results.ConcetrationPerCell)
            {
                var x = concPercell.Value.Select(c => c.T).ToList();
                var y = concPercell.Value.Select(c => c.Concetration).ToList();
                var name = $"Ячейка {concPercell.Key}";
                ConcetrationLineSeries.Add(buildLineSeries(name, x,y));
            }
        
            ConcetraionSeries = new SeriesCollection(){};

            foreach (var lineSeries in ConcetrationLineSeries)
            {
                ConcetraionSeries.Add(lineSeries);
            }

            TableRows = buildTable(NumericalMethodMath.Results);
            OnPropertyChanged(nameof(ConcetraionSeries));
            OnPropertyChanged(nameof(NumericalMethodMath));

        }
        
        private bool _isCalculated;

        public bool IsCalculated
        {
            get
            {
                return _isCalculated;
            }
            set
            {
                _isCalculated = value;

                if (IsCalculated)
                {
                    TabControlVisibility = Visibility.Visible;
                }
                else
                {
                    TabControlVisibility = Visibility.Hidden;
                }
            }
        }

        private Visibility _tabControlVisibility;

        public Visibility TabControlVisibility
        {
            get
            {
                return _tabControlVisibility;
            }
            set
            {
                _tabControlVisibility = value;
                OnPropertyChanged();
            }
        }

    #endregion


    #region Commands

        private RelayCommand _calcCommand;

        /// <summary>
        ///     Команда, выполняющая расчет
        /// </summary>
        public RelayCommand CalcCommand
        {
            get
            {
                return _calcCommand ??= new RelayCommand(o =>
                {
                    IsCalculated = true;

                    var cp = new NumericalParameters
                    {
                        N = N,
                        CAin = Cain,
                        V = V,
                        CA1 = Ca1,
                        CB1 = Cb1,
                        CC1 = Cc1,
                        G = G,
                        K = K,
                        Step = Step,
                        T = T
                    };
                    NumericalMethodMath = new NumericalMethodMath(cp);
                    NumericalMethodMath.Calculate();
                    OnPropertyChanged(nameof(NumericalMethodMath));
                    UpdateInterfaceElelemts();
                });
            }
        }

    #endregion
    }
}
