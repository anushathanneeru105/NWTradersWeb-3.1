using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace NWTradersWeb.Models
{
    public class OrderMetadata
    {

        [DisplayFormat(DataFormatString = "{0: MMM dd, yyyy}")]
        [Display(Name = "Order Placed On: ")]
        public Nullable<System.DateTime> OrderDate { get; set; }

        [DisplayFormat(DataFormatString = "{0: MMM dd, yyyy}")]
        [Display(Name = "Order Required On: ")]
        public Nullable<System.DateTime> RequiredDate { get; set; }

        [DisplayFormat(DataFormatString = "{0: MMM dd, yyyy}")]
        [Display(Name = "Order Shipped On: ")]
        public Nullable<System.DateTime> ShippedDate { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0: $#,##0.00}")]
        [Display(Name = "Order Total")]
        public decimal orderTotal { get; set; }

        public Nullable<int> ShipVia { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0: $#,##0.00}")]
        public Nullable<decimal> Freight { get; set; }

        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }

    }

    public class CategoryMetaData
    {
        [Display(Name = "Product Category ")]
        public string CategoryName { get; set; }
    }

    /// Annotations MetaData and Decorations for EmpCode.
    public class CustomerMetadata
    {

        /// <summary>
        /// User Name
        /// Required, Minimum - 5 characters
        /// </summary>
        [Display(Name = "Customer ID")]
        [Required(ErrorMessage = "The Customer ID is required")]
        [StringLength(5, MinimumLength = 3,
            ErrorMessage = "Customer ID must have atleast 3 characters")]
        public string CustomerID;

        /// <summary>
        /// Company Name- 
        /// Required, Minimum - 5 characters
        /// </summary>
        [Display(Name = "The Company Name")]
        [Required(ErrorMessage = "The Company Name is required")]
        [StringLength(40,
            MinimumLength = 5,
            ErrorMessage = "Company Name must have atleast 5 characters and upto 40")]
        public string CompanyName;

        [Display(Name = "Contact Name")]
        [Required(ErrorMessage = "The Contact Name is required")]
        [StringLength(30)]
        public string ContactName;

        [Display(Name = "Contact Title")]
        [StringLength(30)]
        public string ContactTitle;

        [Display(Name = "Address")]
        [StringLength(60)]
        public string Address;

        [Display(Name = "City")]
        [StringLength(15)]
        public string City;

        [Display(Name = "Region")]
        [StringLength(15)]
        public string Region;

        // Not -Required fields:
        [Display(Name = "Postal Code")]
        [StringLength(10)]
        public string PostalCode;

        [Display(Name = "Country")]
        [StringLength(15)]
        public string Country;

        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}",
            ErrorMessage = "Please enter a valid Phone number in the format (123) 456-7890")]
        [StringLength(24)]
        public string Phone;

        [Display(Name = "Fax")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}",
            ErrorMessage = "Please enter a valid Phone number in the format (123) 456-7890")]
        [StringLength(24)]
        public string Fax;
    }

    public class Order_DetailMetadata
    {
        [Required(ErrorMessage = "The unit price cannot be blank")]
        [Range(0, 10000, ErrorMessage = "Enter a price between 0 and 10000.00")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0: $#,##0.00}")]
        [Display(Name = "Price Per Unit")]
        public decimal UnitPrice { get; set; }


        [Required(ErrorMessage = "The quantity cannot be be blank")]
        [Range(0, 10000, ErrorMessage = "Enter a quantity between 0 and 10000.00")] // Edit this for returns
        [Display(Name = "Quantity Ordered")]
        public short Quantity { get; set; }


        [Required(ErrorMessage = "The unit price cannot be blank")]
        [Range(0, 10000, ErrorMessage = "Enter a discount between 0 and 10000.00")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0: $#,##0.00}")]
        [Display(Name = "Discount applied (Per Unit Price)")]
        [DefaultValue(0.00)]
        public float Discount { get; set; }


        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0: $#,##0.00}")]
        public decimal Total { get; set; }

    }

    public class ProductMetadata
    {
        public int ProductID { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<int> CategoryID { get; set; }


        /// <summary>
        /// Company Name- 
        /// Required, Minimum - 5 characters
        /// </summary>
        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "The Product Name is required")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Product Name must have atleast 3 and upto 40 characters")]
        public string ProductName { get; set; }


        /// <summary>
        /// Company Name- 
        /// Required, Minimum - 5 characters
        /// </summary>
        [Display(Name = "Quantity Per Unit")]
        // Not [Required(ErrorMessage = "The Product Name is required")]
        [StringLength(20, ErrorMessage = "Quantity per unit cannot have more than 20 characters")]
        public string QuantityPerUnit { get; set; }


        [Required(ErrorMessage = "The unit price cannot be blank")]
        [Range(0, 10000, ErrorMessage = "Enter a price between 0 and 10000.00")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}")]
        [Display(Name = "Price Per Unit")]
        public Nullable<decimal> UnitPrice { get; set; }

        [Display(Name = "Units in Stock")]
        [Range(0, 10000, ErrorMessage = "Enter the number of units, between 0 and 10,000 ")]
        public Nullable<short> UnitsInStock { get; set; }

        [Display(Name = "Units on Order")]
        [Range(0, 10000, ErrorMessage = "Enter the units on Order, between 0 and 10,000 ")]
        public Nullable<short> UnitsOnOrder { get; set; }


        [Display(Name = "ReOrder level")]
        [Range(0, 10000, ErrorMessage = "Enter the # of units, between 0 and 10,000, when an automatic re-order is triggered ")]
        public Nullable<short> ReorderLevel { get; set; }

        [Display(Name = "Product Discontinued?")]
        [DefaultValue(false)]
        public bool Discontinued { get; set; }


    }

    public class SupplierMetaData
    {

        [Display(Name = "Product Supplier")]
        public string CompanyName { get; set; }

    }

    public class NWTradersMetadata
    {
    }
}