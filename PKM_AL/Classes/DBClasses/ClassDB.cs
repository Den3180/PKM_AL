using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using MySql.Data;
using SQLitePCL;
using Npgsql;
using Org.BouncyCastle.Bcpg;
using System.Data;
using MsBox.Avalonia.Enums;
using PKM_AL;
using PKM_AL.Classes;
using Icon = System.Drawing.Icon;

namespace PKM
{
    public class ClassDB
    {
        public const int Version = 7;
        public static string AreaName { get; set;} = "Технопром";        
        private static SqliteConnection conn;

        #region [DB]

        /// <summary>
        /// Создание базы данных SQLlite.
        /// </summary>
        /// <param name="PathDB"></param>
        /// <returns></returns>
        public static bool Create(string PathDB)
        {
            if (File.Exists(PathDB))
            {
                File.Delete(PathDB);
            }
            //Класс базы данных.
            SqliteConnectionStringBuilder csb = new SqliteConnectionStringBuilder();
            //Путь к базе.
            csb.DataSource = PathDB;
            //Режим базы данных.
            csb.Mode = SqliteOpenMode.ReadWriteCreate;
            //Создание подключения к базе данных.
            conn = new SqliteConnection(csb.ConnectionString);
            //Открытие соединения с базой.
            try { conn.Open(); }
            //Выход при появлении исключения.
            catch { return false; }
            //Создание объекта команды.
            SqliteCommand cmd = conn.CreateCommand();
            //Создать таблицу info.
            cmd.CommandText = "CREATE TABLE info(version INTEGER, lpu TEXT)";
            //Выполнить команду.
            cmd.ExecuteNonQuery();            
            //Всавить в таблицу info версию БД.
            cmd.CommandText = "INSERT INTO info (version) VALUES(" + Version.ToString() + ")";
            cmd.ExecuteNonQuery();
            //Создать таблицу dev.
            cmd.CommandText = "CREATE TABLE dev(name TEXT, prot INTEGER, ip_adr TEXT, ip_port INTEGER,"
                + " address INTEGER, period INTEGER, port TEXT, sim TEXT, model INTEGER, lat REAL, longt REAL, elev REAL, picket TEXT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу reg.
            cmd.CommandText = "CREATE TABLE reg(name TEXT, dev INTEGER, type INTEGER, adr INTEGER,"
                + " format INTEGER, k NUMERIC, vmax NUMERIC, vmin NUMERIC, rec INTEGER, val NUMERIC,"
                + " dt TEXT, ext INTEGER, accuracy INTEGER, nval NUMERIC)";
            cmd.ExecuteNonQuery();
            //Создать таблицу map.
            cmd.CommandText = "CREATE TABLE map(name TEXT, json TEXT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу obj.
            cmd.CommandText = "CREATE TABLE obj(type INTEGER, map INTEGER, x INTEGER, y INTEGER,"
                + " prop TEXT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу cmd.
            cmd.CommandText = "CREATE TABLE cmd(name TEXT, type INTEGER, dev INTEGER, adr INTEGER,"
                + " val INTEGER, period INTEGER)";
            cmd.ExecuteNonQuery();
            //Создать таблицу link.
            cmd.CommandText = "CREATE TABLE link(event INTEGER, src INTEGER, cmd INTEGER)";
            cmd.ExecuteNonQuery();
            //Создать таблицу user.
            cmd.CommandText = "CREATE TABLE user(name TEXT, login TEXT, pass TEXT, grant INT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу log.
            cmd.CommandText = "CREATE TABLE log(type INTEGER, dt TEXT, param TEXT, val TEXT,"
                + " ack INTEGER, namdev TEXT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу mes.
            cmd.CommandText = "CREATE TABLE mes(type INTEGER, buf BLOB)";
            cmd.ExecuteNonQuery();
            //Создать таблицу sub.
            cmd.CommandText = "CREATE TABLE sub (var TEXT, val INTEGER, txt TEXT, fcolor INTEGER,"
                + " bcolor INTEGER)";
            cmd.ExecuteNonQuery();
            //Создать таблицу az.
            cmd.CommandText = "CREATE TABLE az (Identity TEXT,Location TEXT, TypeOfConstruction TEXT,"
                + "ElectrodeMaterial TEXT, CurrentLoad REAL, DissolutionRate REAL, SurfaceArea REAL, WeightAZ REAL, LenghtAZ INTEGER,"
                + "Diagonal INTEGER, MassAssembly REAL, MountingMethod TEXT, ServiceLife INTEGER, KMA TEXT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу ehz1.
            cmd.CommandText = "CREATE TABLE ehz1 (daterec TEXT, namelpu TEXT, nameobj TEXT, km TEXT, ukz INTEGER, skz INTEGER, typeskz TEXT," +
                " dateinskz TEXT, methodground TEXT, typeground TEXT, elmeter TEXT, typecontrol TEXT, bsz TEXT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу ehz4.
            cmd.CommandText = "CREATE TABLE ehz4(daterec TEXT, namelpu TEXT, nameobj TEXT, upz TEXT, lastrepair TEXT, location TEXT," +
                " typeprotect TEXT, numprotect INTEGER, startupz TEXT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу ehz6.
            cmd.CommandText = "CREATE TABLE ehz6(daterec TEXT, namelpu TEXT, nameobj TEXT, actpit TEXT, datepit TEXT, km TEXT, gps TEXT, " +
                "lenghtpit INTEGER, dnar INTEGER, thickness INTEGER, reasonpit TEXT, deep REAL, typeground TEXT, soilresi INTEGER, " +
                "insulatiomat TEXT, adhesiomat TEXT, damagearea INTEGER, nutcorrdamage TEXT, maxdeepdamage INTEGER, damage1mm INTEGER," +
                "damage13 INTEGER, damage3 INTEGER, ute TEXT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу ehz8q.
            cmd.CommandText = "CREATE TABLE ehz8q(daterec TEXT, namelpu TEXT, nametube TEXT, km TEXT, nameroad TEXT, typeroad TEXT," +
                " datemeasure TEXT, ustart NUMERIC, upatr NUMERIC, usvech NUMERIC, uend NUMERIC, upatrend NUMERIC, usvechend NUMERIC, " +
                "resis INTEGER, note TEXT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу f02year.
            cmd.CommandText = "CREATE TABLE f02year(daterec TEXT,namelpu TEXT,namejbj TEXT, km REAL, typeehz TEXT, numukz TEXT, " +
                "stateukz TEXT,tokukz REAL,numskz TEXT,tokmedskz REAL,uskz REAL,azresist REAL,numupz TEXT,stateupz TEXT,tokupz REAL," +
                "numudz TEXT,stateudz TEXT,tokudz REAL)";
            cmd.ExecuteNonQuery();
            //Создать таблицу f141gas.
            cmd.CommandText = "CREATE TABLE f141gas (datarec TEXT,namelpu TEXT,nameobj TEXT,number INTEGER, typework TEXT, " +
                "smrpot REAL,smrplan REAL, smrfact REAL, mtrpot REAL, mtrplan REAL, mtrfact REAL, pirpot REAL, pirplan REAL," +
                " pirfact REAL, ukzpot INTEGER, ukzplan INTEGER, ukzfact INTEGER, vlpot INTEGER, vlplan INTEGER, vlfact INTEGER," +
                " udzpot INTEGER, udzplan INTEGER, udzfact INTEGER, upzpot INTEGER, upzplan INTEGER, upzfact INTEGER," +
                " vstavkapot INTEGER, vstavkaplan INTEGER, vstavkafact INTEGER, kippot INTEGER, kipplan INTEGER, kipfact INTEGER," +
                " istochtokpot INTEGER, istochtokplan INTEGER, istochtokfact INTEGER, prochpot INTEGER, prochplan INTEGER, " +
                " prochfact INTEGER, note TEXT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу f40year.
            cmd.CommandText = "CREATE TABLE f40year(daterec TEXT,namelpu TEXT,nameobj TEXT,singlepipe INTEGER,countKIP INTEGER," +
                             "countUKZ INTEGER,countUDZ INTEGER,countUPZ INTEGER,countsource  INTEGER,countins INTEGER,lengthLEP INTEGER," +
                             "countSKZtime INTEGER,countSKZtele INTEGER,electricity INTEGER, electricityload INTEGER,rejectionLEP INTEGER," +
                             "rejectionEHZ INTEGER,rejectionSUM INTEGER,securitysinglepipe INTEGER,securityspKPZ INTEGER," +
                             " securitylenght TEXT,securitytime TEXT, securityKPZ TEXT,securityUvom TEXT,lenghtVKO INTEGER," +
                             "lenghtPKO INTEGER,transresisMax INTEGER,transresisMin INTEGER)";
            cmd.ExecuteNonQuery();
            //Создать таблицу f41year.
            cmd.CommandText = "CREATE TABLE f41year(Field2 TEXT,Field3 TEXT,Field4 TEXT,Field5 REAL,Field6 REAL,Field7 REAL," +
                "Field8 INTEGER,Field9 TEXT,Field10 TEXT,Field11 TEXT,Field12 REAL,Field13 REAL,Field14 REAL,Field15 REAL," +
                "Field16 REAL,Field17 REAL,Field18 INTEGER,Field19 INTEGER,Field20 INTEGER,Field21 INTEGER,Field22 INTEGER," +
                "Field23 INTEGER,Field24 INTEGER,Field25 INTEGER,Field26 INTEGER,Field27 INTEGER,Field28 INTEGER,Field29 INTEGER," +
                "Field30 REAL,Field31 REAL,Field32 TEXT,Field33 REAL,Field34 REAL,Field35 TEXT,Field36 REAL,Field37 INTEGER," +
                "Field38 INTEGER,Field39 TEXT)";
            cmd.ExecuteNonQuery();
            //Создать таблицу f52gas.
            cmd.CommandText = "CREATE TABLE f52gas(Field1 TEXT,Field2 TEXT,Field3 TEXT,Field4 TEXT,Field5 TEXT,Field6 TEXT," +
                "Field7 REAL,Field8 INTEGER,Field9 INTEGER,Field10 INTEGER,Field11 INTEGER,Field12 INTEGER,Field13 INTEGER," +
                "Field14 INTEGER,Field15 INTEGER,Field16 INTEGER,Field17 INTEGER,Field18 INTEGER,Field19 INTEGER,Field20 INTEGER," +
                "Field21 INTEGER,Field22 INTEGER,Field23 INTEGER,Field24 INTEGER,Field25 INTEGER,Field26 INTEGER,Field27 INTEGER," +
                "Field28 INTEGER,Field29 INTEGER,Field30 INTEGER,Field31  INTEGER,Field32 INTEGER,Field33 INTEGER,Field34 INTEGER," +
                "Field35 INTEGER,Field36 INTEGER,Field37 INTEGER)";
            cmd.ExecuteNonQuery();
            //Создать таблицу skz.
            cmd.CommandText = "CREATE TABLE skz(id INTEGER NOT NULL,unomin REAL,nactivin REAL,nfullin REAL,unomout REAL," +
                "inomout REAL,nnomout REAL,fcode TEXT,fnumber INTEGER,modulescount INTEGER,fyear TEXT, yearstart TEXT," +
                "resource INTEGER, PRIMARY KEY(id))";
            cmd.ExecuteNonQuery();
            //Создать таблицу vei.
            cmd.CommandText = "CREATE TABLE vei(daterec TEXT, namelpu TEXT, typeobj TEXT, location TEXT, km TEXT, typeflange TEXT, " +
                "placement TEXT, datebegin TEXT, manufactory TEXT, datemade TEXT, serial TEXT, dnar INTEGER, pmax INTEGER, " +
                "status TEXT, kip TEXT, spark TEXT, bsz TEXT)";
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// Открытие базы данных(при каждом запуске программы).
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public virtual bool Open(ClassSettings settings)
        {
            SqliteConnectionStringBuilder csb = new SqliteConnectionStringBuilder();
            csb.DataSource = settings.DB;
            csb.Mode = SqliteOpenMode.ReadWrite;
            conn = new SqliteConnection(csb.ConnectionString);
            try { conn.Open(); }
            catch { return false; }
            if (conn.State != System.Data.ConnectionState.Open) return false;
            return true;
        }

        public virtual void Close()
        {
            conn.Close();
        }

        #endregion

        #region [Info]

        public virtual int InfoLoad()
        {
            object obj;
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT version FROM info";
            try { obj = cmd.ExecuteScalar(); }
            catch { return 0; }
            int ID = Convert.ToInt32(obj);
            return ID;
        }

        public virtual string InfoArea()
        {
            object obj;
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT lpu FROM info";
            try { obj = cmd.ExecuteScalar(); }
            catch { return string.Empty; }            
            return obj!=null?obj.ToString():string.Empty;
        }

        public virtual bool InfoEdit(int Version)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE info SET version = @Version";
            cmd.Parameters.AddWithValue("@Version", Version);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            return true;
        }

