using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoBancario.DTOs;

public class ExtratoResponse
{
    public decimal Saldo { get; set; }
    public List<Transacao> Transacoes { get; set; } = [];
}
