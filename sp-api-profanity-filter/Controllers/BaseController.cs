
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Profanity.Helpers;
using SP.Profanity.Interfaces;

namespace SP.Profanity.Controllers
{
    [Authorize(AuthenticationSchemes = ApiDashboardAuthSchemeOptions.Name)]
    public class BaseController : ControllerBase
    {

        public BaseController()
        {
        }

    }
}