using System;

namespace WcfTest.CoreWcf.Server
{
    internal interface IHostableService
    {
        Type[] ServiceContracts { get; }
    }
}
