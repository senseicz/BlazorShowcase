using System.Web;


namespace BlazorShowcase.Data;

public partial class Alert
{
    public string Url => $"https://letmebingthatforyou.com/?q={HttpUtility.UrlEncode(Id)}";
}
