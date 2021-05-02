using UnityEngine;
using System.Collections;
using WebSocketSharp;
using System;

namespace TeamGehem
{
    public class SocketPackage
    {
        public string Key { get; set; }
        public UrlMaker Url_Path { get; set; }
        public WebSocket Web_Socket { get; set; }
        //public  EventHandler Event_Handler_Group {

        public SocketPackage( string key, UrlMaker url_path )
        {
            Key = key;
            Url_Path = url_path;
            Web_Socket = null;
        }

        public SocketPackage( string key, UrlMaker url_path, WebSocket web_socket )
        {
            Key = key;
            Url_Path = url_path;
            Web_Socket = web_socket;
        }

        public void DisconnectSocket()
        {
            if ( Web_Socket != null )
            {
                if ( Web_Socket.IsAlive )
                {
                    try
                    {
                        Web_Socket.Close();
                    }
                    catch ( Exception e )
                    { Debug.LogError( string.Format( "DisconnectSocket() : {0}", e.Message ) ); }
                    finally
                    {
                        Web_Socket = null;
                    }
                }
            }
        }

        public void DisconnectAsyncSocket()
        {
            if ( Web_Socket != null )
            {
                if ( Web_Socket.IsAlive )
                {
                    try
                    {
                        Web_Socket.CloseAsync();
                    }
                    catch ( Exception e )
                    { Debug.LogError( string.Format( "DisconnectSocket() : {0}", e.Message ) ); }
                    finally
                    {
                        Web_Socket = null;
                    }
                }
            }
        }

        public void Send( string str ) { Web_Socket.Send( str ); }
        public void Send( byte[] byte_array ) { Web_Socket.Send( byte_array ); }
    }
}