using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityWebApp.FileUploadService
{
	public interface IFileUploadService
	{
		Task<string> UploadFileAsync(IFormFile file);
		Task<string> UploadDocumentAsync(IFormFile file);
	}
}
