using MobileApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileApp.Data
{
    public class MoistMeterDatabase
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public MoistMeterDatabase()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(MoistMeter).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(MoistMeter)).ConfigureAwait(false);
                }
                initialized = true;
            }
        }

        public Task<List<MoistMeter>> GetItemsAsync()
        {
            return Database.Table<MoistMeter>().ToListAsync();
        }

        public Task<int> EmptyDatabase()
        {
            return Database.DeleteAllAsync<MoistMeter>();
        }

        public Task<MoistMeter> GetItemAsync(int id)
        {
            return Database.Table<MoistMeter>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(MoistMeter item)
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

        public Task<int> DeleteItemAsync(MoistMeter item)
        {
            return Database.DeleteAsync(item);
        }

        public Task<int> DeleteItemByIDAsync(int id)
        {
            return Database.DeleteAsync(id);
        }

        public Task<List<MoistMeter>> GetItemByTopicName(string topicName)
        {
            return Database.QueryAsync<MoistMeter>("SELECT * FROM [MoistMeter] WHERE [Topic] = ? LIMIT 5", topicName);
        }
    }
}
