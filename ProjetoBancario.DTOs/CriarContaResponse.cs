using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoBancario.DTOs;

public class CriarContaResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; }
    public string numeroConta { get; set; }
}
