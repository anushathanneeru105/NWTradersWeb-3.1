using System.ComponentModel.DataAnnotations;
using System.Linq;



namespace NWTradersWeb.Models
{



    [MetadataType(typeof(OrderMetadata))]
    public partial class Order
    {
        public decimal orderTotal
        {
            get
            { return this.Order_Details.Sum(od => od.Total); }
        }

        public decimal TotalSales
        {
            get
            { return this.Order_Details.Sum(od => od.Total); }
        }

        public void AddToCart(Product productToAdd, short quantity = 1)
        {
            Order_Detail odWithProduct = null;

            odWithProduct = this.Order_Details.
                Where(od => od.ProductID == productToAdd.ProductID).
                Select(od => od).
                FirstOrDefault();

            if (odWithProduct == null)
            {
                // If the order detail is not found, then it doesnt exist in the database - 
                // Make a new order detail using the product and the current order (this)
                odWithProduct = new Order_Detail
                {
                    ProductID = productToAdd.ProductID,
                    Product=productToAdd,
                    OrderID = this.OrderID,

                    Discount = 0F,
                    UnitPrice = productToAdd.UnitPrice.Value,
                    Quantity = quantity
                };

                // Add the new order detail to the current order.
                this.Order_Details.Add(odWithProduct);

            }
            // Otherwise, increment quantity.
            else
            {
                odWithProduct.Quantity += quantity;
            }
            return;
        }

        public bool ContainsProduct(Product productToAdd)
        {
            foreach (Order_Detail item in Order_Details)
            {
                if (item.ProductID == productToAdd.ProductID)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// This function is no longer needed or useed because we are not displaying strings in Web Applications.
        /// </summary>
        /// <returns></returns>
        public string OrderInformation()
        {

            string orderInformation = "";

            orderInformation += "Order ID :" + "\t" + this.OrderID + "\n";
            orderInformation += "Order Date :" + "\t" + this.OrderDate.Value.ToShortDateString() + "\n";
            orderInformation += "--------------------------------------------------------------------------------------- \n";

            foreach (Order_Detail theOrderDetail in this.Order_Details)
            {
                orderInformation += theOrderDetail.OrderDetailInformation() + "\n";
            }
            return orderInformation;


        }
    }
}
