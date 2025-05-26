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

        public static void MapApiEndpoints(this WebApplication app)
        {
            MapAuthEndpoints(app);
            MapProjectEndpoints(app);
            MapRequirementEndpoints(app);
            MapTestSuiteEndpoints(app);
            //MapTestCaseEndpoints(app);
            MapTestRunEndpoints(app);
            MapTestRunResultEndpoints(app);
            MapDefectEndpoints(app);
        }
    }
}
