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
            //
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