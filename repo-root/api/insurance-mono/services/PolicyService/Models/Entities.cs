namespace PolicyService.Models
{
    public record Policy(
    int Id,
    string PolicyNumber,
    string Title,
    string Status,
    decimal PremiumAmount,
    DateTime StartDate,
    DateTime? EndDate,
    int? PolicyConfigId,
    int OwnerUserId,
    DateTime CreatedAt   // add and match DB nullability/type
);

    public record PolicyConfig(int Id, string Name, string RulesJson, DateTime CreatedAt);
    public record Nominee(int Id, int PolicyId, string FullName, string Relation, int Percentage);

    public record CreatePolicyDto(string PolicyNumber, string Title, string Status,
    decimal PremiumAmount, DateTime StartDate, DateTime? EndDate, int? PolicyConfigId, DateTime CreatedAt);
    public record UpdatePolicyDto(int Id, string Title, string Status,
    decimal PremiumAmount, DateTime StartDate, DateTime? EndDate, int? PolicyConfigId, DateTime CreatedAt);
}
