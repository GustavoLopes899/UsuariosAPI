using Ferramentas;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace Testes.Ferramentas
{
    public class Tests
    {
        private string caminhoArquivo;

        [SetUp]
        public void SetUp()
        {
            this.caminhoArquivo = Logger.CaminhoArquivoLog(Assembly.GetExecutingAssembly().GetName().Name);
            if (File.Exists(this.caminhoArquivo))
            {
                File.Delete(this.caminhoArquivo);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(this.caminhoArquivo))
            {
                File.Delete(this.caminhoArquivo);
            }
        }

        [Test]
        [TestCase(true, "Erro ao abrir a API\nTente novamente", "Erro ao abrir a API Tente novamente")]
        [TestCase(false, "Erro ao abrir a API\nTente novamente", "Erro ao abrir a API Tente novamente")]
        public void TrataMensagemExcecaoTeste(bool possuiInnerException, string mensagem, string esperado)
        {
            Exception inner = null;
            if (possuiInnerException)
            {
                inner = new Exception(mensagem);
            }
            Exception ex = new Exception(mensagem, inner);
            string mensagemTratada = Logger.TrataMensagemExcecao(ex);
            Assert.AreEqual(esperado, mensagemTratada);
        }

        [TestCase("Arquivo de testes")]
        public void GravarLogTeste(string mensagem)
        {
            Logger.GravarLog(mensagem);
            Assert.IsTrue(File.Exists(this.caminhoArquivo));
        }
    }
}