using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public abstract class BasePaymentFormProcessor : IHttpHandler
    {
        //------// Properties \\--------------------------------------------\\
        public virtual bool IsReusable
        {
            get { return false; }
        }


        public abstract string AuthNetApiLoginID { get; }
        public abstract string AuthNetTransactionKey { get; }
        //------\\ Properties //--------------------------------------------//



        //------// Fields \\------------------------------------------------\\
        
        //------\\ Fields //------------------------------------------------//



        //------// Methods \\-----------------------------------------------\\
        public abstract void ProcessRequest(HttpContext context);


        protected virtual string GetFieldValue(HttpRequest request, string fieldShortName)
        {
            return (request.Params[fieldShortName] ?? String.Empty);
        }


        protected virtual string GetFieldValueFromSession(HttpContext context, string fieldShortName)
        {
            return ((string)context.Session[fieldShortName] ?? String.Empty);
        }


        protected virtual void RedirectForError(HttpContext context, System.Exception exception)
        {
            context.Response.Redirect(context.Request.UrlReferrer + "?error=" + context.Server.UrlEncode(exception.Message), false);
        }
        //------\\ Methods //-----------------------------------------------//
    }
}
