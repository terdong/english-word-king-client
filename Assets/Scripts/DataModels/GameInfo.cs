using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameInfo
{
    private static readonly string login_url_ = "ws://192.168.0.2:10012/";
    //private static readonly string login_url_ = "wss://192.168.0.2:10012/";
    public static string Login_Url { get { return login_url_; } private set { } }
}
