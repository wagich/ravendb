﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Raven.Database.Server.Controllers
{
	public abstract class BundlesApiController : RavenDbApiController
	{
		public abstract string BundleName { get; }

		public override async Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
		{
			InnerInitialization(controllerContext);
			DocumentDatabase db;
            try
            {
                db = await DatabasesLandlord.GetDatabaseInternal(DatabaseName);
            }
            catch (Exception e)
            {
                return GetMessageWithObject(new
                {
                    Error = "Could not open database named: " + DatabaseName + ", " + e.Message
                }, HttpStatusCode.ServiceUnavailable);
            }
			if (!db.Configuration.ActiveBundles.Any(activeBundleName => activeBundleName.Equals(BundleName,StringComparison.InvariantCultureIgnoreCase)))
			{
				return GetMessageWithObject(new
				{
					Error = "Could not figure out what to do"
				}, HttpStatusCode.BadRequest);
			}

			return await base.ExecuteAsync(controllerContext, cancellationToken);
		}
	}
}