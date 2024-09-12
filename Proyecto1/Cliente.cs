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
                return;
            }
            if(partes[0] == "login" && partes.Length ==2){
                username = partes[1];
                EnviarRegistro(username);
            }else if(partes[0] == "public" && partes.Length >= 2){
                string mensaje = string.Join(" ", partes, 1, partes.Length - 1);
                EnviarMensajePublico(mensaje);

            }else{
                Console.WriteLine("Comando inválido");
            }
        }
        static void EnviarRegistro(string nombre){
            var mensaje = new{
                type = "IDENTIFY",
                username = nombre
            };
            EnviarMensaje(mensaje);
            Console.WriteLine($"Te has registrado como : {nombre}.");
        }
        static void EnviarMensajePublico(string texto){
            if(string.IsNullOrEmpty(username)){
                Console.WriteLine("Debes identificarte para enviar mensajes, 'login' [nombre]");
            }
            var mensaje = new { type = "PUBLIC_TEXT", text = texto};
            EnviarMensaje(mensaje);
            Console.WriteLine($"Mensaje público : {texto}");
        }
        static void EnviarMensaje(object mensaje){
            string  json = Newtonsoft.Json.JsonConvert.SerializeObject(mensaje);
            byte[] data = Encoding.ASCII.GetBytes(json);
            stream.Write(data, 0, data.Length);
        }

    }
}
