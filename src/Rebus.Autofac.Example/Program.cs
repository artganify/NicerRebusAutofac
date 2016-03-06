using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Rebus.Bus;

namespace Rebus.Autofac.Example
{
    class Program
    {
        static void Main(string[] args)
        {

            var containerBuilder = new ContainerBuilder();

            // hookup rebus
            containerBuilder.UseRebus(rebus =>
            {
                rebus.Options(options => options.LogPipeline());
            });

            // and build the container
            using (var container = containerBuilder.Build())
            {
                var bus = container.Resolve<IBus>();
                bus.Publish(new object());

                // when the container is being disposed, rebus is too :)
            }

        }
    }
}
