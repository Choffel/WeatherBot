using Weather_Bot.DTOs.IssDTO;

namespace Weather_Bot.Contract.Iss;

public interface ISatelliteStateService
{
    Task<SatelliteStateResponse> GetSatelliteStateAsync(CancellationToken cancellationToken);
}