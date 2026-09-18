using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoBancario.DTOs;

public class FazerLoginRequest
{
    public string NumeroConta { get; set; }
    public string Senha { get; set; }
}
