using System.Windows;
using System.Windows.Controls;

using PlenkaWpf.VM;


namespace PlenkaWpf.View;

public partial class Tabs : UserControl, IСhangeableControl
{
    public Tabs()
    {
        InitializeComponent();
    }

    public WindowState PreferedWindowState { get; set; } = WindowState.Maximized;
    public string WindowTitle { get; set; } = "Лабораторная работа №3";
    public double? PreferedHeight { get; set; }
    public double? PreferedWidth { get; set; }
    public event IСhangeableControl.ChangingRequestHandler ChangingRequest;

    public void OnChangingRequest(UserControl newControl)
    {
        ChangingRequest?.Invoke(this, newControl);
    }
}

