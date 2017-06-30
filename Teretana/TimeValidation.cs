using System;
using System.ComponentModel.DataAnnotations;

namespace Teretana
{
    public class TimeValidation : RangeAttribute
    {
        public TimeValidation() : base(typeof(DateTime), DateTime.Now.ToShortDateString(), DateTime.Now.AddYears(2).ToShortDateString())
        {

        }

    }
}