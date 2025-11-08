using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RentersController : ControllerBase
    {
        [Authorize]
        [HttpGet("renters")]
        public ActionResult<List<Renter>> GetRenters()
        {
            using RentHubContext context = new();
            List<Renter> RentersList = context.Renters.ToList();
            if (RentersList.IsNullOrEmpty())
            {
                return NotFound("Список клиентов пуст");
            }
            return RentersList;
        }

        [Authorize]
        [HttpGet("renter-by-id/{id}")]
        public ActionResult<Renter> GetRenter(int id)
        {
            using RentHubContext context = new();
            Renter? renter = context.Renters.FirstOrDefault(r => r.RenterId == id);

            if (renter == null)
            {
                return NotFound($"Клиент с ID {id} не найден");
            }

            return Ok(renter);
        }

        [Authorize]
        [HttpPost("renter")]
        public ActionResult AddRenter(RenterDTO renterdto)
        {
            using RentHubContext context = new();
            Renter renter = new Renter
            {
                Lastname = renterdto.Lastname,
                Name = renterdto.Name,
                Patronymic = renterdto.Patronymic,
                PhoneNumber = renterdto.PhoneNumber
            };
            context.Renters.Add(renter).Context.SaveChanges();
            return Ok($"Клиент {renter.Name} {renter.Lastname} успешно добавлен");
        }

        [Authorize]
        [HttpPut("renter-data/{id}")]
        public ActionResult ChangeRenterData(int id, RenterDTO renterDTO)
        {
            using RentHubContext context = new();
            Renter? renter = context.Renters.FirstOrDefault(r => r.RenterId == id);
            if (renter == null)
            {
                return NotFound($"Клиент с ID {id} не найден");
            }
            renter.Lastname = renterDTO.Lastname;
            renter.Name = renterDTO.Name;
            renter.Patronymic = renterDTO.Patronymic;
            renter.PhoneNumber = renterDTO.PhoneNumber;
            context.SaveChanges();

            return Ok("Данные клиента успешно изменены");
        }

        [Authorize]
        [HttpDelete("renter/{id}")]
        public ActionResult DeleteRenter(int id)
        {
            using RentHubContext context = new();
            Renter? renter = context.Renters.FirstOrDefault(r => r.RenterId == id);
            if (renter == null)
            {
                return NotFound($"Клиент с ID {id} не найден");
            }
            context.Renters.Remove(renter);
            context.SaveChanges();
            return Ok("Клиент успешно удален");
        }
    }
}
