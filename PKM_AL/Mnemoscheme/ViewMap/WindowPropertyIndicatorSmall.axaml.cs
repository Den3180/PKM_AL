using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using PKM_AL.Mnemoscheme.ServiceClasses;

namespace PKM_AL.Mnemoscheme.ViewMap;

public partial class WindowPropertyIndicatorSmall : Window, INotifyPropertyChanged
{
    private double _currentSize;
    private ClassDevice _selectedDevice;
    private ClassChannel _selectedChannel;
    public ObservableCollection<ClassDevice> DeviceList { get; set; }
    private ObservableCollection<ClassChannel> _channelList;



    public List<double> FontSizeList { get; set; } = new List<double>() { 
       8,9,10,11,12,13,14,15,16,18,20,24,26,28,36,48,72 };
    
    public WindowPropertyIndicatorSmall()
    {
        InitializeComponent();
    }
    
    public WindowPropertyIndicatorSmall(ClassWidget classWidget):this()
    {
        //TODO Привязать объект привязки, когда будет интеграция в СОТКУ.
        _currentSize = classWidget.FontSizeUnit;
        if (MainWindow.currentMainWindow != null)
        {
            DeviceList=new ObservableCollection<ClassDevice>(MainWindow.Devices);
            _selectedDevice = DeviceList.Count>0 ? DeviceList.FirstOrDefault(dev=>dev.ID==classWidget.BindingObjectUnit?.IdDevice) : DeviceList[0];
            if (_selectedDevice != null)
            {
                _channelList = new ObservableCollection<ClassChannel>(_selectedDevice.Channels);
                _selectedChannel = _channelList.Count>0 ? _channelList.FirstOrDefault(ch=>
                    ch.ID==classWidget.BindingObjectUnit?.IdParam) : _channelList[0];
            }
        }
        DataContext = this;
    }
    
    public double CurrentSize
    {
        get => _currentSize;
        set
        {
            if (value.Equals(_currentSize)) return;
            _currentSize = value;
            OnPropertyChanged();
        }
    }
    
    public ClassDevice SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            if (Equals(value, _selectedDevice)) return;
            _selectedDevice = value;
            ChannelList = new ObservableCollection<ClassChannel>(_selectedDevice.Channels);
            SelectedChannel = ChannelList.Count>0 ? ChannelList[0] : null;
            OnPropertyChanged();
        }
    }

    public ClassChannel SelectedChannel
    {
        get => _selectedChannel;
        set
        {
            if (Equals(value, _selectedChannel)) return;
            _selectedChannel = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ClassChannel> ChannelList
    {
        get => _channelList;
        set
        {
            if (Equals(value, _channelList)) return;
            _channelList = value;
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
                FontSizeUnit = _currentSize,
                BindingObjectUnit = new BindingObject()
                {
                    IdDevice = SelectedDevice.ID,
                    IdParam = SelectedChannel.ID,
                    NameParam = SelectedChannel.Name
                }
                
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