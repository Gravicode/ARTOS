using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Artos.Entities;
using Artos.Tools;

namespace Artos.Services.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserServiceController : Controller
    {
        RedisDB db { get; set; }
      
        public UserServiceController()
        {
            db = ObjectContainer.Get<RedisDB>();
           
        }

        [HttpPost("[action]")]
        public IActionResult Login([FromBody]LoginData data)
        {
            var hasil = new OutputData() { IsSucceed = true };
            var datas = (from c in db.GetAllData<UserProfile>()
                        where c.Username == data.UserName
                        select c).ToList();
            if(datas!=null && datas.Count() > 0)
            {
                if(datas[0].Password== data.Password)
                    hasil.Data = UserToken.GenerateToken(datas[0].Username);
                else {
                    hasil.IsSucceed = false;
                    hasil.ErrorMessage = "wrong password";
                }
            }
            else
            {
                hasil.IsSucceed = false;
                hasil.ErrorMessage = "user not found.";
            }
            return Ok(hasil);

        }
        [HttpPost("[action]")]
        public IActionResult CreateUser([FromBody]UserProfile data)
        {
            var hasil = new OutputData() { IsSucceed = true };
            var datas = (from c in db.GetAllData<UserProfile>()
                         where c.Username == data.Username
                         select c).ToList();
            if (datas != null && datas.Count() > 0)
            {
               
                    hasil.IsSucceed = false;
                    hasil.ErrorMessage = "user is already exists";
                
            }
            else
            {
                data.Id = db.GetSequence<UserProfile>();
                hasil.IsSucceed = db.InsertData<UserProfile>(data);
               
            }
            return Ok(hasil);

        }
        [HttpPost("[action]")]
        public IActionResult UpdateUser([FromBody]UserProfile data)
        {
            var hasil = new OutputData() { IsSucceed = true };
            var datas = (from c in db.GetAllData<UserProfile>()
                         where c.Username == data.Username
                         select c).ToList();
            if (datas != null && datas.Count() > 0)
            {
                var seluser = datas[0];
                data.Id = seluser.Id;
                hasil.IsSucceed = db.InsertData<UserProfile>(data);
            }
            else
            {
                hasil.IsSucceed = false;
                hasil.ErrorMessage = "user is not exists";

            }
            return Ok(hasil);

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var hasil = new OutputData() { IsSucceed = true };
            try
            {
                hasil.IsSucceed = db.DeleteData<UserProfile>(id);
            }
            catch
            {
                hasil.IsSucceed = false;
                hasil.ErrorMessage = "delete failed";
            }

            return Ok(hasil);
        }

       
    }
}
