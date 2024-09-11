using System;
using System.Net.Sockets;
using System.Text;

namespace Servidor{
    public static class MessageSender{
        public static void Send(Socket handler, string datos){
            byte[] byteData = Encoding.ASCII.GetBytes(datos);
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallBack), handler);
        }
        private static void SendCallBack(IAsyncResult ar){

            try{
                Socket handler = (Socket)ar.AsyncState;
                int bytesEnviados = handler.EndSend(ar);
                Console.WriteLine("Se enviaron {0} bytes al cliente.", bytesEnviados);
            }catch(Exception e){
                Console.WriteLine(e.ToString());
            }
        }

        public static void EnviarATodos(string mensaje, Estado reminente){
            foreach(var cliente in Server.Usuarios.Values){
                if(cliente != reminente){
                    Send(cliente.socketCliente, mensaje);

                }
            }
        }

    }

}
