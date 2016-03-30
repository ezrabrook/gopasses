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
    public class ProcessBikeHouseRenewal : BasePaymentFormProcessor
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
		private double BikeHouseRenewal;
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

				BikeHouseRenewal = 75.00;
                
				List<string> lineItems = new List<string>();
 
				string PaymentMethod = base.GetFieldValue(request, "PaymentValueField");
				if (PaymentMethod == "credit")
				{
					AuthorizeNetUtils.RedirectToAuthorizeNetForm(context, AuthNetApiLoginID, AuthNetTransactionKey, "Bike House Renewal Form", false, BikeHouseRenewal.ToString("C").Replace("$", String.Empty), null, null, null, null, null, null, null, "United States of America", null, null, null, lineItems);
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
