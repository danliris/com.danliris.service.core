using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models.Account_and_Roles
{
    public class AccountProfile
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }

        public int AccountId { get; set; }
        //public virtual Account Account { get; set; }
    }
}
