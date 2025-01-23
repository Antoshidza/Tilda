using System;

namespace Cmd
{
    public class CmdException : Exception
    {
        public CmdException(string msg) : base(msg) { }
    }
}