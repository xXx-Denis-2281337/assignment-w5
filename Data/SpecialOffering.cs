namespace KmaOoad18.Assignments.Week5.Data
{
    public class SpecialOffering
    {
        public int Id { get; set; }
        public Product Product { get; internal set; }
        public Promotion PromotionType { get; internal set; }
        public int Extra { get; internal set; }
    }
}
