using API.Models;

namespace API.Data
{
    public interface ISharedRepository
    {
        Projects GetSharedElementById(int elementId);
        IEnumerable<Projects> GetAllSharedElements();
        void AddSharedElement(Projects sharedElement);
        void UpdateSharedElement(Projects sharedElement);
        void DeleteSharedElement(int elementId);
        Projects ShareElementWithUser(int elementId, int userId, SharedRole role);
        Projects ChangeUserRole(int elementId, int userId, SharedRole newRole);
        Projects RemoveUserFromSharedElement(int elementId, int userId);
        IEnumerable<Projects> GetSharedElementsByUserId(int userId);
    }
}
