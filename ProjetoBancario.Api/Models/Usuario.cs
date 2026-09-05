namespace ProjetoBancario.Api;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string? SenhaHash { get; set; }
}
