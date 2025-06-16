using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Entities
{
    public enum TestExecutionStatus
    {
        NotExecuted, // - The test case has not been executed yet.

        Passed, // - The test case executed successfully, and the actual result matches the expected result.

        Failed, // - The test case executed, but the actual result does not match the expected result.

        Blocked, // - The test case cannot be executed due to external issues (e.g., environment issues, defects in the system, or missing prerequisites).

        Skipped, // - The test case was intentionally not executed (e.g., due to time constraints or irrelevance to the current testing scope).

        Running, // - The test case is currently being executed.

        Incomplete, // - The test case execution was started but not completed (e.g., due to interruptions or crashes).

        Deferred, // - The test case execution is postponed to a later cycle or release.

        Invalid, // - The test case is not applicable or contains errors (e.g., incorrect steps or outdated requirements).

        Retest, // - The test case needs to be re-executed (often after a fix for a previously failed test).

        Pending, // - The test case is awaiting approval, clarification, or additional information before execution.
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

    public enum PriorityLevel
    {
        High,
        Medium,
        Low
    }

    public enum RoleType
    {
        Admin,
        Tester
    }
}
