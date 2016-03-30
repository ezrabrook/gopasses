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
    public class ProcessBikeLockerRenewal : BasePaymentFormProcessor
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
		private double BikeLockerRenewal;
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
				
				
				

				BikeLockerRenewal = 60.00;
                
				List<string> lineItems = new List<string>();
 
				string PaymentMethod = base.GetFieldValue(request, "PaymentValueField");
				if (PaymentMethod == "credit")
				{
						AuthorizeNetUtils.RedirectToAuthorizeNetForm(context, AuthNetApiLoginID, AuthNetTransactionKey, "Bike Locker Renewal Form", false, BikeLockerRenewal.ToString("C").Replace("$", String.Empty), firstName, lastName, businessName, address, city, state, zip, "United States of America", phone, null, email, lineItems);
			
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
