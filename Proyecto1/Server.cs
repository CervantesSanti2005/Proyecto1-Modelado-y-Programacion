using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chat
{
    public class Server
    {
        public static ManualResetEvent terminado = new ManualResetEvent(false);
        public static Dictionary<int, Estado> Clientes = new Dictionary<int, Estado>();
        public static Dictionary<string, Estado> Usuarios = new Dictionary<string, Estado>();
        public static int ClienteConectado = 0;

        public static void empiezaEscuchar()
        {
            int Puerto = 1122;
            IPAddress IP = IPAddress.Any;
            IPEndPoint EP = new IPEndPoint(IP, Puerto);
            Socket escucha = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                escucha.Bind(EP);
                escucha.Listen(100);

                while (true)
                {
                    terminado.Reset();
                    Console.WriteLine("Esperando Conexión...");
                    escucha.BeginAccept(new AsyncCallback(AcceptCallBack), escucha);
                    terminado.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ocurrió un error en el método empiezaEscuchar: " + e.ToString());
            }
            Console.WriteLine("\nPresiona ENTER para continuar");
            Console.Read();
        }

        // Aceptar una conexión nueva
        public static void AcceptCallBack(IAsyncResult ar)
        {
            ClienteConectado++;
            Console.WriteLine("Número de cliente: " + ClienteConectado);
            terminado.Set();

            Socket escucha = (Socket)ar.AsyncState;
            Socket handler = escucha.EndAccept(ar);

            Estado estado = new Estado();
            estado.numeroCliente = ClienteConectado;
            estado.socketCliente = handler;

            // Agregar el cliente al diccionario
            Clientes.Add(ClienteConectado, estado);
            Console.WriteLine("Clientes totales: {0}", Clientes.Count());

            // Empezar a recibir datos de este cliente
            handler.BeginReceive(estado.buffer, 0, Estado.TamanoBuffer, 0, new AsyncCallback(ReadCallback), estado);
        }

        // Leer datos del cliente
        public static void ReadCallback(IAsyncResult ar)
        {
            String contenido = String.Empty;
            try
            {
                Estado estado = (Estado)ar.AsyncState;
                Socket handler = estado.socketCliente;
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    estado.sb.Append(Encoding.ASCII.GetString(estado.buffer, 0, bytesRead));
                    contenido = estado.sb.ToString();

                    Console.WriteLine("Contenido recibido: {0}", contenido);

                    // Pasamos el manejo del protocolo a otra clase
                    ProtocoloHandler.ProcesarMensaje(contenido, estado);

                    estado.sb.Clear();
                }

                handler.BeginReceive(estado.buffer, 0, Estado.TamanoBuffer, 0, new AsyncCallback(ReadCallback), estado);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Ocurrió una excepción llamando el regreso: " + e.Message.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Ocurrió una excepción llamando el regreso: " + e.Message.ToString());
            }
        }

        public static int Main(string[] args)
        {
            empiezaEscuchar();
            return 0;
        }
    }
}
