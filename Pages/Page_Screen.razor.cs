using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Resize_Image.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO.Compression;
using Image = SixLabors.ImageSharp.Image;



namespace Resize_Image.Pages
{
    public partial class Page_Screen
    {
       
        [Inject]
        public IJSRuntime JS { get; set; } = null!;    
        protected override void OnInitialized()
        {
            ValueDefault.ResetImage();
        }
        public async Task HandleFileSelected(FileChangedEventArgs e)
        {
            const int maxImages = 5;

            if (ValueDefault._imagesBase64.Count < maxImages)
            { 
                foreach (var file in e.Files)
                {                    
                    using (var stream = file.OpenReadStream(ValueDefault._maxFileSize))

                    using (var fileMemoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(fileMemoryStream);

                        var bytes = fileMemoryStream.ToArray();

                        if (ValueDefault._imageCount < maxImages)
                        {
                            ValueDefault._imagesBase64.Add($"data:{file.UploadUrl};base64,{Convert.ToBase64String(bytes)}");
                            ValueDefault._imagesBytes.Add(bytes);
                            ValueDefault._imageCount++;
                        }
                    }
                }
                ValueDefault._btnResize = false;
            }
            else
            {
                await JS.InvokeVoidAsync("alert", "You can only upload 5 images at a time.");
            }     
        }
        public void DeleteImage(string img) => ValueDefault._imagesBase64.Remove(img);
        public async void ResizeImage(int width, int height)
        {            
            DirectoryManager.CreateDirectory(Path.Combine(ValueDefault._path, ValueDefault._folderImages));

            DirectoryInfo directoryInPut = new DirectoryInfo(Path.Combine(ValueDefault._path, ValueDefault._folderImages));

            foreach (var index in Enumerable.Range(0, ValueDefault._imagesBytes.Count))
            {
                var item = ValueDefault._imagesBytes[index];

                var fileName = $"{ValueDefault._fileNameOne}_{index}.png";

                var outputPath = Path.Combine(ValueDefault._path, ValueDefault._folderImages, fileName);

                using (Image image = Image.Load(item))
                {
                    image.Mutate(x => x.Resize(width == 0 ? 200 : width, height == 0 ? 200 : height));
                    image.Save(outputPath);                    
                }            
            }

            Console.WriteLine("The images were resized correctly.");
            ValueDefault._btnResize = true;
            ValueDefault._btnDownLoad = false;

            await JS.InvokeVoidAsync("alert", "The images were resized correctly.");         
        }
        public  void SaveImage()
        {
            ValueDefault._btnDownLoad = true;

            DirectoryManager.CreateDirectory(Path.Combine(ValueDefault._path, ValueDefault._folderZip));

            DirectoryInfo files = new DirectoryInfo(Path.Combine(ValueDefault._path, ValueDefault._folderImages));

            ConvetToZip(files.GetFiles(), ValueDefault._path, ValueDefault._folderZip);

            ValueDefault.ResetImage();
        }
        public async  void ConvetToZip(FileInfo[] files, string path, string folderZip)
        {
            string zipPath = Path.Combine(path, folderZip, ValueDefault._fileNameOne + ".zip");

            File.Delete(zipPath);

            using (FileStream zipFile = new FileStream(zipPath, FileMode.Create))
            {
                using (ZipArchive zip = new ZipArchive(zipFile, ZipArchiveMode.Create))
                {
                    foreach (var file in files)
                    {
                        zip.CreateEntryFromFile(file.FullName, file.Name);
                    }
                }
            }
                byte[] fileZip = File.ReadAllBytes(zipPath);

                await JS.InvokeVoidAsync("downloadFileZip", ValueDefault._fileNameOne + ".zip", fileZip);     
        }
    }
}
