using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        // Represents distributed cache of searlized values
        private readonly IDistributedCache _rediscache;

        public BasketRepository(IDistributedCache rediscache)
        {
            _rediscache = rediscache ?? throw new ArgumentNullException(nameof(rediscache));
        }

        public async Task<ShoppingCart> GetBasket(string username)
        { 
            var basket = await _rediscache.GetStringAsync(username);
            if (string.IsNullOrEmpty(basket))
                return null;

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);

        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        { 
            await _rediscache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            return await GetBasket(basket.UserName);
        }

        public async Task DeleteBasket(string username)
        { 
            await _rediscache.RemoveAsync(username);
        }
    }
}
