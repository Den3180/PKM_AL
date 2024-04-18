namespace PKM_AL.Classes;

public class ClassUser : MyPropertyChanged
{
    private string _Name;
    private string _Login;
    private string _Pass;
    private int _Grant;

    public int ID { get; set; }

    public string Name
    {
        get { return _Name; }
        set
        {
            _Name = value;
            OnPropertyChanged("Name");
        }
    }
    public string Login
    {
        get { return _Login; }
        set
        {
            _Login = value;
            OnPropertyChanged("Login");
        }
    }
    public string Pass
    {
        get { return _Pass; }
        set
        {
            _Pass = value;
            OnPropertyChanged("Pass");
        }
    }
    public int Grant
    {
        get { return _Grant; }
        set
        {
            _Grant = value;
            OnPropertyChanged("GrantAck");
            OnPropertyChanged("GrantControl");
            OnPropertyChanged("GrantConfig");
        }
    }

    public bool GrantAck
    {
        get
        {
            if ((Grant & 0x01) != 0) return true;
             return false;
        } 
    }
    public bool GrantControl
    {
        get
        {
            if ((Grant & 0x02) != 0) return true;
             return false;
        }
    }
    public bool GrantConfig
    {
        get
        {
            if ((Grant & 0x04) != 0) return true;
             return false;
        }
    }

    public ClassUser()
    {
        ID = 0;
    }
}