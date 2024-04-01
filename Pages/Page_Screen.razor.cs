using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Resize_Image.Helpers;
using Resize_Image.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO.Compression;
using System.Text;



namespace Resize_Image.Pages
{
    public partial class Page_Screen
    {
       
        [Inject]
        public IJSRuntime JS { get; set; } = null!;

        [Inject]
        public HttpClient fetch { get; set; } = null!;

        protected override void OnInitialized()
        {
            ValueDefault.ResetValues();       
        }
        
        public async Task HandleFileSelected(FileChangedEventArgs e)
        {
            const int maxImages = 5;

            if (ValueDefault._srcImg.Count < maxImages)
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
                            ValueDefault._srcImg.Add($"data:{file.UploadUrl};base64,{Convert.ToBase64String(bytes)}");
                            ValueDefault._imagesBytes.Add(bytes);
                            ValueDefault._imgBase64.Add(Convert.ToBase64String(bytes));
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
        
        public void DeleteImage(string img) => ValueDefault._srcImg.Remove(img);
        
        public async void ResizeImage(int width, int height)
        {
            if (ValueDefault._fromAPI == "api")
            {
                var data = new DataUser
                {
                    imgBase64 = ValueDefault._imgBase64,
                    option = 2,
                    width = width == 0 ? 500 : width,
                    height = height == 0 ? 500 : height
                };

                var json = JsonConvert.SerializeObject(data);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await fetch.PostAsync("Resize_Images", content);


                if (response.IsSuccessStatusCode)
                {

                    var fileStream = response.Content.ReadAsStream();

                    using (var ms = new MemoryStream())
                    {
                        fileStream.CopyTo(ms);
                        ValueDefault._file = ms.ToArray();
                        ValueDefault._btnResize = true;
                    }

                    await JS.InvokeVoidAsync("alert", "The images were resized correctly.");
                    ValueDefault._taskComplete = true;
                    await InvokeAsync(StateHasChanged);
                }
            }
            else
            {
                DirectoryManager.CreateDirectory(Path.Combine(ValueDefault._path, ValueDefault._folderImages));

                DirectoryInfo directoryInPut = new DirectoryInfo(Path.Combine(ValueDefault._path, ValueDefault._folderImages));

                foreach (var index in Enumerable.Range(0, ValueDefault._imagesBytes.Count))
                {
                    var item = ValueDefault._imagesBytes[index];

                    var fileName = $"{ValueDefault._fileNameOne}_{index}.png";

                    var outputPath = Path.Combine(ValueDefault._path, ValueDefault._folderImages, fileName);

                    using (var image = SixLabors.ImageSharp.Image.Load(item))
                    {
                        image.Mutate(x => x.Resize(width == 0 ? 200 : width, height == 0 ? 200 : height));
                        image.Save(outputPath);
                    }
                }

                await JS.InvokeVoidAsync("alert", "The images were resized correctly.");
                ValueDefault._taskComplete = true;
                await InvokeAsync(StateHasChanged);
            }         
        }
        
        public async  void SaveImage()
        {
            if (ValueDefault._fromAPI == "api")
            {
                ValueDefault._taskComplete = false;
                ValueDefault._btnResize = true;
                await InvokeAsync(StateHasChanged);

                await JS.InvokeVoidAsync("downloadFileZip", ValueDefault._fileNameOne + ".zip", ValueDefault._file);

                ValueDefault.ResetValues();
            }
            else
            {
                ValueDefault._taskComplete = false;
                ValueDefault._btnResize = true;
                await InvokeAsync(StateHasChanged);

                DirectoryManager.CreateDirectory(Path.Combine(ValueDefault._path, ValueDefault._folderZip));

                DirectoryInfo files = new DirectoryInfo(Path.Combine(ValueDefault._path, ValueDefault._folderImages));

                ConvetToZip(files.GetFiles(), ValueDefault._path, ValueDefault._folderZip);

                ValueDefault.ResetValues();
            }
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
