using BudgetPlanner.DAL;
using BudgetPlanner.DAL.ModelData;
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

            var retData = new
            {
                plannerStatus = expenditureDataLst.Any() ? budgetStatus.PlannerStatus.ToString() : PlannerStatus.New.ToString(),
                columns = expenditureCols,
                budget = expenditureDataLst.ToArray()
            };

            return Json(retData, JsonRequestBehavior.AllowGet);
        }
    }
}