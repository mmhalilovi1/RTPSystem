using Domain.Entities;
using Domain.Enums;
using Xunit;

namespace RecruitmentPipeline.Tests.Domain
{
    public class PositionTests
    {
        private static Position CreateValidPosition()
        {
            return new Position(
                title: "Junior Software Engineer",
                description: "Opis pozicije",
                location: "Sarajevo",
                emplType: EmploymentType.Internship,
                skills: new List<Skill> { new Skill("C++") },
                experience: 0,
                deadline: DateTime.UtcNow.AddDays(30));
        }

        // Testiranje konstruktora
        [Fact]
        public void Constructor_InvalidTitle_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Position(
                title: "",
                description: "Opis pozicije",
                location: "Sarajevo",
                emplType: EmploymentType.Internship,
                skills: new List<Skill> { new Skill("C++") },
                experience: 0,
                deadline: DateTime.UtcNow.AddDays(30)));
        }

        [Fact]
        public void Constructor_InvalidExperience_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Position(
                title: "Junior Software Engineer",
                description: "Opis pozicije",
                location: "Sarajevo",
                emplType: EmploymentType.Internship,
                skills: new List<Skill> { new Skill("C++") },
                experience: -4,
                deadline: DateTime.UtcNow.AddDays(30)));
        }

        [Fact]
        public void Constructor_InvalidDeadline_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Position(
                title: "",
                description: "Opis pozicije",
                location: "Sarajevo",
                emplType: EmploymentType.Internship,
                skills: new List<Skill> { new Skill("C++") },
                experience: 0,
                deadline: DateTime.UtcNow.AddDays(-2)));
        }

        // Testiranje Publish() metode
        [Fact]
        public void Publish_WhenPositionIsDraft_ChangesStatusToOpen()
        {            
            var position = CreateValidPosition();

            position.Publish();

            Assert.Equal(PositionStatus.Open, position.Status);
        }

        [Fact]
        public void Publish_WhenPositionIsAlreadyOpen_ThrowsArgumentException()
        {
            var position = CreateValidPosition();
            position.Publish();
         
            Assert.Throws<ArgumentException>(() => position.Publish());
        }

        // Testiranje Close() metode
        [Fact]
        public void Close_WhenPositionIsDraft_ThrowsArgumentException()
        {
            var position = CreateValidPosition();

            Assert.Throws<ArgumentException>(() => position.Close());
        }

        [Fact]
        public void Close_WhenPositionIsOpen_ChangesStatusToClosed()
        {
            var position = CreateValidPosition();

            position.Publish();
            position.Close();

            Assert.Equal(PositionStatus.Closed, position.Status);
        }

        // Testiranje Archive() metode
        public void Archive_WhenPositionIsNotClosed_ThrowsArgumentException()
        {
            var position = CreateValidPosition();

            Assert.Throws<ArgumentException>(() => position.Archive());
        }

        [Fact]
        public void Archive_WhenPositionIsClosed_ChangesStatusToArchived()
        {
            var position = CreateValidPosition();

            position.Publish();
            position.Close();
            position.Archive();

            Assert.Equal(PositionStatus.Archived, position.Status);
        }

        // Testiranje UpdateDetails() metode
        [Fact]
        public void UpdateDetails_WhenPositionIsClosed_ThrowsInvalidOperationException()
        {
            var position = CreateValidPosition();

            position.Publish();
            position.Close();

            Assert.Throws<InvalidOperationException>(() => position.UpdateDetails("Junior Backend Dev"));
        }

        [Fact]
        public void UpdateDetails_WhenPositionIsArchived_ThrowsInvalidOperationException()
        {
            var position = CreateValidPosition();

            position.Publish();
            position.Close();
            position.Archive();

            Assert.Throws<InvalidOperationException>(() => position.UpdateDetails("Senior"));
        }

        [Fact]
        public void UpdateDetails_WhenPositionIsOpen_UpdatesPosition()
        {
            var position = CreateValidPosition();

            position.Publish();
            position.UpdateDetails("Senior", null, "London");

            Assert.Equal("Senior", position.Title);
            Assert.Equal("Opis pozicije", position.Description);
            Assert.Equal("London", position.Location);
        }

        // Testiranje AddRequiredSkill() metode
        [Fact]
        public void AddRequiredSkill_WhenSkillDoesntExist_AddsSkill()
        {
            var position = CreateValidPosition();

            position.AddRequiredSkill(new Skill("Java"));

            Assert.Equal(2, position.RequiredSkills.Count);
            Assert.Contains(position.RequiredSkills, x => x.Name == "C++");
            Assert.Contains(position.RequiredSkills, x => x.Name == "Java");
        }

        [Fact]
        public void AddRequiredSkill_WhenSkillExists_ThrowsInvalidOperationException()
        {
            var position = CreateValidPosition();

            Assert.Throws<InvalidOperationException>(() => position.AddRequiredSkill(new Skill("C++")));
        }

        // Testiranje RemoveSkill() metode
        [Fact]
        public void RemoveRequiredSkill_WhenSkillExists_RemovesSkill()
        {
            var position = CreateValidPosition();

            position.RemoveRequiredSkill(new Skill("C++"));

            Assert.Equal(0, position.RequiredSkills.Count);
        }

        [Fact]
        public void RemoveRequiredSkill_WhenSkillDoesntExist_ThrowsInvalidOperationException()
        {
            var position = CreateValidPosition();

            Assert.Throws<InvalidOperationException>(() => position.RemoveRequiredSkill(new Skill("Java")));
        }
    }
}