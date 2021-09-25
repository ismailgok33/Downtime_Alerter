using AutoMapper;
using DowntimeAlerter.Application.Interfaces;
using DowntimeAlerter.Application.Models.Target;
using DowntimeAlerter.Domain.Entities;
using DowntimeAlerter.EntityFrameworkCore.TargetDb;
using DowntimeAlertereAlerter.Application.Exceptions;
using Hangfire;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DowntimeAlerter.Application.Services
{
    public class TargetService : ITargetService
    {
        private readonly TargetDbContext _context;
        private readonly IMapper _mapper;
        private readonly IRecurringJobManager _jobManager;
        private readonly IHealthCheckService _healthCheckService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TargetService(TargetDbContext context, IMapper mapper, IRecurringJobManager jobManager, 
            IHealthCheckService healthCheckService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _jobManager = jobManager;
            _healthCheckService = healthCheckService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserName()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task RunHealthCheck(Guid id, string userMail)
        {
            await _healthCheckService.RunHealthCheck(id, userMail);
        }

        private void AddOrUpdateHealthCheckJob(Guid id, int intervalInMinutes, string userMail)
        {
            var job = Hangfire.Common.Job.FromExpression(() => RunHealthCheck(id, userMail));
            _jobManager.AddOrUpdate(id.ToString(), job, cronExpression: $"*/{intervalInMinutes} * * * *",
               new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc, QueueName = "default" });
        }

        private void RemoveHealthCheckJob(Guid id)
        {
            _jobManager.RemoveIfExists(id.ToString());
        }

        public List<TargetVm> GetTargets()
        {
            var targets = _context.Target.Where(x=>x.CreatedBy==GetUserName()).ToList();
            List<TargetVm> list = new List<TargetVm>();
            foreach (var target in targets)
            {
                list.Add(_mapper.Map<TargetVm>(target));
            }
            return list;
        }

            public async Task<Guid> CreateTargetAsync(CreateVm createVm)
        {         
            var target = _mapper.Map<Target>(createVm);
            target.CreationDate = DateTime.Now;
            target.CreatedBy = GetUserName();

            await _context.Target.AddAsync(target);
            await _context.SaveChangesAsync();

            if (target.Id != Guid.Empty)
            {
                AddOrUpdateHealthCheckJob(target.Id, target.MonitoringIntervalInMinutes, target.CreatedBy);
            }

            return target.Id;
        }


        private async Task<Target> GetTargetAsync(Guid id)
        {
            var target = await _context.Target.FindAsync(id);
            if (target == null)
                throw new NotFoundException(id);

            if (target.CreatedBy != GetUserName())
                throw new NotAuthorizedException(GetUserName(), id);

            return target;
        }
        

            public async Task DeleteTargetAsync(Guid id)
        {
            var target = await GetTargetAsync(id);
            _context.Target.Remove(target);
            await _context.SaveChangesAsync();
            RemoveHealthCheckJob(id);
        }

        public async Task<EditVm> GetTargetEditAsync(Guid id)
        {
            var target = await GetTargetAsync(id);
            return _mapper.Map<EditVm>(target);
        }

        public async Task EditTargetAsync(EditVm editVm)
        {
            var target = await GetTargetAsync(editVm.Id);
            target.Name = editVm.Name;
            target.Url = editVm.Url;
            target.MonitoringIntervalInMinutes= editVm.MonitoringIntervalInMinutes;
            target.LastUpdateDate = DateTime.Now;
      
            _context.Target.Update(target);
            await _context.SaveChangesAsync();

            if (editVm.Id != Guid.Empty)
            {
                AddOrUpdateHealthCheckJob(target.Id, target.MonitoringIntervalInMinutes, target.CreatedBy);
            }
        }

        public async Task<DetailVm> GetTargetDetail(Guid id)
        {
            var target = await GetTargetAsync(id);
            var checkResults = _context.HealthCheckResult.Where(x => x.TargetId==id).ToList();
            List<CheckResultVm> checkResultVmlist = new List<CheckResultVm>();
            foreach (var result in checkResults)
            {
                checkResultVmlist.Add(_mapper.Map<CheckResultVm>(result));
            }

            return new DetailVm()
            {
                Name = target.Name,
                Url = target.Url,
                MonitoringIntervalInMinutes = target.MonitoringIntervalInMinutes,
                CheckResults = checkResultVmlist
            };
        }

     
        
    }
}
