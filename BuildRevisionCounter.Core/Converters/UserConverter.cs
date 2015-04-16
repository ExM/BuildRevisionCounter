using AutoMapper;
using BuildRevisionCounter.Core.DomainObjects;

namespace BuildRevisionCounter.Core.Converters
{
    public static class UserConverter
    {
        static UserConverter()
        {
            Mapper.CreateMap<User, Contract.User>();
        }

        public static Contract.User ToContract(this User user)
        {
            var userContract = new Contract.User();

            Mapper.Map(user, userContract);

            return userContract;
        }
    }
}