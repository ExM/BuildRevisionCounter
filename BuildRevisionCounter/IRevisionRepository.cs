using BuildRevisionCounter.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildRevisionCounter.Exceptions;

namespace BuildRevisionCounter
{
	/// <summary>
	/// Интерфей репозитория ревизий.
	/// </summary>
	public interface IRevisionRepository
	{
		/// <summary>
		/// Возвращает все ривизии постранично.
		/// </summary>
		/// <param name="pageSize">Размер страницы.</param>
		/// <param name="pageNumber">Номер страницы.</param>
		/// <returns>Таск с результатом коллекцией ревизий на странице.</returns>
		Task<IReadOnlyCollection<Revision>> GetAllRevision(int pageSize = 20, int pageNumber = 1);

		/// <summary>
		/// Возвращает текущий номер ревизии.
		/// </summary>
		/// <param name="revisionName">Идентификатор ревизии.</param>
		/// <returns>Текущий номер ревизии.</returns>
		/// <exception cref="RevisionNotFoundException">Генирируется в случае, если ревизия не была найдена.</exception>
		Task<long> Current(string revisionName);

		/// <summary>
		/// Обновляет ревизию, или если ее не было создает новую.
		/// </summary>
		/// <param name="revisionName">Идентификатор ревизии.</param>
		/// <returns>Новый текущий номер ревизии.</returns>		
		Task<long> Bumping(string revisionName);
	}
}
