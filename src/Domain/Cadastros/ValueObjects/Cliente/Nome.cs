namespace Domain.Cadastros.ValueObjects.Cliente
{
    public class Nome
    {
        private readonly string _valor;

        // Parameterless constructor for EF Core
        private Nome() { }

        public Nome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome não pode ser vazio");

            if (nome.Length > 200)
                throw new ArgumentException("Nome não pode ter mais de 200 caracteres");

            _valor = nome;
        }

        public string Valor => _valor;


    }
}
