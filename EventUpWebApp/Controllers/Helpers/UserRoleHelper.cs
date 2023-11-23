using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventUpLib;

namespace EventUpWebApp.Controllers.Helpers
{
    public static class UserRoleHelper
    {
        public static string GetSelectedRole(User user)
        {
            if (user.Role_Admin == true)
            {
                return "Admin";
            }


            if (user.Role_Planner == true)
            {
                return "Planner";
            }


            if (user.Role_Supplier == true)
            {
                return "Supplier";
            }

            return string.Empty;
        }
    }
}