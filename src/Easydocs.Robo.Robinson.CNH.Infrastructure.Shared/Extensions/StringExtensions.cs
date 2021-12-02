﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string DecimalToFormatString(this decimal value)
        {             
            return String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:N2}", value).Replace(",", "").Replace(".", "");                                    
        }
        public static string DateTimeToFormatString(this DateTime dateTime)
        {
            var date = dateTime.ToString("dd-MM-yyyy'T'HH:mm:ss");
            return $"{date}";
        }
        public static string FileName(this string file)
        {
            var fileInfo = new FileInfo(file);
            var extesion = fileInfo.Extension;
            var name = fileInfo.Name;
            name = name.Replace(extesion, "");
            var fileName = $"{name}_{DateTime.Now.ToString("yyyyMMddhhMMss").Trim()}{extesion}";
            return fileName;
        }

        public static string OnlyNumbers(this string value)
        {
            return new string(value.Where(c => Char.IsDigit(c)).ToArray());
        }
    }
}
