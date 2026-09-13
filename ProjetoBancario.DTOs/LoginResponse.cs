using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoBancario.DTOs;

public class LoginResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; }
    public string Token { get; set; }
    public string NumeroConta { get; set; }
}
