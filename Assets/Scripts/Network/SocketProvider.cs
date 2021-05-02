using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using WebSocketSharp;

namespace TeamGehem
{
    public class SocketProvider
    {
        public WebSocket CreateWebSocket(string url)
        {
            WebSocket ws = new WebSocket(url);

            //TODO: 이거 적용하면 왜 exception 에러나는지 모르겠다. [Terdong : 2014-09-15]
            //ws.Compression = CompressionMethod.DEFLATE;

            ws.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                Debug.Log(String.Format("ws.ServerCertificateValidationCallback :: certificate.Issuer = {0}, certificate.Subject = {1}", certificate.Issuer, certificate.Subject));
                return true; // If the server cert is valid
            };

            ws.OnOpen += (sender, e) =>
            {
                Debug.Log("ws.OnOpen :: Connection Success!, isOpen = " + (ws.ReadyState == WebSocketState.OPEN));
               // Toast.ShowToast(string.Format(StringFormat.Server_Connection_Success));
            };
            ws.OnMessage += (sender, e) =>
            {
                if (e.Type == Opcode.TEXT)
                {
                    Debug.Log(string.Format("ws.OnMessage :: e.Data = {0}", e.Data));
                }
            };
            ws.OnError += (sender, e) =>
            {
                Debug.LogError(string.Format("ws.OnError :: channel = {0}\n, ws.IsAlive = {1}, exception = {2}", url, ws.IsAlive, e.Message));
            };
            ws.OnClose += (sender, e) =>
            {
                if (e.WasClean)
                {
                    Debug.Log(string.Format("ws.OnClose :: Disconnect Success!, channel = {0}, isClosed = {1}", url, (ws.ReadyState == WebSocketState.CLOSED)));
                    //Toast.ShowToast(string.Format(StringFormat.Server_DisConnection_Success));
                }
                else
                {
                    Debug.LogError(string.Format("ws.OnClose :: channel = {0}, e.Code = {1}, e.Reason= {2}", url, e.Code, e.Reason));
                    //Toast.ShowToast(string.Format(StringFormat.Server_Connection_Fail));
                }
            };
            return ws;
        }
    }
}