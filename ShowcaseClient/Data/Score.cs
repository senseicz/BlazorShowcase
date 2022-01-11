namespace BlazorShowcase.Data
{
    public partial class Score
    {
        public DateTime CreatedOnDateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.CreatedOn).UtcDateTime;
    }
}
