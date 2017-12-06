using Artos.Entities;
using Artos.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artos.Services.Logics
{
    public class EMoneyLogic
    {
        RedisDB db { get; set; }
        public EMoneyLogic()
        {
            db = ObjectContainer.Get<RedisDB>();
        }

        public EMoney CheckBalance(long UserId)
        {
          
            var datas = (from c in db.GetAllData<EMoney>()
                         where c.OwnerId == UserId
                         select c).ToList();
            if (datas != null && datas.Count() > 0)
            {
                foreach (var item in datas)
                {
                    if (item.IsActive && item.IsDefault)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public bool CutBalance(long UserId, Int64 Amount)
        {
            bool IsSucceed = false;
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

                            IsSucceed = db.InsertData<EMoney>(item);
                            var seluser = db.GetDataById<UserProfile>(UserId);
                            foreach (var x in seluser.MyWallet)
                            {
                                if (item.Id == x.Id)
                                {
                                    x.Balance = item.Balance;
                                }
                            }
                            db.InsertData<UserProfile>(seluser);
                            
                        }
                    }


                }
            }


            return IsSucceed;

        }
    }
}
