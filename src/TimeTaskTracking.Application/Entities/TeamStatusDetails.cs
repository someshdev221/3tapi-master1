using System.ComponentModel.DataAnnotations;

namespace TimeTaskTracking.Models.Entities;

public class TeamStatusDetails<TKey>
{
    public string TeamLeadId { get; set; }
    public TKey EmployeeId { get; set; }
    public DateTime StatusDate { get; set; }
}
