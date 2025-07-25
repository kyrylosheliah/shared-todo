namespace TodoClientWpf.Models;

public class TaskDto
{
    public int Id { get; set; } = 0;
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public bool IsCompleted { get; set; } = false;
}
