using ExampleEmpty.UI.Models;
using ExampleEmpty.UI.Models.Repository.IRepository;
using ExampleEmpty.UI.Security;
using ExampleEmpty.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace ExampleEmpty.UI.Controllers
{

    [Authorize(Policy = "SuperAdminPolicy")]
    public class AdminController : Controller
    {
        private readonly ICustomerRepository _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AdminController> _logger;
        private IDataProtector _dataProtector;
        public AdminController(ICustomerRepository unitOfWork,
            IWebHostEnvironment webHostEnvironment, ILogger<AdminController> logger,
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings
           )
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);

        }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Index()
        {
            ViewData["ListOfCustomers"] = "Customer List";
            var list = _unitOfWork.Get()
                                .Select(e =>
                                {
                                    e.EncryptedCustomerId = _dataProtector.Protect(e.CustomerId.ToString());
                                    return e;
                                }).ToList();
            return View(list);


        }
        [HttpGet]
        public ViewResult Details(string id)
        {
            int employeeId = Convert.ToInt32(_dataProtector.Unprotect(id));


            Customer myCus = _unitOfWork.Get(employeeId);
            if (myCus == null)
            {
                Response.StatusCode = 404;
                _logger.LogError($"The customer with the specified Id = {employeeId} cannot be found!");
                return View("CustomerNotFound", employeeId);
            }

            ViewData["CustomerDetails"] = "Customer Details";

            return View(myCus);



        }

        [HttpGet]
        public IActionResult Upsert(string id)
        {
            int employeeId = Convert.ToInt32(_dataProtector.Unprotect(id));

            Customer findCustomerObj = _unitOfWork.Get(employeeId);
            if (findCustomerObj == null)
            {
                Response.StatusCode = 404;
                _logger.LogError($"The customer with the specified Id = {employeeId} cannot be found!");
                return View("CustomerNotFound", employeeId);
            }

            if (findCustomerObj != null)
            {
                CustomerEditViewModel customerEditViewModel = new()
                {
                    CustomerId = findCustomerObj.CustomerId,
                    EncryptedCustomerId = id,
                    Name = findCustomerObj.Name,
                    Address = findCustomerObj.Address,
                    Gender = findCustomerObj.Gender
                };

                if (findCustomerObj.PhotoPath != null)
                {
                    if (customerEditViewModel.ExistingPhotoPath == null)
                    {
                        customerEditViewModel.ExistingPhotoPath = findCustomerObj.PhotoPath;
                    }
                }

                return View(customerEditViewModel);
            }

            return NotFound(findCustomerObj);
        }

        [HttpPost]
        public IActionResult Upsert(CustomerEditViewModel model)
        {
            int employeeId = Convert.ToInt32(_dataProtector.Unprotect(model.EncryptedCustomerId));
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    if (employeeId == 0)
                    {
                        //Create Request
                        //Add new user if the user does not select an image initially!
                        if (model.Photo == null)
                        {
                            Customer addCustomer = new()
                            {
                                Name = model.Name,
                                Address = model.Address,
                                Gender = model.Gender
                            };

                            _unitOfWork.Create(addCustomer);
                            return RedirectToAction("Index");
                        }

                        if (model.Photo != null)
                        {
                            //User has selected an image
                            Customer customerModel = GetCustomerModel(model);
                            _unitOfWork.Create(customerModel);
                            return RedirectToAction("Details", new { id = employeeId });
                        }
                        else
                        {
                            //User has selected an existing image
                            Customer customer = _unitOfWork.Get(employeeId);

                            if (model.Photo != null)
                            {
                                if (model.ExistingPhotoPath != null)
                                {
                                    DeleteUserImageOnTheServer(model);
                                }
                                customer.PhotoPath = ProcessImageOnServer(model);
                            }
                            _unitOfWork.Update(customer);
                            return RedirectToAction("index");
                        }



                    }
                    else
                    {
                        //Update Request
                        Customer customer = _unitOfWork.Get(employeeId);

                        if (model.Photo != null)
                        {
                            if (model.ExistingPhotoPath != null)
                            {
                                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "iamtuse_uploads", model.ExistingPhotoPath);
                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath);
                                }
                            }
                            customer.PhotoPath = ProcessImageOnServer(model);
                        }
                        else
                        {
                            Customer updateCustomer = new()
                            {
                                CustomerId = employeeId,
                                Name = model.Name,
                                Address = model.Address,
                                Gender = model.Gender
                            };
                            if (updateCustomer.PhotoPath == null)
                            {
                                _unitOfWork.Update(updateCustomer);
                                return RedirectToAction("Index");
                            }
                        }

                        Customer updatedCustomer = new()
                        {
                            Name = model.Name,
                            Gender = model.Gender,
                            Address = model.Address,
                            CustomerId = employeeId
                        };


                        if (updatedCustomer.PhotoPath == null || updatedCustomer.PhotoPath == string.Empty)
                        {
                            updatedCustomer.PhotoPath = customer.PhotoPath;
                        }

                        _unitOfWork.Update(updatedCustomer);
                        return RedirectToAction("Index");

                    }
                }
            }

            return View(model);


        }

        private void DeleteUserImageOnTheServer(CustomerEditViewModel model)
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "iamtuse_uploads", model.ExistingPhotoPath);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public IActionResult Delete(string id)
        {
            int employeeId = Convert.ToInt32(_dataProtector.Unprotect(id));
            Customer findCustomerObj = _unitOfWork.Get(employeeId);
            if (findCustomerObj.PhotoPath != null)
            {
                CustomerEditViewModel model = new()
                {
                    ExistingPhotoPath = findCustomerObj.PhotoPath
                };

                DeleteUserImageOnTheServer(model);
            }
            if (findCustomerObj == null)
            {
                _logger.LogError($"The customer with the specified Id = {employeeId} cannot be found!");
                return View("CustomerNotFound", employeeId);
            }

            _unitOfWork.Remove(employeeId);
            return RedirectToAction("Index");
        }
        private string ProcessImageOnServer(CustomerCreateViewModel model)
        {
            //wwwroot folder
            var getRootWebRootPath = _webHostEnvironment.WebRootPath;
            string uniqueFileName = null;
            var fullImagePathOnServer = Path.Combine(getRootWebRootPath, "images", "iamtuse_uploads");
            if (model.Photo != null)
            {
                string uploadedFolder = fullImagePathOnServer;
                uniqueFileName = Guid.NewGuid().ToString() + " _ " + model.Photo.FileName;
                string filePath = Path.Combine(uploadedFolder, uniqueFileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                model.Photo.CopyTo(fileStream);
            }

            return uniqueFileName;

        }
        private Customer GetCustomerModel(CustomerCreateViewModel model)
        {
            int employeeId = Convert.ToInt32(_dataProtector.Unprotect(model.EncryptedCustomerId));
            Customer customerModel = new()
            {
                CustomerId = employeeId,
                Address = model.Address,
                Name = model.Name,
                Gender = model.Gender,
                PhotoPath = ProcessImageOnServer(model)
            };
            return customerModel;
        }
    }
}
