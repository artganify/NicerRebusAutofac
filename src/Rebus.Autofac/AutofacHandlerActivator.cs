using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Rebus.Activation;
using Rebus.Extensions;
using Rebus.Handlers;
using Rebus.Transport;

namespace Rebus.Autofac
{

    /// <summary>
    ///     Implementation of a <see cref="IHandlerActivator"/> which resolves <see cref="IHandleMessages{TMessage}">message handlers</see>
    ///     through a provided <see cref="ILifetimeScope">Autofac lifetime scope</see>
    /// </summary>
    internal class AutofacHandlerActivator : IHandlerActivator
    {

        private readonly ILifetimeScope _lifetimeScope;

        /// <summary>
        ///     Creates a new <see cref="AutofacHandlerActivator"/> with the provided <see cref="ILifetimeScope"/>
        /// </summary>
        public AutofacHandlerActivator(ILifetimeScope lifetimeScope)
        {
            if(lifetimeScope == null)
                throw new ArgumentNullException(nameof(lifetimeScope));
            _lifetimeScope = lifetimeScope;
        }

        /// <summary>
        ///     Must return all relevant handler instances for the given message
        /// </summary>
        public async Task<IEnumerable<IHandleMessages<TMessage>>> GetHandlers<TMessage>(TMessage message, ITransactionContext transactionContext)
        {
            var nestedLifetimeScope = transactionContext
               .GetOrAdd("current-autofac-lifetime-scope", () =>
               {
                   var scope = _lifetimeScope.BeginLifetimeScope();

                   transactionContext.OnDisposed(() => scope.Dispose());

                   return scope;
               });

            var handledMessageTypes = typeof(TMessage).GetBaseTypes()
                .Concat(new[] { typeof(TMessage) });

            return handledMessageTypes
                .SelectMany(handledMessageType =>
                {
                    var implementedInterface = typeof(IHandleMessages<>).MakeGenericType(handledMessageType);
                    var implementedInterfaceSequence = typeof(IEnumerable<>).MakeGenericType(implementedInterface);

                    return (IEnumerable<IHandleMessages>)nestedLifetimeScope.Resolve(implementedInterfaceSequence);
                })
                .Cast<IHandleMessages<TMessage>>();
        }

    }
}
