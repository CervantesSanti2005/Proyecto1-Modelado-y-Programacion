using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace Servidor{

    public class Estado{

        public Socket socketCliente = null;

        public const int TamanoBuffer = 1024;

        public byte[] buffer = new byte[TamanoBuffer];

        public StringBuilder sb = new StringBuilder();

        public int numeroCliente;
    }
    public class SocketAsincrono{
        public static ManualResetEvent terminado = new ManualResetEvent(false);

        public static Dictionary<int, Estado> Clientes = new Dictionary<int, Estado>();

        public static int ClienteConectado = 0;

        public SocketAsincrono(){

        }

        public static void empiezaEscuchar(){

            Byte[] bytes = new Byte[1024];
            int Puerto = 1122;
            IPAddress IP = IPAddress.Any;
            IPEndPoint EP = new IPEndPoint(IP, Puerto);
            Socket escucha = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try{
                escucha.Bind(EP);
                escucha.Listen(100);

                while(true){
                    terminado.Reset();

                    Console.WriteLine("Esperando Conexión");

                    escucha.BeginAccept(new AsyncCallback(AcceptCallBack), escucha);

                    terminado.WaitOne();
                }

            }
            catch(Exception e){
                Console.WriteLine("Ocurrio un error, en el método empiezaEscuchar"+e.ToString());
            }
            Console.WriteLine("\nPresiona ENTER para continuar");
            Console.Read();

        }
        public static void AcceptCallBack(IAsyncResult ar){
            ClienteConectado++;
            Console.WriteLine("Numero de cliente: " + ClienteConectado);
            terminado.Set();

            Socket escucha = (Socket) ar.AsyncState;
            Socket handler = escucha.EndAccept(ar);

            Estado estado = new Estado();
            estado.numeroCliente = ClienteConectado;

            Clientes.Add(ClienteConectado, estado);
            Console.WriteLine("Clientes totales {0}", Clientes.Count());

            estado.socketCliente = handler;
            handler.BeginReceive(estado.buffer, 0, Estado.TamanoBuffer, 0, new AsyncCallback(ReadCallback),estado);
        }
        public static void ReadCallback(IAsyncResult ar){
            String contenido = String.Empty;

            try{
                Estado estado = (Estado) ar.AsyncState;
                estado.sb.Clear();
                Socket handler = estado.socketCliente;

                int bytesRead = handler.EndReceive(ar);

                if(bytesRead > 0){
                    estado.sb.Append(Encoding.ASCII.GetString(estado.buffer,0,bytesRead));

                    contenido = estado.sb.ToString();

                    if(contenido.Substring(0, 3) == "cmd"){
                            foreach(Estado Cliente in Clientes.Values){
                                if(Cliente.numeroCliente == 1){
                                    Console.WriteLine("El valor es: "+Cliente.numeroCliente);
                                    if(estaConectado(Cliente.socketCliente)){
                                        Send(Cliente.socketCliente, "Recibiste mi mensaje?");
                                    }
                                    else{
                                        string respondeMensaje = "Numero de cliente: " + Cliente.numeroCliente + "está desconectado";
                                        Console.WriteLine(respondeMensaje);
                                        Send(handler, respondeMensaje);
                                    }
                                }
                            }
                    }

                }
                Console.WriteLine("Leer {0} bytes del cliente {1} socket. \n Datos: {2}", contenido.Length, estado.numeroCliente, contenido);

                if(estaConectado(handler)){
                    Send(handler, contenido);
                }
                handler.BeginReceive(estado.buffer,0, Estado.TamanoBuffer,0, new AsyncCallback(ReadCallback), estado);
            }
            catch(SocketException e){
                Console.WriteLine("Ocurrio una excepcion llamando el regreso : " + e.Message.ToString());
            }
            catch(Exception e){
                Console.WriteLine("Ocurrio una excepcion llamando el regreso : " + e.Message.ToString());
            }
        }
        private static void Send(Socket handler, String datos){
            byte[] byteData = Encoding.ASCII.GetBytes(datos);

            handler.BeginSend(byteData, 0, byteData.Length,0, new AsyncCallback(SendCallBack), handler);
        }
        private static void SendCallBack(IAsyncResult ar){
            try{
                Socket handler = (Socket)ar.AsyncState;

                int bytesEnviados = handler.EndSend(ar);
                Console.WriteLine("Se enviaron {0} bytes al cliente.", bytesEnviados);

            }
            catch(Exception e){
                Console.WriteLine(e.ToString());
            }
        }
        public static bool estaConectado(Socket handler){
            return handler.Connected;
        }
        public static int Main(string[] args){
            empiezaEscuchar();
            return 0;
        }


    }


}
