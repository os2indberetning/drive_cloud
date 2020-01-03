namespace Core.DomainModel
{
    public class APIOrgUnit
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public APIAddress Address { get; set; }
        public string CostCenter { get; set; }
    }
}
