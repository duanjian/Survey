using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using DFHE.Survey.API;
using DFHE.Survey.DAL;
using DFHE.Survey.Model;


namespace LoginDemo.API
{
    public class AutofacConfig
    {
        public static void AutofacConfiguration()
        {
            var container = AutofacContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

        }

        public static IContainer AutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Entities>().As(typeof(DbContext)).InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().AsImplementedInterfaces().InstancePerLifetimeScope();

            // 注册ApiController
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly())
                .Where(t => !t.IsAbstract && typeof(ApiController).IsAssignableFrom(t));
            // BPL的注册放这里
            builder.RegisterAssemblyTypes(Assembly.Load("DFHE.Survey.BLL"))
                      .Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerLifetimeScope();

            // DAL的注册放这里
            builder.RegisterAssemblyTypes(Assembly.Load("DFHE.Survey.DAL"))
                      .Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerLifetimeScope();


         
            IContainer container = builder.Build();
            SecurityFactory.InitSecurityFactory(container.Resolve<IUserInfoRepository>());

            return container;
        }
    }
}