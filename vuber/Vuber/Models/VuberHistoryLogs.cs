using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Vuber.Models
{
    public class VuberHistoryLogs
    {
        public VuberHistoryLogs()
        {
        }

        [Key]
        public int HistoryId { get; set; }

        [MaxLength(20)]
        public string LogicalGroup { get; set; }

        public DateTime Execution { get; set; }

        [MaxLength(10)]
        public string State { get; set; }

        [MaxLength(50)]
        public string FileName { get; set; }

        public string FileContext { get; set; }

        public string ExecutionResult { get; set; }

        [MaxLength(64)]
        public string ExecutionIdentity { get; set; }

        public string UserBy { get; set; }

        public string Owner { get; set; }
    }

    public class HistoryContext : DbContext
    {
        public HistoryContext()
            : base()
        {
        }

        public DbSet<VuberHistoryLogs> Histories { get; set; }
    }
}