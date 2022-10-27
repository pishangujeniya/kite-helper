using System.Security.Policy;
using KiteConnect;
using KiteHelper.Models;

namespace KiteHelper.Helpers
{
    public static class KiteConnectSdkHelper
    {
        public static KiteConnectSdk.KiteConnectSdk KiteConnectSdk = new();

        public static bool Login(LoginModel loginModel)
        {
            try
            {
                return KiteConnectSdk.Login(loginModel.UserId, loginModel.Password, loginModel.AppCode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static Profile GetProfile()
        {
            return KiteConnectSdk.GetProfile();
        }
    }
}
