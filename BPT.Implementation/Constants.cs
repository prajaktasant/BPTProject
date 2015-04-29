using System;
using System.Collections.Generic;
using System.Text;

namespace BPT.Implementation
{
    public static class Constants
    {
        public static int LEAF_ORDER = 2; //Maximum number of keys for a leaf node.
        public static int INTERNAL_ORDER = 2;   //Maximum number of keys for a internal node
        public static int RECORDS_PER_BLOCK = 2; //Number of records a block can contain.
    }
}
