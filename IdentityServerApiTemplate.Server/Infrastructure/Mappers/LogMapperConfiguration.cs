using Calabonga.UnitOfWork;
using IdentityServerApiTemplate.Entities;
using IdentityServerApiTemplate.Server.Infrastructure.Mappers.Base;
using IdentityServerApiTemplate.Server.ViewModels.LogViewModels;

namespace IdentityServerApiTemplate.Server.Infrastructure.Mappers
{
    /// <summary>
    /// Mapper Configuration for entity Log
    /// </summary>
    public class LogMapperConfiguration : MapperConfigurationBase
    {
        /// <inheritdoc />
        public LogMapperConfiguration()
        {
            CreateMap<LogCreateViewModel, Log>()
                .ForMember(x => x.Id, o => o.Ignore());

            CreateMap<Log, LogViewModel>();

            CreateMap<Log, LogUpdateViewModel>();

            CreateMap<LogUpdateViewModel, Log>()
                .ForMember(x => x.CreatedAt, o => o.Ignore())
                .ForMember(x => x.ThreadId, o => o.Ignore())
                .ForMember(x => x.ExceptionMessage, o => o.Ignore());

            CreateMap<IPagedList<Log>, IPagedList<LogViewModel>>()
                .ConvertUsing<PagedListConverter<Log, LogViewModel>>();
        }
    }
}
