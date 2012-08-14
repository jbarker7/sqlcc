using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace SQLCC.Impl.HtmlCodeHighlighter.Helpers
{
   /// <summary>
   /// JSON Serialization and Deserialization Assistant Class
   /// </summary>
   public class JsonHelper
   {
      /// <summary>
      /// JSON Serialization
      /// </summary>
      public static string JsonSerializer<T>(T t)
      {
         var ser = new DataContractJsonSerializer(typeof(T));
         var ms = new MemoryStream();
         ser.WriteObject(ms, t);
         var jsonString = Encoding.UTF8.GetString(ms.ToArray());
         ms.Close();
         return jsonString;
      }
      /// <summary>
      /// JSON Deserialization
      /// </summary>
      public static T JsonDeserialize<T>(string jsonString)
      {
         var ser = new DataContractJsonSerializer(typeof(T));
         var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
         var obj = (T)ser.ReadObject(ms);
         return obj;
      }
   }
}
