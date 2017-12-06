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
    public class EmoneyServiceController : Controller
    {

        RedisDB db { get; set; }

        public EmoneyServiceController()
        {
          
                db = ObjectContainer.Get<RedisDB>();

        }

        [HttpPost("[action]")]
        public IActionResult AddEMoney([FromBody]EMoney data)
        {
            var hasil = new OutputData() { IsSucceed = true };
            //verifikasi emoney
            //...
            data.Id = db.GetSequence<EMoney>();
            hasil.IsSucceed = db.InsertData<EMoney>(data);
            var seluser = db.GetDataById<UserProfile>(data.OwnerId);
            if (seluser.MyWallet == null) seluser.MyWallet = new List<EMoney>();
            seluser.MyWallet.Add(data);
            db.InsertData<UserProfile>(seluser);
            return Ok(hasil);

        }
        [HttpPost("[action]")]
        public IActionResult UpdateEMoney([FromBody]EMoney data)
        {
            var hasil = new OutputData() { IsSucceed = true };
            var datas = (from c in db.GetAllData<EMoney>()
                         where c.Id == data.Id
                         select c).ToList();
            if (datas != null && datas.Count() > 0)
            {
                var seldata = datas[0];
                data.Id = seldata.Id;
                hasil.IsSucceed = db.InsertData<EMoney>(data);
                var seluser = db.GetDataById<UserProfile>(data.OwnerId);
                foreach(var item in seluser.MyWallet)
                {
                    if(item.Id == data.Id)
                    {
                        item.Balance = data.Balance;
                        item.IsActive = data.IsActive;
                        item.IsDefault = data.IsDefault;
                        
                    }
                }
                db.InsertData<UserProfile>(seluser);
            }
            else
            {
                hasil.IsSucceed = false;
                hasil.ErrorMessage = "card is not exists";

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
                hasil.IsSucceed = db.DeleteData<EMoney>(id);
            }
            catch
            {
                hasil.IsSucceed = false;
                hasil.ErrorMessage = "delete failed";
            }

            return Ok(hasil);
        }
        [HttpGet("[action]")]
        public IActionResult CheckBalance(long UserId)
        {
            var hasil = new OutputData() { IsSucceed = false, ErrorMessage = "card is not exists, or no default card" };

            var datas = (from c in db.GetAllData<EMoney>()
                         where c.OwnerId == UserId
                         select c).ToList();
            if (datas != null && datas.Count() > 0)
            {
                foreach (var item in datas)
                {
                    if (item.IsActive && item.IsDefault)
                    {
                        hasil.Data = item;
                        hasil.IsSucceed = true;
                        hasil.ErrorMessage = "";
                    }
                }
            }
            return Ok(hasil);
        }
       
        [HttpGet("[action]")]
        public IActionResult CutBalance(long UserId, Int64 Amount)
        {
            var hasil = new OutputData() { IsSucceed = false, ErrorMessage="your card balance is not enough." };

            var datas = (from c in db.GetAllData<EMoney>()
                         where c.OwnerId == UserId
                         select c).ToList();
            if (datas != null && datas.Count() > 0)
            {
                foreach (var item in datas)
                {
                    if (item.IsActive && item.IsDefault)
                    {
                        if (item.Balance >= Amount)
                        {
                            item.Balance -= Amount;

                            hasil.IsSucceed = db.InsertData<EMoney>(item);
                            var seluser = db.GetDataById<UserProfile>(UserId);
                            foreach (var x in seluser.MyWallet)
                            {
                                if (item.Id == x.Id)
                                {
                                    item.Balance = x.Balance;
                                }
                            }
                            db.InsertData<UserProfile>(seluser);
                            hasil.Data = item;
                            hasil.IsSucceed = true;
                            hasil.ErrorMessage = "";
                        }
                    }


                }
            }


            return Ok(hasil);

        }
    }
}
