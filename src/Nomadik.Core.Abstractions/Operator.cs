namespace Nomadik.Core.Abstractions;

/// <summary>
/// Set of supported Operations for serialization of filter expressions
/// </summary>
public enum Operator 
{
    /// <summary>Equals</summary>
    EQ,

    /// <summary>Not Equals</summary>
    NE,

    /// <summary>Greater Than</summary>
    GT,

    /// <summary>Greater Than or Equals</summary>
    GTE,

    /// <summary>Less Than</summary>
    LT,

    /// <summary>Less Than or Equals</summary>
    LTE,

    /// <summary>Like</summary>
    LI,

    /// <summary>Contains</summary>
    CO,

    /// <summary>All Match</summary>
    All,

    /// <summary>Any Match</summary>
    Any
}
