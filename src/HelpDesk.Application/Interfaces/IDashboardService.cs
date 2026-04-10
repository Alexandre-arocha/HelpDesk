using HelpDesk.Application.DTOs;

namespace HelpDesk.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetStatsAsync();
}
