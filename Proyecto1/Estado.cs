using System.Net.Sockets;
using System.Text;

namespace Chat{

    public class Estado{

        public Socket socketCliente = null;
        public const int TamanoBuffer = 1024;
        public byte[] buffer = new byte[TamanoBuffer];
        public StringBuilder sb = new StringBuilder();
        public int numeroCliente;
        public string username = "";
        public bool identificado = false;
        public string status = "ACTIVE";
    }

}
