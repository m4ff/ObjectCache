using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CacheReferenceExpiredExeption : Exception
{
    private const string ErrorMsg = "This reference has expired.";

    public CacheReferenceExpiredExeption() : base(ErrorMsg)
    {

    }
}