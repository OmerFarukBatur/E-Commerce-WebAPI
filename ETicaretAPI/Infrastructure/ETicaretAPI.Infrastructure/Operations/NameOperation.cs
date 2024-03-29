﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Operations
{
    public static class NameOperation
    {
        public static string CharacterRegulatory(string name)        
            => name.Replace("\"", "")
                .Replace("!", "")
                .Replace("'", "")
                .Replace("^", "")
                .Replace("+", "")
                .Replace("%", "")
                .Replace("&", "")
                .Replace("/", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("=", "")
                .Replace("?", "")
                .Replace("@", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("~", "")
                .Replace("£", "")
                .Replace("#", "")
                .Replace("$", "")
                .Replace("½", "")
                .Replace("*", "")
                .Replace("_", "")
                .Replace("æ", "")
                .Replace("ß", "")
                .Replace("´", "")
                .Replace(";", "")
                .Replace(",", "")
                .Replace(">", "")
                .Replace("é", "")
                .Replace("Ö", "o")
                .Replace("ö", "o")
                .Replace("Ü", "u")
                .Replace("ü", "u")
                .Replace("Ğ", "g")
                .Replace("ğ", "g")
                .Replace("ı", "i")
                .Replace("İ", "i")
                .Replace("¨", "")
                .Replace("Ç", "c")
                .Replace("ç", "c")
                .Replace("Ş", "s")
                .Replace("ş", "s")
                .Replace("|", "");
          
    }
}
