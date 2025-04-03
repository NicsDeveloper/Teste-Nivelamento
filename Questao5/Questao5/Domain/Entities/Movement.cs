namespace Questao5.Domain.Entities
{
    public class Movement
    {
        public string MovementId { get; set; } = string.Empty;
        public string CurrentAccountId { get; set; } = string.Empty;
        public string MovementData { get; set; } = string.Empty;
        public string MovementType { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
