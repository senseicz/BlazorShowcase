namespace BlazorShowcase.Data
{
    public partial class Alert
    {
        public DateTime CreatedOnDateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.CreatedOn).UtcDateTime;
    }
}
