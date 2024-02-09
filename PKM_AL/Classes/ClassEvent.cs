using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKM_AL
{
    public class ClassEvent : MyPropertyChanged
    {
        /// <summary>
        /// Тип случившегося события.
        /// </summary>
        public enum EnumType
        {
            Start = 1,     //Старт программы.
            Finish = 2,    //Окончание работы.  
            Measure = 3,   //Измерение значения.
            Login = 4,     //Запись в лог.
            Connect = 5,   //Установление связи.
            Disconnect = 6,//Потеря связи.
            Over = 7,      //Превышение.
            Less = 8       //Понижение.
        }

        public enum EnumCategory
        {
            None = 0,
            Fault = 1,
            Norma = 2,
            Alarm = 3
        }

        private int _Ack;

        public List<string> LstDev { get; set; } = new List<string>();

        //ID записи события.
        public int ID { get; set; }
        //Время записи.
        public DateTime DT { get; set; }
        //Тип записи.
        public EnumType Type { get; set; }
        //Имя параметра.
        public string Param { get; set; }
        //Значение параметра.
        public string Val { get; set; }
        //ID источника.
        public int SourceID { get; set; }
        //Имя устройства.
        public string NameDevice { get; set; }
        //Статус действия в случае тревоги(снято/не снято).
        public int Ack
        {
            get { return _Ack; }
            set
            {
                _Ack = value;
                OnPropertyChanged("Ack");
                OnPropertyChanged("IsAlarm");
            }
        }

        public string StrDT { get { return DT.ToString("dd.MM.yyyy HH:mm:ss"); } }//Формат вывода даты(до секунд).
        public string StrDTM { get { return DT.ToString("dd.MM.yyyy HH:mm:ss.fff"); } }//Формат вывода даты(до милисекунд)

        public string Name
        {
            get
            {
                return Type switch
                {
                    EnumType.Start => "Запуск программы",
                    EnumType.Finish => "Завершение программы",
                    EnumType.Measure => "Изменение параметра",
                    EnumType.Login => "Регистрация пользователя",
                    EnumType.Connect => "Установлена связь",
                    EnumType.Disconnect => "Потеряна связь",
                    EnumType.Over => "Значение выше допустимого",
                    EnumType.Less => "Значение ниже допустимого",
                    _ => "Нет данных",
                };
            }
        }
        public EnumCategory Category
        {
            get
            {
                return Type switch
                {
                    EnumType.Connect => EnumCategory.Norma,
                    EnumType.Disconnect => EnumCategory.Fault,
                    EnumType.Over => EnumCategory.Fault,
                    EnumType.Less => EnumCategory.Fault,
                    _ => EnumCategory.None,
                };
            }
        }
        public bool IsAlarm
        {
            get
            {
                if ((Category == EnumCategory.Fault) && (_Ack == 0)) return true;
                else return false;
            }
        }

        public ClassEvent()
        {
            ID = 0;
            DT = DateTime.Now;
            Param = "";
            Val = "";
        }

        public static void SaveNewEvent(ClassEvent ev, bool Archive = true)
        {
            //Обращение к базе и сохранение лога, если включен флаг архивирования.
            if (Archive) 
                Task.Run(() => MainWindow.DB.EventAdd(ev));
            MainWindow.Events.Add(ev);
            // for (int i = 0; i < MainWindow.Links.Count; i++)
            // {
            //     ClassLink link = MainWindow.Links[i];
            //     if (link.EventType != ev.Type) continue;
            //     if (link.SourceID != ev.SourceID) continue;
            //     link.DT = DateTime.Now;
            //     ClassCommand cmd = MainWindow.Commands.FirstOrDefault(x => x.ID == link.Command.ID);
            //     if (cmd != null) MainWindow.QueueCommands.Enqueue(cmd);
            // }
        }

        /// <summary>
        /// Возвращает тип события.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static string GetEventName(ClassEvent.EnumType Type)
        {
            ClassEvent obj = new ClassEvent();
            obj.Type = Type;
            return obj.Name;
        }
        /// <summary>
        /// Возвращает все типы событий.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetEventNames()
        {
            Array values = Enum.GetValues(typeof(EnumType));
            List<string> lst = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                ClassEvent ev = new ClassEvent();
                ev.Type = (EnumType)values.GetValue(i);
                lst.Add(ev.Name);
            }
            return lst;
        }
        /// <summary>
        /// Сортировка объектов по ID.
        /// </summary>
        /// <param name="events"></param>
        public static void EventSortByID(List<ClassEvent> events)
        {
            for (int i = 0; i < events.Count; i++)
            {
                events[i].ID = i + 1;
            }
        }

    }
}
