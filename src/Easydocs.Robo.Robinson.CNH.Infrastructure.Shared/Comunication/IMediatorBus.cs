﻿using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.DomainObjects;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Comunication
{
    public interface IMediatorBus
    {
        Task PublishEventAsync<T>(T @event) where T : Event;
        Task<ResultCommand> SendCommadAsync<T>(T command) where T : Command;
        Task<ResultCommand<T2>> SendCommadAsync<T, T2>(T command) where T : Command<T2>;
    }
}
