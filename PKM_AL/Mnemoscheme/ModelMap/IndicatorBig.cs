using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using PKM_AL.Mnemoscheme.AbstractUnit;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewModelMap;
using TestGrathic.ViewMap;
using WindowPopertyIndicatorBig = PKM_AL.Mnemoscheme.ViewMap.WindowPopertyIndicatorBig;

namespace PKM_AL.Mnemoscheme.ModelMap;

public class IndicatorBig : AbstractControl
{
    private decimal _paramValue;
    private string _paramName = "Параметр, ед.изм.";
    private ClassWidget _settings;
    public BindingObject? BindingObject { get; set; }

    public IndicatorBig()
    {
        CreateIndicatorBig();
        DataContext = this; 
    }

    private void CreateIndicatorBig()
    {
        _settings = new ClassWidget();
        ContextMenu=CreateContextMenu(); 
    }

    public IndicatorBig(Rect bounds):this()
    {
        Canvas.SetLeft(this, bounds.X+50);
        Canvas.SetTop(this, bounds.Y+50);
    }
    
    public string ParamName
    {
        get => _paramName;
        set
        {
            if (value == _paramName) return;
            OnPropertyChanged();
        }
    }

    public decimal ParamValue
    {
        get => _paramValue;
        set
        {
            _paramValue = value;
            OnPropertyChanged();
        }
    }
   
    protected override async void MenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if(sender is not MenuItem menuItem) return;
        var tt = Parent as ItemsControl;
        switch (menuItem.Header)
        {
            case "Копировать" :
                CanvasViewModel.BufferCopiedOneUnit = new IndicatorBig(Bounds);
                break;
            case "Удалить":
                (tt?.ItemsSource as ObservableCollection<object>)?.Remove(this);
                break;
            case "Закрепить":
                IsBlocked = !IsBlocked;
                ((CheckBox)menuItem.Icon).IsChecked = IsBlocked;
                break;
            case "Свойства":
                WindowPopertyIndicatorBig windowPropertyIndicatorBig = new WindowPopertyIndicatorBig();
                await windowPropertyIndicatorBig.ShowDialog(MainWindow.currentMainWindow);
                if(windowPropertyIndicatorBig.Tag is not null) 
                    RefreshIndicatorSmall((ClassWidget)windowPropertyIndicatorBig.Tag);
                //TODO Поворот надписи.
                break;
        }
    }

    private void RefreshIndicatorSmall(ClassWidget tag)
    {
        if (tag.BindingObjectUnit != null)
        {
            BindingObject = new BindingObject();
            BindingObject.IdDevice = tag.BindingObjectUnit.IdDevice;
            BindingObject.IdParam = tag.BindingObjectUnit.IdParam;
            BindingObject.NameParam = tag.BindingObjectUnit.NameParam;
        }
    }
   
}
