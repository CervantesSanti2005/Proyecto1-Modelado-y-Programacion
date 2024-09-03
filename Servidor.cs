using.System.Text;
using.System.Threading.Task;
using.System.Threading;
using.System.Net;
using.System.Net.Sockets;

namespace Servidor
{
    class Servidor
    {
        /*
        Declaracion del Socket publico y un buffer
         */
        public class Declaracion
        {
            public Socket pSocket = null;
            public const int tamanoBuffer = 1024;
            public byte[] buffer = new byte[tamanoBuffer];
            public StringBuilder s = new StringBuilder();
        }
        public class EscuchaSocketAsincrono
        {
            public static void Escuchando()
            {
                /*
                 Creamos un socket TCP y lo comenzamos en un purto en específico
                 */
                byte b = new byte[1024];
                IPHostEntry ipHostEntry = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAdress = ipHostEntry.AdressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAdress, 8888);
                Socket escucha = new Socket(ip.AdressFamily,SocketType.Stream,ProtocolType.Tcp);
                try
                {
                    escucha.Bind(ipEndPoint);
                    escucha.List
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }



    }
}
