using System.Collections.Generic;
using System.ComponentModel;

namespace SQLCC.Core.Helpers
{
   public static class DictionaryHelper
   {
      private static T Convert<T>(this string input)
      {
         var converter = TypeDescriptor.GetConverter(typeof(T));
         if (converter != null)
         {
            return (T) converter.ConvertFromString(input);
         }
         return default(T);
      }

      public static void AddOrUpdate(this Dictionary<string, string> dictionary, string key, string value)
      {
         if (dictionary.ContainsKey(key))
         {
            dictionary[key] = value;
         }
         else
         {
            dictionary.Add(key, value);
         }
      }

      //public static void AddOrUpdate<T>(this Dictionary<string, string> dictionary, string key, T value)
      //{
      //   if (dictionary.ContainsKey(key))
      //   {
      //      dictionary[key] = Convert<T>(value);
      //   }
      //   else
      //   {
      //      dictionary.Add(key, value);
      //   }
      //}
   }
}
