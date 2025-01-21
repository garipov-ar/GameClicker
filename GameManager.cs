using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GameManager
{
    private List<Event> _positiveEvents;
    private List<Event> _negativeEvents;

    // Метод для загрузки событий из JSON-файла
    private EventsData LoadEvents(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<EventsData>(json);
    }

    // Метод для инициализации событий
    public void InitializeEvents()
    {
        string filePath = "Events.json"; // Укажите путь к вашему JSON-файлу
        if (File.Exists(filePath))
        {
            var eventsData = LoadEvents(filePath);
            _positiveEvents = eventsData.PositiveEvents;
            _negativeEvents = eventsData.NegativeEvents;
        }
        else
        {
            throw new FileNotFoundException($"Файл {filePath} не найден.");
        }
    }

    // Пример использования событий
    public void TriggerRandomEvent()
    {
        Random _random = new Random();
        bool isPositive = _random.NextDouble() < 0.5; // 50% шанс
        Event chosenEvent;

        if (isPositive)
        {
            chosenEvent = _positiveEvents[_random.Next(_positiveEvents.Count)];
            Console.WriteLine($"Позитивное событие: {chosenEvent.Name}, изменение трафика: +{chosenEvent.TrafficChange}");
        }
        else
        {
            chosenEvent = _negativeEvents[_random.Next(_negativeEvents.Count)];
            Console.WriteLine($"Негативное событие: {chosenEvent.Name}, изменение трафика: {chosenEvent.TrafficChange}");
        }
    }

    public class Event
    {
        public string Name { get; set; }
        public int TrafficChange { get; set; }
    }

    public class EventsData
    {
        public List<Event> PositiveEvents { get; set; }
        public List<Event> NegativeEvents { get; set; }
    }
}
