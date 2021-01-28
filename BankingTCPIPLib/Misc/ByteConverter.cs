using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BankingTCPIPLib.Misc
{
    public static class ByteConverter
    {
        // Convert an object to a byte array
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;

            var bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf?.Serialize(ms, obj);

            return ms.ToArray();
        }

        // Convert a byte array to an Object
        public static object ByteArrayToObject<T>(byte[] arrBytes)
        {
            using var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = (T) binForm.Deserialize(memStream);

            return obj;
        }
    }
}