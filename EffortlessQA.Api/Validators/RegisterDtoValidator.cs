using EffortlessQA.Data.Dtos;
using FluentValidation;

namespace EffortlessQA.Api.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(255);
            //RuleFor(x => x.TenantId).NotEmpty().MaximumLength(50);
        }
    }

    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class ProjectCreateDtoValidator : AbstractValidator<ProjectCreateDto>
    {
        public ProjectCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public class TestSuiteCreateDtoValidator : AbstractValidator<TestSuiteCreateDto>
    {
        public TestSuiteCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public class TestCaseCreateDtoValidator : AbstractValidator<TestCaseCreateDto>
    {
        public TestCaseCreateDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(1000);
            RuleFor(x => x.Priority).IsInEnum();
            RuleFor(x => x.Tags)
                .Must(tags => tags == null || tags.All(t => t.Length <= 50))
                .WithMessage("Each tag must be 50 characters or less.");
        }
    }

    public class TestRunCreateDtoValidator : AbstractValidator<TestRunCreateDto>
    {
        public TestRunCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.TestCaseIds)
                .NotEmpty()
                .Must(ids => ids.Any())
                .WithMessage("At least one test case ID is required.");
        }
    }

    public class TestRunResultCreateDtoValidator : AbstractValidator<TestRunResultCreateDto>
    {
        public TestRunResultCreateDtoValidator()
        {
            RuleFor(x => x.TestCaseId).NotEmpty();
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.Comments).MaximumLength(1000);
        }
    }

    public class TestRunResultBulkUpdateDtoValidator : AbstractValidator<TestRunResultBulkUpdateDto>
    {
        public TestRunResultBulkUpdateDtoValidator()
        {
            //RuleFor(x => x.Results).NotEmpty();
            //RuleForEach(x => x.Results).SetValidator(new TestRunResultCreateDtoValidator());
        }
    }

    public class DefectCreateDtoValidator : AbstractValidator<CreateDefectDto>
    {
        public DefectCreateDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(1000);
            RuleFor(x => x.Severity).IsInEnum();
            RuleFor(x => x.ExternalId).MaximumLength(100);
        }
    }

    public class RequirementCreateDtoValidator : AbstractValidator<RequirementCreateDto>
    {
        public RequirementCreateDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(1000);
            RuleFor(x => x.Tags)
                .Must(tags => tags == null || tags.All(t => t.Length <= 50))
                .WithMessage("Each tag must be 50 characters or less.");
        }
    }
}
