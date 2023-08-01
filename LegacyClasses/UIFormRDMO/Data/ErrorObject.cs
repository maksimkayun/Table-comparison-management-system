using System;
using UIFormRDMO.Data.Models;

namespace UIFormRDMO.Data
{
    public class ErrorObject : IPerson
    {
        public ErrorObject(string message = "default", string code = "default")
        {
            Message = message;
            ErrorCode = code;
        }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string? DateAttest { get; set; }
        public string? DateMed { get; set; }
        
        public string? ErrorCode { get; set; }
        
        public string? Message { get; set; }
    }

    public static class Extensions
    {
        public static ErrorObject ToErrorObject(this Exception e)
        {
            return new ErrorObject(message: e.Message);
        }
    }
}