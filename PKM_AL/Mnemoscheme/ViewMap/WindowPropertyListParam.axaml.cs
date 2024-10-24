using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit.Utils;
using PKM_AL.Mnemoscheme.ServiceClasses;

namespace PKM_AL.Mnemoscheme.ViewMap;

public partial class WindowPropertyListParam : ClassWindowPKM, INotifyPropertyChanged
{
    private ClassDevice _selectedDevice;
    private ClassChannel _selectedChannel;
    private ClassChannel _selectedChannelFromList;
    private ObservableCollection<ClassChannel> _channelList;
    public ObservableCollection<ClassDevice> DeviceList { get; set; }

    public ObservableCollection<ClassChannel> SelectedChannelList { get; set; } =
        new ObservableCollection<ClassChannel>();
    
    public WindowPropertyListParam()
    {
        InitializeComponent();
    }

    public WindowPropertyListParam(ClassWidget classWidget) : this()
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
            if (classWidget.BindingObjects.Count > 0)
            {
                foreach (var bind in classWidget.BindingObjects)
                {
                    ClassChannel cc = _channelList.FirstOrDefault(c => c.ID == bind.IdParam);
                   if(cc!=null) SelectedChannelList.Add(cc);                    
                }
            }
        }
        DataContext = this;
    }

    #region [Привязки свойств]

    /// <summary>
    /// Привязка к выбранному параметру в ListBox выбранных параметров.
    /// </summary>
    public ClassChannel SelectedChannelFromList
    {
        get => _selectedChannelFromList;
        set
        {
            if (Equals(value, _selectedChannelFromList)) return;
            _selectedChannelFromList = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Выбранное устройство.
    /// </summary>
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

    /// <summary>
    /// Выбранный параметр.
    /// </summary>
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

    /// <summary>
    /// Список каналов выбранного устройства.
    /// </summary>
    public ObservableCollection<ClassChannel> ChannelList
    {
        get => _channelList;
        set
        {
            if (Equals(value, _channelList)) return;
            _channelList = value;
            OnPropertyChanged();
            DellAllParam();
        }
    }
    

    #endregion
    
    #region [Commands]

    public void AddParam()
    {
        SelectedChannelList?.Add(SelectedChannel);
    }

    public void DellParam()
    {
        if(SelectedChannelList.Count>0)
            SelectedChannelList.Remove(SelectedChannelFromList);
    }

    public void DellAllParam()
    {
        if(SelectedChannelList.Count>0)
            SelectedChannelList.Clear();
    }

    #endregion

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button ?? throw new InvalidOperationException();
        if (button.Name is "SaveBtn" && SelectedChannelList?.Count > 0)
        {
            var settingsObject = new ClassWidget
            {
                BindingObjectUnit = new BindingObject
                {
                    IdDevice = SelectedDevice.ID,
                }
            };
            foreach (var channel in SelectedChannelList)
            {
                settingsObject.BindingObjects.Add
                (
                    new BindingObject
                    {
                        IdDevice = SelectedDevice.ID,
                        IdParam = channel.ID,
                        NameParam = channel.Name
                    }
                );
            }
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

    