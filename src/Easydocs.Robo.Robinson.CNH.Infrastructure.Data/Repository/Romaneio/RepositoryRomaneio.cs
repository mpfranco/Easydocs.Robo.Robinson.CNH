using Microsoft.Extensions.Configuration;
using Easydocs.Robo.Robinson.CNH.Domain.Entities;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Data.Repository.Base;
using System.Threading.Tasks;
using Easydocs.Robo.Robinson.CNH.Domain.Interfaces.IRepository.Invoices;
using System.Collections.Generic;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Settings;
using System;

namespace Easydocs.Robo.Robinson.CNH.Infrastructure.Data.Repository.Invoices
{

    public class RepositoryRomaneio : RepositoryBase<romaneio>, IFindInvoice
    {
        private readonly RoboRobinsonSettings _roboVazFielSettings;        
        public RepositoryRomaneio(IConfiguration configuration
                                , RoboRobinsonSettings roboVazFielSettings) : base(configuration)
        {
            _roboVazFielSettings = roboVazFielSettings;
        }
        public async Task<romaneio> SaveAsync(romaneio entity)
        {
            var id = await base.SaveAsync<long, romaneio>(entity);
            entity.AssociateId(id);
            return entity;
        }

        public async Task<romaneio> UpdateAsync(romaneio entity)
        {
            var id = await  base.UpdateAsync(entity);            
            return entity;
        }

        public async Task<IEnumerable<romaneio>> FindInvoicesPending()
        {
            var result = await base.QueryAsync<romaneio>("");//_roboVazFielSettings.queryFindInvoicesPending);
            return result;
        }

        public async Task<int> UpdateDateByIdAsync(DateTime date, long id, int Nr_Paginas, string Download)
        {

            //var result = await ExecuteUpdateAsync(_roboVazFielSettings.queryUpdateInvoice,new {id, date, Nr_Paginas, Download});
            //var result = await ExecuteUpdateAsync(""_roboVazFielSettings.queryUpdateInvoice.Replace("@date", date.ToString("dd/MM/yyyy")), new { id });
            //result += await ExecuteUpdateAsync(_roboVazFielSettings.queryUpdateInvoice_2.Replace("@date", date.ToString("dd/MM/yyyy")), new { id });
            //return result;
            return 0;
        }
    }
}
