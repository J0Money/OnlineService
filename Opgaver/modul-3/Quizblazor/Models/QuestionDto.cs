namespace Quizblazor.Models;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public string[] Options { get; set; } = Array.Empty<string>();
}