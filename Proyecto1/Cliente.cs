using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
namespace Chat{
    class Cliente{
        static TcpClient cliente;
        static NetworkStream stream;
        static string username = "";

        public static void Run(){
            try{
                cliente = new TcpClient("127.0.0.1", 1122);//Ip y puerto de mi servidor
                stream = cliente.GetStream();

                Console.WriteLine("Conectado al servidor. Regístrate con 'login [nombre]'.");
                while(true){
                    string entrada = Console.ReadLine();
                    ProcesarComando(entrada);
                }
            }catch(Exception e){
                Console.WriteLine("Error : {0}", e.Message);
            }finally{
                stream.Close();
                cliente.Close();
            }
        }
        static void ProcesarComando(string entrada){
            string[] partes = entrada.Split(' ');
            if(partes.Length ==0){
                Console.WriteLine("Comando no válido.");
            }
        }
    }
}
