using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoBancario.DTOs;

public class TransferenciaRequest
{
    public decimal Valor { get; set; }
    public string TransfConta { get; set; }
}
