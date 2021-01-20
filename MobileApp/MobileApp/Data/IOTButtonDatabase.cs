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

        /// <summary>
        /// Returns all IOTButton items from the database
        /// </summary>
        /// <returns></returns>
        public Task<List<IOTButton>> GetItemsAsync()
        {
            return Database.Table<IOTButton>().ToListAsync();
        }

        /// <summary>
        /// Clear the database
        /// </summary>
        /// <returns></returns>
        public Task<int> EmptyDatabase()
        {
            return Database.DeleteAllAsync<IOTButton>();
        }

        /// <summary>
        /// Retreives a single IOTButton item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IOTButton object</returns>
        public Task<IOTButton> GetItemAsync(int id)
        {
            return Database.Table<IOTButton>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Save IOTButton object to the database
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete IOTButton item from database
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Boolean based on success</returns>
        public Task<int> DeleteItemAsync(IOTButton item)
        {
            return Database.DeleteAsync(item);
        }

        /// <summary>
        /// Delete IOTButton object by ID from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Boolean based on success</returns>
        public Task<int> DeleteButtonByID(int id)
        {
            return Database.DeleteAsync(id);
        }
    }
}
