using SearchIMG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace SearchIMG
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            ImageProcessing baseimg = ImageProcessing.getSingleton();
            baseimg.imgStart();

        }
    }
}
