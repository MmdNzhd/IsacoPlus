using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Services
{
    public interface ISmsSender
    {
        Task<string> SendWithPattern(string phone, string patternCode, string data);
    }
}
