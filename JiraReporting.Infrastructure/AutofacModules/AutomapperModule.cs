using Autofac;
using AutoMapper;
using JiraReporting.Infrastructure.AutomapperProfiles;

namespace JiraReporting.Infrastructure.AutofacModules
{
    /// <summary>
    /// Autofac module for automapper
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class AutomapperModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be
        /// registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            //register your configuration as a single instance
            builder.Register(c => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            })).AsSelf().SingleInstance();

            //register your mapper
            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve)).As<IMapper>().InstancePerLifetimeScope();
        }
    }
}