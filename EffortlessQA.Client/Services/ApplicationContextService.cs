using EffortlessQA.Data.Dtos;
using Microsoft.JSInterop;

namespace EffortlessQA.Client.Services
{
    public class ApplicationContextService
    {
        private readonly ProjectService _projectService;
        private readonly IJSRuntime _jsRuntime;
        private Guid? _selectedProjectId;
        private List<ProjectDto> _projects = new();
        private const string StorageKey = "selectedProjectId";

        public ProjectDto SelectedProject { get; set; }
        public event Func<Task>? OnProjectChanged;

        public ApplicationContextService(ProjectService projectService, IJSRuntime jsRuntime)
        {
            _projectService = projectService;
            _jsRuntime = jsRuntime;
        }

        public Guid? SelectedProjectId
        {
            get => _selectedProjectId;
            set
            {
                _selectedProjectId = value;
                PersistSelectedProjectAsync().GetAwaiter().GetResult();
                NotifyProjectChangedAsync().GetAwaiter().GetResult();
            }
        }

        public async Task InitializeAsync()
        {
            // Load projects
            _projects = await _projectService.GetProjectsAsync();

            // Load selected project from local storage
            var storedProjectId = await _jsRuntime.InvokeAsync<string>(
                "localStorage.getItem",
                StorageKey
            );
            if (
                Guid.TryParse(storedProjectId, out var projectId)
                && _projects.Any(p => p.Id == projectId)
            )
            {
                _selectedProjectId = projectId;
            }
            else if (_projects.Any())
            {
                _selectedProjectId = _projects.First().Id;
                await PersistSelectedProjectAsync();
            }
            else
            {
                _selectedProjectId = null;
            }

            await NotifyProjectChangedAsync();
        }

        public async Task<List<ProjectDto>> GetProjectsAsync()
        {
            if (!_projects.Any())
            {
                _projects = await _projectService.GetProjectsAsync();
            }
            return _projects;
        }

        private async Task PersistSelectedProjectAsync()
        {
            if (_selectedProjectId.HasValue)
            {
                await _jsRuntime.InvokeVoidAsync(
                    "localStorage.setItem",
                    StorageKey,
                    _selectedProjectId.ToString()
                );
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
            }
        }

        public async Task NotifyProjectChangedAsync()
        {
            if (OnProjectChanged != null)
            {
                await OnProjectChanged.Invoke();
            }
        }
    }
}
