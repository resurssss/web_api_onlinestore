using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Interfaces;
using OnlineStore.Services.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        [Authorize] // Только авторизованные пользователи могут получить корзину
        public async Task<ActionResult<CartResponseDto>> GetCart([FromQuery] int id, CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверка прав доступа: пользователь может получить только свою корзину
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var cart = await _cartService.GetCartAsync(id, cancellationToken);
                
                // Проверка, что корзина принадлежит текущему пользователю или пользователь - админ/модератор
                if (cart.UserId != currentUserId && !User.IsInRole("Администратор") && !User.IsInRole("Модератор"))
                {
                    return Forbid(); // Запрет доступа к чужой корзине
                }
                
                return Ok(cart);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Cart with ID {id} not found");
            }
        }

        [HttpPost("items")]
        [Authorize] // Только авторизованные пользователи могут добавлять товары в корзину
        public async Task<ActionResult<CartResponseDto>> AddToCart([FromBody] CartItemCreateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Получаем ID текущего пользователя из Claims
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                // Для новых корзин используем ID = 0, который будет заменен на новый уникальный ID
                var cart = await _cartService.AddItemAsync(0, dto.ProductId, dto.Quantity, cancellationToken);
                
                // Устанавливаем UserId в корзине
                if (cart != null && cart.Id == 0) // Новая корзина
                {
                    // Здесь должна быть логика установки UserId в сервисе
                    // Для упрощения примера предполагаем, что сервис это обрабатывает
                }
                
                return cart != null ? Ok(cart) : NotFound("Product not found");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("items")]
        [Authorize] // Только авторизованные пользователи могут обновлять корзину
        public async Task<ActionResult<CartResponseDto>> UpdateItemQuantity([FromQuery] int cartId, [FromQuery] int productId, [FromBody] CartItemUpdateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверка прав доступа: пользователь может обновлять только свою корзину
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var cart = await _cartService.GetCartAsync(cartId, cancellationToken);
                
                // Проверка, что корзина принадлежит текущему пользователю или пользователь - админ/модератор
                if (cart.UserId != currentUserId && !User.IsInRole("Администратор") && !User.IsInRole("Модератор"))
                {
                    return Forbid(); // Запрет доступа к чужой корзине
                }
                
                var updatedCart = await _cartService.UpdateItemQuantityAsync(cartId, productId, dto.Quantity, cancellationToken);
                return updatedCart != null ? Ok(updatedCart) : NotFound("Cart or Item not found");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("items")]
        [Authorize] // Только авторизованные пользователи могут удалять из корзины
        public async Task<ActionResult> RemoveFromCart([FromQuery] int cartId, [FromQuery] int productId, CancellationToken cancellationToken = default)
        {
            // Проверка прав доступа: пользователь может удалять только из своей корзины
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var cart = await _cartService.GetCartAsync(cartId, cancellationToken);
            
            // Проверка, что корзина принадлежит текущему пользователю или пользователь - админ/модератор
            if (cart.UserId != currentUserId && !User.IsInRole("Администратор") && !User.IsInRole("Модератор"))
            {
                return Forbid(); // Запрет доступа к чужой корзине
            }
            
            await _cartService.RemoveItemAsync(cartId, productId, cancellationToken);
            return Ok(new { Message = "Item removed from cart" });
        }

        [HttpPost("checkout")]
        [Authorize] // Только авторизованные пользователи могут оформлять заказ
        public async Task<ActionResult> Checkout([FromQuery] int cartId, CancellationToken cancellationToken = default)
        {
            // Проверка прав доступа: пользователь может оформить только свой заказ
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var cart = await _cartService.GetCartAsync(cartId, cancellationToken);
            
            // Проверка, что корзина принадлежит текущему пользователю или пользователь - админ/модератор
            if (cart.UserId != currentUserId && !User.IsInRole("Администратор") && !User.IsInRole("Модератор"))
            {
                return Forbid(); // Запрет доступа к чужой корзине
            }
            
            if (cart == null || cart.TotalItems == 0)
                return BadRequest("Cart is empty");

            await _cartService.ClearCartAsync(cartId, cancellationToken);
            return Ok(new { Message = "Order placed", Total = cart.TotalPrice });
        }

        [HttpPost("apply-coupon")]
        [Authorize] // Только авторизованные пользователи могут применять купоны
        public async Task<ActionResult<CartResponseDto>> ApplyCoupon([FromQuery] int cartId, [FromBody] string code, CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверка прав доступа: пользователь может применить купон только к своей корзине
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var cart = await _cartService.GetCartAsync(cartId, cancellationToken);
                
                // Проверка, что корзина принадлежит текущему пользователю или пользователь - админ/модератор
                if (cart.UserId != currentUserId && !User.IsInRole("Администратор") && !User.IsInRole("Модератор"))
                {
                    return Forbid(); // Запрет доступа к чужой корзине
                }
                
                if (string.IsNullOrWhiteSpace(code))
                    return BadRequest("Coupon code is required");

                var updatedCart = await _cartService.ApplyCouponAsync(cartId, code, cancellationToken);
                return updatedCart != null ? Ok(updatedCart) : NotFound("Cart or coupon not found");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}