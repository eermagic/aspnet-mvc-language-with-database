using Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace LanguageWithDatabase.Controllers
{
    public class ProjectBase : Controller
    {
        /// <summary>
        /// 覆寫 ActionExecuting 事件
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 語系名稱
            var langName = "";

            //從cookie裡讀取語言設定
            HttpCookie cookie = filterContext.HttpContext.Request.Cookies["Localization.CurrentUICulture"];
            if (cookie != null && cookie.Value != "")
            {
                //根據 cookie 值設定語言
                langName = cookie.Value;
            }
            else if (filterContext.HttpContext.Request.UserLanguages != null)
            {
                // 使用瀏覽器預設語言
                if (filterContext.HttpContext.Request.UserLanguages.Length > 0)
                {
                    langName = filterContext.HttpContext.Request.UserLanguages[0];
                }
            }

            //自行判斷可接受的語系名稱，不符名稱則採用預設語系
            if (langName != "zh-TW" && langName != "en-US")
            {
                langName = "zh-TW";
            }
            ViewData["_Language"] = langName;

            // 更換語系設定
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langName);

            // 產生 JavaScript 語系對照表
            ResourceSet languageSet = Language.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            var sbInitial = " var JsLangObj = {";
            var sbRes = new StringBuilder(sbInitial);
            var resEnum = languageSet.GetEnumerator();
            while (resEnum.MoveNext())
            {
                if (sbRes.ToString() != sbInitial)
                {
                    sbRes.Append(",");
                }
                sbRes.Append("\"" + resEnum.Key + "\":\"" + resEnum.Value.ToString().Replace("\r\n", "").Replace("\"", "\\\"") + "\"");
            }
            sbRes.Append("}");
            ViewData["_JsLangObj"] = sbRes.ToString();

            // 把設定儲存進cookie
            if (cookie == null)
            {
                cookie = new HttpCookie("cookie");
            }
            cookie.Value = langName;
            cookie.Expires = DateTime.Now.AddMonths(1); //儲存 1 個月
            cookie.Secure = true;
            cookie.HttpOnly = true;
            cookie.SameSite = SameSiteMode.Lax;
            filterContext.HttpContext.Response.Cookies.Add(cookie);

            base.OnActionExecuting(filterContext);
        }
    }
}