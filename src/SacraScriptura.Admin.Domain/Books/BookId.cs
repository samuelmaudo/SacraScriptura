using SacraScriptura.Shared.Domain;

namespace SacraScriptura.Admin.Domain.Books;

public sealed class BookId : EntityId
{
    public BookId() { }

    public BookId(string value)
        : base(value) { }

    public BookId(DateTimeOffset dateTimeOffset)
        : base(dateTimeOffset) { }
}
