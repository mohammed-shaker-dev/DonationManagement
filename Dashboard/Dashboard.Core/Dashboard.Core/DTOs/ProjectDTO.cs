// New file: ProjectDTO.cs
using SharedKernel.Enums;
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
        public ProjectStatus Status { get; set; }
        public string StatusText
        {
            get
            {
                return Status switch
                {
                    ProjectStatus.Planned => "مخطط",
                    ProjectStatus.InProgress => "قيد التنفيذ",
                    ProjectStatus.Completed => "مكتمل",
                    _ => Status.ToString()
                };
            }
        }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Dictionary<string, decimal> CurrencyTotals { get; set; } = new Dictionary<string, decimal>();
        public List<string> Images { get; set; } = new List<string>();
        public List<string> Videos { get; set; } = new List<string>();
        public List<ExpenseDTO> Expenses { get; set; } = new List<ExpenseDTO>();
        public ProjectType ProjectType { get; set; } = ProjectType.Donation;

    }

    public class ExpenseDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Code { get; set; }
        public string CurrencyCode { get; set; }
    }
}
