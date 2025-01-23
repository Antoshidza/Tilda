using System;

namespace Tilda
{
    internal class CmdException : Exception
    {
        public CmdException(string msg) : base(msg) { }
    }
}