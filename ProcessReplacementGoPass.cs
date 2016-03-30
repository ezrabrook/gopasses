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
    public class ProcessReplacementGoPass : BasePaymentFormProcessor
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
		protected string LostQty;
		protected string LostTotal;
		protected double LostTotalDouble;
		protected string StolenQty;
		protected string StolenTotal;
		protected double StolenTotalDouble;
		protected string DamagedQty;
		protected string DamagedTotal;
		protected double DamagedTotalDouble;
		protected string ReplacementQty;
		protected string ReplacementTotal;
		protected double ReplacementTotalDouble;
		protected double Lost;
		protected double Stolen;
		protected double Damaged;
		protected double Replacement;
        //------\\ Fields //------------------------------------------------//



        //------// Methods \\-----------------------------------------------\\
        public override void ProcessRequest(HttpContext context)
        {
            try
            {
                HttpRequest request = context.Request;


				Lost = 25.00;
				Stolen = 0.00;
				Damaged = 0.00;
				Replacement = 15.00;
                LostQty = base.GetFieldValue(request, "LostQty");
				if (LostQty == "0")
				{
					LostQty = null;
				}
                LostTotal = base.GetFieldValue(request, "LostTotal").Replace("$", String.Empty).Trim();
				if (!Double.TryParse(LostTotal, out LostTotalDouble)) { LostTotalDouble = 0d; }
                StolenQty = base.GetFieldValue(request, "StolenQty");
				if (StolenQty == "0")
				{
					StolenQty = null;
				}
                StolenTotal = base.GetFieldValue(request, "StolenTotal").Replace("$", String.Empty).Trim();
				if (!Double.TryParse(StolenTotal, out StolenTotalDouble)) { StolenTotalDouble = 0d; }
				DamagedQty = base.GetFieldValue(request, "DamagedQty");
				if (DamagedQty == "0")
				{
					DamagedQty = null;
				}
                DamagedTotal = base.GetFieldValue(request, "DamagedTotal").Replace("$", String.Empty).Trim();
				if (!Double.TryParse(DamagedTotal, out DamagedTotalDouble)) { DamagedTotalDouble = 0d; }
                ReplacementQty = base.GetFieldValue(request, "ReplacementQty");
				if (ReplacementQty == "0")
				{
					ReplacementQty = null;
				}
                ReplacementTotal = base.GetFieldValue(request, "ReplacementTotal").Replace("$", String.Empty).Trim();
				if (!Double.TryParse(ReplacementTotal, out ReplacementTotalDouble)) { ReplacementTotalDouble = 0d; }
                
				
				double grandTotal = LostTotalDouble + StolenTotalDouble + DamagedTotalDouble + ReplacementTotalDouble;
				string grandTotalString = grandTotal.ToString();
				string LostString = Lost.ToString();
				string StolenString = Stolen.ToString();
				string DamagedString = Damaged.ToString();
				string ReplacementString = Replacement.ToString();
				
				List<string> lineItems = new List<string>();
 
				string product1 = AuthorizeNetUtils.BuildLineItem("0001", "Lost/Stolen Pass", " ", LostQty, LostString, false);
				if (product1 != null)
				{
					lineItems.Add(product1);
				}
				
				string product2 = AuthorizeNetUtils.BuildLineItem("0002", "Stolen, with Police Report", " ", StolenQty, StolenString, false);
				if (product2 != null)
				{
					lineItems.Add(product2);
				}
				
				string product3 = AuthorizeNetUtils.BuildLineItem("0003", "Damaged", " ", DamagedQty, DamagedString, false);
				if (product3 != null)
				{
					lineItems.Add(product3);
				}
				
				string product4 = AuthorizeNetUtils.BuildLineItem("0004", "Replacement Pass", " ", ReplacementQty, ReplacementString, false);
				if (product4 != null)
				{
					lineItems.Add(product4);
				}

				string PaymentMethod = base.GetFieldValue(request, "PaymentValueField");
				if (PaymentMethod == "credit")
				{
					AuthorizeNetUtils.RedirectToAuthorizeNetForm(context, AuthNetApiLoginID, AuthNetTransactionKey, "Replacement GoPass Order Form", false, grandTotalString, null, null, null, null, null, null, null, "United States of America", null, null, null, lineItems);
				}
				else
				{
					context.Response.Redirect("/gopass/Orderreplacementgopasses/thankyou", false);
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
