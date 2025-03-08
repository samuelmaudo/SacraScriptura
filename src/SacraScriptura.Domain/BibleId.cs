using SacraScriptura.Domain.Common;

namespace SacraScriptura.Domain;

public sealed class BibleId : EntityId
{
    public BibleId()
    {
    }

    public BibleId(string value) : base(value)
    {
    }

    public BibleId(DateTimeOffset dateTimeOffset) : base(dateTimeOffset)
    {
    }
}