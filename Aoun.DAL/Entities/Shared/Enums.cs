namespace Aoun.DAL.Entities;

public enum CaseStatus
{
    Pending = 0,
    Active = 1,
    Completed = 2,
    Urgent = 3,
    Rejected = 4
}

public enum UserType
{
    Admin = 1,
    Donor = 2,
    Charity = 3
}

public enum ProfileStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
