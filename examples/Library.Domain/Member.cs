namespace Library.Domain;

public class Member
{
    public string Id { get; }
    public string Name { get; }
    public MembershipTier Tier { get; }
    public bool IsSuspended { get; set; }

    public Member(string id, string name, MembershipTier tier)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Tier = tier;
    }
}
