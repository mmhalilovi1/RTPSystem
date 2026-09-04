using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IPositionService
    {
        Task<PositionResponseDto> CreateAsync(PositionRequestDto request);
        Task<PositionResponseDto?> GetByIdAsync(Guid positionId);
        Task<PositionResponseDto?> GetbyIdOpenAsync(Guid positionId);
        Task<List<PositionResponseDto>> GetAllAsync();
        Task<List<PositionResponseDto>> GetAllOpenAsync();
        Task<PositionResponseDto?> UpdateAsync(Guid positionId, PositionUpdateDto updateDto);
        Task DeleteAsync(Guid positionId);
        Task PublishAsync(Guid positionId);
        Task CloseAsync(Guid positionId);
        Task ArchiveAsync(Guid positionId);
    }
}
