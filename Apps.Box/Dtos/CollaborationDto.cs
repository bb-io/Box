using Box.V2.Models;

namespace Apps.Box.Dtos;

public class CollaborationDto
{
    public CollaborationDto(BoxCollaboration collaboration)
    {
        Collaboration = collaboration.Id;
        Role = collaboration.Role;
        Status = collaboration.Status;
    }
    
    public string Collaboration { get; set; }
    public string Role { get; set; }
    public string Status { get; set; }
}