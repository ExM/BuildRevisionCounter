using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildRevisionCounter.Model;
using MongoDB.Driver;

namespace BuildRevisionCounter.Interfaces
{
	/// <summary>
	/// Интерфейс для получения данных из БД.
	/// </summary>
	public interface IDataProvider
	{
	    string AdminName { get; }

	    Task<IReadOnlyCollection<RevisionModel>> GetAllRevision(Int32 pageSize, Int32 pageNumber);

	    Task<long?> CurrentRevision(string revisionName);

	    Task RevisionInsertAsync(string revisionName);

	    Task<RevisionModel> FindOneAndUpdateRevisionModelAsync(string revisionName);

	    Task SetUp();

	    Task DropDatabaseAsync();

	    Task<UserModel> FindUser(string name);



	    Task CreateUser(string name, string password, string[] roles);

	    Task EnsureAdminUser();
    }
}