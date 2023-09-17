namespace KaizenCase.Models
{
    public class SlipSummary
    {
        public int Line { get; set; }
        public string Text { get; set; }
    }

    public class WorkflowSlipSummary
    {
        public int Id { get; set; }
        public int Line { get; set; }
        public string Text { get; set; }
        public BoundingPoly BoundingPoly { get; set; }
    }

}
