using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Ferramentas
{
    public class ArquivoIni
    {
        private readonly string caminho;
        private readonly string nomeExecutavel = Assembly.GetCallingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern long WritePrivateProfileString(string secao, string chave, string valor, string caminho);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileString(string secao, string chave, string padrao, StringBuilder retorno, int tamanho, string caminho);

        public ArquivoIni(string caminho = null)
        {
            this.caminho = new FileInfo(caminho ?? this.nomeExecutavel + ".ini").FullName;
        }

        public string Ler(string chave, string secao = null)
        {
            StringBuilder retorno = new StringBuilder(255);
            GetPrivateProfileString(secao ?? this.nomeExecutavel, chave, string.Empty, retorno, 255, this.caminho);
            return retorno.ToString();
        }

        public void Escrever(string chave, string valor, string secao = null)
        {
            WritePrivateProfileString(secao ?? this.nomeExecutavel, chave, valor, this.caminho);
        }

        public void DeletarChave(string chave, string secao = null)
        {
            this.Escrever(chave, null, secao ?? this.nomeExecutavel);
        }

        public void DeletarSecao(string secao = null)
        {
            this.Escrever(null, null, secao ?? this.nomeExecutavel);
        }

        public bool ChaveExiste(string chave, string secao = null)
        {
            return this.Ler(chave, secao).Length > 0;
        }
    }
}
