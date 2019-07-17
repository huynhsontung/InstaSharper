using System.Threading.Tasks;

namespace InstaSharper.API.Processors
{
    public interface IPushProcessor
    {
        /// <summary>
        /// Registers application for push notifications
        /// </summary>
        /// <returns></returns>
        Task<bool> RegisterPush();
    }
}
