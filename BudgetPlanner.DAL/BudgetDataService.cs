using BudgetPlanner.DAL.ModelData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetPlanner.DAL
{
    public static class BudgetDataService
    {
        private static readonly Random rnd = new Random();
        public static List<string> FyscalYears
        {
            get
            {
                var fyYrs = new List<string>
                {
                    "FY2018-19",
                    "FY2017-18",
                    "FY2016-17",
                    "FY2015-16",
                    "FY2014-15",
                    "FY2013-14",
                    "FY2012-13"
                };
                return fyYrs;
            }
        }

        public static List<Organization> Ogranizations { get; private set; }

        public static List<BudgetStatus> BudgetStatuses { get; private set; }

        public static List<Expense> Expenses { get; private set; }

        public static void SeedBudgetData()
        {
            Ogranizations = new List<Organization>();
            int fyMnth = rnd.Next(1, 13);
            int orgId = rnd.Next(1, 6);
            int status = rnd.Next(1, 4);

            for (int i = 1; i <= 5; i++)
            {
                Ogranizations.Add(new Organization
                {
                    OrgId = i,
                    OrgName = string.Format("OrgName - {0}", i),
                    FyscalMonth = (Month)fyMnth
                });
                fyMnth = rnd.Next(1, 13);
            }

            BudgetStatuses = new List<BudgetStatus>();
            for (int k = 1; k <= 10; k++)
            {
                var staus = new BudgetStatus
                {
                    BudgetStatusId = k,
                    FyscalYear = FyscalYears[orgId],
                    OrgId = orgId,
                    PlannerStatus = (PlannerStatus)status
                };
                BudgetStatuses.Add(staus);
                orgId = rnd.Next(1, 6);
                status = rnd.Next(1, 4);
            }

            Expenses = new List<Expense>();
            for (int i = 1; i <= 100; i++)
            {
                var budgetData = new List<Budget>();
                for (int j = 1; j < fyMnth; j++)
                {
                    budgetData.Add(new Budget
                    {
                        BudgetId = j,
                        Month = (Month)j,
                        Value = 100 * j
                    });
                }
                var exp = new Expense
                {
                    ExpenseId = i,
                    Expenditure = string.Format("Expenditure - {0}", i),
                    FyscalYear = FyscalYears[orgId],
                    OrgId = orgId,
                    BudgetPlan = budgetData
                };
                Expenses.Add(exp);
                fyMnth = rnd.Next(1, 13);
                orgId = rnd.Next(1, 6);
            }
        }

        public static Dictionary<int, Month> GetFyscalMonths(Month fyMonth)
        {
            var fyMonthDic = new Dictionary<int, Month>();
            var fyMntInt = (int)fyMonth;
            for (int i = 1; i <= 12; i++)
            {
                fyMonthDic.Add(i, (Month)fyMntInt);

                fyMntInt = (fyMntInt == 12) ? 1 : (fyMntInt + 1);
            }
            return fyMonthDic;
        }
    }
}
