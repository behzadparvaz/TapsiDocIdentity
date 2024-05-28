using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Common.Auth
{
    public class JwtOptions
    {
        public string SecretKey = "g30zQaUBLjfI0UeDJ1uWVLMLmQSQugUisP1EiPgl8H5i3wscbWyGS84YNbVCP00SHx6ESSBWre6azGvBMQkpdIxuQfv2li7XkGzq1kS5DUOpyzjoKjEZB253K443cY3TI6yzyzpzWMVyTEHyy1BuwccNYOyYfMzBZ7gAAQhIJHwqi1jY3eN1mxLoAXfjAxtSHevCGXHhTaSvuPkXXikVrcJZFeWVUQJO0E2Vlv2qqt5naGqHVvJyVc5Ew56LsidUDFlT8aQUy0wZkIQ1sOpjKRbketkLqwl9B73ZZcvXc1NbH2afmmkFoHR47XuwFhjf"
;        public int ExpiryMinutes = 60;
        public string Issuer { get; set; }

    }
}
