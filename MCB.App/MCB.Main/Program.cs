using MCB.Service;
using Newtonsoft.Json.Linq;

namespace MCB.Main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = JObject.Parse(File.ReadAllText("config.json"));

            string corruptionDataPath = config["filePaths"]["corruptionData"].ToString();
            string developmentDataPath = config["filePaths"]["developmentData"].ToString();

            var dataService = new DataService(corruptionDataPath, developmentDataPath);
            dataService.ProcessData();
        }
    }
}
