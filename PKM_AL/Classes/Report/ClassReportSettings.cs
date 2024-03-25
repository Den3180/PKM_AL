using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace PKM
{
    /// <summary>
    /// Тип отчетов.
    /// </summary>
    public enum EnumTypeReport
    {
        //Тип = Количество столбцов таблицы БД + заголовок.
        VEIyear = 18,
        EHZ01year = 14,
        EHZ04year = 10,
        EHZ06year = 24,
        EHZ08quarter = 16,
        F40year = 29,
        F41year = 39,
        F47year = 2,
        F52gas = 38,
        F02year = 19,
        F141gas = 40
    }
    /// <summary>
    /// Столбцы ЭХЗ-01-год.
    /// </summary>
    public struct EHZ1sYear
    {
        public string dateRec;
        public string nameLPU;
        public string nameObj;
        public string km;
        public int UKZ;
        public int SKZ;
        public string typeSKZ;
        public string dateinSKZ;
        public string methodGround;
        public string typeGround;
        public string elMeter;
        public string typeControl;
        public string bsz;
    }
    /// <summary>
    /// Столбцы ЭХЗ-04-год.
    /// </summary>
    public struct EHZ4sYear
    {
        public string dateRec;
        public string nameLPU;
        public string nameObj;
        public string UPZ;
        public string lastRepair;
        public string location;
        public string typeProtect;
        public int numProtect;
        public string startUPZ;
    }
    /// <summary>
    /// Столбцы ЭХЗ-06-год.
    /// </summary>
    public struct EHZ6sYear
    {
        public string dateRec;
        public string nameLPU;
        public string nameObj;
        public string actPit;
        public string datePit;
        public string km;
        public string gps;
        public int lenghtPit;
        public int dNar;
        public int thickness;
        public string reasonPit;
        public double deep;
        public string typeGround;
        public int soilResi;
        public string insulatiomat;
        public string adhesiomat;
        public int damageArea;
        public string nutCorrDamage;
        public int maxDeepDamage;
        public int damage1;
        public int damage13;
        public int damage3;
        public string ute;
    }
    /// <summary>
    /// Столбцы ЭХЗ-08-квартал.
    /// </summary>
    public struct EHZ8sQuarter
    {
        public string dateRec;
        public string nameLPU;
        public string nameTube;
        public string km;
        public string nameRoad;
        public string typeRoad;
        public string dateMeasure;
        public double uStart;
        public double uPatr;
        public double uSvech;
        public double uEnd;
        public double uPatrEnd;
        public double uSvechEnd;
        public double resis;
        public string note;
    }
    /// <summary>
    /// Столбцы ВЭИ-год.
    /// </summary>
    public struct VeiYear
    {
        public string dataRec;
        public string nameLPU;
        public string typeObj;
        public string location;
        public string km;
        public string typeFlange;
        public string placement;
        public string dateBegin;
        public string manufactory;
        public string dateMade;
        public string serial;
        public int dOut;
        public int pMax;
        public string status;
        public string kip;
        public string spark;
        public string bsz;
    }
    /// <summary>
    /// Столбцы Форма №40-год.
    /// </summary>
    public struct F40year
    {
        public string datarec;
        public string namelpu;
        public string nameobj;
        public string singlepipe;
        public string countKIP;
        public string countUKZ;
        public string countUDZ;
        public string countUPZ;
        public string countsource;
        public string countins;
        public string lengthLEP;
        public string countSKZtime;
        public string countSKZtele;
        public string electricity;
        public string electricityload;
        public string rejectionLEP;
        public string rejectionEHZ;
        public string rejectionSUM;
        public string securitysinglepipe;
        public string securityspKPZ;
        public string securitylenght;
        public string securitytime;
        public string securityKPZ;
        public string securityUvom;
        public string lenghtVKO;
        public string lenghtPKO;
        public string transresisMax;
        public string transresisMin;
    }    

    /// <summary>
    /// Класс настроек отчетов(заголовки, название столбцов и т.д.)
    /// </summary>
    class ClassReportSettings: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        /// <summary>
        /// Конструктор.
        /// </summary>
        public ClassReportSettings()
        { }
        #region[Паттерны регулярных выражений]
        public string DecimalPattern { get; } = @"(^-?0\.\d{1,4}$)|(^-?\d+(\.\d{1,4})$)|(^-?[1-9]+$)|(^0$)";//Для чисел +/- с с точкой и без.
        public string U_IntPattern { get; } = @"(^[1-9]+)$|^0$";//Для целых чисел без знака.
        public string Date_YearPattern { get; } = @"^[1-2]\d{3}"; //Дата в формате гггг.
        public string U_DecimalPattern { get; } = @"(^0\.\d{1,4}$)|(^\d+(\.\d{1,4})$)|(^[1-9]+$)|(^0$)"; //Для положительных целых чисел с точкой.
        public string Date_PointFullPattern { get; } = @"^[0-3]\d\.[0-1]\d\.[1-2]\d{3}$"; //Дата в формате ДД.ММ.ГГГГ.
        public string Date_TimeFullPattern { get; } = @"^[0-3]\d\.[0-1]\d\.[1-2]\d{3} ([0-1]\d|2[0-4]):[0-5]\d:[0-5]\d$"; //Дата в формате ДД.ММ.ГГГГ ЧЧ:ММ:СС
        public string GPS_Pattern { get; } = @"^[1-9]?\d [1-9]?\d\.\d{4}$";//ГГ°ММ.ММММ(в поле вводится пробел вместо градуса).
        public string Date_FullPattern { get; } = @"^[1-2]\d{3}-[0-1]\d-[0-3]\d$"; //Дата в формате гггг-мм-дд.
        public string U_Int_AdvancePattern { get; } = @"^\d+\W?\d*$";  //Целое число, не буквенно-цифровой символ,целое число.

        public string GetPattern(string unitType)
        {
            return unitType switch
            {                
                "dateyear"  => Date_YearPattern,
                "decimal"   => DecimalPattern,
                "uint"      => U_IntPattern,
                "udecimal"  => U_DecimalPattern,
                "datefull"  => Date_FullPattern,
                "uint_adv"  => U_Int_AdvancePattern,
                _ =>string.Empty
            };
        }
        #endregion

        #region[Заголовки отчетов]

        public List<(string, string)> VEIHeaders { get; } = new List<(string, string)>
        {
            ("Отчет о состоянии вставок (фланцев) электроизолирующих","string"),
            ("№п/п","uint"),
            ("Дата заполнения отчета","datefull"), 
            ("Наименование ЛПУ,(ГПУ и пр.)","string"),
            ("Тип объекта установки","string"),
            ("Место установки","string"),
            ("Км подключения","udecimal"),
            ("Тип","string"),
            ("Размещение","string"),
            ("Дата ввода в эксплуатацию","datefull"),
            ("Изготовитель","string"),
            ("Дата изготовления","datefull"),
            ("Серийный номер","string"),
            ("Dнар.,мм","uint"),
            ("Pmax,кгс/см2","uint"),
            ("Статус","string"),
            ("Наличие КИП","string"),
            ("Наличие искроразрядника","string"),
            ("Наличие БСЗ","string")            
        };
        public List<(string, string)> EHZ01Headers { get; } = new List<(string, string)>
        {
            ("Основные данные по установкам катодной защиты","string"),
            ("№п/п","uint"),
            ("Дата заполнения отчета","datefull"),
            ("Наименование ЛПУ,(ГПУ и пр.)","string"),
            ("Наименование объекта","string"),
            ("Км установки","udecimal"),
            ("№ УКЗ","uint"),
            ("№ СКЗ","uint"),
            ("Тип СКЗ(преобразователя)","string"),
            ("Дата ввода СКЗ в эксплуат.(установки)","datefull"),
            ("Вид заземления","string"),
            ("Тип заземлителя","string"),
            ("Наличие счетчиков электроэнергии или моточасов","string"),
            ("Тип системы телеконтроля","string"),
            ("Наличие блока совместной защиты","string")
        };      

        public List<(string, string)> EHZ04Headers { get; } = new List<(string, string)>
        {
            ("Основные данные по установкам протекторной защиты","string"),
            ("№п/п","uint"),
            ("Дата заполнения отчета","datefull"),
            ("Наименование ЛПУ,(ГПУ и пр.)","string"),
            ("Наименование объекта","string"),
            ("№ УПЗ","uint_adv"),
            ("Место подключения, км","udecimal"),
            ("Тип протектора","string"),
            ("Количество протекторов в УПЗ, шт.","uint"),
            ("Год ввода в эксплуатацию УПЗ","dateyear"),
            ("Год последнего ремонта УПЗ","dateyear")
        };

        public List<(string,string)> EHZ06Headers { get; } = new List<(string,string)>
        {
            ("Отчет о коррозионном состоянии трубопроводов по результатам шурфовочных работ","string"),
            ("№п/п","uint"),
            ("Дата заполнения отчета","datefull"),
            ("Наименование ЛПУ,(ГПУ и пр.)","string"),
            ("Наименование объекта","string"),
            ("№ Акта шурфовки","string"),
            ("Дата проведения шурфования","dateyear"),
            ("Координаты места шурфования(км)","udecimal"),
            ("Координаты места шурфования(GPS, град)","string"),
            ("Длина шурфа, м","uint"),
            ("Наружный диаметр трубопровода, мм","uint"),
            ("Толщина стенки трубопровода, мм","uint"),
            ("Основания для проведения шурфования","string"),
            ("Глубина заложения от верхней образующей трубопровода до поверхности земли, м","udecimal"),
            ("Тип грунта","string"),
            ("Удельное сопротивление грунта, Ом.м","udecimal"),
            ("Материал изоляционного покрытия","string"),
            ("Адгезия изоляционного покрытия","string"),
            ("Площадь сквозных повреждений изоляционного покрытия, см2","uint"),
            ("Характер коррозионных повреждений","string"),
            ("Максимальная глубина обнаруженного коррозионного повреждения, мм","uint"),
            ("Коррозионные повреждения глубиной до 1 мм, см 2","uint"),
            ("Коррозионные повреждения глубиной 1-3 мм, шт.","uint"),
            ("Коррозионные повреждения глубиной свыше 3 мм, шт.","uint"),
            ("Разность потенциалов 'труба-земля' в шурфе, В","decimal")

        };
        public List<(string,string)> EHZ08Headers { get; } = new List<(string,string)>
        {
            ("Сведения о результатах обследований перходов МГ через автомобильные и железные дорог","string"),
            ("№п/п","decimal"),
            ("Дата заполнения отчета","datefull"),
            ("Наименование ЛПУ,(ГПУ и пр.)","string"),
            ("Наименование труб-да","string"),
            ("Км по труб.","udecimal"),
            ("Наименование дороги","string"),
            ("Кат. дор./ кол-во ж/д путей","string"),
            ("Дата замера","datefull"),
            ("Uтр в начале перехода, В","decimal"),
            ("Uпатр в начале перехода, В","decimal"),
            ("Uсвечи в начале перехода, В","decimal"),
            ("Uтр в конце перехода,В","decimal"),
            ("Uпатр в конце перехода,В","decimal"),
            ("Uсвечи в конце перехода,В","decimal"),
            ("Сопротивление 'кожух-труба', Ом","decimal"),
            ("Примечание","string")
        };

        public List<(string, string)> F40yearHeaders { get; } = new List<(string, string)>
        {
            ("Отчет о работающих средствах ЭХЗ и защищенности от коррозии подземных трубопроводов","string"),
            ("№п/п","uint"),
            ("Дата заполнения отчета","datefull"),
            ("Наименование ЛПУ","string"),
            ("Наименование объекта","string"),
            ("Протяженность в однониточном исчислении, км.","udecimal"),
            ("Количество КИП, шт.","uint"),
            ("Количество УКЗ, шт.","uint"),
            ("Количество УДЗ, шт","uint"),
            ("Количество УПЗ, шт.","uint"),
            ("Количество автономных источников питания, шт.","uint"),
            ("Количество вставок электроизолирующих, шт.","uint"),
            ("Протяженность ЛЭП, км.","udecimal"),
            ("Количество СКЗ, оснащенных средствами учета времени работы, шт.","uint"),
            ("Количество СКЗ подключенных к системе телеконтроля, шт","uint"),
            ("Потребление электроэнергии, кВт*ч","udecimal"),
            ("Среднегодовая токовая нагрузка одной СКЗ, А","udecimal"),
            ("Продолжительность отказов в работе средств ЭХЗ по причине отказов ЛЭП, час","udecimal"),
            ("Продолжительность отказов в работе средств ЭХЗ по причине отказов элементов средств ЭХЗ, час","udecimal"),
            ("Суммарная продолжительность отказов в работе средств ЭХЗ, час","udecimal"),
            ("Защищено в однониточном исчислении, км.","udecimal"),
            ("Защищено в однониточном исчислении по комплексному показателю защищенности, км.","udecimal"),
            ("Защищенность по протяженности, %","udecimal"),
            ("Защищенность во времени, %","udecimal"),
            ("Защищенность по комплексному показателю защищенности","udecimal"),
            ("Защищенность по потенциалу без омической составляющей","udecimal"),
            ("Протяженность зон ВКО, км.","udecimal"),
            ("Протяженность зон ПКО, км.","udecimal"),
            ("Переходное сопротивление труба-земля Max, Ом*кв.м","udecimal"),
            ("Переходное сопротивление труба-земля Min, Ом*кв.м","udecimal")
        };        

        public List<(string, string)> F41yearHeaders { get; } = new List<(string, string)>()
        {            
            ("Отчет о результатах электрометрического обследования систем противокоррозионной защиты линейной части трубопроводов","string"),
            ("№п/п","uint"),
            ("Дата заполнения отчета","datefull"),
            ("Наименование ЛПУ","string"),
            ("Наименование объекта","string"),
            ("Километр начала обследования","udecimal"),
            ("Километр конца обследования","udecimal"),
            ("Протяженность, км","udecimal"),
            ("Ду,мм","uint"),
            ("Вид обследования","string"),
            ("Сроки проведения обследования","datefull"),
            ("Организация-исполнитель","string"),
            ("Протяженность зоны перезащиты,км","udecimal"),
            ("Протяженность зоны недозащиты,км","udecimal"),
            ("Протяженность зоны повреждепния изоляции,км","udecimal"),
            ("Протяженность зоны воздействия блуждающих токов,км","udecimal"),
            ("Протяженность зоны высокой коррозионной опасности,км","udecimal"),
            ("Протяженность зоны повышенной коррозионной опасности,км","udecimal"),
            ("Установки катодной защиты,всего,шт","uint"),
            ("Неисправные установки катодной защиты,шт ","uint"),
            ("Установки дренажной защиты, всего, шт.","uint"),
            ("Неисправные установки дренажной защиты, шт.","uint"),
            ("Установки протекторной защиты, всего, шт.","uint"),
            ("Неисправные установки протекторной защиты, шт.","uint"),
            ("Контрольно-измерительные пункты, всего, шт.","uint"),
            ("Неисправные контрольно-измерительные пункты, шт.","uint"),
            ("Дорожных переходов, всего, шт.","uint"),
            ("Дорожные переходы, имеющие контакт с патроном, шт.","uint"),
            ("Общее количество шурфов, шт.","uint"),
            ("Количество шурфов по результатам электрометрических обследований,шт","uint"),
            ("Суммарная площадь осмотра, м2","udecimal"),
            ("Площадь повреждения изоляционного покрытия, см2","udecimal"),
            ("Преобладающий характер повреждений","string"),
            ("Площадь осмотра металла трубы, см2","udecimal"),
            ("Площадь коррозионных поражений см2","udecimal"),
            ("Преобладающий характер коррозионных поражений","string"),
            ("Каверны глубиной до 1 мм, см2","udecimal"),
            ("Каверны глубиной 1-3 мм, шт.","uint"),
            ("Каверны глубиной свыше 3 мм, шт.","uint"),
            ("Примечание","string")
        };

        public List<(string, string)> F02yearHeaders { get; } = new List<(string, string)>()
        {
           ("Эксплуатационные характеристики средств ЭХЗ","string"),
           ("№п/п","uint"),
           ("Дата заполнения отчета","datefull"),
           ("Наименование ЛПУ","string"),
           ("Наименование объекта","string"),
           ("Км установки","udecimal"),
           ("Тип средства защиты","string"),
           ("№ УКЗ","uint_adv"),
           ("Состояние УКЗ","string"),
           ("Потребление электроэнергии УКЗ, кВт*час","udecimal"),
           ("№ СКЗ","uint_adv"),
           ("Средний (годовой) ток СКЗ,А","udecimal"),
           ("Среднее (годовое) напряжение СКЗ,В","decimal"),
           ("Сопротивление растеканию тока АЗ,Ом","decimal"),
           ("№ УПЗ","uint_adv"),
           ("Состояние УПЗ","string"),
           ("Средний (годовой) ток УПЗ,А","udecimal"),
           ("№ УДЗ","uint_adv"),
           ("Состояние УДЗ","string"),
           ("Средняя величина тока дренажа УДЗ,А","udecimal")
        };

        public List<(string, string)> F52gasHeaders { get; } = new List<(string, string)>()
        {
            ("Сводный отчет о ходе строительства средств электрохимзащиты на вновь строящихся объектах","string"),
            ("№п/п","uint"),
            ("Дата заполнения отчета","datefull"),
            ("Наименование ЛПУ","string"),
            ("Наименование объекта","string"),
            ("Наименование проектной организации","string"),
            ("Наименование эксплуатирующей организации","string"),
            ("Изоляционное покрытие.Тип покрытия","string"),
            ("Изоляционное покрытие.Протяженность, км","udecimal"),
            ("КИП.Проект,шт","uint"),
            ("КИП.Построено,шт","uint"),
            ("КИП.Включено в работу,шт","uint"),
            ("СКЗ.Проект,шт","uint"),
            ("СКЗ.Построено,шт","uint"),
            ("СКЗ.Включено в работу,шт","uint"),
            ("АЗ.Проект,шт","uint"),
            ("АЗ.Построено,шт","uint"),
            ("АЗ.Включено в работу,шт","uint"),
            ("СДЗ.Проект,шт","uint"),
            ("СДЗ.Построено,шт","uint"),
            ("СДЗ.Включено в работу,шт","uint"),
            ("УПЗ.Проект,шт","uint"),
            ("УПЗ.Построено,шт","uint"),
            ("УПЗ.Включено в работу,шт","uint"),
            ("Автономные источники питания.Проект,шт","uint"),
            ("Автономные источники питания.Построено,шт","uint"),
            ("Автономные источники питания.Включено в работу,шт","uint"),
            ("Вставки электроизолирующие.Проект,шт","uint"),
            ("Вставки электроизолирующие.Построено,шт","uint"),
            ("Вставки электроизолирующие.Включено в работу,шт","uint"),
            ("Системы коррозионного мониторинга.Проект,шт","uint"),
            ("Системы коррозионного мониторинга.Построено,шт","uint"),
            ("Системы коррозионного мониторинга.Включено в работу,шт","uint"),
            ("Дорожные переходы.Проект,шт","uint"),
            ("Дорожные переходы.Построено,шт","uint"),
            ("Дорожные переходы.Включено в работу,шт","uint"),
            ("Протяженность объекта в строящемся однониточном исчислении, шт.Проект,шт","uint"),
            ("Протяженность объекта в строящемся однониточном исчислении, шт.Построено,шт","uint"),
            ("Протяженность объекта в строящемся однониточном исчислении, шт.Включено в работу,шт","uint")
        };

        public List<(string, string)> F141gasHeaders { get; } = new List<(string, string)>()
        {
            ("Отчет о выполнении плана технического обслуживания и ремонта средств электрохимической защиты трубопроводов","string"),
            ("№п/п","uint"),
            ("Дата заполнения отчета","datefull"),
            ("Наименование ЛПУ","string"),
            ("Наименование объекта","string"),
            ("Инв.№","uint"),
            ("Вид работ","string"),
            ("СМР.Потребность,тыс.руб.(без НДС)","udecimal"),
            ("СМР.План,тыс.руб.(без НДС)","udecimal"),
            ("СМР.Факт,тыс.руб.(без НДС)","udecimal"),
            ("МТР.Потребность,тыс.руб.(без НДС)","udecimal"),
            ("МТР.План,тыс.руб.(без НДС)","udecimal"),
            ("МТР.Факт,тыс.руб.(без НДС)","udecimal"),
            ("ПИР.Потребность,тыс.руб.(без НДС)","udecimal"),
            ("ПИР.План,тыс.руб.(без НДС)","udecimal"),
            ("ПИР.Факт,тыс.руб.(без НДС)","udecimal"),
            ("УКЗ,шт.Потребность","uint"),
            ("УКЗ,шт.План","uint"),
            ("УКЗ,шт.Факт","uint"),
            ("ВЛ,км.Потребность","udecimal"),
            ("ВЛ,км.План","udecimal"),
            ("ВЛ,км.Факт","udecimal"),
            ("УДЗ,шт.Потребность","uint"),
            ("УДЗ,шт.План","uint"),
            ("УДЗ,шт.Факт","uint"),
            ("УПЗ,шт.Потребность","uint"),
            ("УПЗ,шт.План","uint"),
            ("УПЗ,шт.Факт","uint"),
            ("Вставки электроизолирующие,шт.Потребность","uint"),
            ("Вставки электроизолирующие,шт.План","uint"),
            ("Вставки электроизолирующие,шт.Факт","uint"),
            ("КИП,шт.Потребность","uint"),
            ("КИП,шт.План","uint"),
            ("КИП,шт.Факт","uint"),
            ("Автономные источники тока,шт.Потребность","uint"),
            ("Автономные источники тока,шт.План","uint"),
            ("Автономные источники тока,шт.Факт","uint"),
            ("Прочие объекты ЭХЗ,шт.Потребность","uint"),
            ("Прочие объекты ЭХЗ,шт.План","uint"),
            ("Прочие объекты ЭХЗ,шт.Факт","uint"),
            ("Примечание","string"),

        };
        #endregion
        /// <summary>
        /// Выбор списка заголовков.
        /// </summary>
        /// <param name="typeReport"></param>
        /// <returns></returns>
        public List<(string,string)> GetReportHaders(EnumTypeReport typeReport)
        {          
            return typeReport switch
            {
                EnumTypeReport.VEIyear => VEIHeaders,
                EnumTypeReport.EHZ01year => EHZ01Headers,
                EnumTypeReport.EHZ04year => EHZ04Headers,
                EnumTypeReport.EHZ06year => EHZ06Headers,
                EnumTypeReport.EHZ08quarter => EHZ08Headers,
                EnumTypeReport.F40year => F40yearHeaders,
                EnumTypeReport.F41year => F41yearHeaders,
                EnumTypeReport.F02year => F02yearHeaders,
                EnumTypeReport.F52gas => F52gasHeaders,
                EnumTypeReport.F141gas=>F141gasHeaders,
                _ => new List<(string,string)>()
            };
        }
        /// <summary>
        /// Выбор перечисления отчета в зависимости от вкладки меню.
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static EnumTypeReport GetEnumTypeReport(string headers)
        {
            return headers switch
            {
                "Форма № ВЭИ-год"=>EnumTypeReport.VEIyear,
                "Форма № ЭХЗ-01-год"=>EnumTypeReport.EHZ01year,
                "Форма № ЭХЗ-04-год"=>EnumTypeReport.EHZ04year,
                "Форма № ЭХЗ-06-год"=>EnumTypeReport.EHZ06year,
                "Форма № ЭХЗ-08-квартал"=>EnumTypeReport.EHZ08quarter,
                "Форма № 41-год"=>EnumTypeReport.F41year,
                "Форма № 40-год"=>EnumTypeReport.F40year,
                "Форма № 52-газ"=>EnumTypeReport.F52gas,
                "Форма № 47-год"=>EnumTypeReport.F47year,
                "Форма № 02-год"=>EnumTypeReport.F02year,
                _=> EnumTypeReport.F141gas
            };
        }


    }
}
