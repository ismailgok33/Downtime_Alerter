using DowntimeAlerter.Application.Models.Target;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DowntimeAlerter.Application.Interfaces
{
    public interface ITargetService
    {
        List<TargetVm> GetTargets();
        Task<EditVm> GetTargetEditAsync(Guid id);
        Task<DetailVm> GetTargetDetail(Guid id);
        Task<Guid> CreateTargetAsync(CreateVm createVm);
        Task EditTargetAsync(EditVm updateVm);
        Task DeleteTargetAsync(Guid id);
    }
}
