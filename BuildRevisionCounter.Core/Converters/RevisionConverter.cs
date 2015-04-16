using AutoMapper;
using BuildRevisionCounter.Core.DomainObjects;

namespace BuildRevisionCounter.Core.Converters
{
    public static class RevisionConverter
    {
        static RevisionConverter()
        {
            Mapper.CreateMap<Revision, Contract.Revision>();
        }

        public static Contract.Revision ToContract(this Revision revision)
        {
            var revisionContract = new Contract.Revision();

            Mapper.Map(revision, revisionContract);

            return revisionContract;
        }
    }
}