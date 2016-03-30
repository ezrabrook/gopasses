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
    public class ProcessAdditionalGoPass : BasePaymentFormProcessor
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
		protected string AdditionalPassQty;
		protected double AdditionalPassQtyDouble;
		protected double AdditionalPassTotal;
		protected double GrandTotalDouble;
		protected double AdditionalPass;
        //------\\ Fields //------------------------------------------------//



        //------// Methods \\-----------------------------------------------\\
        public override void ProcessRequest(HttpContext context)
        {
            try
            {
                HttpRequest request = context.Request;


				AdditionalPass = 10.00;
                AdditionalPassQty = base.GetFieldValue(request, "AdditionalPassQty");
				if (AdditionalPassQty == "0")
				{
					AdditionalPassQty = null;
				}
				if (!Double.TryParse(AdditionalPassQty, out AdditionalPassQtyDouble)) { AdditionalPassQtyDouble = 0d; }
                AdditionalPassTotal = AdditionalPassQtyDouble * 10.00;
                
				
				string grandTotal = base.GetFieldValue(request, "AdditionalTotal").Replace("$", String.Empty).Trim();
				if (!Double.TryParse(grandTotal, out GrandTotalDouble)) { GrandTotalDouble = 0d; }
				
				string AdditionalPassTotalString = AdditionalPassTotal.ToString();
				string AdditionalPassString = AdditionalPass.ToString();
				
				List<string> lineItems = new List<string>();
 
				string product1 = AuthorizeNetUtils.BuildLineItem("0001", "Additional Passes Requested", " ", AdditionalPassQty, AdditionalPassString, false);
				if (product1 != null)
				{
					lineItems.Add(product1);
				}
				
				string PaymentMethod = base.GetFieldValue(request, "PaymentValueField");
				if (PaymentMethod == "credit")
				{
					AuthorizeNetUtils.RedirectToAuthorizeNetForm(context, AuthNetApiLoginID, AuthNetTransactionKey, "Additional GoPass Order Form", false, GrandTotalDouble.ToString("C").Replace("$", String.Empty), null, null, null, null, null, null, null, "United States of America", null, null, null, lineItems);
				}
				else
				{
					context.Response.Redirect("/gopass/Orderadditionalgopasses/thankyou", false);
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
