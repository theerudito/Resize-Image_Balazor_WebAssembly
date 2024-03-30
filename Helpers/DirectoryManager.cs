namespace Resize_Image.Helpers
{
    public class DirectoryManager
    {
        public static void CreateDirectory(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
}
