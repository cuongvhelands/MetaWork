using System;

namespace MetaWork.Data.ViewModel
{
    internal class StringLengthAttribute : Attribute
    {
        public string ErrorMessage { get; set; }
    }
}