using System;
using System.Collections.Generic;
using System.Xml;

namespace CarNamespace
{
    // Клас, що представляє окремий автомобіль
    public class Car
    {
        public string Id { get; private set; }
        public string Model { get; private set; }
        public int Year { get; private set; }
        public int Price { get; private set; }

        public Car(string id, string model, int year, int price)
        {
            Id = id;
            Model = model;
            Year = year;
            Price = price;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Model: {Model}, Year: {Year}, Price: {Price}";
        }
    }

    // Статичний клас, що містить список автомобілів і метод для завантаження з XML
    public static class Cars
    {
        public static List<Car> CarList { get; private set; } = new List<Car>();

        public static void LoadFromXml(string filePath)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(filePath);

            // Очищення списку перед завантаженням нових даних
            CarList.Clear();

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("shop"))
            {
                string id = node.Attributes["id"].InnerText;
                string model = node["model"].InnerText;
                int year = Int32.Parse(node["year"].InnerText);
                int price = Int32.Parse(node["price"].InnerText);

                CarList.Add(new Car(id, model, year, price));
            }
        }

        public static void Show()
        {
            foreach (Car car in CarList)
            {
                Console.WriteLine(car);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Завантаження даних з XML і виведення їх на консоль
            Cars.LoadFromXml(@"E:\source\ConsoleApp6\Cars.xml");
            Cars.Show();
        }
    }
}
