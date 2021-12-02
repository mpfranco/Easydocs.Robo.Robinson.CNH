using Easydocs.Robo.Robinson.CNH.Domain.Entities;
using Easydocs.Robo.Robinson.CNH.Domain.Interfaces.IRepository.Invoices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Easydocs.Robo.Robinson.CNH.Application.UseCases.Queries.Occurrences
{
    public class FindInvoiceQuerie : IFindInvoiceQuerie
    {
        private readonly IFindInvoice _repository;
        public FindInvoiceQuerie(IFindInvoice repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<romaneio>> FindInvoicesPending()
        {
            try
            {
                return await _repository.FindInvoicesPending();
            }
            catch(Exception err)
            {
                return null;
            }
            

        }
    }
}
