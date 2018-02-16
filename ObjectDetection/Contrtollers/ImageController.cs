using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System;

namespace ObjectDetection.Contrtollers
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly string workingFolder = Directory.GetCurrentDirectory() + @"/Content/UploadFiles/";

        // public async Task<IActionResult> Get()
        public async Task<IActionResult> Get()
        {
            var photos = new List<Models.ImageViewModel>();

            var photoFolder = new DirectoryInfo(workingFolder);

            await Task.Factory.StartNew(() =>
            {
                photos = photoFolder.EnumerateFiles()
                    .Where(fi => new[] { ".jpeg", ".jpg", ".bmp", ".png", ".gif", ".tiff" }
                        .Contains(fi.Extension.ToLower()))
                    .Select(fi => new Models.ImageViewModel
                    {
                        Name = fi.Name,
                        Created = fi.CreationTime,
                        Modified = fi.LastWriteTime,
                        Size = fi.Length / 1024
                    })
                    .ToList();
            });

            return Json(new { Photos = photos });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string fileName)
        {
            if (!FileExists(fileName))
            {
                return NotFound();
            }

            try
            {
                var filePath = Directory.GetFiles(workingFolder, fileName)
                    .FirstOrDefault();

                await Task.Factory.StartNew(() =>
                {
                    if (filePath != null)
                        System.IO.File.Delete(filePath);
                });

                var result = new Models.ImageActionResult
                {
                    Successful = true,
                    Message = fileName + "deleted successfully"
                };
                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                var result = new Models.ImageActionResult
                {
                    Successful = false,
                    Message = "error deleting fileName " + ex.GetBaseException().Message
                };
                return BadRequest(result.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(string fileName)
        {
            Processing processing = new Processing(workingFolder + fileName);
            var res = processing.Process();

            //res.Key.Save(workingFolder + fileName + "-processed.jpg", ImageFormat.Jpeg);

            return Json(res.Value);
        }

        public async Task<IActionResult> Add()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }
            try
            {
                var provider = new CustomMultipartFormDataStreamProvider(workingFolder);
                //await Request.Content.ReadAsMultipartAsync(provider);
                await Task.Run(async () => await Request.Content.ReadAsMultipartAsync(provider));

                var photos = new List<Models.ImageViewModel>();

                foreach (var file in provider.FileData)
                {
                    var fileInfo = new FileInfo(file.LocalFileName);

                    photos.Add(new Models.ImageViewModel
                    {
                        Name = fileInfo.Name,
                        Created = fileInfo.CreationTime,
                        Modified = fileInfo.LastWriteTime,
                        Size = fileInfo.Length / 1024
                    });
                }
                return Ok(new { Message = "Photos uploaded ok", Photos = photos });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        public bool FileExists(string fileName)
        {
            var file = Directory.GetFiles(workingFolder, fileName)
                .FirstOrDefault();

            return file != null;
        }
    }
}