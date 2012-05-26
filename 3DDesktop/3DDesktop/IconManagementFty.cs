using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DDesktop
{
    public class IconManagementFty
    {
        private static IconManagement instance = null;

        public static IconManagement Instance
        {
            get
            {
                if (instance == null)
                    instance = new IconManagement();
                return instance;}
        }

    }
}
