using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;

using Artemis.Mail;
using Artemis.Web;
using Artemis.Web.Dnn;
using Artemis.Web.Dnn.UI.Controls;

using Artemis.GetDowntown.GoPass.Components;

namespace Artemis.GetDowntown.GoPass
{
    public class ProcessExpressRide : BasePaymentFormProcessor
    {
        //------// Properties \\--------------------------------------------\\
        private string _authNetApiLoginID = null;
        public override string AuthNetApiLoginID
        {
            get
            {
				if (_authNetApiLoginID == null)
                {
                    _authNetApiLoginID = System.Configuration.ConfigurationManager.AppSettings["GDTAuthNetApiLoginID"];
                }

                return _authNetApiLoginID;
            }
        }


        private string _authNetTransactionKey = null;
        public override string AuthNetTransactionKey
        {
            get
            {
				if (_authNetTransactionKey == null)
                {
                    _authNetTransactionKey = System.Configuration.ConfigurationManager.AppSettings["GDTAuthNetTransactionKey"];
                }

                return _authNetTransactionKey;
            }
        }
        //------\\ Properties //--------------------------------------------//



        //------// Fields \\------------------------------------------------\\
		protected string ChelseaMonthQty;
		protected double ChelseaMonthQtyDouble;
		protected double ChelseaMonthTotal;
		protected string ChelseaTenQty;
		protected double ChelseaTenQtyDouble;
		protected double ChelseaTenTotal;
		protected string CantonMonthQty;
		protected double CantonMonthQtyDouble;
		protected double CantonMonthTotal;
		protected string CantonTenQty;
		protected double CantonTenQtyDouble;
		protected double CantonTenTotal;
		protected double GrandTotalDouble;
		protected double ChelseaMonth;
		protected double ChelseaTen;
		protected double CantonMonth;
		protected double CantonTen;
		private string firstName;
		private string lastName;
		private string address;
		private string city;
		private string state;
		private string phone;
		private string email;
		private string zip;
		private string businessName;
        //------\\ Fields //------------------------------------------------//



        //------// Methods \\-----------------------------------------------\\
        public override void ProcessRequest(HttpContext context)
        {
            try
            {
                HttpRequest request = context.Request;
				firstName = GetFieldValue(request, "FirstName");
				lastName = GetFieldValue(request, "LastName");
				address = GetFieldValue(request, "Address");
				city = GetFieldValue(request, "City");
				zip = GetFieldValue(request, "Zip");
				state  = GetFieldValue(request, "State");
				phone = GetFieldValue(request, "Phone");
				email = GetFieldValue(request, "Email");
				businessName = GetFieldValue(request, "BusinessName");

				ChelseaMonth = 62.50;
				ChelseaTen = 31.25;
				CantonMonth = 62.50;
				CantonTen = 31.25;
				
                ChelseaMonthQty = base.GetFieldValue(request, "ChelseaMonthQty");
				if (ChelseaMonthQty == "0")
				{
					ChelseaMonthQty = null;
				}
				if (!Double.TryParse(ChelseaMonthQty, out ChelseaMonthQtyDouble)) { ChelseaMonthQtyDouble = 0d; }
                ChelseaMonthTotal = ChelseaMonthQtyDouble * 62.50;
				
                ChelseaTenQty = base.GetFieldValue(request, "Chelsea10Qty");
				if (ChelseaTenQty == "0")
				{
					ChelseaTenQty = null;
				}
				if (!Double.TryParse(ChelseaTenQty, out ChelseaTenQtyDouble)) { ChelseaTenQtyDouble = 0d; }
                ChelseaTenTotal = ChelseaTenQtyDouble * 31.25;
				
				CantonMonthQty = base.GetFieldValue(request, "CantonMonthQty");
				if (CantonMonthQty == "0")
				{
					CantonMonthQty = null;
				}
				if (!Double.TryParse(CantonMonthQty, out CantonMonthQtyDouble)) { CantonMonthQtyDouble = 0d; }
                CantonMonthTotal = CantonMonthQtyDouble * 62.50;
				
                CantonTenQty = base.GetFieldValue(request, "Canton10Qty");
				if (CantonTenQty == "0")
				{
					CantonTenQty = null;
				}
				if (!Double.TryParse(CantonTenQty, out CantonTenQtyDouble)) { CantonTenQtyDouble = 0d; }
                CantonTenTotal = CantonTenQtyDouble * 31.25;
				
				
                
				//System.IO.File.WriteAllText("D:\\\\inetpub\\wwwroot\\org.theride.www\\debug.txt", base.GetFieldValue(request, "PaymentMethod"));
				string grandTotal = base.GetFieldValue(request, "GoPassTotal").Replace("$", String.Empty).Trim();
				if (!Double.TryParse(grandTotal, out GrandTotalDouble)) { GrandTotalDouble = 0d; }
				
				string ChelseaMonthTotalString = ChelseaMonthTotal.ToString();
				string ChelseaTenTotalString = ChelseaTenTotal.ToString();
				string ChelseaMonthString = ChelseaMonth.ToString();
				string ChelseaTenString = ChelseaTen.ToString();
				string CantonMonthTotalString = CantonMonthTotal.ToString();
				string CantonTenTotalString = CantonTenTotal.ToString();
				string CantonMonthString = CantonMonth.ToString();
				string CantonTenString = CantonTen.ToString();
				
				List<string> lineItems = new List<string>();
 
				string product1 = AuthorizeNetUtils.BuildLineItem("0001", "Chelsea Month Pass", " ", ChelseaMonthQty, ChelseaMonthString, false);
				if (product1 != null)
				{
					lineItems.Add(product1);
				}
				
				string product2 = AuthorizeNetUtils.BuildLineItem("0002", "Chelsea 10 Day Pass", " ", ChelseaTenQty, ChelseaTenString, false);
				if (product2 != null)
				{
					lineItems.Add(product2);
				}
				
				string product3 = AuthorizeNetUtils.BuildLineItem("0003", "Canton Month Pass", " ", CantonMonthQty, CantonMonthString, false);
				if (product3 != null)
				{
					lineItems.Add(product3);
				}
				
				string product4 = AuthorizeNetUtils.BuildLineItem("0004", "Canton 10 Day Pass", " ", CantonTenQty, CantonTenString, false);
				if (product4 != null)
				{
					lineItems.Add(product4);
				}
				
				string PaymentMethod = base.GetFieldValue(request, "PaymentValueField");
				if (PaymentMethod == "credit")
				{
					AuthorizeNetUtils.RedirectToAuthorizeNetForm(context, AuthNetApiLoginID, AuthNetTransactionKey, "GoPass Order Form", false, GrandTotalDouble.ToString("C").Replace("$", String.Empty), firstName, lastName, businessName, address, city, state, zip, "United States of America", phone, null, email, lineItems);
				}
				else
				{
					context.Response.Redirect("/gopass/OrderExpressRidePasses/ThankYou", false);
				}
            }
            catch (System.Exception exception)
            {
                Exceptions.LogException(exception);
                base.RedirectForError(context, exception);
            }
        }
        //------\\ Methods //-----------------------------------------------//
    }
}
