using System.Linq.Expressions;

namespace Nomadik.Core.Extensions;

/// <summary>
/// Extends <see cref="Expression"/>
/// </summary>
public static class ExpressionExtensions 
{
    /// <summary>
    /// Decomposes a <see cref="MemberInitExpression"/> based Expression
    /// into a lookup table of its individual <see cref="MemberAssignment"/>
    /// with the Keys being it's individual Member Names
    /// </summary>
    public static IReadOnlyDictionary<string, Expression> ToMemberTable<T1, T2>(
        this Expression<Func<T1, T2>> expression
    ) 
    {
        if (expression.Body is not MemberInitExpression init)
        {
            throw new NotSupportedException(
                $"Expression body root must be a MemberInitExpression (new()), was iinstead unsupported node type: '{expression.Body.NodeType}'"
            );
        }
        Dictionary<string, Expression> lookupTable = [];
        for (var b = 0; b < init.Bindings.Count; b++)
        {
            var binding = init.Bindings[b];
            if (binding is not MemberAssignment assignment)
            {
                throw new NotSupportedException(
                    $"MemberInitExpression Bindings must all be MemberAssignments. Binding [{b}] was instead unsupported node type: '{expression.Body.NodeType}'"
                );
            }

            lookupTable[assignment.Member.Name.ToLower()] = assignment.Expression;
        }

        return lookupTable;
    }
}
