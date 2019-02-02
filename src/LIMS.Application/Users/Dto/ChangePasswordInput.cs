using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Users.Dto
{
    public class ChangePasswordInput
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
