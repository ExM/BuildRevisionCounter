using System;
using BuildRevisionCounter.DAL.Repositories;

namespace BuildRevisionCounter.Tests
{
    public static class RepositoriesFactory
    {
        private static readonly Lazy<UserRepository> _userRepository =
            new Lazy<UserRepository>();

        private static readonly Lazy<RevisionRepository> _revisionRepository =
            new Lazy<RevisionRepository>();

        public static UserRepository UserRepoInstance { get { return _userRepository.Value; } }

        public static RevisionRepository RevisionRepoInstance { get { return _revisionRepository.Value; } }

        
    }
}