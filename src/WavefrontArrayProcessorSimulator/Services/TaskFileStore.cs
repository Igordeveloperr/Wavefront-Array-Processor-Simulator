using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using WaveVS.WaveFrontVS.Task;

namespace WaveVS.Services
{
    internal static class TaskFileStore
    {
        // BinaryFormatter is retained for compatibility with existing .shizotex files.
        public static void Save(string path, TaskData task)
        {
            using (var stream = new FileStream(path, FileMode.Create))
                new BinaryFormatter().Serialize(stream, task);
        }

        public static TaskData Load(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
                return (TaskData)new BinaryFormatter().Deserialize(stream);
        }
    }
}
