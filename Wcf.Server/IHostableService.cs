using System;

namespace WcfTest.Wcf.Server
{
    internal interface IHostableService
    {
        Type[] ServiceContracts { get; }
    }
}
