using ExampleEmpty.UI.Models.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleEmpty.UI.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetDataController : Controller
    {
        private readonly ICustomerRepository _unitOfWork;
        public GetDataController(ICustomerRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var customerList = _unitOfWork.Get()
                                               .Select(c => new
                                               {
                                                   id = c.CustomerId,
                                                   fullName = c.Name,
                                                   sex = Enum.GetName(c.Gender.Value),
                                                   location = c.Address,

                                               }).ToList();
            return Json(new { data = customerList });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var customer = _unitOfWork.Get(id);
            if (customer != null)
            {
                _unitOfWork.Remove(customer);
                return Json(new { success = true, message = "Delete successful" });
            }
            else
            {
                return Json(new { success = false, message = "Error why deleting." });
            }


        }

        //[HttpPut("{id}")]
        //public IActionResult Edit(int id)
        //{
        //    var customer = _unitOfWork.CustomerRepository.Get(id);
        //    if (customer != null)
        //    {
        //      return View("")
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "Error why deleting." });
        //    }


        //}
    }
}
