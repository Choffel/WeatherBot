namespace Weather_Bot.Configuration;

/// <summary>
/// Утилита для загрузки переменных окружения из файла .env
/// </summary>
public static class EnvironmentSetup
{
    /// <summary>
    /// Загружает переменные окружения из файла .env
    /// </summary>
    public static void LoadDotEnv()
    {
        try
        {
            // Ищем файл .env в текущей директории или в базовой директории приложения
            var candidates = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), ".env"),
                Path.Combine(AppContext.BaseDirectory, ".env")
            };

            var envPath = candidates.FirstOrDefault(File.Exists);
            
            if (envPath == null)
            {
                Console.WriteLine("⚠️  Предупреждение: файл .env не найден. Используются переменные окружения системы.");
                return;
            }

            Console.WriteLine($"✅ Загружен файл конфигурации: {envPath}");

            foreach (var line in File.ReadAllLines(envPath))
            {
                var trimmedLine = line.Trim();
                
                // Пропускаем пустые строки и комментарии
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                    continue;

                var separatorIndex = trimmedLine.IndexOf('=');
                if (separatorIndex <= 0)
                    continue;

                var key = trimmedLine.Substring(0, separatorIndex).Trim();
                var value = trimmedLine.Substring(separatorIndex + 1).Trim();

                // Убираем кавычки если они есть
                if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                    (value.StartsWith("'") && value.EndsWith("'")))
                {
                    value = value.Substring(1, value.Length - 2);
                }

                Environment.SetEnvironmentVariable(key, value);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при загрузке .env: {ex.Message}");
            throw;
        }
    }
}

