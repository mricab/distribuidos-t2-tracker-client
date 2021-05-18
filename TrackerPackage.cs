using System;
using System.Text;

namespace TrackerPackage
{
    class TrackerPkg
    {
        public int type; // 0 = location, 1 = ACK.
        public int device;
        public string message_id;
        public string message;

        public TrackerPkg(int hdr_type, int hdr_dev, string msg_id, string msg)
        {
            type = hdr_type; device = hdr_dev; message_id = msg_id;  message = msg;
        }

        public TrackerPkg(byte[] b)
        {
            byte[] t = new byte[1];
            byte[] d = new byte[1];
            byte[] mi = new byte[18];
            byte[] m = new byte[b.Length-20];

            System.Buffer.BlockCopy(b, 0, t, 0, t.Length);
            System.Buffer.BlockCopy(b, 1, d, 0, d.Length);
            System.Buffer.BlockCopy(b, 2, mi, 0, mi.Length);
            System.Buffer.BlockCopy(b, 20, m, 0, m.Length);
            
            type = int.Parse(Encoding.ASCII.GetString(t));
            device = int.Parse(Encoding.ASCII.GetString(d));
            message_id = Encoding.ASCII.GetString(mi);
            message = Encoding.ASCII.GetString(m);
        }

        public byte[] GetBytes()
        {
            byte[] t = Encoding.ASCII.GetBytes(type.ToString());    // 1 byte
            byte[] d = Encoding.ASCII.GetBytes(type.ToString());    // 1 byte
            byte[] mi = Encoding.ASCII.GetBytes(message_id);        // 18 bytes
            byte[] m = Encoding.UTF8.GetBytes(message);             // X bytes

            byte[] stream = new byte[20+m.Length];
            System.Buffer.BlockCopy(t, 0, stream, 0, t.Length);
            System.Buffer.BlockCopy(d, 0, stream, 1, d.Length);
            System.Buffer.BlockCopy(mi, 0, stream, 2, mi.Length);
            System.Buffer.BlockCopy(m, 0, stream, 20, m.Length);

            return stream;
        }
    }
}