namespace OnlineStore.Core.DTOs
{
    /// <summary>
    /// Конфигурация путей к файлам данных
    /// </summary>
    public class DataPathsConfiguration
    {
        /// <summary>
        /// Путь к файлу с корзинами
        /// </summary>
        public string CartsFilePath { get; set; } = "OnlineStore.API/Data/carts.json";
        
        /// <summary>
        /// Путь к файлу с продуктами
        /// </summary>
        public string ProductsFilePath { get; set; } = "OnlineStore.API/Data/products.json";
        
        /// <summary>
        /// Путь к файлу с купонами
        /// </summary>
        public string CouponsFilePath { get; set; } = "OnlineStore.API/Data/coupons.json";
        
        /// <summary>
        /// Путь к файлу с отзывами
        /// </summary>
        public string ReviewsFilePath { get; set; } = "OnlineStore.API/Data/reviews.json";
    }
}