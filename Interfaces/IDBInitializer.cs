using TimelyTastes.Data;

namespace TimelyTastes.Interfaces
{
    public interface IDBInitializer
    {
        void Initialize(SQLiteDbContext context);
    }
}