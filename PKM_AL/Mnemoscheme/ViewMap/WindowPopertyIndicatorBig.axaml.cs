using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using PKM_AL.Mnemoscheme.ServiceClasses;

namespace PKM_AL.Mnemoscheme.ViewMap;

public partial class WindowPopertyIndicatorBig : Window, INotifyPropertyChanged
{
    private ClassDevice _selectedDevice;
    private ClassChannel _selectedChannel;
    public ObservableCollection<ClassDevice> DeviceList { get; set; }
    private ObservableCollection<ClassChannel> _channelList;
    public WindowPopertyIndicatorBig()
    {
        InitializeComponent();
    }

    public WindowPopertyIndicatorBig(ClassWidget classWidget) : this()
    {
        if (MainWindow.currentMainWindow != null)
        {
            DeviceList=new ObservableCollection<ClassDevice>(MainWindow.Devices);
            var dev = DeviceList.FirstOrDefault(dev => dev.ID == classWidget.BindingObjectUnit?.IdDevice);
            _selectedDevice = dev ?? DeviceList[0];
            if (_selectedDevice != null)
            {
                _channelList = new ObservableCollection<ClassChannel>(_selectedDevice.Channels);
                var channel = _channelList.FirstOrDefault(ch => ch.ID == classWidget.BindingObjectUnit?.IdParam);
                _selectedChannel = channel ?? _channelList[0];
            }
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
            var settingsObject = new ClassWidget
            {
                BindingObjectUnit = new BindingObject()
                {
                    IdDevice = SelectedDevice.ID,
                    IdParam = SelectedChannel.ID,
                    NameParam = SelectedChannel.Name
                }
            };
            Tag = settingsObject;
        }
        Close();
    }
    
    public new event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}