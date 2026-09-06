using Domain.Entities;

public class SkillTests
{
    // Testiranje konstruktora
    [Fact]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Skill(""));
    }

    [Fact]
    public void Constructor_WhitespaceName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Skill("   "));
    }

    [Fact]
    public void Constructor_NullName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Skill(null!));
    }

    [Fact]
    public void Constructor_ValidName_CreatesSkill()
    {
        var skill = new Skill("C#");

        Assert.NotEqual(Guid.Empty, skill.Id);
        Assert.Equal("C#", skill.Name);
    }

    [Fact]
    public void Constructor_NameWithWhitespace_TrimsName()
    {
        var skill = new Skill("  C#  ");

        Assert.Equal("C#", skill.Name);
    }
}