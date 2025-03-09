using SacraScriptura.Domain.Common;

namespace SacraScriptura.Domain.Divisions;

public sealed class DivisionId : EntityId
{
    public DivisionId()
    {
    }

    public DivisionId(string value) : base(value)
    {
    }

    public DivisionId(DateTimeOffset dateTimeOffset) : base(dateTimeOffset)
    {
    }
}
