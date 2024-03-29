﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;
using MySql.Data.MySqlClient;
using System.Web.Http.Cors;

namespace DirtBnBWebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HostController : ApiController
    {
        [Route("api/hosts")]
        [HttpGet]
        public HttpResponseMessage GetHosts()
        {
            HostPersistenceService hostPersistenceService = new HostPersistenceService();
            var hosts = hostPersistenceService.GetHosts();
            HttpResponseMessage response;
            if (hosts == null || hosts.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No hosts found.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, hosts);
            return response;
        }

        [Route("api/hosts/{id}/{password}")]
        [HttpGet]
        public HttpResponseMessage GetHost(long id, string password)
        {
            HostPersistenceService hostPersistenceService = new HostPersistenceService();
            Host host = hostPersistenceService.GetHost(id);
            HttpResponseMessage response;
            if (host == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Host not found.");
                return response;
            }
            if (password != host.password)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "Incorrect password. Please try logging again.");
                return response;
            }

            return Request.CreateResponse(HttpStatusCode.OK, host);
        }

        [Route("api/hosts/")]
        [HttpPost]
        public HttpResponseMessage CreateHost([FromBody]Host host)
        {
            HostPersistenceService hostPersistenceService = new HostPersistenceService();
            HttpResponseMessage response;

            if (string.IsNullOrEmpty(host.name)
                || string.IsNullOrEmpty(host.phoneNumber)
                || string.IsNullOrEmpty(host.emailAddress)
                || string.IsNullOrEmpty(host.password))
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "All fields are mandatory. Please try again.");
                return response;
            }

            var id = hostPersistenceService.SaveHost(host);
            if (id < 0)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "A user with the same email address has already been created.");
                return response;
            }
            host.userID = id;
            response = Request.CreateResponse(HttpStatusCode.Created, host);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("hosts/{0}", id));
            return response;
        }

        [Route("api/hosts/{id}")]
        [HttpPatch]
        [HttpPut]
        public HttpResponseMessage UpdateHost(long id, [FromBody]Host host)
        {
            HostPersistenceService hostPersistenceService = new HostPersistenceService();
            bool userExists = false;
            HttpResponseMessage response;
            try
            {
                userExists = hostPersistenceService.UpdateHost(id, host); if (userExists)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, host);
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, "Guest not found.");
                    return response;
                }
            }
            catch (MySqlException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "The email is already in use. Please try again.");
                return response;
            }
        }

        [Route("api/hosts/{id}")]
        [HttpDelete]
        public HttpResponseMessage DeleteHost(long id)
        {
            HostPersistenceService hostPersistenceService = new HostPersistenceService();
            bool userExists = false;
            userExists = hostPersistenceService.DeleteHost(id);

            HttpResponseMessage response;
            if (userExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, "Host deleted.");
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Host not found.");
                return response;
            }
        }
    }
}