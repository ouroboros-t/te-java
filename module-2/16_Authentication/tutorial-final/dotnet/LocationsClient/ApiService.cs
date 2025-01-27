﻿using System;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Authenticators;
using Locations.Models;

namespace Locations
{
    public class ApiService
    {
        const string API_BASE = "https://localhost:44387";
        const string API_URL = API_BASE + "/locations";
        private ApiUser user = new ApiUser();
        private static RestClient authClient = new RestClient();

        public bool LoggedIn { get { return !string.IsNullOrWhiteSpace(user.Token); } }

        public List<Location> GetAllLocations()
        {
            RestRequest request = new RestRequest(API_URL);
            IRestResponse<List<Location>> response = authClient.Get<List<Location>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                //response not received
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                //response non-2xx
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return null;
            }
            else
            {
                //success
                return response.Data;
            }
        }

        public Location GetDetailsForLocation(int locationId)
        {
            RestRequest requestOne = new RestRequest(API_URL + "/" + locationId);
            IRestResponse<Location> response = authClient.Get<Location>(requestOne);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                //response not received
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                //response non-2xx
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return null;
            }
            else
            {
                //success
                return response.Data;
            }
        }

        public Location AddLocation(Location newLocation)
        {
            RestRequest request = new RestRequest(API_URL);
            request.AddJsonBody(newLocation);
            IRestResponse<Location> response = authClient.Post<Location>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                //response not received
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                //response non-2xx
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return null;
            }
            else
            {
                //success
                Console.WriteLine("Location successfully added");
                return response.Data;
            }
        }

        public Location UpdateLocation(Location locationToUpdate)
        {
            RestRequest request = new RestRequest(API_URL + "/" + locationToUpdate.Id);
            request.AddJsonBody(locationToUpdate);
            IRestResponse<Location> response = authClient.Put<Location>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                //response not received
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                //response non-2xx
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return null;
            }
            else
            {
                //success
                Console.WriteLine("Location successfully updated");
                return response.Data;
            }
        }

        public void DeleteLocation(int locationId)
        {
            RestRequest request = new RestRequest(API_URL + "/" + locationId);
            IRestResponse response = authClient.Delete(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                //response not received
                Console.WriteLine("An error occurred communicating with the server.");
            }
            else if (!response.IsSuccessful)
            {
                //response non-2xx
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
            }
            else
            {
                //success
                Console.WriteLine("Location successfully deleted");
            }
        }

        public bool Login(string submittedName, string submittedPass)
        {
            LoginUser loginUser = new LoginUser { Username = submittedName, Password = submittedPass };
            RestClient client = new RestClient(API_BASE);
            RestRequest request = new RestRequest("/login");
            request.AddJsonBody(loginUser);
            IRestResponse<ApiUser> response = client.Post<ApiUser>(request);

            if (response.ResponseStatus != ResponseStatus.Completed) {
                Console.WriteLine("An error occurred communicating with the server.");
                return false;
            } else if (!response.IsSuccessful) {
                if (!string.IsNullOrWhiteSpace(response.Data.Message)) {
                    Console.WriteLine("An error message was received: " + response.Data.Message);
                } else {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
                return false;
            } else {
                user.Token = response.Data.Token;
                authClient.Authenticator = new JwtAuthenticator(user.Token);
                return true;
            }
        }

        public void Logout()
        {
            user = new ApiUser();
            authClient.Authenticator = null;
        }
    }
}