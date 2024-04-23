using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MsBox.Avalonia.Enums;
using PKM_AL.Windows;

namespace PKM_AL.Classes.TransferClasses;

public class ServerClass
{
    int port = 50602;
    private List<int[]> deviceArchive;
    private TcpListener tcpListener;
    private List<byte> bytesArray = new List<byte>();
    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<int[]>));

    public ServerClass()
    {
        tcpListener = new TcpListener(IPAddress.Any, port);
        SetServer();
    }        

    public int GetPortServer()
    {
        return port;
    }

    /// <summary>
    /// Получить IP текущего хоста.
    /// </summary>
    /// <returns></returns>
    public string GetIPHost()
    {
        string ipText = string.Empty;
        string nameHost = Dns.GetHostName();
        IPAddress[] addresses = System.Net.Dns.GetHostAddresses(nameHost);
        foreach (var addrs in addresses)
        {
            byte[] addrbytes = addrs.GetAddressBytes();
            if (addrs.AddressFamily.Equals(AddressFamily.InterNetwork) && addrbytes[^1] != 1)
                ipText = addrs.ToString();
        }
        return !string.IsNullOrEmpty(ipText) ? "IP: " + ipText : "не известно";
    }


    /// <summary>
    /// Настройка и запуск сервера.
    /// </summary>
    private async void SetServer()
    {
        while (true)
        {
            try
            {
                tcpListener.Start();
                using TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                Thread.Sleep(100);
                NetworkStream stream = tcpClient.GetStream();
                bytesArray.Clear();
                Thread.Sleep(50);
                while (stream.DataAvailable)
                {
                    bytesArray.Add((byte)stream.ReadByte());
                }
                tcpListener.Stop();
                using MemoryStream msD = new MemoryStream(bytesArray.ToArray());
                deviceArchive = (List<int[]>)xmlSerializer.Deserialize(msD);
                await ClassMessage.ShowMessage(MainWindow.currentMainWindow, "Архив получен!", "Сохранение архива",
                    icon: Icon.Success);
                WindowDeviceToArchive deviceToArchive = new WindowDeviceToArchive();
                deviceToArchive.WindowShow(MainWindow.currentMainWindow);
                object obj = deviceToArchive.Tag;
                _ =Task.Run(() => ClassDeviceArchive.SaveArchiveToDB(deviceArchive, obj));
            }
            catch
            {
                port++;
                tcpListener = new TcpListener(IPAddress.Any, port);
            }
            finally
            {
                tcpListener.Stop();
            }
        }
    }
}
