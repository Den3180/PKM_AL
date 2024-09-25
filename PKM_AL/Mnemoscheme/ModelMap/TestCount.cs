using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Avalonia.Threading;
using TestGrathic.ViewModelMap;

namespace TestGrathic.ModelMap;

public class TestCount : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private decimal _count = 0.00M;
    private static int _instansecount = 0;
    private int _instanceNumber = 0;

    public decimal Count
    {
        get => _count;
        set
        {
            _count = value;
            OnPropertyChanged();
            if (MainViewModel.GraphicUnitObjects.Count == 0) return;
            if (MainViewModel.GraphicUnitObjects[_instanceNumber] is IndicatorBig indicatorBig)
                indicatorBig.ParamValue = decimal.Round(_count, 2);
        }
    }

    DispatcherTimer TimerSec = new DispatcherTimer();

    public TestCount()
    {
        _instanceNumber = _instansecount++;
        TimerSec.Tick += TimerSec_Tick;
        TimerSec.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        TimerSec.Start();
    }

    private void TimerSec_Tick(object? sender, EventArgs e)
    {
        Count++;
    }
}