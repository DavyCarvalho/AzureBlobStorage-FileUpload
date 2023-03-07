using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.Azure.Storage.Blob;

namespace AzureBlobStorage.Controllers
{
    public class FileController : Controller
    {
        private readonly CloudBlobClient _blobClient;

        public FileController(CloudBlobClient blobClient)
        {
            _blobClient = blobClient;
        }

        [HttpPost]
        public async Task<ActionResult> UploadImage(IFormFile file)
        {
            var container = _blobClient.GetContainerReference("images");
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
            await blob.UploadFromStreamAsync(file.OpenReadStream());
            return Ok(blob.Uri);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetImage(string id)
        {
            var container = _blobClient.GetContainerReference("images");
            var blob = container.GetBlockBlobReference(id);
            if (!await blob.ExistsAsync())
            {
                return NotFound();
            }
            return File(await blob.OpenReadAsync(), blob.Properties.ContentType);
        }
    }
}
