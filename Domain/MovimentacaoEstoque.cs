namespace NAC2.Domain
{
    public class MovimentacaoEstoque
    {
        public string Tipo { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string Lote { get; set; }
        public DateTime DataValidade { get; set; }
        public int CodSKU { get; set; }
    }
}
