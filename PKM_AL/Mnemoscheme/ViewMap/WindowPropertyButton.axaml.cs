using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using PKM_AL.Mnemoscheme.ServiceClasses;

namespace PKM_AL.Mnemoscheme.ViewMap;

public partial class WindowPropertyButton : Window, INotifyPropertyChanged
{
    private double _scaleUnitUnit;
    private ClassDevice _selectedDevice;
    public ObservableCollection<ClassDevice> DeviceList { get; set; }
    
    public WindowPropertyButton()
    {
        InitializeComponent();
        if (MainWindow.currentMainWindow != null)
        {
            DeviceList=new ObservableCollection<ClassDevice>(MainWindow.Devices);
            _selectedDevice = DeviceList.Count>0 ? DeviceList[0] : null;
            _scaleUnitUnit = 1.0;
        }
    } 
    
    public WindowPropertyButton(ClassWidget classWidget):this()
    {
        if (classWidget.BindingObjectUnit != null)
        {
            SelectedDevice = DeviceList.FirstOrDefault(dev=>dev.ID==classWidget.BindingObjectUnit.IdDevice);
        }
        DataContext = this;
    }


    public ClassDevice SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            if (Equals(value, _selectedDevice)) return;
            _selectedDevice = value;
            OnPropertyChanged();
        }
    }
    public double ScaleUnit
    {
        get => _scaleUnitUnit;
        set
        {
            if (value.Equals(_scaleUnitUnit)) return;
            _scaleUnitUnit = value;
            OnPropertyChanged();
        }
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        Button button = sender as Button ?? throw new InvalidOperationException();
        if (button.Name is "SaveBtn")
        {
            var settingsUnitObject = new ClassWidget
            {
                ScaleUnit = this.ScaleUnit,
                BindingObjectUnit = new BindingObject()
                {
                    IdDevice = SelectedDevice.ID,
                },
                IsDevicePoll = SelectedDevice.IsPoll
            };
            Tag=settingsUnitObject;
        }
        Close();
    }
    
    public new event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}