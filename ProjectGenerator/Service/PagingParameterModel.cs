using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectGenerator.Service
{
    public class PagingParameterModel
    {
        const int maxPageSize = 20;
        public int pageNumber { get; set; } = 1;
        public int _pageSize { get; set; } = 10;
        public int pageSize
        {

            get { return _pageSize; }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
        public int sortBy { get; set; } = 1;
        public bool isAsc { get; set; } = true;
        public string SearchData { get; set; } = "";

    }
}