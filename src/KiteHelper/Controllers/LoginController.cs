using KiteHelper.Helpers;
using KiteHelper.Models;
using Microsoft.AspNetCore.Mvc;

namespace KiteHelper.Controllers
{
    public class LoginController : Controller
    {
        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult<bool> KiteLogin(LoginModel loginModel)
        {
            return KiteConnectSdkHelper.Login(loginModel);
        }
    }
}
