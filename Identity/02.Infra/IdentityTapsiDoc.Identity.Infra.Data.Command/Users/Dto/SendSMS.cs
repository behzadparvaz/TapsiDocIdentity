using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Infra.Data.Command.Users.Dto
{
    public class SendSMS
    {
        public List<Model> model { get; set; }
        public SendSMS()
        {
            model = new List<Model>();
        }
    }
    public class Model
    {
        public string PhoneNumber { get; set; }
        public string MessageBody { get; set; }
    }
}
