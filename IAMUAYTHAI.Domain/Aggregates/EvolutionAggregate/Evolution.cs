namespace IAMUAYTHAI.Domain.Aggregates.EvolutionAggregate
{
    public class Evolution
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string CurrentLevel { get; set; } = string.Empty;
        public string NextLevel { get; set; } = string.Empty;
        public DateTime NextKruangExpectedDate { get; set; }
        public bool EligibleForNextLevel { get; set; }
    }
}