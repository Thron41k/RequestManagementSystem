using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestManagement.Common.Models.Enums;

namespace RequestManagement.WarehouseScan.Services
{
    public class AuthTokenStore
    {
        private string _token;
        private UserRole _role;
        public void SetToken(string token) => _token = token;

        public string GetToken() => _token;

        public void ClearToken() => _token = null;

        public void SetRole(UserRole role) => _role = role;
        public UserRole GetRole() => _role;
    }
}
