using Easydocs.Robo.Robinson.CNH.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Easydocs.Robo.Robinson.CNH.Application.UseCases.Queries.Occurrences
{
    public interface IFindInvoiceQuerie
    {
        Task<IEnumerable<Domain.Entities.romaneio>> FindInvoicesPending();
    }
}
