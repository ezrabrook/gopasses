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
    public class ProcessGoPass : BasePaymentFormProcessor
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
		private string FullTimeQty;
		private double FullTimeQtyDouble;
		private double FullTimeTotal;
		private string PartTimeQty;
		private double PartTimeQtyDouble;
		private double PartTimeTotal;
		private double GrandTotalDouble;
		private double FullTime;
		private double PartTime;
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

				FullTime = 10.00;
				PartTime = 10.00;
                FullTimeQty = base.GetFieldValue(request, "FullTimeQty");
				if (FullTimeQty == "0")
				{
					FullTimeQty = null;
				}
				if (!Double.TryParse(FullTimeQty, out FullTimeQtyDouble)) { FullTimeQtyDouble = 0d; }
                FullTimeTotal = FullTimeQtyDouble * 10.00;
                PartTimeQty = base.GetFieldValue(request, "PartTimeQty");
				if (PartTimeQty == "0")
				{
					PartTimeQty = null;
				}
				if (!Double.TryParse(PartTimeQty, out PartTimeQtyDouble)) { PartTimeQtyDouble = 0d; }
                PartTimeTotal = PartTimeQtyDouble * 10.00;
                
				//System.IO.File.WriteAllText("D:\\\\inetpub\\wwwroot\\org.theride.www\\debug.txt", base.GetFieldValue(request, "PaymentMethod"));
				string grandTotal = base.GetFieldValue(request, "GoPassTotal").Replace("$", String.Empty).Trim();
				if (!Double.TryParse(grandTotal, out GrandTotalDouble)) { GrandTotalDouble = 0d; }
				
				string FullTimeTotalString = FullTimeTotal.ToString();
				string PartTimeTotalString = PartTimeTotal.ToString();
				string FullTimeString = FullTime.ToString();
				string PartTimeString = PartTime.ToString();
				
				List<string> lineItems = new List<string>();
 
				string product1 = AuthorizeNetUtils.BuildLineItem("0001", "Full Time Employees", " ", FullTimeQty, FullTimeString, false);
				if (product1 != null)
				{
					lineItems.Add(product1);
				}
				
				string product2 = AuthorizeNetUtils.BuildLineItem("0002", "Part Time Employees", " ", PartTimeQty, PartTimeString, false);
				if (product2 != null)
				{
					lineItems.Add(product2);
				}
				
				string PaymentMethod = base.GetFieldValue(request, "PaymentValueField");
				if (PaymentMethod == "credit")
				{
					AuthorizeNetUtils.RedirectToAuthorizeNetForm(context, AuthNetApiLoginID, AuthNetTransactionKey, "GoPass Order Form", false, GrandTotalDouble.ToString("C").Replace("$", String.Empty), firstName, lastName, businessName, address, city, state, zip, "United States of America", phone, null, email, lineItems);
				}
				else
				{
					context.Response.Redirect("/gopass/ThankYou", false);
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
