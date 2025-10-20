using System;
using UnityEngine;

namespace Unity_CS
{
    public static class Unity_CSEncrypt
    {
        #region Base64
        /// <summary>
        /// Expects a string value to encrypt it by Base64 conversion
        /// </summary>
        /// <param name="plainText"><see cref="string"/></param>
        /// <returns></returns>
        public static string Base64Encode(this string plainText)
        {
            var result = "";
            try
            {
                if (!string.IsNullOrEmpty(plainText))
                {
                    var bytes = System.Text.UnicodeEncoding.UTF8.GetBytes(plainText);
                    result = Convert.ToBase64String(bytes);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"There is an error in encryption : {e}");
                throw;
            }
            return result;
        }
        /// <summary>
        /// Expects a string value to decrypt it by Base64 conversion
        /// </summary>
        /// <param name="encrtypedValue"></param>
        /// <returns></returns>
        public static string Base64Decode(this string encrtypedValue)
        {
            var result = "";

            try
            {
                var bytes = Convert.FromBase64String(encrtypedValue);
                result = System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result;
        }
        #endregion
    }
}