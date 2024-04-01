using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Resize_Image.Helpers;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace Resize_Image.Pages
{
    public partial class Page_LandScape
    {      
        private string _title = "";
        private string _company = "";
        private string _colorBackgroud = "yellow";
        private string _colorTitle = "blue";
        private string _colorCompany = "red";

        [Inject]
        public IJSRuntime JS { get; set; } = null!;

        protected override void OnInitialized()
        {
            ValueDefault.ResetValues();
        }

        public async Task HandleFileSelected(FileChangedEventArgs e)
        {
            const int maxImages = 4;

            if (ValueDefault._srcImg.Count < maxImages)
            {
                foreach (var file in e.Files)
                {
                    using (var stream = file.OpenReadStream(ValueDefault._maxFileSize))

                    using (var fileMemoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(fileMemoryStream);

                        var bytes = fileMemoryStream.ToArray();

                        ValueDefault._imagesBytes.Add(bytes);

                        ValueDefault._srcImg.Add($"data:{file.UploadUrl};base64,{Convert.ToBase64String(bytes)}");

                        ValueDefault._imageCount++;
                    }
                }

                if (ValueDefault._imageCount == maxImages)
                {
                    ValueDefault._btnResize = false;
                } else
                {
                    ValueDefault._btnResize = true;
                }
            }
            else
            {
                await JS.InvokeVoidAsync("alert", "You can only upload 3 images at a time.");
            }
        }


        public async void ResizeImage(int width, int height)
        {
            ValueDefault._btnResize = true;
            List<Image<Rgba32>> images = new List<Image<Rgba32>>();

            Console.WriteLine(images.Count + " inicia");

            using (var firstImage = SixLabors.ImageSharp.Image.Load<Rgba32>(ValueDefault._imagesBytes[0]))
            {
                firstImage.Mutate(x => x.Resize(1024, 500));
                //var font = SystemFonts.CreateFont("Segoe UI", 39, FontStyle.Regular);
                //firstImage.Mutate(x => x.DrawText("Hello World", font, SixLabors.ImageSharp.Color.Black, new PointF(10, 10)));
                images.Add(firstImage.Clone());


                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(ValueDefault._imagesBytes[1]))
                {
                    image.Mutate(x => x.Resize(180, 350));
                    AddBorder(image, 1, SixLabors.ImageSharp.Color.Black);
                    images.Add(image.Clone());
                }

                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(ValueDefault._imagesBytes[2]))
                {
                    image.Mutate(x => x.Resize(180, 350));
                    AddBorder(image, 1, SixLabors.ImageSharp.Color.Black);
                    images.Add(image.Clone());
                }

                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(ValueDefault._imagesBytes[3]))
                {
                    image.Mutate(x => x.Resize(200, 400));
                    AddBorder(image, 1, SixLabors.ImageSharp.Color.Black);
                    images.Add(image.Clone());
                }
            }

            Console.WriteLine(images.Count + " termina");

            using (var outputImage = new Image<Rgba32>(1024, 500))
            {
                var mainImage = images[0];
                outputImage.Mutate(x => x.DrawImage(mainImage, new Point(0, 0), 1));

                var secondImage = images[1];
                outputImage.Mutate(x => x.DrawImage(secondImage, new Point(20, 70), 1));

                var thirdImage = images[2];
                outputImage.Mutate(x => x.DrawImage(thirdImage, new Point(800, 50), 1));

                var fourImage = images[2];
                outputImage.Mutate(x => x.DrawImage(fourImage, new Point(800, 50), 1));

                string outputPath = Path.Combine(ValueDefault._path, ValueDefault._folderImages, "avatar.png");

                DirectoryManager.CreateDirectory(Path.Combine(ValueDefault._path, ValueDefault._folderImages));

                outputImage.Save(outputPath);

                ValueDefault._file = File.ReadAllBytes(outputPath);

                Console.WriteLine("The image were resized correctly.");

              
                ValueDefault._btnDownLoad = false;
            }

            await JS.InvokeVoidAsync("alert", "The image were resized correctly.");
        }


        public async void SaveImage()
        {
            ValueDefault._btnDownLoad = true;

            string outputPath = Path.Combine(ValueDefault._path, ValueDefault._folderImages, "avatar.png");

            await JS.InvokeVoidAsync("downloadFileFromByte", outputPath, ValueDefault._file);
            ValueDefault.ResetValues();
        }

        private void AddBorder(Image<Rgba32> image, int borderSize, SixLabors.ImageSharp.Color borderColor)
        {
            var borderedImage = new Image<Rgba32>(image.Width + 2 * borderSize, image.Height + 2 * borderSize);

            borderedImage.Mutate(x => x.BackgroundColor(borderColor));

            borderedImage.Mutate(x => x.DrawImage(image, new Point(borderSize, borderSize), 1f));

            image.Mutate(ctx => ctx.DrawImage(borderedImage, new Point(0, 0), 1f));
        }
    }
}
