using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankingTCPIPLib.Banking_System.Miscellaneous
{
    public static class BankSerializable
    {
        /// <summary>
        ///     Serializes objects
        /// </summary>
        /// <param name="obj">Your object</param>
        /// <param name="path">Path to file</param>
        public static async Task SerializeAsync(object obj, string path)
        {
            await using var fs = new FileStream(path, FileMode.Create);

            await JsonSerializer.SerializeAsync(fs, obj);
        }

        /// <summary>
        ///     Deserializes objects of given type
        /// </summary>
        /// <param name="path">Path to file</param>
        public static async Task<T> DeSerializeAsync<T>(string path)
        {
            await using var fs = new FileStream(path, FileMode.Open);
            return await JsonSerializer.DeserializeAsync<T>(fs);
        }
    }
}