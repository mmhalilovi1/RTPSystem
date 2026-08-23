using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Skill
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        private Skill() { }

        public Skill(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Naziv vještine je obavezan.", nameof(name));

            Id = Guid.NewGuid();
            Name = name.Trim();
        }
    }
}
