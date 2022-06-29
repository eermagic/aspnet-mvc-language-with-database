# aspnet-mvc-language-with-database
[ASP.NET MVC] 多國語系切換 – 使用資料庫管理與兼容 JavaScript 顯示

網站的多國語系切換是走向國際發展的第一步，本篇文章將會教你如何使用 ASP.NET MVC 的多國語系切換功能，教學內容是接近實務應用的作法，使用資料庫管理多國語言，以及讓 JavaScript 檔案也可以顯示不同語系。

在官方的教學中是使用 Resources 儲存多國語系，當有多個語系時就要同時編寫多個檔案。
我的設計方式是將多國語系存放在資料庫裡面統一管理，再利用程式將資料庫裡面語系輸出至 Resources 裡面，與官方教學模式有些不同，有興趣的朋友可以多學一種設計模式。

之前寫多國語系時，*.cshtml 上文字可以使用資源檔的值，可是 JavaScript 的 *.js 檔案就無法使用，所以我另外提供一種讓 JavaScript 檔案也可以顯示多語系的方法給各位參考。

[完整文章教學](https://blog.hungwin.com.tw/aspnet-mvc-language-with-database/)
