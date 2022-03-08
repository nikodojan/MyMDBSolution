// See https://aka.ms/new-console-template for more information
global using System.Data.SqlClient;
using DbInserterApp;

Console.WriteLine("Hello, World!");

SqlConnection connection = new SqlConnection("Data Source=NIKOSNOTEBOOK;Initial Catalog=MyMDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

//string titlesBasicsFile = @"D:\_4thSem\_DATABASES\Mandatory assignment\data\title.basics.tsv\data.tsv";
//TitlesBasicsInserter titlesBasicsInserter = new TitlesBasicsInserter(connection, titlesBasicsFile);
//titlesBasicsInserter.BulkInsertData(50_000);

//string namesFile = @"D:\_4thSem\_DATABASES\Mandatory assignment\data\name.basics.tsv\data.tsv";
//NameBasicsInserter nameBasicsInserter = new NameBasicsInserter(connection, namesFile);
//nameBasicsInserter.BulkInsertData(50_000);

string crewFile = @"D:\_4thSem\_DATABASES\Mandatory assignment\data\title.crew.tsv\data.tsv";
TitleCrewInserter crewInserter = new TitleCrewInserter(connection, crewFile);
crewInserter.BulkInsertData(50_000);


Console.WriteLine("End");
Console.ReadKey();

