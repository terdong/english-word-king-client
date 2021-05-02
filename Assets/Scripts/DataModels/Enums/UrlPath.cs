using System;

namespace TeamGehem
{
    public enum UrlPath : int
    {
        echo = 1000,

        login = 2000,
        logout,

        chat = 3000,
        chat_req_talk,
        chat_res_talk,

        game = 4000,
        lobby = 5000,
    }
}
