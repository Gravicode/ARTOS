using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Artos.Tools;
using Artos.Entities;

namespace Artos.Services.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PublicServiceController : Controller
    {
        RedisDB db { get; set; }

        public PublicServiceController()
        {
            db = ObjectContainer.Get<RedisDB>();

        }

        [HttpPost("[action]")]
        public IActionResult CreatePublicService([FromBody]PublicService data)
        {
            var hasil = new OutputData() { IsSucceed = true };


            data.Id = db.GetSequence<PublicService>();
            data.GenerateQRCode();
            hasil.IsSucceed = db.InsertData<PublicService>(data);


            return Ok(hasil);

        }
        [HttpPost("[action]")]
        public IActionResult UpdatePublicService([FromBody]PublicService data)
        {
            var hasil = new OutputData() { IsSucceed = true };
            var datas = (from c in db.GetAllData<PublicService>()
                         where c.Id == data.Id
                         select c).ToList();
            if (datas != null && datas.Count() > 0)
            {
                var seldata = datas[0];
                data.Id = seldata.Id;
                hasil.IsSucceed = db.InsertData<PublicService>(data);
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
                hasil.IsSucceed = db.DeleteData<PublicService>(id);
            }
            catch
            {
                hasil.IsSucceed = false;
                hasil.ErrorMessage = "delete failed";
            }

            return Ok(hasil);
        }

      

        [HttpGet("[action]")]
        public IActionResult GetQRCodeById(long Id)
        {
            var hasil = new OutputData() { IsSucceed = true };
            var datas = (from c in db.GetAllData<PublicService>()
                         where c.Id == Id
                         select c).ToList();
            if (datas != null && datas.Count() > 0)
            {
                hasil.Data = datas[0].QRCode;
            }
            else
            {
                hasil.IsSucceed = false;
                hasil.ErrorMessage = "data is not exists";

            }
            return Ok(hasil);

        }
    }
}
