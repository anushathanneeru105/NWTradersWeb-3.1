using System;
using System.Collections.Generic;
using System.Linq;

namespace NWTradersWeb.Models
{
    public class NWTradersUtilities
    {

        private static NorthwindEntities nwEntities = new NorthwindEntities();

        public static List<string> AllCountries()
        {
            List<string> allCountries = new List<string>();
            allCountries =
                nwEntities.
                Customers.
                Select(c => c.Country).
                Distinct().
                ToList();

            return allCountries;
        }

        public static List<string> AllCompanyNames()
        {
            return nwEntities.
                Customers.
                OrderBy(c => c.CompanyName).
                Select(c => c.CompanyName).
                Distinct().
                ToList();
        }

        public static List<String> AllTitles()
        {
            List<String> allTitles = new List<string>();

            // From the customers table, 
            // where the contact title is not null or empty, 
            // select every contact title 
            // keep only the distinct ones 
            // then convert it to a list.
            allTitles = nwEntities.Customers.
                Where(c => string.IsNullOrEmpty(c.ContactTitle) == false).
                Select(c => c.ContactTitle).
                Distinct().
                ToList();

            return allTitles;
        }

        public static List<String> AllRegions()
        {
            List<String> allTitles = new List<string>();

            // From the customers table, 
            // where the contact title is not null or empty, 
            // select every contact title 
            // keep only the distinct ones 
            // then convert it to a list.
            allTitles = nwEntities.Customers.
                Where(c => string.IsNullOrEmpty(c.Region) == false).
                Select(c => c.Region).
                OrderBy(r => r).
                Distinct().
                ToList();

            return allTitles;
        }

        public static List<String> AllEmployeeLastNames()
        {
            return nwEntities.Employees.
                Select(c => c.LastName).
                OrderBy(r => r).
                Distinct().
                ToList();
        }

        public static List<String> AllProductCategories()
        {
            List<String> allProductCategories = new List<string>();

            // From the customers table, 
            // where the contact title is not null or empty, 
            // select every contact title 
            // keep only the distinct ones 
            // then convert it to a list.
            allProductCategories = nwEntities.Categories.
                Where(c => string.IsNullOrEmpty(c.CategoryName) == false).
                Select(c => c.CategoryName).
                OrderBy(name => name).
                Distinct().
                ToList();

            return allProductCategories;
        }

        public static IEnumerable<Category> AllCategories()
        {
            return nwEntities.Categories.OrderBy(c => c.CategoryID);
        }

        /// <summary>
        /// Will generate 3 or specified number of random characters
        /// </summary>
        /// <param name="length"></param>
        public static string GenerateRandomUpperCaseCharacters(int length = 3)
        {
            const string possibleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string randomString = "";
            int randomLocation;
            char randomCharacter;

            Random random = new Random();

            // run this "length" times.
            for (int i = 0; i < length; i++)
            {
                // Get a random number no bigger than the number of possible characters A .. Z
                randomLocation = random.Next(possibleCharacters.Length);

                // Get the character at the random location in possible characters.
                randomCharacter = possibleCharacters.ToCharArray()[randomLocation];

                // Attach the random character to the randomString
                randomString += randomCharacter;

                // repeat "length" times.
            }

            // And return the random string.
            return randomString;
        }

        public static List<string> itemsPerPage
        {
            get { return (new List<string> { "10", "15", "25", "50", "All" }); }
        }


        /// <summary>
        /// Returns the beginning date of the Year supplied, or of the current year if blank
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public static DateTime BeginningOfYear(string Year="")
        {

            int year;

            if (string.IsNullOrEmpty(Year))
                year = DateTime.Now.Year;
            else
                year = int.Parse(Year);

            return new DateTime(year, 1, 1);

        }

        /// <summary>
        /// Returns the ENDING date of the Year supplied, or of the current year if blank
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public static DateTime EndOfYear(string Year = "")
        {
            return BeginningOfYear(Year).AddYears(1).AddDays(-1);
        }

        /// <summary>
        /// returns the month name of the MonthNumber supplied
        /// </summary>
        /// <param name="MonthNumber"></param>
        /// <returns></returns>
        public static string MonthName(int MonthNumber)
        {
            return new DateTime(DateTime.Now.Year, MonthNumber, 1).ToString("MMMM");
        }

        public static List<string> BeginToEndOrderYears()
        {
            int FirstYear = nwEntities.Orders.OrderBy(o => o.OrderDate.Value.Year).First().OrderDate.Value.Year;
            int LastYear = nwEntities.Orders.OrderByDescending(o => o.OrderDate.Value.Year).First().OrderDate.Value.Year;

            List<string> Years= new List<string>();
            for (int i = LastYear; i >= FirstYear; i--)
            {
                Years.Add(i.ToString());
            }

            return Years;
        }

    }
}
