using System;

namespace Teuria;

[Obsolete("Everything in the SpatialHash is unfinished")]
public interface IHashable 
{
    (CellIndex, CellIndex)? RegisteredHashBounds { get; set; }
    int QueryId { get; set; }
}
