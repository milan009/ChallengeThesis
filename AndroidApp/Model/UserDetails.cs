using Models;

namespace OdborkyApp.Model
{
    public class UserDetails
    {
        public int UserId { get; }
        public int UnitId { get; }
        public int PersonId { get; }
        public string UserName { get; }

        public bool IsUnitAdmin { get; }
        public MembershipCategoryId CategoryId { get; }

        public UserDetails(int userId, int unitId, int personId, string userName, bool isUnitAdmin, MembershipCategoryId categoryId)
        {
            UserId = userId;
            UnitId = unitId;
            PersonId = personId;
            UserName = userName;
            IsUnitAdmin = isUnitAdmin;
            CategoryId = categoryId;
        }
    }
}