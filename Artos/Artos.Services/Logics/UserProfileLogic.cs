using Artos.Entities;
using Artos.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artos.Services.Logics
{
    public class UserProfileLogic
    {
        RedisDB db { get; set; }

        public UserProfileLogic()
        {

            db = ObjectContainer.Get<RedisDB>();

        }
        public UserProfile GetUserById(long Id)
        {
            var datas = (from c in db.GetAllData<UserProfile>()
                         where c.Id == Id
                         select c).ToList();
            return datas[0];
        }
    }
}
