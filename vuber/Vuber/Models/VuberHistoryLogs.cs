using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vuber.Models
{
    public class VuberHistoryLogs
    {

        public VuberHistoryLogs()
        {

        }

        public int HistoryID { get; set; }

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
