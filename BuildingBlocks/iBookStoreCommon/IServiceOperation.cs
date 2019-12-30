using System;
using System.Collections.Generic;
using System.Text;

namespace iBookStoreCommon
{
    public interface IServiceOperation
    {
        string HttpMethod { get; }

        string Path { get; }
    }
}
