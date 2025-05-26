using EffortlessQA.Data.Dtos;

namespace EffortlessQA.Api.Services.Interface
{
    public interface ITestFolderService
    {
        Task<TestFolderDto> CreateTestFolderAsync(
            Guid projectId,
            string tenantId,
            CreateTestFolderDto dto
        );
        Task<PagedResult<TestFolderDto>> GetTestFoldersAsync(
            Guid projectId,
            string tenantId,
            int page,
            int limit,
            string? filter
        );
        Task<TestFolderDto> GetTestFolderAsync(Guid folderId, Guid projectId, string tenantId);
        Task<TestFolderDto> UpdateTestFolderAsync(
            Guid folderId,
            Guid projectId,
            string tenantId,
            UpdateTestFolderDto dto
        );
        Task DeleteTestFolderAsync(Guid folderId, Guid projectId, string tenantId);
    }
}
