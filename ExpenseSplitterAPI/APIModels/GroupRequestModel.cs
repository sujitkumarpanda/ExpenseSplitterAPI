namespace ExpenseSplitterAPI.APIModels
{
    public class GroupRequestModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<int> Members { get; set; }  
    }
}
