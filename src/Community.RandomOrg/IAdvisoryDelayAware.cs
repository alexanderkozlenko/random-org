using System;

namespace Community.RandomOrg
{
    internal interface IAdvisoryDelayAware
    {
        TimeSpan AdvisoryDelay { get; }
    }
}