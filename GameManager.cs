using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GameManager
{
    private List<Event> _positiveEvents;
    private List<Event> _negativeEvents;

    // ����� ��� �������� ������� �� JSON-�����
    private EventsData LoadEvents(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<EventsData>(json);
    }

    // ����� ��� ������������� �������
    public void InitializeEvents()
    {
        string filePath = "Events.json"; // ������� ���� � ������ JSON-�����
        if (File.Exists(filePath))
        {
            var eventsData = LoadEvents(filePath);
            _positiveEvents = eventsData.PositiveEvents;
            _negativeEvents = eventsData.NegativeEvents;
        }
        else
        {
            throw new FileNotFoundException($"���� {filePath} �� ������.");
        }
    }

    // ������ ������������� �������
    public void TriggerRandomEvent()
    {
        Random _random = new Random();
        bool isPositive = _random.NextDouble() < 0.5; // 50% ����
        Event chosenEvent;

        if (isPositive)
        {
            chosenEvent = _positiveEvents[_random.Next(_positiveEvents.Count)];
            Console.WriteLine($"���������� �������: {chosenEvent.Name}, ��������� �������: +{chosenEvent.TrafficChange}");
        }
        else
        {
            chosenEvent = _negativeEvents[_random.Next(_negativeEvents.Count)];
            Console.WriteLine($"���������� �������: {chosenEvent.Name}, ��������� �������: {chosenEvent.TrafficChange}");
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
