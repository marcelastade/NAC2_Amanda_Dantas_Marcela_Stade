namespace NAC2.Domain
{
    public class Produto
    {
        public int CodSKU { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public double PrecoUnitario { get; set; }
        public int QuantMinima { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
