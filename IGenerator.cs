using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WPFAccelerators
{
    public interface IGenerator
    {
        string Source { get; set; }

        string LastResult { get; }

        Task InitSource();
        void Transform();
    }
}
