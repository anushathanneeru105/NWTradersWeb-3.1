using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NWTradersWeb.Models
{


    [MetadataType(typeof(Order_DetailMetadata))]
    public partial class Order_Detail
    {

        public string OrderDetailInformation()
        {
            string orderDetailInformation = "";
            orderDetailInformation +=
                this.Product.ProductName + "\t" + "\t" +
               this.Quantity + "\t" + "\t" +
                this.UnitPrice.ToString("C") + "\t" + "\t" +
                this.Discount.ToString("C") + "\t" + "\t" +
                Total.ToString("C") + "\t";
                
            return orderDetailInformation;
        }

        public decimal Total
        {
            get
            {                return ((UnitPrice - (decimal) Discount) * Quantity);}
        }
    }

}
