namespace WebApplication1.BackgroundJobs.OrderJobs
{
    public interface IOrderBackgroundJobs
    {
        Task CheckAndCancelUnpaidOrderAsyn(int OrderId);
    }
}
