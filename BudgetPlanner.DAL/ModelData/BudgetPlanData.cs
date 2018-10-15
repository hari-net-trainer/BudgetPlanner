using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetPlanner.DAL.ModelData
{
    public enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public enum PlannerStatus
    {
        New,
        Draft,
        Submited
    }

    public class Organization
    {
        public int OrgId { get; set; }

        public string OrgName { get; set; }

        public Month FyscalMonth { get; set; }
    }

    public class Budget
    {
        public int BudgetId { get; set; }

        public Month Month { get; set; }

        public decimal Value { get; set; }
    }

    public class BudgetStatus
    {
        public int BudgetStatusId { get; set; }

        public int OrgId { get; set; }

        public string FyscalYear { get; set; }

        public PlannerStatus PlannerStatus { get; set; }
    }

    public class Expense
    {
        public int ExpenseId { get; set; }

        public int OrgId { get; set; }

        public string Expenditure { get; set; }

        public string FyscalYear { get; set; }

        public List<Budget> BudgetPlan { get; set; }
    }
}
