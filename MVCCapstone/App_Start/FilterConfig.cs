﻿using System.Web;
using System.Web.Mvc;

namespace MVCCapstone
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute(){View = "/Error/"});

        }
    }
}