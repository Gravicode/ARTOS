using Artos.Entities;
using Artos.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artos.Services.Logics
{
    public class PublicServiceLogic
    {
        RedisDB db { get; set; }

        public PublicServiceLogic()
        {
            db = ObjectContainer.Get<RedisDB>();

        }

        public PublicService GetPublicServiceById(long Id)
        {
            var datas = (from c in db.GetAllData<PublicService>()
                         where c.Id == Id
                         select c).ToList();
            return datas[0];
        }

        public PublicService GetPublicServiceByQrCode(string QRCode)
        {
            var datas = (from c in db.GetAllData<PublicService>()
                         where c.QRCode == QRCode
                         select c).ToList();
            return datas[0];
        }
    }
}
