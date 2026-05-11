namespace OnlineService.Models;

public class TrainingProgram
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public List<string> Exercises { get; set; } = new();
}