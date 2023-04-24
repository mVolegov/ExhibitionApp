using ExhibitionApp.Models;

namespace ExhibitionApp.Data
{
    public class DbInitializer
    {
        public static void Initialize(ExhibitionAppDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();

            // Add cities
            City city1 = new City()
            {
                Id = 57401000000,
                Name = "Пермь"
            };

            City city2 = new City()
            {
                Id = 57405000000,
                Name = "Алексадровск"
            };

            City city3 = new City()
            {
                Id = 57408000000,
                Name = "Березники"
            };

            dbContext.AddRange(city1, city2, city3);

            // Add streets
            Street street1 = new Street()
            {
                Id = 59000001000063300,
                Name = "Ленина Улица",
                City = city1
            };

            Street street2 = new Street()
            {
                Id = 59000001000075300,
                Name = "Мира Улица",
                City = city1
            };

            Street street3 = new Street()
            {
                Id = 59000002000012100,
                Name = "Ленина Проспект",
                City = city3
            };

            Street street4 = new Street()
            {
                Id = 59000003000000100,
                Name = "Мехоношина Улица",
                City = city2
            };

            dbContext.Streets.AddRange(street1, street2, street3, street4);

            // Add countries
            Country country1 = new Country()
            {
                Id = 643,
                ShortName = "РОССИЯ"
            };

            Country country2 = new Country()
            {
                Id = 642,
                ShortName = "РУМЫНИЯ"
            };

            Country country3 = new Country()
            {
                Id = 646,
                ShortName = "РУАНДА"
            };

            dbContext.Countries.AddRange(country1, country2, country3);

            // Add Exhibit types
            ExhibitType exhibitType1 = new ExhibitType() { 
                Id = 9701100000,
                TypeName = "Картины, рисунки и пастели"
            };

            ExhibitType exhibitType2 = new ExhibitType()
            {
                Id = 9701900000,
                TypeName = "Коллажи и аналогичные декоративные изображения"
            };

            ExhibitType exhibitType3 = new ExhibitType()
            {
                Id = 9702000000,
                TypeName = "Подлинники гравюр, эстампов и литографий"
            };

            ExhibitType exhibitType4 = new ExhibitType()
            {
                Id = 9703000000,
                TypeName = "Подлинники скульптур и статуэток из любых материалов"
            };

            ExhibitType exhibitType5 = new ExhibitType()
            {
                Id = 9704000000,
                TypeName = "Марки почтовые или марки госпошлин, знаки почтовой оплаты гашеные," +
                " в т.ч. первого дня гашения, почтовые канцелярские принадлежности (гербовая бумага) " +
                "и аналогичные предметы"
            };

            ExhibitType exhibitType6 = new ExhibitType()
            {
                Id = 9705000000,
                TypeName = "Коллекции и предметы коллекционирования по зоологии, ботанике, " +
                "минералогии, анатомии, истории, археологии, палеонтологии, этнографии или нумизматике"
            };

            ExhibitType exhibitType7 = new ExhibitType()
            {
                Id = 9706000000,
                TypeName = "Антиквариат возрастом более 100 лет"
            };

            dbContext.ExhibitTypes.AddRange(
                exhibitType1, 
                exhibitType2, 
                exhibitType3, 
                exhibitType4, 
                exhibitType5, 
                exhibitType6, 
                exhibitType7
            );

            // Add sexes
            Sex male = new Sex()
            {
                Id = 1,
                Name = "Мужской",
            };

            Sex female = new Sex()
            {
                Id = 2,
                Name = "Женский",
            };

            dbContext.Sexes.AddRange(male, female);

            // Add Authors
            Author author1 = new Author()
            {
                FirstName = "Alex",
                LastName = "Alexov",
                Patronymic = "Alexeevixh",
                Pseudonym = "Leha",
                Birthday = new DateOnly(1999, 5, 5),
                Country = country1,
                Sex = male
            };

            Author author2 = new Author()
            {
                FirstName = "Daria",
                LastName = "Darieva",
                Patronymic = "Alexeevna",
                Pseudonym = "Dasha",
                Birthday = new DateOnly(1995, 6, 6),
                Country = country2,
                Sex = female
            };

            dbContext.Authors.AddRange(author1, author2);

            // Add addresses
            Address address1 = new Address()
            {
                Street = street1,
                HouseNumber = "1a"
            };

            Address address2 = new Address()
            {
                Street = street1,
                HouseNumber = "2"
            };

            Address address3 = new Address()
            {
                Street = street3,
                HouseNumber = "4"
            };

            dbContext.Addresses.AddRange(address1, address2, address3);

            // Add Warehouses
            Warehouse warehouse1 = new Warehouse()
            {
                Area = 100.2,
                Address = address1
            };

            Warehouse warehouse2 = new Warehouse()
            {
                Area = 200.4,
                Address = address2
            };

            Warehouse warehouse3 = new Warehouse()
            {
                Area = 300.6,
                Address = address3
            };

            dbContext.Warehouses.AddRange(warehouse1, warehouse2, warehouse3);

            // Add Exhibits
            Exhibit exhibit1 = new Exhibit()
            {
                Name = "Утро в лесу",
                CreationDate = new DateOnly(2015, 7, 20),
                ArrivalDate = new DateOnly(2020, 8, 21),
                ExhibitType = exhibitType1,
                Authors = new List<Author>() { author1, author2},
                Warehouse = warehouse1,
            };

            Exhibit exhibit2 = new Exhibit()
            {
                Name = "Гравюра",
                CreationDate = new DateOnly(2016, 7, 20),
                ArrivalDate = new DateOnly(2021, 8, 21),
                ExhibitType = exhibitType2,
                Authors = new List<Author>() { author1, author2 },
                Warehouse = warehouse1,
            };

            dbContext.Exhibits.AddRange(exhibit1, exhibit2);

            // Add exhibitions
            Exhibition exhibition1 = new Exhibition()
            {
                Name = "Название выставки 1",
                Address = address1,
                HostingDate = new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                ExpirationDate = new DateTime(2020, 2, 2, 14, 0, 0, DateTimeKind.Utc),
                Exhibits = new List<Exhibit>() {exhibit1, exhibit2},
            };

            Exhibition exhibition2 = new Exhibition()
            {
                Name = "Название выставки 2",
                Address = address2,
                HostingDate = new DateTime(2021, 2, 2, 13, 30, 0, DateTimeKind.Utc),
                ExpirationDate = new DateTime(2021, 3, 3, 15, 0, 0, DateTimeKind.Utc),
                Exhibits = new List<Exhibit>() { exhibit2 },
            };

            dbContext.Exhibitions.AddRange(exhibition1, exhibition2);

            dbContext.SaveChanges();
        }
    }
}
