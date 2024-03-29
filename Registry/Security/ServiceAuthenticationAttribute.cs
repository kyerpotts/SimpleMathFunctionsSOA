﻿using APIEndpoint;
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

namespace Registry.Security
{
    public class ServiceAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, new ReturnStatus { Status = "Denied", Reason = "Authentication Error" });
            }
            else
            {
                string authInfo = actionContext.Request.Headers.Authorization.Parameter;
                int token;
                if (int.TryParse(authInfo, out token))
                {
                    try
                    {
                        AuthServer valUser = new AuthServer();
                        //if(!(token == 1234567)) Testing to see if this works accurately.
                        if (!(valUser.Validate(token).Equals("validated")))
                        {
                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, new ReturnStatus { Status = "Denied", Reason = "Authentication Error 1" });
                        }
                    }
                    catch (FaultException<AuthenticationException>)
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, new ReturnStatus { Status = "Denied", Reason = "Authentication Error 2" });
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, new ReturnStatus { Status = "Denied", Reason = "Authentication Error 3" });
                }
            }
            base.OnAuthorization(actionContext);
        }
    }
}