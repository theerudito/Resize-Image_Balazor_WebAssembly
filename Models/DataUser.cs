namespace Resize_Image.Models
{
    public class DataUser
    {
        public List<string> ImgBase64 { get; set; } = new List<string>();
        public int option { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}
