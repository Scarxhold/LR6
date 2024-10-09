using LR6.Models;
using Microsoft.AspNetCore.Mvc;

namespace LR6.Controllers
{
    public class OrderController : Controller
    {
        // Доступные продукты
        private static List<Product> AvailableProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Pizza Margherita", Price = 150 },
            new Product { Id = 2, Name = "Pizza Pepperoni", Price = 180 },
            new Product { Id = 3, Name = "Pizza Four Seasons", Price = 200 }
        };

        // Экран регистрации (не изменен)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (user.Age < 16)
            {
                return Content("You must be older than 16 to make an order.");
            }
            return RedirectToAction("Order", new { userId = user.Id });
        }

        // Экран ввода количества пицц
        [HttpGet]
        public IActionResult Order(int userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [HttpPost]
        public IActionResult Order(int userId, int quantity)
        {
            if (quantity <= 0)
            {
                ModelState.AddModelError("", "Quantity must be a positive number.");
                return View();
            }
            return RedirectToAction("CreateOrderForms", new { userId, quantity });
        }

        // Экран выбора пицц с ограничением на количество
        [HttpGet]
        public IActionResult CreateOrderForms(int userId, int quantity)
        {
            var model = new CreateOrderFormsModel
            {
                Quantity = quantity,  // передаем максимальное количество пицц
                AvailableProducts = AvailableProducts
            };
            ViewBag.Quantity = quantity;
            return View(model);
        }

        // Обработка выбора продуктов и проверка на превышение количества

        [HttpPost]
        public IActionResult CreateOrderForms(int userId, List<int> productIds, List<int> quantities, int quantity)
        {
            var orderedProducts = new List<OrderProduct>();
            int totalSelected = 0;

            // Проверка всех введенных количеств
            for (int i = 0; i < productIds.Count; i++)
            {
                if (i < quantities.Count)
                {
                    var productId = productIds[i];
                    var selectedQuantity = quantities[i];

                    if (selectedQuantity > 0)
                    {
                        totalSelected += selectedQuantity;

                        // Проверка, если суммарное количество превышает введенное количество
                        if (totalSelected > quantity)
                        {
                            ModelState.AddModelError("", $"You cannot select more than {quantity} pizzas.");
                            var model = new CreateOrderFormsModel
                            {
                                Quantity = quantity,
                                AvailableProducts = AvailableProducts
                            };
                            return View(model);  // Возвращаем то же представление с ошибками
                        }

                        var product = AvailableProducts.FirstOrDefault(p => p.Id == productId);
                        if (product != null)
                        {
                            orderedProducts.Add(new OrderProduct
                            {
                                Product = product,
                                Quantity = selectedQuantity
                            });
                        }
                    }
                }
            }

            // Проверка, если выбрано меньше пицц, чем указано
            if (totalSelected < quantity)
            {
                ModelState.AddModelError("", $"You must select exactly {quantity} pizzas.");
                var model = new CreateOrderFormsModel
                {
                    Quantity = quantity,
                    AvailableProducts = AvailableProducts
                };
                return View(model);  // Возвращаем то же представление с ошибками
            }

            // Если все правильно, показываем сводку заказа
            return View("OrderSummary", orderedProducts);
        }

    }
}
