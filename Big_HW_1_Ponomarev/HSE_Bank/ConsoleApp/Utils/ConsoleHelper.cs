using System;
using HSE_Bank.Domain.Entities;

namespace HSE_Bank.ConsoleApp.Utils
{
    public static class ConsoleHelper
    {
        public static int GetIntInput(string message, int min, int max)
        {
            int result;
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out result) && result >= min && result <= max)
                {
                    return result;
                }
                Console.WriteLine($"Ошибка: Введите число от {min} до {max}.");
            }
        }

        public static string GetStringInput(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(input))
                {
                    return input;
                }
                Console.WriteLine("Ошибка: Введите непустую строку.");
            }
        }

        public static TypeCategory GetTypeCategory(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();
                if (input == "+" || input == "-")
                {
                    if (input == "+")
                    {
                        return TypeCategory.Income;
                    }
                    return TypeCategory.Expense;
                }
                Console.WriteLine("Ошибка введите символ +/-!");
            }
        }
        public static decimal GetDecimalInput(string message)
        {
            decimal result;
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (decimal.TryParse(input, out result) && result >= 0)
                {
                    return result;
                }
                Console.WriteLine("Ошибка: Введите корректное положительное число.");
            }
        }

        public static void WaitForKey()
        {
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}