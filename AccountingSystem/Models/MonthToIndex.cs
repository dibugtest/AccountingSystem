using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class MonthToIndex
    {
        public int change(string nepDate)
        {
            int monthIndex = 0;
            if (nepDate != null)
            {

                if (nepDate != "")
                {
                    string month = nepDate.Substring(5, 2);
                    int intMonth = Convert.ToInt32(month);
                    if (intMonth >= 4)
                    {
                        monthIndex = (intMonth - 4);
                    }
                    else if (intMonth <= 3)
                    {
                        monthIndex = (intMonth + 8);
                    }
                }

            }
            return monthIndex;

        }
        public int UniToNum(string str)
        {
            string uni="";
            int result = 0;
            foreach (char item in str)
            {
                switch (item)
                {
                    case '०':
                        uni += "0";
                        break;
                    case '१':
                        uni += "1";
                        break;
                    case '२':
                        uni += "2";
                        break;
                    case '३':
                        uni += "3";
                        break;
                    case '४':
                        uni += "4";
                        break;
                    case '५':
                        uni += "5";
                        break;
                    case '६':
                        uni += "6";
                        break;
                    case '७':
                        uni += "7";
                        break;
                    case '८':
                        uni += "8";
                        break;
                    case '९':
                        uni += "9";
                        break;
                    default:
                        uni += item;
                        break;
                }
            }

            try
            {
                result = Convert.ToInt32(uni);
            }
            catch
            {
                result = 0;
            }
            return result;
        }

    }
}