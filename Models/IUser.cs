namespace Models
{
    public interface IUser
    {
        int Id { get; set; }
        string Name { get; set; }
        bool UnitAdmin { get; set; }

        MembershipCategoryId Category { get; set; }
        byte[] PublicKeyEncoded { get; set; }

        int UnitId { get; set; }
    }
}