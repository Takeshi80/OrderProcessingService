namespace OrderProcessingService.Data.Models;

public enum OrderStatus
{
    Created = 1,
    FailedValidation = 2,
    Processed = 3,
    Completed = 4,
}