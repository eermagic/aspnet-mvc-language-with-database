using Dapper;
using Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace LanguageWithDatabase.Controllers
{
    public class HomeController : ProjectBase
    {
        public ActionResult Index()
        {
            //更換資料庫欄位語系
            StringBuilder sbTest = new StringBuilder();
            // 資料庫連線字串
            string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

            // 目前語系檔
            ResourceSet langSet = Language.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            using (var cn = new SqlConnection(connStr))
            {
                // 取得欄位資料
                var list = cn.Query("SELECT  Lang_Key FROM [Language]");
                foreach (var item in list)
                {
                    // 從目前語系檔裡面取得 Key/Value
                    if (langSet.GetObject(item.Lang_Key) != null)
                    {
                        // 存在語系檔裡面
                        sbTest.Append(langSet.GetObject(item.Lang_Key) + "<br>");
                    }
                    else
                    {
                        sbTest.Append(item.Lang_Key + "<br>");
                    }
                }
            }

            ViewData["ConvertColumnLang"] = sbTest.ToString();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// 同步更新資料庫語系檔至 Resources
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateLang()
        {
            // 資源檔可寫入物件
            ResXResourceWriter resxWriterTW = new ResXResourceWriter(Server.MapPath("~/App_GlobalResources/Language.resx"));
            ResXResourceWriter resxWriterUS = new ResXResourceWriter(Server.MapPath("~/App_GlobalResources/Language.en-US.resx"));

            // 取得資料庫語系
            string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
            using (var cn = new SqlConnection(connStr))
            {
                // 使用 Dapper 查詢資料庫
                var list = cn.Query(
              "SELECT Lang_Key, Lang_zhTW, Lang_enUS FROM [Language]");
                foreach (var item in list)
                {
                    // 寫入預設語系資源檔
                    resxWriterTW.AddResource(item.Lang_Key, item.Lang_zhTW);
                    // 寫入英文資源檔
                    resxWriterUS.AddResource(item.Lang_Key, item.Lang_enUS);
                }
            }
            // 關閉資源檔
            resxWriterTW.Close();
            resxWriterUS.Close();

            // 導回首頁
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 切換語系
        /// </summary>
        /// <param name="langCode"></param>
        /// <returns></returns>
        public ActionResult ChangeLang(string langName)
        {
            // 檢查輸入值
            if (langName != "zh-TW" && langName != "en-US")
            {
                langName = "zh-TW";
            }

            // 把設定儲存進cookie
            HttpCookie cookie = new HttpCookie("Localization.CurrentUICulture");
            cookie.Value = langName;
            cookie.Expires = DateTime.Now.AddMonths(1); //儲存 1 個月
            cookie.Secure = true;
            cookie.HttpOnly = true;
            cookie.SameSite = SameSiteMode.Lax;
            HttpContext.Response.Cookies.Add(cookie);

            // 導回首頁
            return RedirectToAction("Index", "Home");
        }
    }
}