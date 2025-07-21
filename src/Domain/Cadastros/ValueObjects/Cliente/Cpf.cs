using System.Net;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Cliente
{
    public class Cpf
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private Cpf() { }

        public Cpf(string cpf)
        {
            if (!ValidarCpf(cpf))
                throw new DomainException("CPF inválido", HttpStatusCode.BadRequest);

            _valor = CleanCpf(cpf);
        }

        public string Valor => _valor;

        public static bool ValidarCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = CleanCpf(cpf);

            if (cpf.Length != 11)
                return false;

            if (cpf.All(c => c == cpf[0]))
                return false;

            int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf[..9];
            int sum = 0;

            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];

            int remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;

            string digit = remainder.ToString();
            tempCpf += digit;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];

            remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;

            digit += remainder.ToString();

            return cpf.EndsWith(digit);
        }

        private static string CleanCpf(string cpf)
        {
            return string.Join("", cpf.Where(char.IsDigit));
        }

        public override string ToString()
        {
            return _valor;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Cpf other)
                return false;

            return _valor == other._valor;
        }

        public override int GetHashCode()
        {
            return _valor.GetHashCode();
        }
    }
}
