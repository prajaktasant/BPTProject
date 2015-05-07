using System;
using System.Collections.Generic;
using System.Text;

namespace BPT.Implementation
{
    public class DataBlock
    {
        String name;
        String information;
        public DataBlock(String _name, String _information)
        {
            name = _name;
            information = _information;
        }
        public String getStudentName()
        {
            return name;
        }
        public String getStudentConfInformation()
        {
            if (string.Compare(information, Constants.NULL_STRING, StringComparison.OrdinalIgnoreCase) == 0)
                return string.Empty;

            return information;
        }
    }
}
