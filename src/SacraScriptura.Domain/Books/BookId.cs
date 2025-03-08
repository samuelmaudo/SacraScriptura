using SacraScriptura.Domain.Common;

namespace SacraScriptura.Domain.Books;

public sealed class BookId : EntityId
{
    public BookId()
    {
    }

    public BookId(string value) : base(value)
    {
    }

    public BookId(DateTimeOffset dateTimeOffset) : base(dateTimeOffset)
    {
    }
}
