using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// <summary>
    ///     VM для окна исследователя
    /// </summary>
    internal class AnaliticalMethodControlVM : ViewModelBase

    {
        public List<int> ErrorList { get; set; }


    #region Functions

    #region Constructors

        public AnaliticalMethodControlVM()
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

    #endregion


    #region Properties

    #region input

        public int N { get; set; } = 3;
        public double M { get; set; } = 8;
        public double CAIn { get; set; } = 20;
        public double V { get; set; } = 80;
        public double G { get; set; } = 50;
        public double Step { get; set; } = 1;

    #endregion

    #region Graphics
        
        /// <summary>
        ///     Серия точек концетрации
        /// </summary>
        private List<LineSeries> ConcetrationLineSeries { get; set; }

        public SeriesCollection ConcetraionSeries { get; set; }

    #endregion


        public ObservableCollection<ConcetrationPerTau> TableData
        {
            get
            {
                if (AnaliticalMethodMath is not null)
                {
                    return new ObservableCollection<ConcetrationPerTau>(AnaliticalMethodMath.Results.ConcetrationPerCell[N-1]);
                }

                return null;
            }
        }
        
        private AnaliticalMethodMath _analiticalMethodMath;
        public AnaliticalMethodMath AnaliticalMethodMath
        {
            get
            {
                return _analiticalMethodMath;
            }
            set
            {
                _analiticalMethodMath = value;
                OnPropertyChanged();
            }
        }

        private void UpdateInterfaceElelemts()
        {
            ConcetrationLineSeries = new List<LineSeries>();
            foreach (var concPercell in AnaliticalMethodMath.Results.ConcetrationPerCell)
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
            OnPropertyChanged(nameof(ConcetraionSeries));
            OnPropertyChanged(nameof(AnaliticalMethodMath));
            OnPropertyChanged(nameof(TableData));

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

                    var cp = new CalculationParameters()
                    {
                        ConcetraionIn = CAIn,
                        G = G,
                        M = M,
                        N = N,
                        V = V,
                        Step = Step,
                    };
                    var math =  new AnaliticalMethodMath(cp);
                    math.Calculate();
                    AnaliticalMethodMath =math;
                    OnPropertyChanged(nameof(AnaliticalMethodMath));
                    UpdateInterfaceElelemts();
                });
            }
        }

    #endregion
    }
}
