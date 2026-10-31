using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _25DH110096_MyStoreBE.Models.ViewModel
{
    public class PersonalMenuVM
    {
        public bool IsLoggedIn { get; set; }
        public string Username { get; set; }
        public int CartCount { get; set; }
    }
}