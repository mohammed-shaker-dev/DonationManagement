// New file: ProjectDTO.cs
using System;
using System.Collections.Generic;

namespace Dashboard.Core.ProjectAggregate
{
    public class ProjectDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AdditionalText { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public decimal TotalBudget { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<string> Videos { get; set; } = new List<string>();
        public List<ExpenseDTO> Expenses { get; set; } = new List<ExpenseDTO>();
    }

    public class ExpenseDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Code { get; set; }
    }
}
