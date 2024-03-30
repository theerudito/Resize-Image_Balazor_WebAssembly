using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Resize_Image.Helpers;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using SixLabors.Fonts;


namespace Resize_Image.Pages
{
    public partial class Page_Resize
    {
        [Inject]
        public IJSRuntime JS { get; set; } = null!;
        protected override void OnInitialized()
        {
            ValueDefault.ResetImage();
        }
        private async Task HandleFileSelected(FileChangedEventArgs e)
        {
            var files = e.Files;

            foreach (var file in files)
            {
                using (var stream = file.OpenReadStream(ValueDefault._maxFileSize))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);

                        var bytes = memoryStream.ToArray();

                        ValueDefault._file = bytes;

                        ValueDefault._base64 = $"data:{file.UploadUrl};base64,{Convert.ToBase64String(bytes)}";

                        Console.WriteLine("The image was uploaded successfully.");
                    }
                }
                ValueDefault._btnResize = false;             
            }

            DirectoryManager.CreateDirectory(Path.Combine(ValueDefault._path, ValueDefault._folderImages));
            ValueDefault._btnResize = false;
        }
        public async void ResizeImage(int width, int height)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(ValueDefault._file))
            {
                image.Mutate(x => x.Resize(width == 0 ? 200 : width, height == 0 ? 200 : height));

                string outputPath = Path.Combine(ValueDefault._path, ValueDefault._folderImages, ValueDefault._fileNameOne + ".png");

                image.Save(outputPath);

                Console.WriteLine("The image were resized correctly.");

                Console.WriteLine($"the image was saved in the path: {outputPath}");

                ValueDefault._file = File.ReadAllBytes(outputPath);

                ValueDefault._btnDownLoad = false;
                ValueDefault._btnResize = true;
            }
            await JS.InvokeVoidAsync("alert", "The image were resized correctly.");
        }
        public async void SaveImage()
        {
            ValueDefault._btnDownLoad = true;
            string outputPath = Path.Combine(ValueDefault._path, ValueDefault._folderImages, ValueDefault._fileNameOne + ".png"); 
            await JS.InvokeVoidAsync("downloadFileFromByte", outputPath, ValueDefault._file);
            ValueDefault.ResetImage();
        }   
    }
}
