using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityWebApp.Areas.Identity.Data;
using IdentityWebApp.FileUploadService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityWebApp.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IFileUploadService _fileUploadService;
        public string filepath, docupload = string.Empty;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IFileUploadService fileUploadService,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileUploadService = fileUploadService;
            _environment = environment;
        }

        public string Username { get; set; }
        public string pimage { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            //Srinath, added the below field for update under Profile
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            //Srinath, added the below field for update under Profile
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            //Srinath, added the below field for update under Profile
            [DataType(DataType.Text)]
            [Display(Name = "Profile Image")]
            public string ProfileImage { get; set; }

            //Srinath, added the below field for update under Profile
            [DataType(DataType.Text)]
            [Display(Name = "Upload Document")]
            public string UploadDocument { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            //srinath added the below code to get the information
            var firstname = user.FirstName;
            var lastname = user.LastName;
            var profileimage = user.ProfileImage;
            var uploaddoc = user.UploadDocument;

            Username = userName;
            //filepath = _environment.WebRootPath + "/images/" +  profileimage;            
            filepath = profileimage;
            docupload = uploaddoc;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                //Srinath, added the below code to add the data into model
                FirstName = firstname,
                LastName = lastname,
                ProfileImage = profileimage,
                UploadDocument = uploaddoc
            };            
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                await LoadAsync(user);
                return Page();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public async Task<IActionResult> OnPostAsync(IFormFile UploadPhoto = null, IFormFile UploadDocument = null)
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                if (!ModelState.IsValid)
                {
                    await LoadAsync(user);
                    return Page();
                }

                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set phone number.";
                        return RedirectToPage();
                    }
                }

                //Srinath, added the code to update the new values to DB.
                if (user.FirstName != Input.FirstName)
                    user.FirstName = Input.FirstName;

                if (user.LastName != Input.LastName)
                    user.LastName = Input.LastName;

                if (UploadPhoto != null && UploadPhoto.Length > 0)
                {
                    filepath = await _fileUploadService.UploadFileAsync(UploadPhoto);

                    if (filepath.Length > 0)
                        user.ProfileImage = UploadPhoto.FileName;
                }

                if (UploadDocument != null && UploadDocument.Length > 0)
                {
                    filepath = await _fileUploadService.UploadDocumentAsync(UploadDocument);

                    if (filepath.Length > 0)
                        user.UploadDocument = UploadDocument.FileName;
                }

                await _userManager.UpdateAsync(user);

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        //Srinath, this function deletes the image uploaded to wwwroot/images folder.
        public async Task<ActionResult> OnPostWay2(string data)
        {
            try
            {
                string imagedel;
                imagedel = Path.Combine(_environment.WebRootPath, "images", data);
                FileInfo fi = new FileInfo(imagedel);
                if (fi != null)
                {
                    System.IO.File.Delete(imagedel);
                    fi.Delete();

                    var user = await _userManager.GetUserAsync(User);
                    user.ProfileImage = "";
                    await _userManager.UpdateAsync(user);

                    filepath = "";

                    await _signInManager.RefreshSignInAsync(user);
                    StatusMessage = "Your profile image has been deleted";

                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        //Srinath, this function downloads the document uploaded to wwwroot/document folder.
        public async Task<ActionResult> OnPostWay3(string data)
        {
            try
            {
                var path = Path.Combine(_environment.WebRootPath, "documents", data);
                var memory = new MemoryStream();

                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                var contentType = "Application/octet-stream";
                var filename = Path.GetFileName(path);

                return File(memory, contentType, filename);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
    }
}