        #endregion

        #region [Devices]

        public virtual List<ClassDevice> DevicesLoad()
        {
            List<ClassDevice> lst = new List<ClassDevice>();
            SqliteCommand cmd = conn.CreateCommand();
            string commandTextDevices = "SELECT rowid, * FROM dev ORDER BY name";
            cmd.CommandText = commandTextDevices;
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ClassDevice obj = new ClassDevice();
                    obj.ID = Convert.ToInt32(reader["rowid"]);
                    obj.Name = reader["name"].ToString();
                    if (reader["prot"] != DBNull.Value)
                        obj.Protocol = (ClassDevice.EnumProtocol)Convert.ToInt32(reader["prot"]);
                    obj.Period = Convert.ToInt32(reader["period"]);
                    obj.IPAddress = Convert.ToString(reader["ip_adr"]);
                    if (reader["ip_port"] != DBNull.Value)
                        obj.IPPort = Convert.ToInt32(reader["ip_port"]);
                    obj.Address = Convert.ToInt32(reader["address"]);
                    if (reader["port"] != DBNull.Value)
                        obj.ComPort = (string)reader["port"];
                    if (reader["sim"] != DBNull.Value)
                        obj.SIM = (string)reader["sim"];
                    if (reader["model"] != DBNull.Value)
                        obj.Model = (ClassDevice.EnumModel)Convert.ToInt32(reader["model"]);
                    if (reader["lat"] != DBNull.Value)
                        obj.Latitude = Convert.ToDouble(reader["lat"]);
                    if (reader["longt"] != DBNull.Value)
                        obj.Longitude = Convert.ToDouble(reader["longt"]);
                    if (reader["elev"] != DBNull.Value)
                        obj.Elevation = Convert.ToDouble(reader["elev"]);
                    if (reader["picket"] != DBNull.Value)
                        obj.Picket = (string)(reader["picket"]);
                    lst.Add(obj);

                    //Загрузка СКЗ.
                    if (obj.Model == ClassDevice.EnumModel.SKZ_IP || obj.Model == ClassDevice.EnumModel.SKZ)
                    {
                        LoadSKZData(obj);
                    }
                }
            }
            return lst;
        }

        /// <summary>
        /// Загрузка данных СКЗ.
        /// </summary>
        /// <param name="obj"></param>
        private void LoadSKZData(ClassDevice obj)
        {
            string commandTextSKZ = $"SELECT * FROM skz WHERE id={obj.ID}";
            SqliteCommand cmdSKZ = conn.CreateCommand();
            cmdSKZ.CommandText = commandTextSKZ;
            using (SqliteDataReader readerSKZ = cmdSKZ.ExecuteReader())
            {
                while (readerSKZ.Read())
                {
                    obj.UnomInSKZ = Convert.ToDouble(readerSKZ["unomin"]);
                    obj.NactiveSKZ = Convert.ToDouble(readerSKZ["nactivin"]);
                    obj.NfullInSKZ = Convert.ToDouble(readerSKZ["nfullin"]);
                    obj.UnomOutSKZ = Convert.ToDouble(readerSKZ["unomout"]);
                    obj.InomOutSKZ = Convert.ToDouble(readerSKZ["inomout"]);
                    obj.NnomOutSKZ = Convert.ToDouble(readerSKZ["nnomout"]);
                    obj.FactoryCode = Convert.ToString(readerSKZ["fcode"]);
                    obj.FactoryNumber = Convert.ToInt32(readerSKZ["fnumber"]);
                    obj.ModulesCount = Convert.ToInt32(readerSKZ["modulescount"]);
                    obj.FactoryYear = Convert.ToInt32(readerSKZ["fyear"]);
                    obj.DateStart = Convert.ToDateTime(readerSKZ["yearstart"]);
                    obj.Resource = readerSKZ["resource"] != DBNull.Value ? Convert.ToInt32(readerSKZ["resource"]) : 0;
                }
            }
        }


        /// <summary>
        /// Добавление устройства в БД.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool DeviceAdd(ClassDevice obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO dev (name, prot, period, ip_adr, ip_port, address, port, sim,"
                + " model, lat, longt, elev, picket)"
                + " VALUES(@Name, @Protocol, @Period, @IPAddress, @IPPort, @Address, @Port, @SIM,"
                + " @Model, @Latitude, @Longitude, @Elevation, @Picket)";
            cmd.Parameters.AddWithValue("@Name", obj.Name);
            cmd.Parameters.AddWithValue("@Protocol", (int)obj.Protocol);
            cmd.Parameters.AddWithValue("@Period", obj.Period);
            cmd.Parameters.AddWithValue("@IPAddress", obj.IPAddress);
            cmd.Parameters.AddWithValue("@IPPort", obj.IPPort);
            cmd.Parameters.AddWithValue("@Address", obj.Address);
            cmd.Parameters.AddWithValue("@Port", obj.ComPort);
            cmd.Parameters.AddWithValue("@SIM", obj.SIM);
            cmd.Parameters.AddWithValue("@Model", (int)obj.Model);
            cmd.Parameters.AddWithValue("@Latitude", obj.Latitude);
            cmd.Parameters.AddWithValue("@Longitude", obj.Longitude);
            cmd.Parameters.AddWithValue("@Elevation", obj.Elevation);
            cmd.Parameters.AddWithValue("@Picket", obj.Picket);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            cmd.CommandText = "SELECT last_insert_rowid()";
            obj.ID = Convert.ToInt32(cmd.ExecuteScalar());

            //Добавление СКЗ.
            if (obj.Model == ClassDevice.EnumModel.SKZ_IP || obj.Model == ClassDevice.EnumModel.SKZ)
            {
                AddSKZData(cmd, obj);
            }
            return true;
        }

        /// <summary>
        /// Добавление паспортных параметров СКЗ.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="obj"></param>
        private void AddSKZData(SqliteCommand cmd, ClassDevice obj)
        {
            cmd.CommandText = "INSERT INTO skz (id,unomin,nactivin,nfullin,unomout,inomout,nnomout,fcode,fnumber," +
                "modulescount,fyear,yearstart) VALUES(@ID,@UnomInSKZ,@NactiveSKZ,@NfullInSKZ,@UnomOutSKZ,@InomOutSKZ,@NnomOutSKZ," +
                "@FactoryCode,@FactoryNumber,@ModulesCount,@factoryYear,@DateStart)";
            cmd.Parameters.AddWithValue("@ID", obj.ID);
            cmd.Parameters.AddWithValue("@UnomInSKZ", obj.UnomInSKZ);
            cmd.Parameters.AddWithValue("@NactiveSKZ", obj.NactiveSKZ);
            cmd.Parameters.AddWithValue("@NfullInSKZ", obj.NfullInSKZ);
            cmd.Parameters.AddWithValue("@UnomOutSKZ", obj.UnomOutSKZ);
            cmd.Parameters.AddWithValue("@InomOutSKZ", obj.InomOutSKZ);
            cmd.Parameters.AddWithValue("@NnomOutSKZ", obj.NnomOutSKZ);
            cmd.Parameters.AddWithValue("@FactoryCode", obj.FactoryCode);
            cmd.Parameters.AddWithValue("@FactoryNumber", obj.FactoryNumber);
            cmd.Parameters.AddWithValue("@ModulesCount", obj.ModulesCount);
            cmd.Parameters.AddWithValue("@factoryYear", obj.FactoryYear);
            cmd.Parameters.AddWithValue("@DateStart", obj.DateStart.ToString("dd.MM.yyyy"));
            try { cmd.ExecuteNonQuery(); }
            catch { return; }
        }

        /// <summary>
        /// Редактирование устройства в БД.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool DeviceEdit(ClassDevice obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE dev SET name = @Name, prot = @Protocol, period = @Period,"
                + " ip_adr = @IPAddress, ip_port = @IPPort, address = @Address, port = @Port,"
                + " sim = @SIM, model = @Model, lat=@Latitude, longt=@Longitude, elev=@Elevation, picket=@Picket"
                + " WHERE rowid = @ID";
            cmd.Parameters.AddWithValue("@Name", obj.Name);
            cmd.Parameters.AddWithValue("@Protocol", (int)obj.Protocol);
            cmd.Parameters.AddWithValue("@Period", obj.Period);
            cmd.Parameters.AddWithValue("@IPAddress", obj.IPAddress);
            cmd.Parameters.AddWithValue("@IPPort", obj.IPPort);
            cmd.Parameters.AddWithValue("@Address", obj.Address);
            cmd.Parameters.AddWithValue("@Port", obj.ComPort);
            cmd.Parameters.AddWithValue("@SIM", obj.SIM);
            cmd.Parameters.AddWithValue("@Model", (int)obj.Model);
            cmd.Parameters.AddWithValue("@ID", obj.ID);
            cmd.Parameters.AddWithValue("@Latitude", obj.Latitude);
            cmd.Parameters.AddWithValue("@Longitude", obj.Longitude);
            cmd.Parameters.AddWithValue("@Elevation", obj.Elevation);
            cmd.Parameters.AddWithValue("@Picket", obj.Picket);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }

           // Редактирование СКЗ.
            if (obj.Model == ClassDevice.EnumModel.SKZ_IP || obj.Model == ClassDevice.EnumModel.SKZ)
            {
                EditSKZData(cmd, obj);
            }
            return true;
        }


        /// <summary>
        /// Обновление паспортных параметров СКЗ.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="obj"></param>
        private void EditSKZData(SqliteCommand cmd, ClassDevice obj)
        {
            cmd.CommandText = $"UPDATE skz SET unomin = @UnomInSKZ, nactivin = @NactiveSKZ, nfullin = @NfullInSKZ, " +
                $"unomout = @UnomOutSKZ, inomout = @InomOutSKZ, nnomout = @NnomOutSKZ," +
                $"fcode = @FactoryCode, fnumber = @FactoryNumber, modulescount = @ModulesCount, " +
                $"fyear = @factoryYear, yearstart=@DateStart,resource = @Resource   WHERE id = @ID";
            cmd.Parameters.AddWithValue("@UnomInSKZ", obj.UnomInSKZ);
            cmd.Parameters.AddWithValue("@NactiveSKZ", obj.NactiveSKZ);
            cmd.Parameters.AddWithValue("@NfullInSKZ", obj.NfullInSKZ);
            cmd.Parameters.AddWithValue("@UnomOutSKZ", obj.UnomOutSKZ);
            cmd.Parameters.AddWithValue("@InomOutSKZ", obj.InomOutSKZ);
            cmd.Parameters.AddWithValue("@NnomOutSKZ", obj.NnomOutSKZ);
            cmd.Parameters.AddWithValue("@FactoryCode", obj.FactoryCode);
            cmd.Parameters.AddWithValue("@FactoryNumber", obj.FactoryNumber);
            cmd.Parameters.AddWithValue("@ModulesCount", obj.ModulesCount);
            cmd.Parameters.AddWithValue("@factoryYear", obj.FactoryYear);
            cmd.Parameters.AddWithValue("@DateStart", obj.DateStart.ToString("dd.MM.yyyy"));
            cmd.Parameters.AddWithValue("@Resource", obj.Resource);
            try { cmd.ExecuteNonQuery(); }
            catch { return; }
        }

        /// <summary>
        /// Удаление устройства из базы данных.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool DeviceDel(ClassDevice obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM dev WHERE rowid = @ID";
            cmd.Parameters.AddWithValue("@ID", obj.ID);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }

            //Удаление СКЗ.
            if (obj.Model == ClassDevice.EnumModel.SKZ_IP || obj.Model == ClassDevice.EnumModel.SKZ)
            {
                SqliteCommand cmdSKZ = conn.CreateCommand();
                cmdSKZ.CommandText = $"DELETE FROM skz WHERE id ={obj.ID} ";
                try { cmdSKZ.ExecuteNonQuery(); }
                catch { return false; }
            }
            return true;
        }

        #endregion

        #region [Registries]
        
        /// <summary>
        /// Загрузка регистров.
        /// </summary>
        /// <param name="DeviceID"></param>
        /// <returns></returns>
        public virtual List<ClassChannel> RegistriesLoad(int DeviceID)
        {
            List<ClassChannel> lst = new List<ClassChannel>();//Пустой список каналов.
            SqliteCommand cmd = conn.CreateCommand();//Обьект sql команд.
            cmd.CommandText = "SELECT reg.rowid, reg.*, dev.name AS d_name, dev.address AS d_adr"
                + " FROM reg LEFT JOIN dev ON reg.dev = dev.rowid"; //Текст команды.
            //Если передан номер устройства больше нуля.
            if (DeviceID != 0) cmd.CommandText += " WHERE reg.dev = " + DeviceID.ToString();
            cmd.CommandText += " ORDER by type DESC, adr ASC";
            //Чтение регистров из базы и добавление их в список.
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ClassChannel obj = new ClassChannel();
                    obj.ID = Convert.ToInt32(reader["rowid"]);
                    obj.Name = (string)reader["name"];
                    obj.TypeRegistry = (ClassChannel.EnumTypeRegistry)Convert.ToInt32(reader["type"]);
                    obj.Address = Convert.ToInt32(reader["adr"]);
                    if (reader["format"] != DBNull.Value)
                        obj.Format = (ClassChannel.EnumFormat)Convert.ToInt32(reader["format"]);
                    if (reader["k"] != DBNull.Value)
                        obj.Koef = Convert.ToSingle(reader["k"]);
                    if (reader["vmax"] != DBNull.Value)
                        obj.Max = Convert.ToDecimal(reader["vmax"]);
                    if (reader["vmin"] != DBNull.Value)
                        obj.Min = Convert.ToDecimal(reader["vmin"]);
                    if (reader["rec"] != DBNull.Value)
                        obj.Archive = Convert.ToBoolean(reader["rec"]);
                    if (reader["dev"] != DBNull.Value)
                        obj.Device.ID = Convert.ToInt32(reader["dev"]);
                    if (reader["d_name"] != DBNull.Value)
                        obj.Device.Name = reader["d_name"].ToString();
                    if (reader["d_adr"] != DBNull.Value)
                        obj.Device.Address = Convert.ToInt32(reader["d_adr"]);
                    if (reader["ext"] != DBNull.Value)
                        obj.Ext = Convert.ToInt32(reader["ext"]);
                    if (reader["accuracy"] != DBNull.Value)
                        obj.Accuracy = Convert.ToInt32(reader["accuracy"]);
                    if (reader["val"] != DBNull.Value && reader["dt"] != DBNull.Value) //last!
                        obj.LoadSavedValue(Convert.ToDecimal(reader["val"]),
                            DateTime.Parse(reader["dt"].ToString()));
                    lst.Add(obj);
                }
            }
            return lst;//Возврат списка.
        }
        object locker = new object();
        public virtual bool RegistryAdd(ClassChannel obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO reg (name, dev, type, adr, format, k, vmax, vmin, rec, ext, accuracy)"
                + " VALUES(@Name, @Device, @Type, @Address, @Format, @K, @Vmax, @Vmin, @Rec, @Ext, @Accuracy)";
            cmd.Parameters.AddWithValue("@Name", obj.Name);
            cmd.Parameters.AddWithValue("@Device", obj.Device.ID);
            cmd.Parameters.AddWithValue("@Type", (int)obj.TypeRegistry);
            cmd.Parameters.AddWithValue("@Address", obj.Address);
            cmd.Parameters.AddWithValue("@Format", (int)obj.Format);
            cmd.Parameters.AddWithValue("@K", obj.Koef);
            if (obj.Max.HasValue) cmd.Parameters.AddWithValue("@Vmax", obj.Max.Value);
            else cmd.Parameters.AddWithValue("@Vmax", DBNull.Value);
            if (obj.Min.HasValue) cmd.Parameters.AddWithValue("@Vmin", obj.Min.Value);
            else cmd.Parameters.AddWithValue("@Vmin", DBNull.Value);
            cmd.Parameters.AddWithValue("@Rec", obj.Archive);
            if (obj.Ext.HasValue) cmd.Parameters.AddWithValue("@Ext", obj.Ext.Value);
            else cmd.Parameters.AddWithValue("@Ext", DBNull.Value);
            if (obj.Accuracy.HasValue) cmd.Parameters.AddWithValue("@Accuracy", obj.Accuracy.Value);
            else cmd.Parameters.AddWithValue("@Accuracy", DBNull.Value);
            try
            {
                lock (locker)
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            cmd.CommandText = "SELECT last_insert_rowid()";
            obj.ID = Convert.ToInt32(cmd.ExecuteScalar());
            return true;
        }

        /// <summary>
        /// Изменение всей таблицы регистров.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool RegistryEdit(ClassChannel obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE reg SET name = @Name, dev = @Device, type = @Type,"
                + " adr = @Address, format = @Format, k = @K, vmax = @Vmax, vmin = @Vmin,"
                + " rec = @Rec, ext = @Ext, accuracy = @Accuracy"
                + " WHERE rowid = @ID";
            cmd.Parameters.AddWithValue("@Name", obj.Name);
            cmd.Parameters.AddWithValue("@Device", obj.Device.ID);
            cmd.Parameters.AddWithValue("@Type", (int)obj.TypeRegistry);
            cmd.Parameters.AddWithValue("@Address", obj.Address);
            cmd.Parameters.AddWithValue("@Format", (int)obj.Format);
            cmd.Parameters.AddWithValue("@K", obj.Koef);
            if (obj.Max.HasValue) cmd.Parameters.AddWithValue("@Vmax", obj.Max.Value);
            else cmd.Parameters.AddWithValue("@Vmax", DBNull.Value);
            if (obj.Min.HasValue) cmd.Parameters.AddWithValue("@Vmin", obj.Min.Value);
            else cmd.Parameters.AddWithValue("@Vmin", DBNull.Value);
            cmd.Parameters.AddWithValue("@Rec", obj.Archive);
            if (obj.Ext.HasValue) cmd.Parameters.AddWithValue("@Ext", obj.Ext.Value);
            else cmd.Parameters.AddWithValue("@Ext", DBNull.Value);
            if (obj.Accuracy.HasValue) cmd.Parameters.AddWithValue("@Accuracy", obj.Accuracy.Value);
            else cmd.Parameters.AddWithValue("@Accuracy", DBNull.Value);
            cmd.Parameters.AddWithValue("@ID", obj.ID);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Изменение значения регистра в таблице регистров БД.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool RegistrySaveValue(ClassChannel obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE reg SET val = @Val, dt = datetime('now', 'localtime')"
                + " WHERE rowid = @ID";
            cmd.Parameters.AddWithValue("@Val", obj.Value);
            cmd.Parameters.AddWithValue("@ID", obj.ID);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Обновление поля значения для записи.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public virtual bool RegistrySaveNewValue(int ID, decimal? newValue)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE reg SET nval = @NewVal"
                + " WHERE rowid = @ID";
            if (newValue.HasValue) cmd.Parameters.AddWithValue("@NewValue", newValue.Value);
            else cmd.Parameters.AddWithValue("@NewValue", DBNull.Value);
            cmd.Parameters.AddWithValue("@ID", ID);
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }

        public virtual bool RegistryDel(int ID)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM reg WHERE rowid = @ID";
            cmd.Parameters.AddWithValue("@ID", ID);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            return true;
        }

        public virtual bool RegistryDelDev(int DeviceID)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM reg WHERE dev = @Device";
            cmd.Parameters.AddWithValue("@Device", DeviceID);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Загрузка регистров если в них заполнено поле нового значения.
        /// </summary>
        /// <param name="DeviceID"></param>
        /// <returns></returns>
        public virtual List<ClassChannel> RegistriesLoadNew(int DeviceID)
        {
            //Список каналов пустой.
            List<ClassChannel> lst = new List<ClassChannel>();
            SqliteCommand cmd = conn.CreateCommand();
            //Выбрать по id из reg, где значение nval не равно null.
            cmd.CommandText = "SELECT rowid, * FROM reg WHERE nval IS NOT NULL";
            //Привязка к номеру устройства.
            if (DeviceID != 0) cmd.CommandText += " AND dev = " + DeviceID.ToString();
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ClassChannel obj = new ClassChannel();
                    obj.ID = Convert.ToInt32(reader["rowid"]);
                    if (reader["nval"] != DBNull.Value)
                        obj.NValue = Convert.ToDecimal(reader["nval"]);
                    lst.Add(obj);
                }
            }
            return lst;
        }

        #endregion
        
        #region [Commands]
        public virtual List<ClassCommand> CommandsLoad()
        {
            List<ClassCommand> lst = new List<ClassCommand>();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT cmd.rowid, cmd.*"
                + " FROM cmd LEFT JOIN dev ON cmd.dev = dev.rowid";
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ClassCommand obj = new ClassCommand();
                    obj.ID = Convert.ToInt32(reader["rowid"]);
                    obj.Name = (string)reader["name"];
                    obj.CommandType = (ClassCommand.EnumType)Convert.ToInt32(reader["type"]);
                    obj.Value = Convert.ToInt32(reader["val"]);
                    obj.Address = Convert.ToInt32(reader["adr"]);
                    if (reader["val"] != DBNull.Value)
                        obj.Value = Convert.ToInt32(reader["val"]);
                    if (reader["period"] != DBNull.Value)
                        obj.Period = Convert.ToInt32(reader["period"]);
                    obj.Device.ID = Convert.ToInt32(reader["dev"]);
                    lst.Add(obj);
                }
            }
            return lst;
        }

        public virtual bool CommandAdd(ClassCommand obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO cmd (name, type, dev, adr, val, period)"
                + " VALUES(@Name, @Type, @Device, @Address, @Value, @Period)";
            cmd.Parameters.AddWithValue("@Name", obj.Name);
            cmd.Parameters.AddWithValue("@Type", (int)obj.CommandType);
            cmd.Parameters.AddWithValue("@Device", obj.Device.ID);
            cmd.Parameters.AddWithValue("@Address", obj.Address);
            cmd.Parameters.AddWithValue("@Value", obj.Value);
            cmd.Parameters.AddWithValue("@Period", obj.Period);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            cmd.CommandText = "SELECT last_insert_rowid()";
            obj.ID = Convert.ToInt32(cmd.ExecuteScalar());
            return true;
        }

        public virtual bool CommandEdit(ClassCommand obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE cmd SET name = @Name, type = @Type, dev = @Device,"
                + " adr = @Address, val = @Value, period = @Period WHERE rowid = @ID";
            cmd.Parameters.AddWithValue("@Name", obj.Name);
            cmd.Parameters.AddWithValue("@Type", (int)obj.CommandType);
            cmd.Parameters.AddWithValue("@Device", obj.Device.ID);
            cmd.Parameters.AddWithValue("@Address", obj.Address);
            cmd.Parameters.AddWithValue("@Value", obj.Value);
            cmd.Parameters.AddWithValue("@Period", obj.Period);
            cmd.Parameters.AddWithValue("@ID", obj.ID);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            return true;
        }

        public virtual bool CommandDel(ClassCommand obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM cmd WHERE rowid = @ID";
            cmd.Parameters.AddWithValue("@ID", obj.ID);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            return true;
        }

        #endregion
        
        #region [Links]

        public virtual List<ClassLink> LinksLoad()
        {
            List<ClassLink> lst = new List<ClassLink>();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT link.rowid, link.*, reg.name as r_name, cmd.name AS c_name"
                + " FROM link"
                + " LEFT JOIN reg ON link.src = reg.rowid"
                + " LEFT JOIN cmd ON link.cmd = cmd.rowid";
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ClassLink obj = new ClassLink();
                    obj.ID = Convert.ToInt32(reader["rowid"]);
                    obj.EventType = (ClassEvent.EnumType)Convert.ToInt32(reader["event"]);
                    obj.SourceID = Convert.ToInt32(reader["src"]);
                    if (reader["r_name"] != DBNull.Value)
                        obj.SourceName = (string)reader["r_name"];
                    obj.Command.ID = Convert.ToInt32(reader["cmd"]);
                    obj.Command.Name = (string)reader["c_name"];
                    lst.Add(obj);
                }
            }
            return lst;
        }

        public virtual bool LinkAdd(ClassLink obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO link (event, src, cmd)"
                + " VALUES(@Event, @Src, @Cmd)";
            cmd.Parameters.AddWithValue("@Event", (int)obj.EventType);
            cmd.Parameters.AddWithValue("@Src", obj.SourceID);
            cmd.Parameters.AddWithValue("@Cmd", obj.Command.ID);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            cmd.CommandText = "SELECT last_insert_rowid()";
            obj.ID = Convert.ToInt32(cmd.ExecuteScalar());
            return true;
        }

        public virtual bool LinkEdit(ClassLink obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE link SET event = @Event, src = @Src, cmd = @Cmd"
                + " WHERE rowid = @ID";
            cmd.Parameters.AddWithValue("@Event", (int)obj.EventType);
            cmd.Parameters.AddWithValue("@Src", obj.SourceID);
            cmd.Parameters.AddWithValue("@Cmd", obj.Command.ID);
            cmd.Parameters.AddWithValue("@ID", obj.ID);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            return true;
        }

        public virtual bool LinkDel(ClassLink obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM link WHERE rowid = @ID";
            cmd.Parameters.AddWithValue("@ID", obj.ID);
            try { cmd.ExecuteNonQuery(); }
            catch { return false; }
            return true;
        }

        #endregion

        #region [Events]

        public static List<ClassEvent> AllEventsDeviceLoad(string nameDev, int type)
        {
            List<ClassEvent> lst = new List<ClassEvent>();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT rowid, * FROM log WHERE type={type} AND namdev='{nameDev}' ORDER BY dt ASC";

            using (SqliteDataReader reader = cmd.ExecuteReader())       //Считываем с базы.
            {
                while (reader.Read())                                   //Построчное чтение.
                {
                    ClassEvent ev = new ClassEvent();
                    ev.ID = Convert.ToInt32(reader["rowid"]);
                    ev.Type = (ClassEvent.EnumType)Convert.ToInt32(reader["type"]);
                    ev.DT = DateTime.Parse(reader["dt"].ToString());
                    ev.Param = Convert.ToString(reader["param"]);
                    ev.Val = Convert.ToString(reader["val"]);
                    ev.NameDevice = Convert.ToString(reader["namdev"]);
                    lst.Add(ev);                                         //Добавление считаной строки в список событий.
                }
            }

            return lst;
        }

        /// <summary>
        /// Загрузка событий с учетом заданного периода времени.
        /// </summary>
        /// <param name="dt1">Начальная дата</param>
        /// <param name="dt2">Конечная дата</param>
        /// <param name="types">Тип событий</param>
        /// <param name="str">Название параметра</param>
        /// <param name="nameDev">Название устройства</param>
        /// <returns></returns>
        public virtual List<ClassEvent> EventsLoad(DateTime dt1, DateTime dt2, List<int> types, string str, string nameDev = "")
        {
            string sIN = "";
            if (types != null)//Если тип события не null.
            {
                foreach (int numberType in types)
                {
                    if (sIN != "") sIN += ",";
                    sIN += numberType.ToString();//Заносим тип события в строку.
                }
            }
            List<ClassEvent> lst = new List<ClassEvent>();              //Пустой список событий.
            SqliteCommand cmd = conn.CreateCommand();                   //Создание команды к базе.
            cmd.CommandText = "SELECT rowid, * FROM log";
            cmd.CommandText += " WHERE dt BETWEEN datetime ('" + dt1.ToString("yyyy-MM-dd HH:mm:ss") + "') " +
                "AND datetime('" + dt2.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            if (types != null) cmd.CommandText += $" AND type IN ({sIN})";
            //cmd.CommandText += $" AND param LIKE '%{strResBKM_5}%'"; //Если есть вхождение либо вначале либо в конце.
            if (str != "Нет") cmd.CommandText += $" AND param LIKE '{str}'";
            if (nameDev != "" && sIN.Contains("1") != true && sIN.Contains("2") != true) cmd.CommandText += $" AND namdev='{nameDev}'";
            cmd.CommandText += " ORDER BY dt ASC";
            using (SqliteDataReader reader = cmd.ExecuteReader())       //Считываем с базы.
            {
                while (reader.Read())                                   //Построчное чтение.
                {
                    ClassEvent ev = new ClassEvent();
                    ev.ID = Convert.ToInt32(reader["rowid"]);
                    ev.Type = (ClassEvent.EnumType)Convert.ToInt32(reader["type"]);
                    ev.DT = DateTime.Parse(reader["dt"].ToString());
                    ev.Param = Convert.ToString(reader["param"]);
                    ev.Val = Convert.ToString(reader["val"]);
                    ev.NameDevice = Convert.ToString(reader["namdev"]);
                    lst.Add(ev);                                         //Добавление считаной строки в список событий.
                }
            }
            return lst;                                                 //Возврат списка событий.
        }

        /// <summary>
        /// Сохранить событие в лог архива базы данных.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool EventAdd(ClassEvent obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO log (type, dt, param, val,namdev)"
                + " VALUES(@Type, datetime('now', 'localtime'), @Param, @Val,@NameDevice)";
            cmd.Parameters.AddWithValue("@Type", (int)obj.Type);
            cmd.Parameters.AddWithValue("@Param", obj.Param);
            cmd.Parameters.AddWithValue("@Val", obj.Val);
            obj.NameDevice ??= string.Empty;
            cmd.Parameters.AddWithValue("@NameDevice", obj.NameDevice);
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
                return false;
            }
            //Извлекаем номер(rowID) последнего события.
            cmd.CommandText = "SELECT last_insert_rowid()";
            //Присваиваем ID объекта события ID последней вставленой строки в таблицу событий.  
            obj.ID = Convert.ToInt32(cmd.ExecuteScalar());
            return true;
        }

        /// <summary>
        /// Изменение статуса реакции на конкректное событие.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public virtual bool EventAck(int ID)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE log SET ack = 1"
                + " WHERE rowid = @ID";
            cmd.Parameters.AddWithValue("@ID", ID);
            try { cmd.ExecuteNonQuery(); }
            catch (Exception Ex)
            {
                ClassLog.Write(Ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Изменение статуса реакции на все события.
        /// </summary>
        /// <returns></returns>
        public virtual bool EventAckAll()
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE log SET ack = 1";
            try { cmd.ExecuteNonQuery(); }
            catch (Exception Ex)
            {
                ClassLog.Write(Ex.Message);
                return false;
            }
            return true;
        }

        #endregion

        #region [Update]

        public virtual bool Update(int Version)
        {
            if (Version < 2)
            {
                if (!Update2()) return false;
                InfoEdit(2);
            }
            if (Version < 3)
            {
                if (!Update3()) return false;
                InfoEdit(3);
            }
            if (Version < 4)
            {
                if (!Update4()) return false;
                InfoEdit(4);
            }
            if (Version < 5)
            {
                if (!Update5()) return false;
                InfoEdit(5);
            }
            if (Version < 6)
            {
                if (!Update6()) return false;
                InfoEdit(6);
            }
            if (Version < 7)
            {
                if (!Update7()) return false;
                InfoEdit(7);
            }
            return true;
        }

        private bool Update2()
        {
            var transaction = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DROP TABLE IF EXISTS lib";
            try { cmd.ExecuteNonQuery(); }
            catch
            {
                transaction.Rollback();
                return false;
            }
            cmd.CommandText = "CREATE TABLE lib(name TEXT, pic BLOB)";
            cmd.ExecuteNonQuery();
            try { cmd.ExecuteNonQuery(); }
            catch
            {
                transaction.Rollback();
                return false;
            }
            transaction.Commit();
            return true;
        }

        private bool Update3()
        {
            var transaction = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "ALTER TABLE dev ADD model INTEGER";
            try { cmd.ExecuteNonQuery(); }
            catch
            {
                transaction.Rollback();
                return false;
            }
            transaction.Commit();
            return true;
        }

        private bool Update4()
        {
            var transaction = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "CREATE TABLE sub (var TEXT, val INTEGER, txt TEXT, fcolor INTEGER,"
                + " bcolor INTEGER)";
            try { cmd.ExecuteNonQuery(); }
            catch (Exception Ex)
            {
                transaction.Rollback();
                ClassLog.Write(Ex.Message);
                return false;
            }
            transaction.Commit();
            return true;
        }

        private bool Update5()
        {
            var transaction = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "ALTER TABLE reg ADD accuracy INTEGER";
            try { cmd.ExecuteNonQuery(); }
            catch
            {
                transaction.Rollback();
                return false;
            }
            transaction.Commit();
            return true;
        }

        private bool Update6()
        {
            var transaction = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "ALTER TABLE reg ADD nval NUMERIC";
            try { cmd.ExecuteNonQuery(); }
            catch
            {
                transaction.Rollback();
                return false;
            }
            transaction.Commit();
            return true;
        }

        private bool Update7()
        {
            var transaction = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = "ALTER TABLE info ADD lpu TEXT";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "ALTER TABLE dev ADD lat REAL";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "ALTER TABLE dev ADD longt REAL";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "ALTER TABLE dev ADD elev REAL";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "ALTER TABLE dev ADD picket TEXT";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "ALTER TABLE log ADD namdev TEXT";
                cmd.ExecuteNonQuery();

                //Создать таблицу az.
                cmd.CommandText = "CREATE TABLE az (Identity TEXT,Location TEXT, TypeOfConstruction TEXT,"
                    + "ElectrodeMaterial TEXT, CurrentLoad REAL, DissolutionRate REAL, SurfaceArea REAL, WeightAZ REAL, LenghtAZ INTEGER,"
                    + "Diagonal INTEGER, MassAssembly REAL, MountingMethod TEXT, ServiceLife INTEGER, KMA TEXT)";
                cmd.ExecuteNonQuery();
                //Создать таблицу ehz1.
                cmd.CommandText = "CREATE TABLE ehz1 (daterec TEXT, namelpu TEXT, nameobj TEXT, km TEXT, ukz INTEGER, skz INTEGER, typeskz TEXT," +
                    " dateinskz TEXT, methodground TEXT, typeground TEXT, elmeter TEXT, typecontrol TEXT, bsz TEXT)";
                cmd.ExecuteNonQuery();
                //Создать таблицу ehz4.
                cmd.CommandText = "CREATE TABLE ehz4(daterec TEXT, namelpu TEXT, nameobj TEXT, upz TEXT, lastrepair TEXT, location TEXT," +
                    " typeprotect TEXT, numprotect INTEGER, startupz TEXT)";
                cmd.ExecuteNonQuery();
                //Создать таблицу ehz6.
                cmd.CommandText = "CREATE TABLE ehz6(daterec TEXT, namelpu TEXT, nameobj TEXT, actpit TEXT, datepit TEXT, km TEXT, gps TEXT, " +
                    "lenghtpit INTEGER, dnar INTEGER, thickness INTEGER, reasonpit TEXT, deep REAL, typeground TEXT, soilresi INTEGER, " +
                    "insulatiomat TEXT, adhesiomat TEXT, damagearea INTEGER, nutcorrdamage TEXT, maxdeepdamage INTEGER, damage1mm INTEGER," +
                    "damage13 INTEGER, damage3 INTEGER, ute TEXT)";
                cmd.ExecuteNonQuery();
                //Создать таблицу ehz8q.
                cmd.CommandText = "CREATE TABLE ehz8q(daterec TEXT, namelpu TEXT, nametube TEXT, km TEXT, nameroad TEXT, typeroad TEXT," +
                    " datemeasure TEXT, ustart NUMERIC, upatr NUMERIC, usvech NUMERIC, uend NUMERIC, upatrend NUMERIC, usvechend NUMERIC, " +
                    "resis INTEGER, note TEXT)";
                cmd.ExecuteNonQuery();
                //Создать таблицу f02year.
                cmd.CommandText = "CREATE TABLE f02year(daterec TEXT,namelpu TEXT,namejbj TEXT, km REAL, typeehz TEXT, numukz TEXT, " +
                    "stateukz TEXT,tokukz REAL,numskz TEXT,tokmedskz REAL,uskz REAL,azresist REAL,numupz TEXT,stateupz TEXT,tokupz REAL," +
                    "numudz TEXT,stateudz TEXT,tokudz REAL)";
                cmd.ExecuteNonQuery();
                //Создать таблицу f141gas.
                cmd.CommandText = "CREATE TABLE f141gas (datarec TEXT,namelpu TEXT,nameobj TEXT,number INTEGER, typework TEXT, " +
                    "smrpot REAL,smrplan REAL, smrfact REAL, mtrpot REAL, mtrplan REAL, mtrfact REAL, pirpot REAL, pirplan REAL," +
                    " pirfact REAL, ukzpot INTEGER, ukzplan INTEGER, ukzfact INTEGER, vlpot INTEGER, vlplan INTEGER, vlfact INTEGER," +
                    " udzpot INTEGER, udzplan INTEGER, udzfact INTEGER, upzpot INTEGER, upzplan INTEGER, upzfact INTEGER," +
                    " vstavkapot INTEGER, vstavkaplan INTEGER, vstavkafact INTEGER, kippot INTEGER, kipplan INTEGER, kipfact INTEGER," +
                    " istochtokpot INTEGER, istochtokplan INTEGER, istochtokfact INTEGER, prochpot INTEGER, prochplan INTEGER, " +
                    " prochfact INTEGER, note TEXT)";
                cmd.ExecuteNonQuery();
                //Создать таблицу f40year.
                cmd.CommandText = "CREATE TABLE f40year(daterec TEXT,namelpu TEXT,nameobj TEXT,singlepipe INTEGER,countKIP INTEGER," +
                                 "countUKZ INTEGER,countUDZ INTEGER,countUPZ INTEGER,countsource  INTEGER,countins INTEGER,lengthLEP INTEGER," +
                                 "countSKZtime INTEGER,countSKZtele INTEGER,electricity INTEGER, electricityload INTEGER,rejectionLEP INTEGER," +
                                 "rejectionEHZ INTEGER,rejectionSUM INTEGER,securitysinglepipe INTEGER,securityspKPZ INTEGER," +
                                 " securitylenght TEXT,securitytime TEXT, securityKPZ TEXT,securityUvom TEXT,lenghtVKO INTEGER," +
                                 "lenghtPKO INTEGER,transresisMax INTEGER,transresisMin INTEGER)";
                cmd.ExecuteNonQuery();
                //Создать таблицу f41year.
                cmd.CommandText = "CREATE TABLE f41year(Field2 TEXT,Field3 TEXT,Field4 TEXT,Field5 REAL,Field6 REAL,Field7 REAL," +
                    "Field8 INTEGER,Field9 TEXT,Field10 TEXT,Field11 TEXT,Field12 REAL,Field13 REAL,Field14 REAL,Field15 REAL," +
                    "Field16 REAL,Field17 REAL,Field18 INTEGER,Field19 INTEGER,Field20 INTEGER,Field21 INTEGER,Field22 INTEGER," +
                    "Field23 INTEGER,Field24 INTEGER,Field25 INTEGER,Field26 INTEGER,Field27 INTEGER,Field28 INTEGER,Field29 INTEGER," +
                    "Field30 REAL,Field31 REAL,Field32 TEXT,Field33 REAL,Field34 REAL,Field35 TEXT,Field36 REAL,Field37 INTEGER," +
                    "Field38 INTEGER,Field39 TEXT)";
                cmd.ExecuteNonQuery();
                //Создать таблицу f52gas.
                cmd.CommandText = "CREATE TABLE f52gas(Field1 TEXT,Field2 TEXT,Field3 TEXT,Field4 TEXT,Field5 TEXT,Field6 TEXT," +
                    "Field7 REAL,Field8 INTEGER,Field9 INTEGER,Field10 INTEGER,Field11 INTEGER,Field12 INTEGER,Field13 INTEGER," +
                    "Field14 INTEGER,Field15 INTEGER,Field16 INTEGER,Field17 INTEGER,Field18 INTEGER,Field19 INTEGER,Field20 INTEGER," +
                    "Field21 INTEGER,Field22 INTEGER,Field23 INTEGER,Field24 INTEGER,Field25 INTEGER,Field26 INTEGER,Field27 INTEGER," +
                    "Field28 INTEGER,Field29 INTEGER,Field30 INTEGER,Field31  INTEGER,Field32 INTEGER,Field33 INTEGER,Field34 INTEGER," +
                    "Field35 INTEGER,Field36 INTEGER,Field37 INTEGER)";
                cmd.ExecuteNonQuery();
                //Создать таблицу skz.
                cmd.CommandText = "CREATE TABLE skz(id INTEGER NOT NULL,unomin REAL,nactivin REAL,nfullin REAL,unomout REAL," +
                    "inomout REAL,nnomout REAL,fcode TEXT,fnumber INTEGER,modulescount INTEGER,fyear TEXT, yearstart TEXT," +
                    "resource INTEGER, PRIMARY KEY(id))";
                cmd.ExecuteNonQuery();
                //Создать таблицу vei.
                cmd.CommandText = "CREATE TABLE vei(daterec TEXT, namelpu TEXT, typeobj TEXT, location TEXT, km TEXT, typeflange TEXT, " +
                    "placement TEXT, datebegin TEXT, manufactory TEXT, datemade TEXT, serial TEXT, dnar INTEGER, pmax INTEGER, " +
                    "status TEXT, kip TEXT, spark TEXT, bsz TEXT)";
                cmd.ExecuteNonQuery();
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
            transaction.Commit();
            return true;
        }

        #endregion
        
        #region [Messages]

        public virtual List<ClassMessage> MessagesLoad()
        {
            List<ClassMessage> lst = new List<ClassMessage>();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT rowid, * FROM mes ORDER BY rowid DESC";
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ClassMessage obj = new ClassMessage();
                    obj.ID = Convert.ToInt32(reader["rowid"]);
                    obj.Type = (ClassMessage.EnumType)Convert.ToInt32(reader["type"]);
                    obj.DT = DateTime.Parse(reader["dt"].ToString());
                    obj.Bytes = (byte[])reader["buf"];
                    lst.Add(obj);
                }
            }
            return lst;
        }

        public virtual bool MessageAdd(ClassMessage obj)
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO mes (type, buf) VALUES(@Type, @Buf)";
            cmd.Parameters.AddWithValue("@Type", (int)obj.Type);
            cmd.Parameters.Add(new SqliteParameter("@Buf", SqliteType.Blob) { Value = obj.Bytes });
            try { cmd.ExecuteNonQuery(); }
            catch(Exception ex)
            {
                return false; 
            }
            cmd.CommandText = "SELECT last_insert_rowid()";
            obj.ID = Convert.ToInt32(cmd.ExecuteScalar());
            return true;
        }

        // public virtual List<System.Windows.Media.Imaging.BitmapImage> ImagesLoad()
        // {
        //     List<System.Windows.Media.Imaging.BitmapImage> lst = new List<System.Windows.Media.Imaging.BitmapImage>();
        //     SqliteCommand cmd = conn.CreateCommand();
        //     cmd.CommandText = "SELECT rowid, * FROM lib ORDER BY rowid";
        //     using (SqliteDataReader reader = cmd.ExecuteReader())
        //     {
        //         while (reader.Read())
        //         {
        //             int ID = Convert.ToInt32(reader["rowid"]);
        //             string Name = (string)reader["name"];
        //             byte[] b = (byte[])reader["pic"];
        //             System.IO.MemoryStream stream = new System.IO.MemoryStream(b);
        //             System.Windows.Media.Imaging.BitmapImage image = new System.Windows.Media.Imaging.BitmapImage();
        //             image.BeginInit();
        //             image.StreamSource = stream;
        //             image.EndInit();
        //             lst.Add(image);
        //         }
        //     }
        //     return lst;
        // }

        #endregion
        
        #region [Users]

         public virtual List<ClassUser> UsersLoad()
         {
             List<ClassUser> lst = new List<ClassUser>();
             SqliteCommand cmd = conn.CreateCommand();
             cmd.CommandText = "SELECT rowid, * FROM user ORDER BY name";
             using (SqliteDataReader reader = cmd.ExecuteReader())
             {
                 while (reader.Read())
                 {
                     ClassUser obj = new ClassUser();
                     obj.ID = Convert.ToInt32(reader["rowid"]);
                     obj.Name = (string)reader["name"];
                     obj.Login = Convert.ToString(reader["login"]);
                     obj.Pass = Convert.ToString(reader["pass"]);
                     obj.Grant = Convert.ToInt32(reader["grant"]);
                     lst.Add(obj);
                 }
             }
             return lst;
         }

         public virtual bool UserAdd(ClassUser obj)
         {
             SqliteCommand cmd = conn.CreateCommand();
             cmd.CommandText = "INSERT INTO user (name, login, pass, grant)"
                 + " VALUES(@Name, @Login, @Pass, @Grant)";
             cmd.Parameters.AddWithValue("@Name", obj.Name);
             cmd.Parameters.AddWithValue("@Login", obj.Login);
             cmd.Parameters.AddWithValue("@Pass", obj.Pass);
             cmd.Parameters.AddWithValue("@Grant", obj.Grant);
             try { cmd.ExecuteNonQuery(); }
             catch { return false; }
             cmd.CommandText = "SELECT last_insert_rowid()";
             obj.ID = Convert.ToInt32(cmd.ExecuteScalar());
             return true;
         }

         public virtual bool UserEdit(ClassUser obj)
         {
             SqliteCommand cmd = conn.CreateCommand();
             cmd.CommandText = "UPDATE user SET name = @Name, login = @Login, pass = @Pass, grant = @Grant"
                 + " WHERE rowid = @ID";
             cmd.Parameters.AddWithValue("@Name", obj.Name);
             cmd.Parameters.AddWithValue("@Login", obj.Login);
             cmd.Parameters.AddWithValue("@Pass", obj.Pass);
             cmd.Parameters.AddWithValue("@Grant", obj.Grant);
             cmd.Parameters.AddWithValue("@ID", obj.ID);
             try { cmd.ExecuteNonQuery(); }
             catch (Exception Ex)
             {
                 ClassLog.Write(Ex.Message);
                 return false;
             }
             return true;
         }

         public virtual bool UserDel(ClassUser obj)
         {
             SqliteCommand cmd = conn.CreateCommand();
             cmd.CommandText = "DELETE FROM user WHERE rowid = @ID";
             cmd.Parameters.AddWithValue("@ID", obj.ID);
             try { cmd.ExecuteNonQuery(); }
             catch { return false; }
             return true;
         }

         #endregion

        #region[Отчеты]

        /// <summary>
        /// Сохранение отчета.
        /// </summary>
        /// <param name="itemSours"></param>
        /// <returns></returns>
        public bool SaveReport(IEnumerable itemSours,EnumTypeReport typeReport)
        {
            SqliteCommand cmd = conn.CreateCommand();
            string nameReportTable = GetNameReportTable(typeReport);
            List<string> namesCol = GetColumnName(nameReportTable);
            if (string.IsNullOrEmpty(nameReportTable) || namesCol.Count == 0) 
                return false;            
            string commandPart1 = $"INSERT INTO {nameReportTable}(";            
            for(int i=0;i<namesCol.Count;i++)
            {
                if (i < namesCol.Count - 1)
                    commandPart1 += $"{namesCol[i]},";
                else
                    commandPart1 += $"{namesCol[i]})";
            }
            IEnumerator enumer = itemSours.GetEnumerator();
            while (enumer.MoveNext())
            {
                string commandPart2 = " VALUES(";
                cmd.CommandText = commandPart1;
                ObservableCollection<string> reportDataLst = (enumer.Current as ClassReportData).DateRec;
                for(int i = 1; i < reportDataLst.Count; i++)
                {
                    if(i<reportDataLst.Count-1)
                        commandPart2 += $"'{reportDataLst[i].ToString()}',";
                    else
                        commandPart2 += $"'{reportDataLst[i].ToString()}')";
                }
                cmd.CommandText += commandPart2;
                try 
                { 
                  cmd.ExecuteNonQuery(); 
                }
                catch
                {
                    ClassMessage.ShowMessage(MainWindow.currentMainWindow, "Ошибка!Отчет не сохранен!",buttonEnum:ButtonEnum.Ok,
                        icon: MsBox.Avalonia.Enums.Icon.Error);
                    return false; 
                }                
            }
            ClassMessage.ShowMessage(MainWindow.currentMainWindow, "Отчет сохранен!",buttonEnum:ButtonEnum.Ok,
                icon: MsBox.Avalonia.Enums.Icon.Success);
            return true;
        }
        /// <summary>
        /// Получение названия таблицы с отчетом из БД.
        /// </summary>
        /// <returns></returns>
        private string GetNameReportTable(EnumTypeReport typeReport)
        {
            return typeReport switch
            {
                EnumTypeReport.VEIyear => "vei",
                EnumTypeReport.EHZ01year => "ehz1",
                EnumTypeReport.EHZ04year => "ehz4",
                EnumTypeReport.EHZ06year => "ehz6",
                EnumTypeReport.EHZ08quarter => "ehz8q",
                EnumTypeReport.F40year=> "f40year",
                EnumTypeReport.F02year=> "f02year",
                EnumTypeReport.F41year=> "f41year",
                EnumTypeReport.F47year=> "f47year",
                EnumTypeReport.F52gas=>  "f52gas",
                EnumTypeReport.F141gas => "f141gas",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Получение списка названий колонок таблицы.
        /// </summary>
        /// <returns></returns>
        public List<string> GetColumnName(string nameTable)
        {
            List<string> lstName = new List<string>();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"PRAGMA table_info({nameTable})";
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstName.Add(reader["name"].ToString());
                }
            }
            return lstName;
        }

        /// <summary>
        /// Загрузка отчета из БД.
        /// </summary>
        /// <param name="typeReport"></param>
        /// <param name="dtBegin"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public List<ClassReportData> LoadReport(EnumTypeReport typeReport,DateTime dtBegin, DateTime dtEnd)
        {
            //Количество столбцов в журнале.
            int countTable = (int)typeReport;
            //Список с объектами-данными из БД журналов.
            List<ClassReportData> list = new List<ClassReportData>();
            //Выбор названия таблицы.
            string table = GetNameReportTable(typeReport);           
            //Проверка на существование таблицы в БД.
            if (string.IsNullOrEmpty(table)) 
            {
                return list;
            }     
            SqliteCommand cmd = conn.CreateCommand();
            //Создание запроса базы данных.
            cmd.CommandText = $"SELECT rowid, * FROM {table}";
            if(dtBegin!=DateTime.MinValue && dtEnd != DateTime.MaxValue)
            {
                cmd.CommandText += " WHERE daterec BETWEEN datetime ('" + dtBegin.ToString("yyyy-MM-dd HH:mm:ss") + "') " +
                 "AND datetime('" + dtEnd.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            }
            cmd.CommandText+= " ORDER BY rowid ASC";
            var te = cmd.CommandText;
            //Считывание данных из БД.
            using (SqliteDataReader reader = cmd.ExecuteReader())       
            {
                while (reader.Read())                                   
                {

                    ClassReportData rep = new ClassReportData(typeReport);                   
                    for(int i = 0; i < countTable; i++)
                    {
                        //Нумерация, первый столбец.
                        if(i==0)
                        {
                            rep.DateRec.Add((list.Count+1).ToString());
                        }
                        //Данные из остальных столбцов.
                        else
                        {
                            rep.DateRec.Add(reader[reader.GetName(i)].ToString());
                        }
                    }
                    list.Add(rep);                                         
                }
            }
            return list;
        }
        #endregion

        #region[Backup]
        /// <summary>
        /// Бэкап базы данных.
        /// </summary>
        /// <param name="appDataDirectory"></param>
        public void Backup(string appDataDirectory) 
        {
            var now = DateTime.UtcNow;
            var backupDir = Path.Combine(appDataDirectory, "Backup");
            if (!Directory.Exists(backupDir))
                Directory.CreateDirectory(backupDir);
            var numbFiles = Directory.GetFiles(backupDir);            
            if (numbFiles.Length > 0)
            {
                foreach(var name in numbFiles)
                {
                    //Не переписывать базу данных больше одного раза в день.
                    if (File.GetCreationTime(name).DayOfYear == DateTime.Now.DayOfYear)
                        return;
                    File.Delete(name);
                }
            }
            //if (HasBackupForToday(now, backupDir)) return;
            var backupFile = (now.ToString("yyyy-MM-dd_hh-mm-ss"))+".back";
            backupFile = Path.Combine(backupDir, backupFile);
            string backupConnectionString = $"data source={backupFile}";
            using (var backupConnection = new SqliteConnection(backupConnectionString))
            {
                backupConnection.Open();
                conn.BackupDatabase(backupConnection);
            }
        }
        /// <summary>
        /// Проверка сохранялась ли сегодня база данных.
        /// </summary>
        /// <param name="now"></param>
        /// <param name="backupDirectory"></param>
        /// <returns></returns>
        private bool HasBackupForToday(DateTime now, string backupDirectory)
        {
            var today = now.ToString("yyyy-MM-dd");
            return Directory.EnumerateFiles(backupDirectory).Any(f=>Path.GetFileName(f).StartsWith(today));
        }
            #endregion

        //public void SaveArchiveToDB(List<int[]> archive,object nmDev)
        //{

        //    string devName = (nmDev as ClassDevice).Name;
        //    List<ClassChannel> lstDevCh = MainWindow.Devices.FirstOrDefault(x => x.Name == devName).Channels.
        //            Where(x => x.TypeRegistry == ClassChannel.EnumTypeRegistry.InputRegistry).ToList();
        //    try
        //    {
        //        foreach (var note in archive)
        //        {
        //            int centuryNow = (DateTime.Now.Year / 100)*100;
        //            DateTime date = new DateTime(centuryNow+note[58], note[59], note[60], note[61], note[62], note[63]);
        //            string dateTime = date.ToString("yyyy-MM-dd HH:mm:ss");
        //            int countNoteMax = note[15];
        //            //Временно всегда бкм5.
        //            ClassEvent ev = new ClassEvent() { Type=ClassEvent.EnumType.Measure, NameDevice=devName};
        //            int countList=0;
        //            for(int i=39;i<note.Length;i++)
        //            {
        //                if (countList >= countNoteMax) break;                   
        //                    ClassChannel ch = lstDevCh[countList];
        //                    ev.Param = ch.Name;
        //                    ev.Val = note[i].ToString();
        //                    ev.DT = date;
        //                    EventAddArchive(ev);
        //                    countList++;                    
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }        

        //}

        //public virtual bool EventAddArchive(ClassEvent obj)
        //{
        //    SqliteCommand cmd = conn.CreateCommand();
        //    cmd.CommandText = "INSERT INTO log (type, dt, param, val,namdev)"
        //        + " VALUES(@Type, @Datetime, @Param, @Val,@NameDevice)";
        //    cmd.Parameters.AddWithValue("@Type", (int)obj.Type);
        //    cmd.Parameters.AddWithValue("@Param", obj.Param);
        //    cmd.Parameters.AddWithValue("@Datetime", obj.DT);
        //    cmd.Parameters.AddWithValue("@Val", obj.Val);
        //    obj.NameDevice ??= string.Empty;
        //    cmd.Parameters.AddWithValue("@NameDevice", obj.NameDevice);
        //    try { cmd.ExecuteNonQuery(); }
        //    catch { return false; }
        //    //Извлекаем номер(rowID) последнего события.
        //    cmd.CommandText = "SELECT last_insert_rowid()";
        //    //Присваиваем ID объекта события ID последней вставленой строки в таблицу событий.  
        //    obj.ID = Convert.ToInt32(cmd.ExecuteScalar());
        //    return true;
        //}
    }
}