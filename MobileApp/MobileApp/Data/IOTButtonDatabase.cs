using MobileApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileApp.Data
{
    public class IOTButtonDatabase
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public IOTButtonDatabase()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(IOTButton).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(IOTButton)).ConfigureAwait(false);
                }
                initialized = true;
            }
        }

        public Task<List<IOTButton>> GetItemsAsync()
        {
            return Database.Table<IOTButton>().ToListAsync();
        }

        public Task<int> EmptyDatabase()
        {
            return Database.DeleteAllAsync<IOTButton>();
        }

        public Task<IOTButton> GetItemAsync(int id)
        {
            return Database.Table<IOTButton>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(IOTButton item)
        {
            if (item.ID != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(IOTButton item)
        {
            return Database.DeleteAsync(item);
        }

        public Task<int> DeleteButtonByID(int id)
        {
            return Database.DeleteAsync(id);
        }
    }
}
