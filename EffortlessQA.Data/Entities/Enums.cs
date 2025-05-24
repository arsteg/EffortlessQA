using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Entities
{
    public enum TestExecutionStatus
    {
        Pass,
        Fail,
        Blocked,
        Skipped
    }

    public enum SeverityLevel
    {
        High,
        Medium,
        Low
    }

    public enum DefectStatus
    {
        Open,
        InProgress,
        Resolved,
        Closed
    }

    public enum DefectSeverity
    {
        High,
        Medium,
        Low
    }
}
