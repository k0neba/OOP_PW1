using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace NotepadNamespace
{
    // Клас, що представляє одну нотатку
    public class Note
    {
        public string Id { get; private set; }
        public DateTime Date { get; private set; }
        public TimeSpan Time { get; private set; }
        public string Subject { get; private set; }
        public string Importance { get; private set; }
        public string Text { get; private set; }

        public Note(string id, DateTime date, TimeSpan time, string subject, string importance, string text)
        {
            Id = id;
            Date = date;
            Time = time;
            Subject = subject;
            Importance = importance;
            Text = text;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Date: {Date.ToShortDateString()}, Time: {Time}, Subject: {Subject}, Importance: {Importance}, Text: {Text}";
        }
    }

    // Клас, що представляє блокнот з нотатками
    public class Notepad
    {
        public List<Note> Notes { get; private set; } = new List<Note>();

        public void Add(Note note)
        {
            Notes.Add(note);
        }

        public override string ToString()
        {
            var result = "Notepad:\n";
            foreach (var note in Notes)
            {
                result += $"- {note}\n";
            }
            return result;
        }
    }

    // Статичний клас для завантаження даних з XML
    public static class NotepadLoader
    {
        public static Notepad LoadFromXml(string filePath)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(filePath);

            XmlNode root = xml.DocumentElement;

            Notepad notepad = new Notepad();

            XmlNodeList noteNodes = root.SelectNodes("note");
            foreach (XmlNode noteNode in noteNodes)
            {
                string id = noteNode.Attributes["id"].InnerText;
                DateTime date = DateTime.ParseExact(noteNode.Attributes["date"].InnerText, "MM/dd/yy", CultureInfo.InvariantCulture);
                TimeSpan time = TimeSpan.Parse(noteNode.Attributes["time"].InnerText);
                string subject = noteNode["subject"]?.InnerText;
                string importance = noteNode["importance"]?.InnerText ?? string.Empty;

                string text = noteNode["text"]?.InnerXml;
                // Очистити текст від тегів <tel>
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.Replace("<tel>", "").Replace("</tel>", "");
                }

                Note note = new Note(id, date, time, subject, importance, text);
                notepad.Add(note);
            }

            return notepad;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Завантаження даних з XML і виведення їх на консоль
            Notepad notepad = NotepadLoader.LoadFromXml(@"E:\source\ConsoleApp6\Notepad.xml");
            Console.WriteLine(notepad);
        }
    }
}
