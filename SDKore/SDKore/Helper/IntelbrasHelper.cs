using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDKore.Helper
{
    public static class IntelbrasHelper
    {
        public static string InputMask(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.GetOnlyNumbers();

                switch (value.Length)
                {
                    case 14:
                        value = Convert.ToUInt64(value).ToString(@"00\.000\.000\/0000\-00");
                        break;
                    case 11:
                        value = Convert.ToUInt64(value).ToString(@"000\.000\.000\-00");
                        break;
                }
            }

            return value;
        }

    }
}
