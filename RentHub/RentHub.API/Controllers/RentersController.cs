using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentersController : ControllerBase
    {
        [HttpGet("GetRenters")]
        public ActionResult<List<Renter>> GetRenters()
        {
            List<Renter> RentersList = RentHubContext.Instance.Renters.ToList();
            if (RentersList.IsNullOrEmpty())
            {
                return NotFound("Список клиентов пуст");
            }
            return RentersList;
        }

        [HttpGet("GetRenterById{id}")]
        public ActionResult<Renter> GetRenter(int id)
        {
            List<Renter> RentersList = RentHubContext.Instance.Renters.ToList();
            Renter? renter = RentersList.FirstOrDefault(r => r.RenterId == id);

            if (renter == null)
            {
                return NotFound($"Клиент с ID {id} не найден");
            }

            return Ok(renter);
        }

        [HttpPost("AddRenter")]
        public ActionResult AddRenter(RenterDTO renterdto)
        {
            Renter renter = new Renter
            {
                Lastname = renterdto.Lastname,
                Name = renterdto.Name,
                Patronymic = renterdto.Patronymic,
                PhoneNumber = renterdto.PhoneNumber
            };
            RentHubContext.Instance.Renters.Add(renter).Context.SaveChanges();
            return Ok($"Клиент {renter.Name} {renter.Lastname} успешно добавлен");
        }

        [HttpPut("ChangeRenterData{id}")]
        public ActionResult ChangeRenterData(int id, RenterDTO renterDTO)
        {
            List<Renter> renters = RentHubContext.Instance.Renters.ToList();
            Renter? renter = renters.FirstOrDefault(r => r.RenterId == id);
            if (renter == null)
            {
                return NotFound($"Клиент с ID {id} не найден");
            }
            renter.Lastname = renterDTO.Lastname;
            renter.Name = renterDTO.Name;
            renter.Patronymic = renterDTO.Patronymic;
            renter.PhoneNumber = renterDTO.PhoneNumber;
            RentHubContext.Instance.SaveChanges();

            return Ok("Данные клиента успешно изменены");
        }

        [HttpDelete("DeleteRenter{id}")]
        public ActionResult DeleteRenter(int id)
        {
            List<Renter> renters = RentHubContext.Instance.Renters.ToList();
            Renter? renter = renters.FirstOrDefault(r => r.RenterId == id);
            if (renter == null)
            {
                return NotFound($"Клиент с ID {id} не найден");
            }
            RentHubContext.Instance.Renters.Remove(renter);
            RentHubContext.Instance.SaveChanges();
            return Ok("Клиент успешно удален");
        }
    }
}
