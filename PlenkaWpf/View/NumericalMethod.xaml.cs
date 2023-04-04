using System.Linq;
using System.Windows;
using System.Windows.Controls;

using PlenkaAPI;

using PlenkaWpf.VM;

using MessageBox = HandyControl.Controls.MessageBox;


namespace PlenkaWpf.View;

public partial class NumericalMethod : UserControl
{
    public NumericalMethod()
    {
        InitializeComponent();
        var vm = new NumericalMethodControlVM();
        DataContext = vm;
    }
    private void Button_Click(object sender, RoutedEventArgs e) // нарушение mvvm
    {
        if (!IsValid(MainGrid))
        {
            MessageBox.Show("Невозможно произвести расчёт, есть ошибки ввода данных", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            (DataContext as NumericalMethodControlVM)?.CalcCommand.Execute(null);
        }
    }
    private bool IsValid(DependencyObject obj)
    {
        // The dependency object is valid if it has no errors and all
        // of its children (that are dependency objects) are error-free.
        return !Validation.GetHasError(obj) &&
               LogicalTreeHelper.GetChildren(obj)
                                .OfType<DependencyObject>()
                                .All(IsValid);
    }
}

