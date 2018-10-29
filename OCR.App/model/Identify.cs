using Ocr.Base.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ocr.Base.Extract.invoice
{
    public class Identify
    {

        public enum DataType
        {

            SystemInt32 = 1,
            SystemInt64 = 2,
            SystemDouble = 3,
            SystemDateTime = 4,
            SystemString = 5,
            SystemDecimalFr = 6,
            StstemDecimalIt = 7,
            SystemDecimalDe = 8
        }
        public List<Row> RemoveSpace(List<Row> preSapce)

        {
            try
            {

                string findpreSpace = "";
            if(preSapce.Count>1)
            for (int i = 1; i <= preSapce.Count - 1; i++)
            {
                if (preSapce[i].Word != null)
                {
                    if (preSapce[i].Word.Trim() == ".")
                    {
                        if (Regex.IsMatch(preSapce[i - 1].Word, @"^\$\d") || Regex.IsMatch(preSapce[i - 1].Word, @"^\d"))
                            if (Regex.IsMatch(preSapce[i + 1].Word, @"^\d{2}"))
                            {
                                preSapce[i - 1].Word = preSapce[i - 1].Word + preSapce[i].Word + preSapce[i + 1].Word;
                                preSapce[i - 1].Type = 3;
                                preSapce[i].Word = "";
                                preSapce[i].Type = 5;
                                preSapce[i + 1].Word = "";
                                preSapce[i + 1].Type = 5;
                            }
                    }
                    // 3000a00
                    if(Regex.IsMatch(preSapce[i].Word, @"[\d]{1,4}([a]{1}[\d]{2})"))
                    {
                        Regex rgx2 = new Regex("\\a");
                        preSapce[i].Word = preSapce[i].Word.Replace('a', ',');
                        preSapce[i].Type = 3;
                    }
                    if (Regex.IsMatch(preSapce[i].Word, @"[\d]{1,4}([,]{1}[\d]{2})([|])"))
                    {
                        Regex rgx2 = new Regex("\\a");
                        preSapce[i].Word = preSapce[i].Word.Replace('|', ' ');
                        preSapce[i].Type = 3;
                    }
                    if (Regex.IsMatch(preSapce[i].Word, @"^\$\d$"))
                    {
                        if (Regex.IsMatch(preSapce[i + 1].Word, @"^\d\.$"))
                        {
                            preSapce[i + 1].Word = preSapce[i].Word + preSapce[i + 1].Word;
                            preSapce[i].Word = "";
                            preSapce[i].Type = 5;
                            if (Regex.IsMatch(preSapce[i + 2].Word, @"^\d"))
                            {
                                preSapce[i + 2].Word = preSapce[i + 1].Word + preSapce[i + 2].Word;
                                preSapce[i + 2].Type = 3;
                                preSapce[i + 1].Word = "";
                                preSapce[i + 1].Type = 5;

                            }


                        }
                    }
                    if (Regex.IsMatch(preSapce[i].Word.Trim(), @"^\.\d"))
                    {
                        findpreSpace = "async";
                        if (Regex.IsMatch(preSapce[i - 1].Word, @"^\d"))
                        {
                            preSapce[i - 1].Word = preSapce[i - 1].Word + preSapce[i].Word;
                            preSapce[i].Word = "";
                            preSapce[i].Type = 5;
                        }
                    }

                    if (Regex.IsMatch(preSapce[i].Word.Trim(), @"^\d{1,3}\,\d{2}$") || Regex.IsMatch(preSapce[i].Word.Trim(), @"^\$\d{1,3}\,\d{2}$"))
                    {
                        Regex rgx2 = new Regex("\\,");
                        preSapce[i].Word = rgx2.Replace(preSapce[i].Word, ".");
                    }
                    //if (Regex.IsMatch(preSapce[i].word.Trim(), @"^\d{1,3}\_\d{2}$") || Regex.IsMatch(preSapce[i].word.Trim(), @"^\$\d{1,3}\_\d{2}$"))
                    //{
                    //    Regex rgx2 = new Regex("\\_");
                    //    preSapce[i].word = rgx2.Replace(preSapce[i].word, ".");
                    //}

                }
            }
            return preSapce;
            }
            catch
            {
                return preSapce;
            }
        }
        public   DataType StringType(string str)
        {

            string[] formats = {"M/d/yyyy", "M/d/yyyy",
                   "MM/dd/yyyy", "M/d/yyyy",
                   "M/d/yyyy", "M/d/yyyy",
                   "M/d/yyyy", "M/d/yyyy",
                   "MM/dd/yyyy", "M/dd/yyyy"};
            Int32 intValue;
            Int64 bigintValue;
            double doubleValue;
            DateTime dateValue;
            string dstr = "";

            if (str != null)
            {
                Regex rgx = new Regex("\\$");
                dstr = rgx.Replace(str, "");
            }

            if (str == "") return DataType.SystemString;

            else if (Int32.TryParse(str, out intValue))
                return DataType.SystemInt32;
            else if (Int64.TryParse(str, out bigintValue))
                return DataType.SystemInt64;
            else if (DateTime.TryParseExact(str, formats,
                             new CultureInfo("en-US"),
                             DateTimeStyles.None, out dateValue))
                return DataType.SystemDateTime;
            else if(str != null && Regex.IsMatch(str, @"^\d{4}\.\d{2}\.\d{2}$"))
            {
                return DataType.SystemDateTime;

            }
            else if (str != null && Regex.IsMatch(str, @"^\d{4}\-\d{2}\-\d{2}$"))
            {
                return DataType.SystemDateTime;

            }
            else if (str != null && Regex.IsMatch(str, @"^[0 - 9]{ 4}-(0[1 - 9] | 1[0 - 2]) - (0[1 - 9] |[1 - 2][0 - 9] | 3[0 - 1])$"))
            {
                return DataType.SystemDateTime;

            }
            
            else if (str != null && Regex.IsMatch(str, @"^\d{2}\.\d{2}\.\d{4}$"))
            {
                return DataType.SystemDateTime;

            }
            else if (str != null && Regex.IsMatch(str, @"^\d{2}\.\d{2}\.\d{2}$"))
            {
                return DataType.SystemDateTime;

            }
            else if (str != null && Regex.IsMatch(str, @"^\d{2}\-\d{2}\-\d{4}$"))
            {
                return DataType.SystemDateTime;

            }
            else if (str != null && Regex.IsMatch(str, @"^\d{2}\-\d{2}\-\d{2}$"))
            {
                return DataType.SystemDateTime;

            }
            else if (str != null && Regex.IsMatch(str, @"^\d{4}\-\d{2}\-\d{2}$"))
            {
                return DataType.SystemDateTime;

            }
            else if (str != null && Regex.IsMatch(str, @"^\d{2}\/\d{2}\/\d{2}$"))
            {
                return DataType.SystemDateTime;

            }
            else if (double.TryParse(dstr, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out doubleValue))
                return DataType.SystemDouble;
            else if (double.TryParse(dstr, NumberStyles.Number, CultureInfo.CreateSpecificCulture("fr-FR"), out doubleValue))
                return DataType.SystemDecimalFr;
            else if (double.TryParse(dstr, NumberStyles.Number, CultureInfo.CreateSpecificCulture("it-IT"), out doubleValue))
                return DataType.StstemDecimalIt;
            else if (double.TryParse(dstr, NumberStyles.Number, CultureInfo.CreateSpecificCulture("de-DE"), out doubleValue))
                return DataType.SystemDecimalDe;
           
            else return DataType.SystemString;

        }


    }
}
