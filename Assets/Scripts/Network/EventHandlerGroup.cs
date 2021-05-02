using UnityEngine;
using System;
using WebSocketSharp;
using TeamGehem.DataModels.Protocols;

namespace TeamGehem
{
    public struct EventHandlerGroup
    {
        public EventHandler On_Open                         { private set; get; }
        public EventHandler<MessageEventArgs> On_Message    { private set; get; }
        public EventHandler<CloseEventArgs> On_Close        { private set; get; }
        public EventHandler<ErrorEventArgs> On_Error        { private set; get; }

        public EventHandlerGroup(EventHandler on_open, EventHandler<MessageEventArgs> on_message, EventHandler<CloseEventArgs> on_close, EventHandler<ErrorEventArgs> on_error)
            : this()
        {
            On_Open = on_open;
            On_Message = on_message;
            On_Close = on_close;
            On_Error = on_error;
        }

        public EventHandlerGroup(EventHandler on_open, EventHandler<MessageEventArgs> on_message, EventHandler<CloseEventArgs> on_close)
            : this()
        {
            On_Open = on_open;
            On_Message = on_message;
            On_Close = on_close;
            On_Error = null;
        }

        public EventHandlerGroup(EventHandler on_open, EventHandler<CloseEventArgs> on_close)
            : this()
        {
            On_Open = on_open;
            On_Message = null;
            On_Close = on_close;
            On_Error = null;
        }

        public EventHandlerGroup(EventHandler<MessageEventArgs> on_message)
            : this()
        {
            On_Open = null;
            On_Message = on_message;
            On_Close = null;
            On_Error = null;
        }

        public void AddEventHandler(WebSocket ws)
        {
            if (On_Open != null)    ws.OnOpen    += On_Open;
            if (On_Message != null) ws.OnMessage += On_Message;
            if (On_Close != null)   ws.OnClose   += On_Close;
            if (On_Error != null)   ws.OnError   += On_Error;
        }
    }
}
