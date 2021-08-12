using ExampleEmpty.UI.Models;
using ExampleEmpty.UI.Models.Repository.IRepository;
using ExampleEmpty.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace ExampleEmpty.UI.Controllers
{

    [Authorize(Roles = "Administrator")]
    [Authorize(Roles = "SuperAdministrator")]
    public class AdminController : Controller
    {
        private readonly ICustomerRepository _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AdminController> _logger;
        public AdminController(ICustomerRepository unitOfWork,
            IWebHostEnvironment webHostEnvironment, ILogger<AdminController> logger)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;

        }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Index()
        {
            ViewData["ListOfCustomers"] = "Customer List";
            var list = _unitOfWork.Get()
                                  .OrderBy(c => c.Name)
                                  .ToList();
            return View(list);

        }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Details(int? id)
        {
            if (id.HasValue)
            {
                Customer myCus = _unitOfWork.Get(id.Value);
                if (myCus == null)
                {
                    Response.StatusCode = 404;
                    _logger.LogError($"The customer with the specified Id = {id.Value} cannot be found!");
                    return View("CustomerNotFound", id.Value);
                }
            }
            if (id.HasValue)
            {
                Customer findCustomerObj = _unitOfWork.Get(id.Value);
                if (findCustomerObj == null)
                {
                    Response.StatusCode = 404;
                    _logger.LogError($"The customer with the specified Id = {id.Value} cannot be found!");
                    return View("CustomerNotFound", id.Value);
                }
            }

            if (id == null)
            {
                ViewData["CustomerDetails"] = "Customer Details";
                CustomerCreateViewModel customer = new(101, "Default", "Default", Gender.Male);
                return View(customer);
            }
            var cus = _unitOfWork.Get(id.Value);
            if (cus != null)
            {
                ViewData["CustomerDetails"] = "Customer Details";

                return View(cus);
            }

            return View(cus);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            if (id.HasValue)
            {
                Customer findCustomerObj = _unitOfWork.Get(id.Value);
                if (findCustomerObj == null)
                {
                    Response.StatusCode = 404;
                    _logger.LogError($"The customer with the specified Id = {id.Value} cannot be found!");
                    return View("CustomerNotFound", id.Value);
                }
            }

            if (id == null)
            {
                CustomerEditViewModel newCus = new();
                return View(newCus);
            }

            Customer customer = _unitOfWork.Get(id.Value);
            if (customer != null)
            {
                CustomerEditViewModel customerEditViewModel = new()
                {
                    Id = customer.CustomerId,
                    CustomerId = customer.CustomerId,
                    Name = customer.Name,
                    Address = customer.Address,
                    Gender = customer.Gender
                };

                if (customer.PhotoPath != null)
                {
                    if (customerEditViewModel.ExistingPhotoPath == null)
                    {
                        customerEditViewModel.ExistingPhotoPath = customer.PhotoPath;
                    }
                }

                return View(customerEditViewModel);
            }

            return NotFound(customer);
        }

        [HttpPost]
        public IActionResult Upsert(CustomerEditViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    if (model.CustomerId == 0)
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
                            return RedirectToAction("Details", new { id = customerModel.CustomerId });
                        }
                        else
                        {
                            //User has selected an existing image
                            Customer customer = _unitOfWork.Get(model.CustomerId);

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
                        Customer customer = _unitOfWork.Get(model.CustomerId);

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
                                CustomerId = model.CustomerId,
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
                            CustomerId = model.CustomerId
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
        public IActionResult Delete(int id)
        {
            Customer findCustomerObj = _unitOfWork.Get(id);
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
                _logger.LogError($"The customer with the specified Id = {id} cannot be found!");
                return View("CustomerNotFound", id);
            }

            _unitOfWork.Remove(id);
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
            Customer customerModel = new()
            {
                CustomerId = model.CustomerId,
                Address = model.Address,
                Name = model.Name,
                Gender = model.Gender,
                PhotoPath = ProcessImageOnServer(model)
            };
            return customerModel;
        }
    }
}
