
using System.Collections.Generic;

namespace RouteAPI.Helpers
{
    public class SavingSort : IComparer<Saving>
    {
        public int Compare(object x, object y)
        {
            Saving c1 = (Saving)x;
            Saving c2 = (Saving)y;
            if (c1.getSaving() < c2.getSaving())
            {
                return 1;
            }
            else if (c1.getSaving() > c2.getSaving())
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        public int Compare(Saving x, Saving y)
        {
           if(x.getSaving() < y.getSaving())
           {
               return 1;
           }
           else if (x.getSaving() > y.getSaving())
           {
               return -1;
           }
           else
           {
               return 0;
           }
        }
    }
}