using EffortlessQA.Api.Services.Interface;
using EffortlessQA.Data.Dtos;
using Microsoft.AspNetCore.Authorization; // For RequireAuthorization
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EffortlessQA.Api.Extensions
{
    public static partial class ApiExtensions
    {
        public static string AUTH_TAG = "Authentication";
        public static string PROJECTS_TAG = "Projects";
        public static string REQUIREMENT_TAG = "Requirements";
        public static string TESTSUITE_TAG = "Test Suite";
        public static string TESTCASE_TAG = "Test Case";
        public static string TESTRUN_TAG = "Test Run";
        public static string TESTRUN_RESULT_TAG = "Test Run Result";
        public static string DEFECTS_TAG = "Defects";
        public const string TENANT_TAG = "Tenants";
        public const string TESTFOLDER_TAG = "Test Folders";
        public const string AUDITLOG_TAG = "Audit Logs";
        public const string REPORTING_TAG = "Reporting";
        public const string SEARCH_TAG = "Search";
        public const string PERMISSION_ROLE_TAG = "Permissions & Roles";
        public const string MISCELLANEOUS_TAG = "Miscellaneous";
        public const string COMMON_TAG = "Common";

        public static void MapApiEndpoints(this WebApplication app)
        {
            MapAuditLogEndpoints(app);
            MapAuthEndpoints(app);
            MapDefectEndpoints(app);
            MapMiscellaneousEndpoints(app);
            MapPermissionRoleEndpoints(app);
            MapProjectEndpoints(app);
            MapReportingEndpoints(app);
            MapRequirementEndpoints(app);
            MapSearchEndpoints(app);
            MapTestCaseEndpoints(app);
            MapTestFolderEndpoints(app);
            MapTestRunEndpoints(app);
            MapTestRunResultEndpoints(app);
            MapTestSuiteEndpoints(app);
            MapCommonEndpoints(app);
        }
    }
}
