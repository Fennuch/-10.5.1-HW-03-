namespace Практикум_10._5._1__HW_03_
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    // Интерфейс для операции сложения
    public interface IAdder
    {
        double Add(double a, double b);
    }

    // Интерфейс логгера
    public interface ILogger
    {
        void LogInfo(string message);
        void LogError(string message);
    }

    // Реализация сложения
    public class Adder : IAdder
    {
        public double Add(double a, double b) => a + b;
    }

    // Цветной консольный логгер
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"[INFO] {message}");
            Console.ResetColor();
        }

        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
            Console.ResetColor();
        }
    }

    // Калькулятор с зависимостями
    public class Calculator
    {
        private readonly IAdder _adder;
        private readonly ILogger _logger;

        public Calculator(IAdder adder, ILogger logger)
        {
            _adder = adder;
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInfo("Калькулятор запущен");

            try
            {
                double a = GetNumber("Введите первое число: ");
                double b = GetNumber("Введите второе число: ");

                double result = _adder.Add(a, b);
                _logger.LogInfo($"Результат: {a} + {b} = {result}");
            }
            catch (FormatException)
            {
                _logger.LogError("Некорректный ввод числа!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка: {ex.Message}");
            }
            finally
            {
                _logger.LogInfo("Работа калькулятора завершена");
            }
        }

        private double GetNumber(string prompt)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

            if (!double.TryParse(input, out double number))
            {
                _logger.LogError($"Не удалось распознать число: '{input}'");
                throw new FormatException();
            }

            return number;
        }
    }

    class Program
    {
        static void Main()
        {
            // Настройка DI контейнера
            var services = new ServiceCollection()
                .AddSingleton<IAdder, Adder>()
                .AddSingleton<ILogger, ConsoleLogger>()
                .AddTransient<Calculator>()
                .BuildServiceProvider();

            // Получение экземпляра калькулятора
            var calculator = services.GetRequiredService<Calculator>();
            calculator.Run();
        }
    }
}
