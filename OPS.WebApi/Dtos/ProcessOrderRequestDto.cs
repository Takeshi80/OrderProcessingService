namespace OPS.WebApi.Dtos;

public record ProcessOrderRequestDto
{
    public int CustomerId { get; set; }
    public List<int> ItemIds { get; set; } = new();
    public int TotalAmount { get; set; }
}