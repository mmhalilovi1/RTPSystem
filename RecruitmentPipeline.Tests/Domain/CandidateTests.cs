using Domain.Entities;

public class CandidateTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    private static Candidate CreateValidCandidate()
    {
        return new Candidate(
            userId: UserId,
            fullName: "John Doe",
            phoneNumber: "061234567",
            yearsOfExperience: 2,
            skillss: new List<Skill>
            {
                new Skill("C#")
            });
    }

    // Testiranje konstruktora
    [Fact]
    public void Constructor_InvalidUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Candidate(
            userId: Guid.Empty,
            fullName: "John Doe",
            phoneNumber: "061234567",
            yearsOfExperience: 2,
            skillss: new List<Skill>()));
    }

    [Fact]
    public void Constructor_InvalidFullName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Candidate(
            userId: UserId,
            fullName: "",
            phoneNumber: "061234567",
            yearsOfExperience: 2,
            skillss: new List<Skill>()));
    }

    [Fact]
    public void Constructor_WhitespaceFullName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Candidate(
            userId: UserId,
            fullName: "   ",
            phoneNumber: "061234567",
            yearsOfExperience: 2,
            skillss: new List<Skill>()));
    }

    [Fact]
    public void Constructor_InvalidYearsOfExperience_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Candidate(
            userId: UserId,
            fullName: "John Doe",
            phoneNumber: "061234567",
            yearsOfExperience: -1,
            skillss: new List<Skill>()));
    }

    [Fact]
    public void Constructor_ValidParameters_CreatesCandidate()
    {
        var before = DateTime.UtcNow;

        var candidate = CreateValidCandidate();

        var after = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, candidate.Id);
        Assert.Equal(UserId, candidate.UserId);
        Assert.Equal("John Doe", candidate.FullName);
        Assert.Equal("061234567", candidate.PhoneNumber);
        Assert.Equal(2, candidate.YearsOfExperience);
        Assert.Single(candidate.Skills);
        Assert.Equal("C#", candidate.Skills.First().Name);
        Assert.InRange(candidate.CreatedAt, before, after);
        Assert.Null(candidate.ResumeUrl);
    }

    // Testiranje CanApply() metode
    [Fact]
    public void CanApply_WhenResumeIsNotUploaded_ReturnsFalse()
    {
        var candidate = CreateValidCandidate();

        Assert.False(candidate.CanApply());
    }

    [Fact]
    public void CanApply_WhenResumeIsUploaded_ReturnsTrue()
    {
        var candidate = CreateValidCandidate();

        candidate.UploadResume("https://example.com/resume.pdf");

        Assert.True(candidate.CanApply());
    }

    // Testiranje UploadResume() metode
    [Fact]
    public void UploadResume_WhenUrlIsEmpty_ThrowsArgumentException()
    {
        var candidate = CreateValidCandidate();

        Assert.Throws<ArgumentException>(() =>
            candidate.UploadResume(""));
    }

    [Fact]
    public void UploadResume_WhenUrlIsWhitespace_ThrowsArgumentException()
    {
        var candidate = CreateValidCandidate();

        Assert.Throws<ArgumentException>(() =>
            candidate.UploadResume("   "));
    }

    [Fact]
    public void UploadResume_WhenUrlIsValid_SetsResumeUrl()
    {
        var candidate = CreateValidCandidate();

        candidate.UploadResume("https://example.com/resume.pdf");

        Assert.Equal(
            "https://example.com/resume.pdf",
            candidate.ResumeUrl);
    }

    // Testiranje UpdateProfile() metode
    [Fact]
    public void UpdateProfile_WhenFullNameIsValid_UpdatesFullName()
    {
        var candidate = CreateValidCandidate();

        candidate.UpdateProfile(fullName: "Jane Doe");

        Assert.Equal("Jane Doe", candidate.FullName);
    }

    [Fact]
    public void UpdateProfile_WhenFullNameIsEmpty_ThrowsArgumentException()
    {
        var candidate = CreateValidCandidate();

        Assert.Throws<ArgumentException>(() =>
            candidate.UpdateProfile(fullName: ""));
    }

    [Fact]
    public void UpdateProfile_WhenPhoneNumberIsProvided_UpdatesPhoneNumber()
    {
        var candidate = CreateValidCandidate();

        candidate.UpdateProfile(phoneNumber: "069876543");

        Assert.Equal("069876543", candidate.PhoneNumber);
    }

    [Fact]
    public void UpdateProfile_WhenYearsOfExperienceIsValid_UpdatesExperience()
    {
        var candidate = CreateValidCandidate();

        candidate.UpdateProfile(yearsOfExperience: 5);

        Assert.Equal(5, candidate.YearsOfExperience);
    }

    [Fact]
    public void UpdateProfile_WhenYearsOfExperienceIsNegative_ThrowsArgumentException()
    {
        var candidate = CreateValidCandidate();

        Assert.Throws<ArgumentException>(() =>
            candidate.UpdateProfile(yearsOfExperience: -1));
    }

    [Fact]
    public void UpdateProfile_WhenNoParametersAreProvided_DoesNotChangeProfile()
    {
        var candidate = CreateValidCandidate();

        candidate.UpdateProfile();

        Assert.Equal("John Doe", candidate.FullName);
        Assert.Equal("061234567", candidate.PhoneNumber);
        Assert.Equal(2, candidate.YearsOfExperience);
    }

    // Testiranje AddSkill() metode
    [Fact]
    public void AddSkill_WhenSkillDoesNotExist_AddsSkill()
    {
        var candidate = CreateValidCandidate();

        candidate.AddSkill(new Skill("Java"));

        Assert.Equal(2, candidate.Skills.Count);
        Assert.Contains(candidate.Skills, x => x.Name == "C#");
        Assert.Contains(candidate.Skills, x => x.Name == "Java");
    }

    [Fact]
    public void AddSkill_WhenSkillAlreadyExists_ThrowsInvalidOperationException()
    {
        var candidate = CreateValidCandidate();

        Assert.Throws<InvalidOperationException>(() =>
            candidate.AddSkill(new Skill("C#")));
    }

    // Testiranje RemoveSkill() metode
    [Fact]
    public void RemoveSkill_WhenSkillExists_RemovesSkill()
    {
        var candidate = CreateValidCandidate();

        candidate.RemoveSkill(new Skill("C#"));

        Assert.Empty(candidate.Skills);
    }

    [Fact]
    public void RemoveSkill_WhenSkillDoesNotExist_ThrowsInvalidOperationException()
    {
        var candidate = CreateValidCandidate();

        Assert.Throws<InvalidOperationException>(() =>
            candidate.RemoveSkill(new Skill("Java")));
    }
}