using BudgetPlanner.DAL;
using BudgetPlanner.DAL.ModelData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetPlan.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult BudgetPlan()
        {
            ViewBag.Message = "Budget Planner";
            ViewBag.FinancialYears = new SelectList(BudgetDataService.FyscalYears);
            ViewBag.Organization = new SelectList(BudgetDataService.Ogranizations, "OrgId", "OrgName");

            return View();
        }

        [HttpPost]
        public JsonResult GetBudgetPlanner(string fyscalYear, int orgId)
        {
            var expenses = BudgetDataService.Expenses.Where(x => x.OrgId == orgId && x.FyscalYear == fyscalYear);
            var budgetStatus = BudgetDataService.BudgetStatuses.FirstOrDefault(x => x.OrgId == orgId && x.FyscalYear == fyscalYear);
            var expenditureCols = new List<string>{ "ExpenseId", "Expenditure"};
            var fyMonths = BudgetDataService.GetFyscalMonths(BudgetDataService.Ogranizations.First(x => x.OrgId == orgId).FyscalMonth);
            expenditureCols.AddRange(fyMonths.Values.Select(s => s.ToString()));

            var expenditureDataLst = new List<Dictionary<string, object>>();
            foreach (var exp in expenses)
            {
                var store = new Dictionary<string, object>();
                foreach (var col in expenditureCols)
                {
                    if (col == "ExpenseId")
                    {
                        store.Add(col, exp.ExpenseId);
                    }
                    else if (col == "Expenditure")
                    {
                        store.Add(col, exp.Expenditure);
                    }
                    else
                    {
                        if (Enum.TryParse<Month>(col, out Month colMnth))
                        {
                            var budgetPlan = exp.BudgetPlan.FirstOrDefault(x => x.Month == colMnth);
                            store.Add(col, (budgetPlan == null ? string.Empty : budgetPlan.Value.ToString()));
                        }
                    }
                }

                expenditureDataLst.Add(store);
            }
            var planStatus = expenditureDataLst.Any() ? (budgetStatus ==null ? PlannerStatus.Draft.ToString() : budgetStatus.PlannerStatus.ToString()) : PlannerStatus.New.ToString();
            if (planStatus == "New")
            {
                expenditureDataLst.Add(expenditureCols.ToDictionary(k => k, v => (object)string.Empty));
            }

            var retData = new
            {
                plannerStatus = planStatus,
                columns = expenditureCols,
                budget = expenditureDataLst.ToArray()
            };

            return Json(retData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveBudgetPlan(string fyscalYear, int orgId, int status, string data)
        {
            var updatedData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data);

            var budgetStatus = BudgetDataService.BudgetStatuses.FirstOrDefault(x => x.OrgId == orgId && x.FyscalYear == fyscalYear);
            if (budgetStatus == null)
            {
                int bId = BudgetDataService.BudgetStatuses.Max(x => x.BudgetStatusId) + 1;
                BudgetDataService.BudgetStatuses.Add(new BudgetStatus
                {
                    BudgetStatusId = bId,
                    FyscalYear = fyscalYear,
                    OrgId = orgId,
                    PlannerStatus = (PlannerStatus)status
                });
            }
            else
            {
                budgetStatus.PlannerStatus = (PlannerStatus)status;
            }

            var expenses = BudgetDataService.Expenses.Where(x => x.OrgId == orgId && x.FyscalYear == fyscalYear);
            if (expenses.Count() == 0)
            {
                int exId = BudgetDataService.Expenses.Max(x => x.ExpenseId) + 1;
                foreach (var exp in updatedData)
                {
                    int buId = 1;
                    var newExp = new Expense();
                    newExp.ExpenseId = exId;
                    newExp.FyscalYear = fyscalYear;
                    newExp.OrgId = orgId;
                    newExp.BudgetPlan = new List<Budget>();
                    foreach (var item in exp)
                    {
                        if (item.Key == "ExpenseId")
                        {
                            continue;
                        }
                        else if (item.Key == "Expenditure")
                        {
                            newExp.Expenditure = item.Value.ToString();
                        }
                        else if(!string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            newExp.BudgetPlan.Add(new Budget
                            {
                                BudgetId = buId,
                                Month = (Month)Enum.Parse(typeof(Month), item.Key, true),
                                Value = Convert.ToDecimal(item.Value)
                            });
                            buId++;
                        }
                    }
                    BudgetDataService.Expenses.Add(newExp);
                    exId++;
                }
            }

            ViewBag.Message = "Budget Planner";
            ViewBag.FinancialYears = new SelectList(BudgetDataService.FyscalYears);
            ViewBag.Organization = new SelectList(BudgetDataService.Ogranizations, "OrgId", "OrgName");

            return View("BudgetPlan");
        }
    }
}
