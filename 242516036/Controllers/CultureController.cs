using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace _242516036.Controllers // Proje isminle aynı olsun
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        public IActionResult Set(string culture, string redirectUri)
        {
            if (culture != null)
            {
                // Tarayıcıya dil tercihini çerez olarak kaydet
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)));
            }

            // Geldiği sayfaya geri gönder
            return LocalRedirect(redirectUri);
        }
    }
}