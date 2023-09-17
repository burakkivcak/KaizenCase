using KaizenCase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KaizenCase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesSlipCaseController : Controller
    {
        [HttpPost]
        [Route("SalesSlip")]
        public IActionResult Create([FromBody] List<ResponseJson> responseJsons)
        {
            try
            {
                List<SlipSummary> slipSummaries = new List<SlipSummary>();
                List<WorkflowSlipSummary> workflowSlipSummaries = new List<WorkflowSlipSummary>();
                List<WorkflowSlipSummary> combineProcessSlipSummaries = new List<WorkflowSlipSummary>();
                workflowSlipSummaries.Add(new WorkflowSlipSummary()
                {
                    Id = 1,
                    Line = 0,
                    Text = responseJsons[1].Description,
                    BoundingPoly = responseJsons[1].BoundingPoly
                });
                int pixelGapValue = 10;
                
                for (int i = 2; i < responseJsons.Count; i++)
                {
                    int processId = 1;
                    bool isProcessComplete = false;
                    foreach (var item in workflowSlipSummaries.ToList())
                    {
                        if(item.BoundingPoly.Vertices[0].y + 10 > responseJsons[i].BoundingPoly.Vertices[0].y && item.BoundingPoly.Vertices[0].y - 10 < responseJsons[i].BoundingPoly.Vertices[0].y)
                        {
                            isProcessComplete=true;
                            workflowSlipSummaries.Add(new WorkflowSlipSummary()
                            {
                                Id = item.Id,
                                Line = 0,
                                Text = responseJsons[i].Description,
                                BoundingPoly = responseJsons[i].BoundingPoly
                            });
                            break;
                        }
                        else
                        {
                            processId++;
                        }
                    }
                    if(!isProcessComplete)
                    {
                        workflowSlipSummaries.Add(new WorkflowSlipSummary()
                        {
                            Id = processId,
                            Line = 0,
                            Text = responseJsons[i].Description,
                            BoundingPoly = responseJsons[i].BoundingPoly
                        }); 
                    }
                    else
                    {
                        continue;
                    }
                }
                foreach (var rowId in workflowSlipSummaries.Select(x=>x.Id).Distinct().ToList())
                {
                    var sameRowSlipList = workflowSlipSummaries.Where(x => x.Id == rowId).OrderBy(x => x.BoundingPoly.Vertices[0].x).ToList();
                    combineProcessSlipSummaries.Add(new WorkflowSlipSummary()
                    {
                        Id = 0,
                        Line = 0,
                        Text = string.Join(" " , sameRowSlipList.Select(x=>x.Text).ToArray()),
                        BoundingPoly = sameRowSlipList.FirstOrDefault().BoundingPoly
                    });
                }
                int lineId = 1;
                foreach (var item in combineProcessSlipSummaries.OrderBy(x => x.BoundingPoly.Vertices[0].y))
                {
                    slipSummaries.Add(new SlipSummary()
                    {
                        Line=lineId,
                        Text=item.Text,
                    });
                    lineId++;
                }
                return Ok(slipSummaries);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
