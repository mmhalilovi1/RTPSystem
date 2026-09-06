using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class PositionService : IPositionService
    {
        private readonly IApplicationDbContext _context;

        public PositionService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PositionResponseDto> CreateAsync(PositionRequestDto request)
        {
            var skills = new List<Skill>();
            foreach (var skillName in request.RequiredSkills)
            {
                var existing = await _context.Skills
                    .FirstOrDefaultAsync(s => s.Name == skillName);

                skills.Add(existing ?? new Skill(skillName));
            }

            var position = new Position(
                request.Title,
                request.Description,
                request.Location,
                request.EmploymentType,
                skills,
                request.RequiredExperience,
                request.Deadline
            );

            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            return PositionResponseDto.FromEntity(position);
        }

        public async Task<PositionResponseDto?> GetByIdAsync(Guid positionId)
        {
            var position = await _context.Positions
                .Include(p => p.RequiredSkills)
                .FirstOrDefaultAsync(p => p.Id == positionId);

            if (position == null)
                return null;

            return PositionResponseDto.FromEntity(position);
        }

        public async Task<PositionResponseDto?> GetbyIdOpenAsync(Guid positionId)
        {
            var position = await _context.Positions
                .Include(p => p.RequiredSkills)
                .FirstOrDefaultAsync(p => p.Id == positionId && p.Status == PositionStatus.Open);

            if (position == null)
                return null;

            return PositionResponseDto.FromEntity(position);
        }

        public async Task<List<PositionResponseDto>> GetAllAsync(PositionFilterDto filter)
        {
            var query = _context.Positions
                .Include(p => p.RequiredSkills)
                .Where(p => p.Status == PositionStatus.Open)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Location))
                query = query.Where(p => p.Location.Contains(filter.Location));

            if (filter.EmploymentType.HasValue)
                query = query.Where(p => p.EmploymentType == filter.EmploymentType.Value);

            if (filter.MinRequiredExperience.HasValue)
                query = query.Where(p => p.RequiredExperience >= filter.MinRequiredExperience.Value);

            var positions = await query.ToListAsync();
            return positions.Select(PositionResponseDto.FromEntity).ToList();
        }

        public async Task<List<PositionResponseDto>> GetAllOpenAsync(PositionFilterDto filter)
        {
            var query = _context.Positions
                .Include(p => p.RequiredSkills)
                .Where(p => p.Status == PositionStatus.Open)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Location))
                query = query.Where(p => p.Location.Contains(filter.Location));

            if (filter.EmploymentType.HasValue)
                query = query.Where(p => p.EmploymentType == filter.EmploymentType.Value);

            if (filter.MinRequiredExperience.HasValue)
                query = query.Where(p => p.RequiredExperience >= filter.MinRequiredExperience.Value);

            var positions = await query.ToListAsync();

            return positions.Select(PositionResponseDto.FromEntity).ToList();
        }


        public async Task<PositionResponseDto?> UpdateAsync(Guid positionId, PositionUpdateDto updateDto)
        {
            var position = await _context.Positions
                .Include(p => p.RequiredSkills)
                .FirstOrDefaultAsync(p => p.Id == positionId);

            if (position == null)
                return null;

            position.UpdateDetails(
                updateDto.Title,
                updateDto.Description,
                updateDto.Location,
                updateDto.RequiredExperience,
                updateDto.EmploymentType,              
                updateDto.Deadline
            );

            await _context.SaveChangesAsync();

            return PositionResponseDto.FromEntity(position);
        }

        public async Task DeleteAsync(Guid positionId)
        {
            var position = await _context.Positions.FindAsync(positionId);

            if (position == null)
                throw new KeyNotFoundException("Pozicija nije pronađena.");

            // _context.Positions.Remove(position);     hard delete
            position.Archive(); // soft delete 
            await _context.SaveChangesAsync();
        }

        public async Task PublishAsync(Guid positionId)
        {
            var position = await _context.Positions.FindAsync(positionId);

            if (position == null)
                throw new KeyNotFoundException("Pozicija nije pronađena.");

            position.Publish();
            await _context.SaveChangesAsync();
        }

        public async Task CloseAsync(Guid positionId)
        {
            var position = await _context.Positions.FindAsync(positionId);

            if (position == null)
                throw new KeyNotFoundException("Pozicija nije pronađena.");

            position.Close();
            await _context.SaveChangesAsync();
        }

        public async Task ArchiveAsync(Guid positionId)
        {
            var position = await _context.Positions.FindAsync(positionId);

            if (position == null)
                throw new KeyNotFoundException("Pozicija nije pronađena.");

            position.Archive();
            await _context.SaveChangesAsync();
        }
    }
}