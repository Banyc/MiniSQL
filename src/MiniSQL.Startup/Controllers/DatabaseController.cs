
using System.Collections.Generic;
using System.IO;
using MiniSQL.BufferManager.Controllers;
using MiniSQL.Library.Interfaces;
using MiniSQL.Library.Models;

namespace MiniSQL.Startup.Controllers
{
    public class DatabaseController
    {
        private readonly ApiPagerBuilder _builder;

        private IApi _api;
        private Pager _pager;

        public bool IsUsingDatabase { get; private set; } = false;
        private string nameOfDatabaseInUse;

        public DatabaseController(ApiPagerBuilder builder)
        {
            _builder = builder;
        }

        // use database
        public void ChangeContext(string newDatabaseName)
        {
            if (IsUsingDatabase)
            {
                _pager.Close();
            }
            // init
            this.IsUsingDatabase = true;
            this.nameOfDatabaseInUse = newDatabaseName;
            (_api, _pager) = _builder.UseDatabase(newDatabaseName);
        }

        // delete database file
        public void DropDatabase(string databaseName)
        {
            if (nameOfDatabaseInUse == databaseName)
            {
                _pager.Close();
                IsUsingDatabase = false;
            }
            File.Delete($"{databaseName}.minidb");
            File.Delete($"{databaseName}.indices.dbcatalog");
            File.Delete($"{databaseName}.tables.dbcatalog");
        }

        public void ClosePager()
        {
            _pager.Close();
        }

        public void FlushPages()
        {
            _pager.CleanAllPagesFromMainMemory();
        }

        public List<SelectResult> Query(string input)
        {
            List<SelectResult> selectResults = _api.Query(input.ToString());
            return selectResults;
        }
    }
}
