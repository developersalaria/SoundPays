namespace SoundpaysAdd.Core.Interfaces
{
    public interface ICurrentUserService
    {
        bool IsSuperAdmin { get; }
        bool IsAdvertiser { get; }
        string? UserId { get; }
        List<string>? UserRoles { get; }
        int AdvertiserId { get; }
    }
}
