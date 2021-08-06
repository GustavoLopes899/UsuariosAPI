using System;
using System.IO;
using System.Reflection;

namespace Ferramentas
{
    public class Logger
    {
        private static string caminhoExe = string.Empty;

        public static string TrataMensagemExcecao(Exception excecao)
        {
            while (excecao.InnerException != null)
            {
                excecao = excecao.InnerException;
            }
            return excecao.Message.Replace('\n', ' ');
        }

        public static string CaminhoArquivoLog(string assembly)
        {
            caminhoExe = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            string nomeArquivo = DateTime.Now.Date.ToString("yyyyMMdd - ") + assembly + ".log";
            return Path.Combine(caminhoExe, nomeArquivo);
        }

        public static void GravarLog(string mensagem)
        {
            try
            {
                string caminhoArquivo = CaminhoArquivoLog(Assembly.GetCallingAssembly().GetName().Name);
                if (!File.Exists(caminhoArquivo))
                {
                    FileStream arquivo = File.Create(caminhoArquivo);
                    arquivo.Close();
                }
                using StreamWriter sw = File.AppendText(caminhoArquivo);
                MontarLog(mensagem, sw);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static void MontarLog(string mensagem, TextWriter txtWriter)
        {
            try
            {
                txtWriter.WriteLine($"{DateTime.Now.ToShortDateString()} - {DateTime.Now.ToLongTimeString()}");
                txtWriter.WriteLine($"\tErro: {mensagem}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
