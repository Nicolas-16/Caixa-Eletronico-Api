using System.ComponentModel.DataAnnotations;
namespace ProjetoBancario.Api
{

    public class CriarContaRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")];
    }
}
