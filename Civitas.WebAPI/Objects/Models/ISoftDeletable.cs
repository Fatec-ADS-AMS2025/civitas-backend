namespace Civitas.WebAPI.Objects.Models
{
    public interface ISoftDeletable
    {
        bool Excluido { get; set; }
        DateTime? DataExclusao { get; set; }
    }
}
