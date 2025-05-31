using SacraScriptura.Shared.Domain;

namespace SacraScriptura.Admin.Domain.Divisions;

public sealed class DivisionId : EntityId
{
    public DivisionId() { }

    public DivisionId(string value)
        : base(value) { }

    public DivisionId(DateTimeOffset dateTimeOffset)
        : base(dateTimeOffset) { }
}
