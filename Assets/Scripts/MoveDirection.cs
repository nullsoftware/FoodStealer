using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Flags]
public enum MoveDirection : byte
{
    None = 0,
    Forward = 1 << 0,
    Backward = 1 << 1,
    All = Forward | Backward,
}
