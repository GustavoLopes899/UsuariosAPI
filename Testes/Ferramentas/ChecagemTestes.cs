using Ferramentas;
using NUnit.Framework;

namespace Testes.Ferramentas
{
    public class ChecagemTestes
    {
        [Test]
        [TestCase("", false)]
        [TestCase("91350148229", true)]
        [TestCase("123.456.789-00", false)]
        [TestCase("389.674.671-50", true)]
        [TestCase("529.982.247-25", true)]
        [TestCase("654.362.812-06", true)]
        [TestCase("789.654.32-10", false)]
        public void ChecagemCPFTeste(string cpf, bool esperado)
        {
            bool checagem = Checagem.CPF(cpf);
            Assert.AreEqual(checagem, esperado);
        }
    }
}