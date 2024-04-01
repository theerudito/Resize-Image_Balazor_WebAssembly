namespace Resize_Image.Helpers
{
    public class ValueDefault
    {
        public static string _title = "Name of Application";

        public static string _company = "";

        public static string _colorBackgroud = "#000000";

        public static string _colorTitle = "#000000";

        public static string _colorCompany = "#000000";

        public static bool _withBackground = false;

        public static string _imageDefault = "./assets/logo.jpg";

        public static string _path = Directory.GetCurrentDirectory();

        public static string _folderImages = "img";

        public static string _folderZip = "zip";

        public static string _fromAPI = "api";

        public static string _fileNameOne = DateTime.Now.ToString("yyyyMMddHHmmss");

        public static int _width = 0;

        public static int _height = 0;

        public static int _maxFileSize = 10 * 1024 * 1024;

        public static int _imageCount = 0;

        public static bool _btnResize = false;

        public static bool _taskComplete = false;

        public static string _base64 = null!;

        public static byte[] _file = null!;

        public static List<string> _srcImg = new List<string>();
        
        public static List<string> _imgBase64 = new List<string>();

        public static List<byte[]> _imagesBytes = new List<byte[]>();

        public static void ResetValues()
        {
            _width = 0;
            _height = 0;
            _base64 = null!;
            _file = null!;
            _btnResize = true;
            _taskComplete = false;
            _srcImg = new List<string>();
            _imagesBytes = new List<byte[]>();
            _imgBase64  = new List<string>();
            _imageCount = 0;
            _colorBackgroud = "#000000";
            _colorTitle = "#000000";
            _colorCompany = "#000000";
            _withBackground = false;
            _title = "";
            _company = "";
        }
    }
}
