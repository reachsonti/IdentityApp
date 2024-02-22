using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace IdentityWebApp.FileUploadService
{
	public class LocalFileUploadService : IFileUploadService
	{
		private readonly IWebHostEnvironment _environment;

		public LocalFileUploadService(IWebHostEnvironment environment)
		{
			_environment = environment;
		}

		public async Task<string> UploadFileAsync(IFormFile file)
		{
			var filepath = Path.Combine(_environment.WebRootPath, "images", file.FileName);
			using var filestream = new FileStream(filepath, FileMode.Create);
			await file.CopyToAsync(filestream);
			return filepath;
		}

		public async Task<string> UploadDocumentAsync(IFormFile file)
		{
			var filepath = Path.Combine(_environment.WebRootPath, "documents", file.FileName);
			using var filestream = new FileStream(filepath, FileMode.Create);
			await file.CopyToAsync(filestream);
			return filepath;
		}
	}
}
