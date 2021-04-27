using IdentityServerApiTemplate.Data;
using IdentityServerApiTemplate.Server.Infrastructure.Mappers.Base;
using IdentityServerApiTemplate.Server.ViewModels.AccountViewModels;

namespace IdentityServerApiTemplate.Server.Infrastructure.Mappers
{
    /// <summary>
    /// Mapper Configuration for entity Person
    /// </summary>
    public class ApplicationUserProfileMapperConfiguration : MapperConfigurationBase
    {
        /// <inheritdoc />
        public ApplicationUserProfileMapperConfiguration() => CreateMap<RegisterViewModel, ApplicationUserProfile>()
            .ForAllOtherMembers(x => x.Ignore());
    }
}