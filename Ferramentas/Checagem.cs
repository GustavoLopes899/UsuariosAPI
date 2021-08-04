using System.Linq;

namespace Ferramentas
{
    public class Checagem
    {
        public static bool CPF(string cpf)
        {
            bool checagem;
            int soma = 0;
            int resto;
            int multiplicador = 10;
            int[] digitos = cpf.Where(d => char.IsDigit(d)).Select(d => int.Parse(d.ToString())).ToArray();
            if (digitos.Length != 11)
            {
                return false;
            }

            // Validação primeiro dígito
            for (int i = 0; i < 9; i++)
            {
                soma += digitos[i] * multiplicador--;
            }
            resto = (soma * 10) % 11;
            checagem = (resto == 10 ? 0 : resto) == digitos[9];

            // Validação segundo dígito
            if (checagem)
            {
                soma = 0;
                multiplicador = 11;
                for (int i = 0; i < 10; i++)
                {
                    soma += digitos[i] * multiplicador--;
                }
                resto = (soma * 10) % 11;
                checagem = (resto == 10 ? 0 : resto) == digitos[10];
            }
            return checagem;
        }
    }
}
