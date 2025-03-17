namespace ExpenseSplitterAPI.APIModels
{
    public class GroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<UserModel> Members { get; set; } = new List<UserModel>();
    }
}
