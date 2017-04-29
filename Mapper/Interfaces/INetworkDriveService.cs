namespace Mapper.Interfaces
{
    public interface INetworkDriveService
    {
        MappingNetworkDriveResult MapNetworkDrive(char driveLetter, string path, int waitTimeMs);
    }

    public enum MappingNetworkDriveResult
    {
        Success,
        FailedBecauseOfLogin,
        WrongParameters
    }
}
