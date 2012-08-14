namespace SQLCC.Core.Objects
{
   public class DbCodeSegment
   {
      public string ObjectName { get; set; }
      
      public int LinesOfCode { get; set; }

      public int StartByte { get; set; }

      private int _length;
      public int Length
      {
         get
         {
            return _length;
         }
         set
         {
            _length = value;
            _endByte = this.StartByte + _length;
         }
      }

      private int _endByte;
      public int EndByte { 
         get
         {
            return _endByte;
         }
         set
         {
            _endByte = value;
            _length = _endByte - this.StartByte;
         }
      }
   }
}