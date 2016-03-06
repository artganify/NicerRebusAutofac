using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Registration;
using Rebus.Config;

namespace Rebus.Autofac
{

    /// <summary>
    ///     Extensions for <see cref="ContainerBuilder"/>
    /// </summary>
    public static class AutofacRebusModuleExtensions
    {

        public static IModuleRegistrar UseRebus(this ContainerBuilder containerBuilder,
            Action<RebusConfigurer> rebusBuilder)
        {
            if(containerBuilder == null)
                throw new ArgumentNullException(nameof(containerBuilder));

            return containerBuilder.RegisterModule(new AutofacRebusModule(rebusBuilder));
        }

    }
}
