using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace iBookStoreMVC.ViewModels
{
    public class CatalogManagementIndexViewModel
    {
        public IEnumerable<CatalogItem> CatalogItems { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
