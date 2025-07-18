namespace Catalog.Infrastructure.EventBus.IntegrationEvents
{
    public class ProductAdd : IntegrationEvent
    {
        public int ProductId { get; private set; }

        public ProductAdd(int productId)
        {
            ProductId = productId;
        }
    }
}
