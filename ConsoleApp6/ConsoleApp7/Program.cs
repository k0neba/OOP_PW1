using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace PurchaseOrderNamespace
{
    // Клас, що представляє один товар
    public class Item
    {
        public string PartNumber { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal USPrice { get; private set; }
        public string Comment { get; private set; }
        public DateTime? ShipDate { get; private set; }

        public Item(string partNumber, string productName, int quantity, decimal usPrice, string comment, DateTime? shipDate)
        {
            PartNumber = partNumber;
            ProductName = productName;
            Quantity = quantity;
            USPrice = usPrice;
            Comment = comment;
            ShipDate = shipDate;
        }

        public override string ToString()
        {
            return $"PartNumber: {PartNumber}, ProductName: {ProductName}, Quantity: {Quantity}, USPrice: {USPrice:C}, Comment: {Comment}, ShipDate: {ShipDate?.ToShortDateString()}";
        }
    }

    // Клас, що представляє замовлення
    public class PurchaseOrder
    {
        public string PurchaseOrderNumber { get; private set; }
        public DateTime OrderDate { get; private set; }
        public Address ShippingAddress { get; private set; }
        public Address BillingAddress { get; private set; }
        public string DeliveryNotes { get; private set; }
        public List<Item> Items { get; private set; } = new List<Item>();

        public PurchaseOrder(string purchaseOrderNumber, DateTime orderDate, Address shippingAddress, Address billingAddress, string deliveryNotes)
        {
            PurchaseOrderNumber = purchaseOrderNumber;
            OrderDate = orderDate;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            DeliveryNotes = deliveryNotes;
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public override string ToString()
        {
            var result = $"PurchaseOrderNumber: {PurchaseOrderNumber}\nOrderDate: {OrderDate.ToShortDateString()}\n" +
                         $"ShippingAddress: {ShippingAddress}\nBillingAddress: {BillingAddress}\n" +
                         $"DeliveryNotes: {DeliveryNotes}\nItems:\n";

            foreach (var item in Items)
            {
                result += $"- {item}\n";
            }

            return result;
        }
    }

    // Клас, що представляє адресу
    public class Address
    {
        public string Type { get; private set; }
        public string Name { get; private set; }
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Zip { get; private set; }
        public string Country { get; private set; }

        public Address(string type, string name, string street, string city, string state, string zip, string country)
        {
            Type = type;
            Name = name;
            Street = street;
            City = city;
            State = state;
            Zip = zip;
            Country = country;
        }

        public override string ToString()
        {
            return $"{Name}, {Street}, {City}, {State} {Zip}, {Country}";
        }
    }

    // Статичний клас для завантаження даних з XML
    public static class PurchaseOrderLoader
    {
        public static PurchaseOrder LoadFromXml(string filePath)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(filePath);

            XmlNode root = xml.DocumentElement;

            string purchaseOrderNumber = root.Attributes["PurchaseOrderNumber"].InnerText;
            DateTime orderDate = DateTime.Parse(root.Attributes["OrderDate"].InnerText);
            string deliveryNotes = root["DeliveryNotes"].InnerText;

            XmlNode shippingAddressNode = root.SelectSingleNode("Address[@Type='Shipping']");
            XmlNode billingAddressNode = root.SelectSingleNode("Address[@Type='Billing']");

            Address shippingAddress = new Address(
                "Shipping",
                shippingAddressNode["Name"].InnerText,
                shippingAddressNode["Street"].InnerText,
                shippingAddressNode["City"].InnerText,
                shippingAddressNode["State"].InnerText,
                shippingAddressNode["Zip"].InnerText,
                shippingAddressNode["Country"].InnerText
            );

            Address billingAddress = new Address(
                "Billing",
                billingAddressNode["Name"].InnerText,
                billingAddressNode["Street"].InnerText,
                billingAddressNode["City"].InnerText,
                billingAddressNode["State"].InnerText,
                billingAddressNode["Zip"].InnerText,
                billingAddressNode["Country"].InnerText
            );

            PurchaseOrder purchaseOrder = new PurchaseOrder(
                purchaseOrderNumber,
                orderDate,
                shippingAddress,
                billingAddress,
                deliveryNotes
            );

            XmlNodeList itemNodes = root.SelectNodes("Items/Item");
            foreach (XmlNode itemNode in itemNodes)
            {
                string partNumber = itemNode.Attributes["PartNumber"].InnerText;
                string productName = itemNode["ProductName"].InnerText;
                int quantity = Int32.Parse(itemNode["Quantity"].InnerText);
                string usPriceString = itemNode["USPrice"].InnerText.Replace(',', '.');
                decimal usPrice = Decimal.Parse(usPriceString, CultureInfo.InvariantCulture);
                string comment = itemNode["Comment"]?.InnerText;
                DateTime? shipDate = itemNode["ShipDate"] != null
                    ? (DateTime?)DateTime.Parse(itemNode["ShipDate"].InnerText)
                    : null;

                Item item = new Item(partNumber, productName, quantity, usPrice, comment, shipDate);
                purchaseOrder.AddItem(item);
            }

            return purchaseOrder;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Завантаження даних з XML і виведення їх на консоль
            PurchaseOrder purchaseOrder = PurchaseOrderLoader.LoadFromXml(@"E:\source\ConsoleApp6\PurchaseOrder.xml");
            Console.WriteLine(purchaseOrder);
        }
    }
}
