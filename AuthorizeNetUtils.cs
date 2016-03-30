using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using Artemis.Web;

namespace Artemis.GetDowntown.GoPass.Components
{
    public static class AuthorizeNetUtils
    {
        //------// Methods \\-----------------------------------------------\\
        public static void RedirectToAuthorizeNetForm(HttpContext context, string loginID, string transactionKey, string description, bool relayResponse, string amount, string firstName, string lastName, string businessName, string address, string city, string zip, string phone, string fax, string email, List<string> lineItems)
        {
            RedirectToAuthorizeNetForm(context, loginID, transactionKey, description, relayResponse, amount, firstName, lastName, businessName, address, city, "MI", zip, "United States of America", null, null, email, lineItems);
        }


        public static void RedirectToAuthorizeNetForm(HttpContext context, string loginID, string transactionKey, string description, bool relayResponse, string amount, string firstName, string lastName, string businessName, string address, string city, string state, string zip, string country, string phone, string fax, string email, List<string> lineItems)
        {
            List<string> parameters = new List<string>();

            parameters.Add("x_login");
            parameters.Add(loginID);

            parameters.Add("x_type");
            parameters.Add("AUTH_CAPTURE");

            parameters.Add("x_amount"); // The total of all the line items.
            parameters.Add(amount);

            parameters.Add("x_show_form");
            parameters.Add("PAYMENT_FORM");

            parameters.Add("x_relay_response");
            parameters.Add(relayResponse.ToString().ToUpper());

            parameters.Add("x_relay_always");
            parameters.Add("FALSE");

            parameters.Add("x_invoice_num");
            parameters.Add(DateTime.Now.ToString("yyyyMMddhhmmss"));


            if (!String.IsNullOrWhiteSpace(description))
            {
                parameters.Add("x_description");
                parameters.Add(description);
            }


            if (!String.IsNullOrWhiteSpace(firstName))
            {
                parameters.Add("x_first_name");
                parameters.Add(firstName);
            }


            if (!String.IsNullOrWhiteSpace(lastName))
            {
                parameters.Add("x_last_name");
                parameters.Add(lastName);
            }


            if (!String.IsNullOrWhiteSpace(businessName))
            {
                parameters.Add("x_company");
                parameters.Add(businessName);
            }


            if (!String.IsNullOrWhiteSpace(address))
            {
                parameters.Add("x_address");
                parameters.Add(address);
            }


            if (!String.IsNullOrWhiteSpace(city))
            {
                parameters.Add("x_city");
                parameters.Add(city);
            }


            if (!String.IsNullOrWhiteSpace(state))
            {
                parameters.Add("x_state");
                parameters.Add(state);
            }


            if (!String.IsNullOrWhiteSpace(zip))
            {
                parameters.Add("x_zip");
                parameters.Add(zip);
            }


            if (!String.IsNullOrWhiteSpace(country))
            {
                parameters.Add("x_country");
                parameters.Add(country);
            }


            if (!String.IsNullOrWhiteSpace(phone))
            {
                parameters.Add("x_phone");
                parameters.Add(phone);
            }


            if (!String.IsNullOrWhiteSpace(fax))
            {
                parameters.Add("x_fax");
                parameters.Add(fax);
            }


            if (!String.IsNullOrWhiteSpace(email))
            {
                parameters.Add("x_email");
                parameters.Add(email);
            }

            parameters.Add("x_method");
            parameters.Add("CC");


            if (lineItems != null && lineItems.Count > 0)
            {
                foreach (string lineItem in lineItems)
                {
                    parameters.Add("x_line_item");
                    parameters.Add(lineItem);
                }
            }


            string invoice = DateTime.Now.ToString("yyyyMMddhhmmss");

            Random random = new Random();
            string sequence = (random.Next(0, 1000)).ToString();

            string timeStamp = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();

            string fingerprint = GenerateFingerprint(transactionKey, loginID + "^" + sequence + "^" + timeStamp + "^" + amount + "^");


            parameters.Add("x_fp_sequence");
            parameters.Add(sequence);

            parameters.Add("x_fp_timestamp");
            parameters.Add(timeStamp);

            parameters.Add("x_fp_hash");
            parameters.Add(fingerprint);


            string queryString = WebUtils.BuildQueryString(parameters.ToArray());


            context.Response.Redirect("https://secure.authorize.net/gateway/transact.dll" + queryString, false);
        }


        /// <summary>
        /// Some rules regarding Authorize.net parameters:
        ///   1. The product name MUST be up to 31 chars.
        ///   2. The product name MUST NOT contains some funny chars like curly quote, tm or copyright signs and so on. Try to use a short name containing only letters from the basic alphabet (A-Z a-z).
        ///   3. The product PRICE must be a positive number (>0)
        ///   4. The product QUANTITY must be a positive number (>0)
        /// </summary>
        public static string BuildLineItem(string itemID, string itemName, string itemDescription, string quantity, string price, bool taxable)
        {
            StringBuilder lineItemBuilder = new StringBuilder();


            if (String.IsNullOrWhiteSpace(itemID) || String.IsNullOrWhiteSpace(itemName) || String.IsNullOrWhiteSpace(quantity) || String.IsNullOrWhiteSpace(price))
            {
                // Break out of this method if either the ItemID, ItemName, Quantity, or Price are empty because they are required for every line item. This will have the 
                // effect of the line item being passed in to be skipped over.
                return null;
            }


            // Item ID
            lineItemBuilder.Append(itemID).Append("<|>");


            // Item Name
            lineItemBuilder.Append(itemName).Append("<|>");


            // Item Description
            if (!String.IsNullOrWhiteSpace(itemDescription))
            {
                lineItemBuilder.Append(itemDescription.Trim());
            }

            lineItemBuilder.Append("<|>");


            // Quantity
            lineItemBuilder.Append(quantity).Append("<|>");


            // Price
            lineItemBuilder.Append(price).Append("<|>");


            // Taxable
            if (taxable)
            {
                lineItemBuilder.Append("Y");
            }
            else
            {
                lineItemBuilder.Append("N");
            }


            return lineItemBuilder.ToString();
        }


        private static string GenerateFingerprint(string key, string value)
        {
            // The first two lines take the input values and convert them from strings to Byte arrays
            byte[] HMACkey = (new System.Text.ASCIIEncoding()).GetBytes(key);
            byte[] HMACdata = (new System.Text.ASCIIEncoding()).GetBytes(value);

            // create a HMACMD5 object with the key set
            HMACMD5 myhmacMD5 = new HMACMD5(HMACkey);

            //calculate the hash (returns a byte array)
            byte[] HMAChash = myhmacMD5.ComputeHash(HMACdata);

            //loop through the byte array and add append each piece to a string to obtain a hash string
            string fingerprint = "";
            for (int i = 0; i < HMAChash.Length; i++)
            {
                fingerprint += HMAChash[i].ToString("x").PadLeft(2, '0');
            }

            return fingerprint;
        }
        //------\\ Methods //-----------------------------------------------//
    }
}