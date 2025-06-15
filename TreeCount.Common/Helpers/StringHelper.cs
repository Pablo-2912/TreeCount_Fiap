using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TreeCount.Common.Helpers
{
    public static class StringHelper
    {
        public static bool SecureEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            var result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }

        public static string SanitizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 1. Normaliza para separar acentos (ex: ç -> c + ̧)
            var normalized = input.Normalize(NormalizationForm.FormD);

            // 2. Remove os caracteres não ASCII (acentos, cedilhas etc)
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            var withoutAccents = sb.ToString().Normalize(NormalizationForm.FormC);

            // 3. Remove tudo que não for letra ou dígito
            var onlyLettersAndDigits = Regex.Replace(withoutAccents, @"[^a-zA-Z0-9]", "");

            return onlyLettersAndDigits;
        }

    }
}
