namespace Cadastro_de_receitas_Máquina_de_Oleo.Models
{
    public class Partnumber(string partnumber, string description, string oil, string volume, int dosingTime)
    {
        public string Code { get; set; } = partnumber;
        public string Description { get; set; } = description;
        public string TypeOil { get; set; } = oil;
        public string Volume { get; set; } = volume;
        public int DosingTime { get; set; } = dosingTime;
    }
}
