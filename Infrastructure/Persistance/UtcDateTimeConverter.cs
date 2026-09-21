using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence
{
    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter() : base(
            toProvider => toProvider,
            fromProvider => DateTime.SpecifyKind(fromProvider, DateTimeKind.Utc))
        { }
    }
}