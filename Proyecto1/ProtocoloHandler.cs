using System;
using Newtonsoft.Json;

namespace Chat{
    public static class ProtocoloHandler{

        public static void ProcesarMensaje(string contenido, Estado estado){
            try{
                dynamic mensaje = JsonConvert.DeserializeObject(contenido);
                string tipoMensaje = mensaje.type;

                if (Enum.TryParse(tipoMensaje, out Protocolo.MessageType tipoMensajeEnum)){
                    switch (tipoMensajeEnum){
                        case Protocolo.MessageType.IDENTIFY:
                            identificarUsuario(mensaje, estado);
                            break;
                        case Protocolo.MessageType.STATUS:
                            cambiarEstadoUsuario(mensaje, estado);
                            break;
                        case Protocolo.MessageType.USERS:
                            enviarListaUsuarios(estado);
                            break;
                        case Protocolo.MessageType.TEXT:
                            enviarMensajePrivado(mensaje, estado);
                            break;
                        case Protocolo.MessageType.PUBLIC_TEXT:
                            enviarMensajePublico(mensaje,estado);
                            break;
                        default:
                            if (!estado.identificado){
                                var respuesta = new{
                                    type = Protocolo.MessageType.RESPONSE.ToString(),
                                    operation =Protocolo.OperationType.INVALID.ToString(),
                                    result = Protocolo.OperationResult.INVALID.ToString()
                                };
                                MessageSender.Send(estado.socketCliente, JsonConvert.SerializeObject(respuesta));
                                estado.socketCliente.Close();
                            }else{
                            Console.WriteLine("Mensaje recibido de {0}: {1}", estado.username, contenido);
                            }
                        break;
                    }
                }else{
                    Console.WriteLine("Tipo de mensaje no reconocido: {0}", tipoMensaje);
                }
            }catch (JsonReaderException ex){
                Console.WriteLine("Error al procesar el JSON: " + ex.Message);
                // Enviar un mensaje de error al cliente en caso de que el JSON sea inválido
                var respuesta = new{
                    type = "RESPONSE",
                    operation = "INVALID",
                    result = "INVALID_JSON"
                };
                MessageSender.Send(estado.socketCliente, JsonConvert.SerializeObject(respuesta));
            }catch (Exception ex){
                Console.WriteLine("Error inesperado: " + ex.Message);
            }
        }


        public static void identificarUsuario(dynamic mensaje, Estado estado){
            string username = mensaje.username;
            if(Server.Usuarios.ContainsKey(username)){
                var respuesta = new{
                    type = Protocolo.MessageType.RESPONSE.ToString(),
                    operation = Protocolo.OperationType.IDENTIFY.ToString(),
                    result = Protocolo.OperationResult.USER_ALREADY_EXISTS.ToString(),
                    extra = username
                };
                MessageSender.Send(estado.socketCliente, JsonConvert.SerializeObject(respuesta));
            }else{
                estado.username = username;
                estado.identificado = true;
                Server.Usuarios[username] = estado;

                var respuesta = new{
                    type = Protocolo.MessageType.RESPONSE.ToString(),
                    operation = Protocolo.OperationType.IDENTIFY.ToString(),
                    result = Protocolo.OperationResult.SUCCESS.ToString(),
                    extra = username
                };
                MessageSender.Send(estado.socketCliente, JsonConvert.SerializeObject(respuesta));

                var notificacion = new{
                    type = Protocolo.MessageType.NEW_USER.ToString(),
                    username = username
                };
                MessageSender.EnviarATodos(JsonConvert.SerializeObject(notificacion), estado);
            }
        }

        public static void cambiarEstadoUsuario(dynamic mensaje, Estado estado){
            string nuevoEstado = mensaje.status;
            if(Enum.TryParse(nuevoEstado, out Protocolo.UserStatus estadoUsuario)){

                estado.status = estadoUsuario.ToString();

                var notificacion = new{
                    type = Protocolo.MessageType.NEW_STATUS.ToString(),
                    username = estado.username,
                    status = estado.status
                };
                MessageSender.EnviarATodos(JsonConvert.SerializeObject(notificacion), estado);
            }else{
                Console.WriteLine("Estado inválido");
            }
        }
        public static void enviarListaUsuarios(Estado estado){
            var usuarios = Server.Usuarios.ToDictionary(u => u.Key, u => u.Value.status);
            var respuesta = new{
                type = Protocolo.MessageType.USER_LIST.ToString(),
                users = usuarios
            };
            MessageSender.Send(estado.socketCliente, JsonConvert.SerializeObject(respuesta));
        }
        public static void enviarMensajePublico(dynamic mensaje, Estado estado){
            string texto = mensaje.text;

            var notificacion = new{
                type = Protocolo.MessageType.PUBLIC_TEXT_FROM.ToString(),
                username = estado.username,
                text = texto
            };
            MessageSender.EnviarATodos(JsonConvert.SerializeObject(notificacion), estado);
        }
        public static void enviarMensajePrivado(dynamic mensaje, Estado estado){
            string destinatario = mensaje.para;
            string texto = mensaje.text;

            if(Server.Usuarios.ContainsKey(destinatario)){
                Estado estadoDestinatario = Server.Usuarios[destinatario];

                var mensajePrivado = new{
                    type = "TEXT_FROM",
                    from = estado.username,
                    text = texto
                };

                MessageSender.Send(estadoDestinatario.socketCliente, JsonConvert.SerializeObject(mensajePrivado));
                Console.WriteLine("Mensaje privado de {0} a {1}: {2}", estado.username, destinatario, texto);
            }else{
                var mensajeError = new{
                    type = "RESPONSE",
                    operation = "TEXT",
                    result = "NO_SUCH_USER",
                    extra = destinatario
                };

            MessageSender.Send(estado.socketCliente, JsonConvert.SerializeObject(mensajeError));
            Console.WriteLine("El destinatario {0} no está conectado", destinatario);
            }
        }

    }
}
