using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class SortDescriptor
    {
        public SortingDirection Direction { get; set; }
        public string Field { get; set; }

        public enum SortingDirection
        {
            Ascending,
            Descending
        }
    }
}
