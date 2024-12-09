namespace API;

public class IMC
{
    public int id { get; set; }
    public double Im { get; set; }
    public string? Classificacao { get; set; }
    public double Altura { get; set; }
    public double Peso { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? usuario { get; set; }
}