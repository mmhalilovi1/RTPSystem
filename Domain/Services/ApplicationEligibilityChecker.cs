using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services
{
    public static class ApplicationEligibilityChecker
    {
        public static void EnsureCanApply(Candidate candidate, Position position, bool alreadyApplied)
        {
            if (!candidate.CanApply())
                throw new InvalidOperationException("Kandidat mora imati postavljen CV prije apliciranja.");

            if (!position.isOpen())
                throw new InvalidOperationException("Pozicija nije otvorena za prijave.");

            if (alreadyApplied)
                throw new InvalidOperationException("Kandidat je već aplicirao na ovu poziciju.");
        }
    }
}
