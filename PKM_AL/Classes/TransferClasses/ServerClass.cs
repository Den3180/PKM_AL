using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    /// <summary>
    /// Получение порта сервера для передачи архива.
    /// </summary>
    /// <returns></returns>
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
                //Запуск сервера.
                tcpListener.Start();
                //Получение объекта слиента.
                using TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                Thread.Sleep(100);
                //Получение потока клиента(ожидание).
                NetworkStream stream = tcpClient.GetStream();
                //Очистка буфера.
                bytesArray.Clear();
                Thread.Sleep(50);
                //Цикл загрузки информации.
                while (stream.DataAvailable)
                {
                    bytesArray.Add((byte)stream.ReadByte());
                }
                //Остановка сервера.
                tcpListener.Stop();
                try
                {
                    using MemoryStream msD = new MemoryStream(bytesArray.ToArray());
                    deviceArchive = (List<int[]>)xmlSerializer.Deserialize(msD);

                }
                catch (Exception ex)
                {
                   Console.WriteLine(ex.Message); 
                }
                if (deviceArchive == null)
                {
                    deviceArchive = new List<int[]>();
                    List<int> ttList = new List<int>();
                    IEnumerable<byte[]> tt = bytesArray.Chunk(4);
                    foreach (var item in tt)
                    {
                        Array.Reverse(item);
                        ttList.Add(BitConverter.ToInt32(item, 0));
                    }

                    List<int> startNotes = new List<int>();
                    for(int i=0;i<ttList.Count;i++)
                    {
                        if (ttList[i]==10 && ttList[i + 1] == 31)
                        {
                            startNotes.Add(i);
                        }
                    }


                    for (int i = 0; i < startNotes.Count; i++)
                    {
                        int startNote = startNotes[i];
                        int endNote = 0;
                        int lenghtNote = 0;
                        if (i == startNotes.Count - 1)
                        {
                            endNote = ttList.Count;
                            lenghtNote = endNote - startNotes[i];
                        }
                        else
                        {
                            endNote = startNotes[i + 1];
                            lenghtNote= endNote-startNote;
                        }

                        int[] nt=new int[lenghtNote];
                        for (int j = startNote, k=0; j < endNote; j++,k++)
                        {
                            nt[k] = ttList[j];
                        }
                        deviceArchive.Add(nt);                       
                    }
                }
                await ClassMessage.ShowMessageCustom(MainWindow.currentMainWindow, "Архив получен!", "Сохранение архива",
                    icon: Icon.Success);
                WindowDeviceToArchive deviceToArchive = new WindowDeviceToArchive();
                deviceToArchive.WindowShow(MainWindow.currentMainWindow);
                object obj = deviceToArchive.Tag;
                _ =Task.Run(() => ClassDeviceArchive.SaveArchiveToDB(deviceArchive, obj));
            }
            catch(Exception ex)
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
