using APIEndpoint;
using AuthenticatorInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ServiceProvider.Security
{
    public class ServiceAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                //actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, "No authentication token supplied");
            }
            else
            {
                string authInfo = actionContext.Request.Headers.Authorization.Parameter;
                int token;
                if (int.TryParse(authInfo, out token))
                {
                    try
                    {
                        ValidateUser valUser = new ValidateUser();
                        if (!(valUser.ValidateUserByAuthServer(token).Equals("validated")))
                        {
                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, Newtonsoft.Json.JsonConvert.SerializeObject(new { Status = "Denied", Reason = "Authentication Error" }));
                            //Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(token.ToString()), null);
                        }
                    }
                    catch (FaultException<AuthenticationException>)
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, Newtonsoft.Json.JsonConvert.SerializeObject(new { Status = "Denied", Reason = "Authentication Error" }));
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, Newtonsoft.Json.JsonConvert.SerializeObject(new { Status = "Denied", Reason = "Authentication Error" }));
                }
            }
            base.OnAuthorization(actionContext);
        }
    }
}