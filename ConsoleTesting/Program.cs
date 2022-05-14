// See https://aka.ms/new-console-template for more information

using DataAccessLayer;
using DataAccessLayer.Attributes;

Console.WriteLine("Hello, World!");

ConsoleTesting.Person person = new();

string test = person.GetCreate();