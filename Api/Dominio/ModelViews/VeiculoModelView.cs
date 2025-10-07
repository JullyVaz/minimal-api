namespace MinimalApi.Dominio.ModelViews
{
    public class VeiculoModelView
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public int Ano { get; set; }
    }
}
