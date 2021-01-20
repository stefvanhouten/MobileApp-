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

        /// <summary>
        /// Returns all MoistMeter items from the database
        /// </summary>
        /// <returns>List of MoistMeter objects</returns>
        public Task<List<MoistMeter>> GetItemsAsync()
        {
            return Database.Table<MoistMeter>().ToListAsync();
        }

        /// <summary>
        /// Clear the database
        /// </summary>
        /// <returns></returns>
        public Task<int> EmptyDatabase()
        {
            return Database.DeleteAllAsync<MoistMeter>();
        }

        /// <summary>
        /// Retreives a single MoistMeter item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>MoistMeter object</returns>
        public Task<MoistMeter> GetItemAsync(int id)
        {
            return Database.Table<MoistMeter>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Save MoistMeter object to the database
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete MoistMeter item from database
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Boolean based on success</returns>
        public Task<int> DeleteItemAsync(MoistMeter item)
        {
            return Database.DeleteAsync(item);
        }

        /// <summary>
        /// Delete MoistMeter object by ID from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Boolean based on success</returns>
        public Task<int> DeleteItemByIDAsync(int id)
        {
            return Database.DeleteAsync(id);
        }

        /// <summary>
        /// Retrieves MoistMeter object from database by id
        /// </summary>
        /// <param name="topicName"></param>
        /// <returns></returns>
        public Task<List<MoistMeter>> GetItemByTopicName(string topicName)
        {
            return Database.QueryAsync<MoistMeter>("SELECT * FROM [MoistMeter] WHERE [Topic] = ? ORDER BY [ID] DESC LIMIT 5", topicName);
        }
    }
}
