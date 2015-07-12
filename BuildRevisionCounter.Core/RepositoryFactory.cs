using System;
using BuildRevisionCounter.Core.Converters.Impl;
using BuildRevisionCounter.Core.Repositories;
using BuildRevisionCounter.Core.Repositories.Impl;

namespace BuildRevisionCounter.Core
{
	public class RepositoryFactory
	{
		private static readonly Object SLock = new Object();
		private static volatile RepositoryFactory _instance = null;

		private RepositoryFactory()
		{ }

		public static RepositoryFactory Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (SLock)
					{
						if (_instance == null)
							_instance = new RepositoryFactory();
					}
				}
				return _instance;
			}
		}

		public IRevisionRepository GetRevisionRepository()
		{
			return new RevisionRepository(new RevisionConverter());
		}

		public IUserRepository GetUserRepository()
		{
			return new UserRepository(new UserConverter());
		}

		public IRepository GetRepository()
		{
			return MongoContext.Instance;
		}
	}
}