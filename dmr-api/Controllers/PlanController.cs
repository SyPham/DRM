using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DMR_API.Helpers;
using DMR_API._Services.Interface;
using DMR_API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Tls;

namespace DMR_API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;
        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }
        [HttpPost]
        public async Task<IActionResult> GetBPFCByGlue([FromBody]TooltipParams tooltip)
        {
            var plans = await _planService.GetBPFCByGlue(tooltip);
            return Ok(plans);
        }
        [HttpGet("{ingredientName}/{batch}")]
        public async Task<IActionResult> TroubleShootingSearch(string ingredientName, string batch)
        {
            return Ok(await _planService.TroubleShootingSearch(ingredientName, batch));
        }
        [HttpGet]
        public async Task<IActionResult> GetPlans([FromQuery] PaginationParams param)
        {
            var plans = await _planService.GetWithPaginations(param);
            Response.AddPagination(plans.CurrentPage, plans.PageSize, plans.TotalCount, plans.TotalPages);
            return Ok(plans);
        }
        [HttpGet("{buildingID}")]
        public async Task<IActionResult> GetGlueByBuilding(int buildingID)
        {
            var plans = await _planService.GetGlueByBuilding(buildingID);
            return Ok(plans);
        }
        [HttpGet("{buildingID}/{modelName}")]
        public async Task<IActionResult> GetGlueByBuildingModelName(int buildingID, int modelName)
        {
            var plans = await _planService.GetGlueByBuildingModelName(buildingID, modelName);
            return Ok(plans);
        }

        [HttpGet("{IngredientID}")]
        public async Task<IActionResult> GetBatchByIngredientID(int IngredientID)
        {
            var batchs = await _planService.GetBatchByIngredientID(IngredientID);
            return Ok(batchs);
        }
        [HttpGet(Name = "GetPlans")]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _planService.GetAllAsync();
            return Ok(plans);
        }
        [HttpGet("{from}/{to}")]
        public async Task<IActionResult> GetAllPlansByDate(string from, string to)
        {
            var plans = await _planService.GetAllPlansByDate(from, to);
            return Ok(plans);
        }
        //[HttpGet("{modeNameID}")]
        //public async Task<IActionResult> GetPlanByModelNameID(int modeNameID)
        //{
        //    var lists = await _planService.GetPlanByModelNameID(modeNameID);
        //    return Ok(lists);
        //}
        [HttpGet("{text}")]
        public async Task<IActionResult> Search([FromQuery] PaginationParams param, string text)
        {
            var lists = await _planService.Search(param, text);
            Response.AddPagination(lists.CurrentPage, lists.PageSize, lists.TotalCount, lists.TotalPages);
            return Ok(lists);
        }
        [HttpGet("{buildingID}")]
        public async Task<IActionResult> GetLines(int buildingID)
        {
            var lists = await _planService.GetLines(buildingID);
            return Ok(lists);
        }
       
        [HttpPost]
        public async Task<IActionResult> Create(PlanDto create)
        {

            if (_planService.GetById(create.ID) != null)
                return BadRequest("Plan ID already exists!");
            create.CreatedDate = DateTime.Now;
            return Ok(await _planService.Add(create));
        }

        [HttpPut]
        public async Task<IActionResult> Update(PlanDto update)
        {
            if (await _planService.Update(update))
                return NoContent();
            return BadRequest($"Updating model no {update.ID} failed on save");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _planService.Delete(id))
                return NoContent();
            throw new Exception("Error deleting the model no");
        }
        [HttpGet("{buildingID}")]
        public async Task<IActionResult> Summary(int buildingID)
        {
            var model = await _planService.Summary(buildingID);
            return Ok(model);
        }
        [HttpPost]
        public async Task<IActionResult> ClonePlan(List<PlanForCloneDto> create)
        {
            return Ok(await _planService.ClonePlan(create));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRange(List<int> delete)
        {
            return Ok(await _planService.DeleteRange(delete));
        }
        [HttpPost]
        public async Task<IActionResult> DispatchGlue(BuildingGlueForCreateDto create)
        {
            return Ok(await _planService.DispatchGlue(create));
        }
        [HttpGet("{min}/{max}")]
        public async Task<IActionResult> Search(DateTime min, DateTime max)
        {
            var lists = await _planService.GetAllPlanByRange(min, max);
            return Ok(lists);
        }
        
         [HttpGet("{id}/{qty}")]
        public async Task<IActionResult> EditDelivered(int id, string qty)
        {
            var lists = await _planService.EditDelivered(id, qty);
            return Ok(lists);
        }

        [HttpGet("{id}/{qty}")]
        public async Task<IActionResult> EditQuantity(int id, int qty)
        {
            var lists = await _planService.EditQuantity(id, qty);
            return Ok(lists);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelivered(int id)
        {
            var lists = await _planService.DeleteDelivered(id);
            return Ok(lists);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllPlanByDefaultRange()
        {
            var lists = await _planService.GetAllPlanByDefaultRange();
            return Ok(lists);
        }
        [HttpGet("{startDate}/{endDate}")]
        public async Task<IActionResult> Report(DateTime startDate, DateTime endDate)
        {
            var bin = await _planService.Report(startDate, endDate);
            return File(bin, "application/octet-stream", "report.xlsx");
        }
        [HttpPost]
        public async Task<IActionResult> GetReport(GetReportDto getReportDto)
        {
            var delta = getReportDto.EndDate - getReportDto.StartDate;
            var str = Math.Abs(delta.TotalDays);
            if (str > 31)
            {
                return NotFound();
            } else
            {
                var bin = await _planService.Report(getReportDto.StartDate, getReportDto.EndDate);
                return File(bin, "application/octet-stream", "report.xlsx");
            }
        }
    }
}