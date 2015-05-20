using BuildRevisionCounter.Model.BuildRevisionStorage;
using MongoDB.Driver;

namespace BuildRevisionCounter.Model.Interfaces
{
	/// <summary>
	/// Интерфейс для получения данных из БД Монго.
	/// </summary>
	public interface IMongoDBStorage
	{
		/// <summary>
		/// MongoDB-коллекция ревизий.
		/// </summary>
		IMongoCollection<RevisionModel> Revisions { get; }

		/// <summary>
		/// MongoDB-коллекция пользователей.
		/// </summary>
		IMongoCollection<UserModel> Users { get; }
	}
}