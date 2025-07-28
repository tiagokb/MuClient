using System.Threading.Tasks;

public interface IAsyncPacketBuilder
{
    Task<byte[]> BuildeAsync(); // assíncrono, mais pesado
}