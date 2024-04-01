namespace Resize_Image.Models
{
    public class DataUser
    {
        public List<string> imgBase64 { get; set; } = new List<string>();
        public int option { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string background { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string titleColor { get; set; } = string.Empty;
        public string company { get; set; } = string.Empty;
        public string companyColor { get; set; } = string.Empty;
        public bool withBackground { get; set; }
    }
}
