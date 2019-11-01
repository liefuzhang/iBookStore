﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace iBookStoreMVC.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<CatalogItem> CatalogItems { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public int? CategoryFilterApplied { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
