using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebRole1;

namespace WebRole1
{
    public class PageCount
    {
        public Webpage webpage { get; set; }
        public int count { get; set; }

        public PageCount(Webpage webpage)
        {
            this.webpage = webpage;
            this.count = 1;
        }

    //    public void increment()
    //    {
    //        this.count++;
    //    }
    }
}
