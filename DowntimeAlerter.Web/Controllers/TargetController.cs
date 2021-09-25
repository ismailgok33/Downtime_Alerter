using DowntimeAlerter.Application.Interfaces;
using DowntimeAlerter.Application.Models.Target;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DowntimeAlerter.Web.Controllers
{
    [Authorize]
    public class TargetController : Controller
    {
        private readonly ITargetService _targetService;
        private readonly ILogger<TargetController> _logger;

        public TargetController(ITargetService targetService, ILogger<TargetController> logger)
        {
            _targetService = targetService;
            _logger = logger;
           
        }

        public IActionResult List()
        {
            var list=_targetService.GetTargets();
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVm createVm)
        {
            _logger.LogInformation("test");

            if (ModelState.IsValid)
            {
                await _targetService.CreateTargetAsync(createVm);
               
                return RedirectToAction(nameof(List));
            }
            return View(createVm);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            _logger.LogInformation("test");

            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var editVm = await _targetService.GetTargetEditAsync(id);
            return View(editVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditVm editVm)
        {
            if (editVm.Id == Guid.Empty)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _targetService.EditTargetAsync(editVm);
                return RedirectToAction(nameof(List));
            }
            return View(editVm);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var editVm = await _targetService.GetTargetEditAsync(id);
            return View(editVm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            await _targetService.DeleteTargetAsync(id);

            return RedirectToAction(nameof(List));
        }


        public async Task<IActionResult> Detail(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var detailVm = await _targetService.GetTargetDetail(id);
            return View(detailVm);
        }

        [Authorize(Roles ="Administrator")]
        public IActionResult Logs()
        {
            var list = _targetService.GetTargets();
            return View("List",list);
        }
    }
}
