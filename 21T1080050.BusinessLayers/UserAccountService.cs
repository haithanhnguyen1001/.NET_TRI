using SV21T1080050.DataLayers;
using SV21T1080050.DataLayers.SQLServer;
using SV21T1080050.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1080050.BusinessLayers
{
    public static class UserAccountService
    {

        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;

        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;

            employeeAccountDB = new EmployeeAccountDAL(connectionString);
            customerAccountDB = new CustomerAccountDAL(connectionString);
        }
        //Sử dụng những cách cài đặt khác nhau để thực thi từng trường hợp trong oop gọi là tính đa hình
        //Tức là cùng 1 giao diện(1 mô tả) nhưng có nhiều cách cài đặt khác nhau
        public static UserAccount? Authorize(UserTypes userType, string username, string password)
        {
            if(userType == UserTypes.Employee)
                return employeeAccountDB.Authorize(username, password);
            else
                return customerAccountDB.Authorize(username, password);
        }
    }

    /// <summary>
    /// Loại tài khoản
    /// </summary>
    public enum UserTypes
    {
        Employee,

    }
}
