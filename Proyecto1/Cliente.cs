using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Chat
{
    class Cliente
    {
        static TcpClient cliente;
        static NetworkStream stream;
        static string username = "";

        public static void Run()
        {
            try
            {
                cliente = new TcpClient("127.0.0.1", 1122); // IP y puerto del servidor
                stream = cliente.GetStream();

                Console.WriteLine("Conectado al servidor. Regístrate con 'login [nombre]'.");
                while (true)
                {
                    string entrada = Console.ReadLine();
                    ProcesarComando(entrada);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            finally
            {
                stream.Close();
                cliente.Close();
            }
        }

        // Procesar comandos del cliente
        static void ProcesarComando(string comando)
        {
            string[] partes = comando.Split(' ');
            if (partes.Length == 0)
            {
                Console.WriteLine("Comando no válido.");
                return;
            }

            if (partes[0].ToLower() == "login" && partes.Length == 2)
            {
                string username = partes[1];
                var mensaje = new
                {
                    type = "IDENTIFY",
                    username = username
                };
                EnviarMensaje(mensaje);
            }else if (partes[0].ToLower() == "public" && partes.Length >= 2){
                string mensajeTexto = string.Join(" ", partes, 1, partes.Length - 1);
                var mensaje = new
                {
                    type = "PUBLIC_TEXT",
                    text = mensajeTexto
                };
                EnviarMensaje(mensaje);
            }else if(partes[0].ToLower() == "dm" && partes.Length >= 3){
                string destinatario = partes[1];
                string mensajeDirecto = string.Join(" ", partes, 2, partes.Length - 2);
                var mensaje = new{
                    type = "TEXT",
                    para = destinatario,
                    text = mensajeDirecto
                };
                EnviarMensaje(mensaje);

            }
            else
            {
                Console.WriteLine("Comando inválido.");
            }
        }

        // Enviar mensaje al servidor
        static void EnviarMensaje(object mensaje)
        {
            string json = JsonConvert.SerializeObject(mensaje);
            Console.WriteLine("Enviando JSON: " + json); // Imprimir el JSON que se va a enviar
            json += "\n";  // Agregar un salto de línea al final del mensaje para que el servidor lo reconozca
            byte[] data = Encoding.ASCII.GetBytes(json);
            stream.Write(data, 0, data.Length);
        }
    }
}
