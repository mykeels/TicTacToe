
Partial Class justForFun
    Inherits System.Web.UI.Page
    Sub Page_Load() Handles Me.Load
        Response.Write(String.Format("{0}://{1}{2}/", HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Authority, HttpRuntime.AppDomainAppVirtualPath))
    End Sub
End Class
