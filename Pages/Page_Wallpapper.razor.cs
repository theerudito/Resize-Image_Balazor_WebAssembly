using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Resize_Image.Helpers;
using Resize_Image.Models;
using System.Text;
using System.Text.Json;

namespace Resize_Image.Pages
{
    public partial class Page_Wallpapper
    {
        [Inject]
        public IJSRuntime JS { get; set; } = null!;

        [Inject]
        public HttpClient fetch { get; set; } = null!;

        protected override void OnInitialized()
        {
            ValueDefault.ResetValues();
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

                        ValueDefault._imgBase64.Add(Convert.ToBase64String(bytes));
                    }
                }
            }
            ValueDefault._btnResize = false;
        }
        
        public async void ResizeImage(int width, int height)
        {
            if (ValueDefault._fromAPI == "api")
            {
                var data = new DataUser
                {
                    imgBase64 = ValueDefault._imgBase64,
                    option = 4,
                    width = width == 0 ? 4096 : width,
                    height = height == 0 ? 2304 : height,
                    background = ValueDefault._colorBackgroud
                };

                var json = JsonSerializer.Serialize(data);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await fetch.PostAsync("ResizeImages", content);

                if (response.IsSuccessStatusCode)
                {

                    var fileStream = response.Content.ReadAsStream();

                    using (var ms = new MemoryStream())
                    {
                        fileStream.CopyTo(ms);
                        ValueDefault._file = ms.ToArray();
                    }
                }

                await JS.InvokeVoidAsync("alert", "The image were resized correctly.");
                ValueDefault._taskComplete = true;
                await InvokeAsync(StateHasChanged);
            }
        }

        public async void SaveImage()
        {
            ValueDefault._taskComplete = false;
            ValueDefault._btnResize = true;
            await InvokeAsync(StateHasChanged);
            await JS.InvokeVoidAsync("downloadFileFromByte", ValueDefault._fileNameOne + ".png", ValueDefault._file);
            ValueDefault.ResetValues();
        }
    }
}
