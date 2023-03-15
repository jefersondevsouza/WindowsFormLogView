using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Data;

namespace ConsoleLogAppMonitor
{
    internal class Program
    {
        static long position = 0;
        static void Main(string[] args)
        {
            LerArquivo();
        }

        public static void LerArquivo()
        {
            string caminho = string.Empty;

            if (File.Exists("filetoread.txt"))
            {
                caminho = File.ReadAllText("filetoread.txt");
            }

            if (string.IsNullOrWhiteSpace(caminho) || !File.Exists(caminho))
            {
                Console.WriteLine("Informe o local/Nome do arquivo para monitoramento: ");
                caminho = Console.ReadLine();
            }

            if (File.Exists(caminho))
            {
                position = 0;
                MonitorarArquivo(caminho);
            }
            else
            {
                Console.WriteLine("Não foi possível encontrar o arquivo. ");
                LerArquivo();
            }
        }

        private static void MonitorarArquivo(string caminhoArquivo)
        {
            //C:\Users\jsilveira\Desktop\UaisoftAgendamentoService\Uaisoft.Agendamento.Service\bin\Debug\Log\UaisoftAgendamentoService_Log.txt

            try
            {
                string prov = "prov.txt";
                File.Copy(caminhoArquivo, prov, true);

                using (FileStream fs = new FileStream(prov, FileMode.Open, FileAccess.Read))
                {
                    if (position > 0)
                    {
                        fs.Position = position;
                    }

                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string s = sr.ReadToEnd();
                        if (s.Length > 0)
                        {
                            Console.Write(s);
                            position = fs.Position;
                        }
                    }
                }

                File.Delete(prov);
                Thread.Sleep(10000);
                MonitorarArquivo(caminhoArquivo);

            }
            catch (Exception ex)
            {
                MonitorarArquivo(caminhoArquivo);
            }
        }
    }
}



//using (IsolatedStorageFile isolationStorage = IsolatedStorageFile.GetUserStoreForAssembly())
//{
//    string settingsFileName = string.Format("{0}.xml", HOST_SETTINGS_CONFIGURATION_FILE_NAME);

//    if (isolationStorage.FileExists(settingsFileName))
//    {
//        //isolationStorage.i
//        using (IsolatedStorageFileStream isolationStorageStream = isolationStorage.OpenFile(settingsFileName, FileMode.Open, FileAccess.Read))
//        {
//            XmlSerializer serializer = new XmlSerializer(typeof(HostSettings));
//            settings = (HostSettings)serializer.Deserialize(isolationStorageStream);

//            HostName = settings.HostName;
//            PortNumber = settings.PortNumber;
//            Protocol = settings.Protocol;
//            EnableJobSchedulerService = settings.EnableJobSchedulerService;
//            UseRemoteJobSchedulerService = settings.UseRemoteJobSchedulerService;

//            if (settings.RemoteJobSchedulerSettings != null)
//            {
//                RemoteJobSchedulerSettings = settings.RemoteJobSchedulerSettings;
//            }

//            if (settings.JobSchedulerServiceSettings != null)
//            {
//                JobSchedulerServiceSettings = settings.JobSchedulerServiceSettings;
//            }