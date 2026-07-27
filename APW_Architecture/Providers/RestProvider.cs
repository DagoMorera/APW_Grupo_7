using System;
using System.Threading.Tasks;
using APW.Architecture.Helpers;

namespace APW.Architecture.Providers
{
	/// <summary>
	/// Interface defining methods for RESTful operations.
	/// </summary>
	public interface IRestProvider
	{
		/// <summary>
		/// Deletes a resource asynchronously.
		/// </summary>
		/// <param name="endpoint">The endpoint for the DELETE request.</param>
		/// <param name="id">The ID of the resource to delete.</param>
		/// <returns>A task that represents the asynchronous operation, containing the response as a string.</returns>
		Task<string> DeleteAsync(string endpoint, string id);

		/// <summary>
		/// Retrieves a resource asynchronously.
		/// </summary>
		/// <param name="endpoint">The endpoint for the GET request.</param>
		/// <param name="id">The ID of the resource to retrieve. Can be null if not applicable.</param>
		/// <returns>A task that represents the asynchronous operation, containing the response as a string.</returns>
		Task<string> GetAsync(string endpoint, string? id);

		/// <summary>
		/// Creates a resource asynchronously.
		/// </summary>
		/// <param name="endpoint">The endpoint for the POST request.</param>
		/// <param name="content">The content to send in the request body.</param>
		/// <returns>A task that represents the asynchronous operation, containing the response as a string.</returns>
		Task<string> PostAsync(string endpoint, string content);

		/// <summary>
		/// Updates a resource asynchronously.
		/// </summary>
		/// <param name="endpoint">The endpoint for the PUT request.</param>
		/// <param name="requestUri">The URI of the resource to update.</param>
		/// <param name="content">The content to send in the request body.</param>
		/// <returns>A task that represents the asynchronous operation, containing the response as a string.</returns>
		Task<string> PutAsync(string endpoint, string id, string content);
	}

	/// <summary>
	/// Implementation of the IRestProvider interface, providing methods for RESTful operations.
	/// </summary>
	public class RestProvider : IRestProvider
	{
		/// <summary>
		/// Retrieves a resource asynchronously.
		/// </summary>
		/// <param name="endpoint">The endpoint for the GET request.</param>
		/// <param name="id">The ID of the resource to retrieve. Can be null if not applicable.</param>
		/// <returns>A task that represents the asynchronous operation, containing the response as a string.</returns>
		public async Task<string> GetAsync(string endpoint, string? id)
		{
			try
			{
				using var client = RestProviderHelpers.CreateHttpClient(endpoint);
				var requestUri = string.IsNullOrEmpty(id) ? string.Empty : id;
				// Determine the actual URI we will request.
				// If no id was provided and the endpoint is an absolute URI to a resource
				// (for example: https://.../tdc.json), call GetAsync with the full
				// absolute URI. This avoids appending an extra trailing slash via
				// HttpClient.BaseAddress which could change the requested resource and
				// cause 404 responses.
				string actualRequestTarget;
				if (string.IsNullOrEmpty(requestUri) && Uri.IsWellFormedUriString(endpoint, UriKind.Absolute))
				{
					actualRequestTarget = endpoint;
				}
				else
				{
					actualRequestTarget = requestUri;
				}

				var response = await client.GetAsync(actualRequestTarget);
				return await RestProviderHelpers.GetResponse(response);
			}
			catch (Exception ex)
			{
				// Include the requested URI part to improve diagnostics
				var requestUri = string.IsNullOrEmpty(id) ? string.Empty : id;
				// Recompute the actual target we attempted to request so the error
				// message is accurate.
				string actualRequestTarget = string.IsNullOrEmpty(requestUri) && Uri.IsWellFormedUriString(endpoint, UriKind.Absolute)
					? endpoint
					: requestUri;
				throw RestProviderHelpers.ThrowError(endpoint, actualRequestTarget, ex);
			}
		}

		/// <summary>
		/// Creates a resource asynchronously.
		/// </summary>
		/// <param name="endpoint">The endpoint for the POST request.</param>
		/// <param name="content">The content to send in the request body.</param>
		/// <returns>A task that represents the asynchronous operation, containing the response as a string.</returns>
		public async Task<string> PostAsync(string endpoint, string content)
		{
			try
			{
				using var client = RestProviderHelpers.CreateHttpClient(endpoint);
				// Post to the base address (endpoint) by using an empty relative URI
				var response = await client.PostAsync(string.Empty, RestProviderHelpers.CreateContent(content));
				return await RestProviderHelpers.GetResponse(response);
			}
			catch (Exception ex)
			{
				throw RestProviderHelpers.ThrowError(endpoint, string.Empty, ex);
			}
		}

		/// <summary>
		/// Updates a resource asynchronously.
		/// </summary>
		/// <param name="endpoint">The endpoint for the PUT request.</param>
		/// <param name="id">The ID of the resource to update.</param>
		/// <param name="content">The content to send in the request body.</param>
		/// <returns>A task that represents the asynchronous operation, containing the response as a string.</returns>
		public async Task<string> PutAsync(string endpoint, string id, string content)
		{
			try
			{
				using var client = RestProviderHelpers.CreateHttpClient(endpoint);
				var requestUri = string.IsNullOrEmpty(id) ? string.Empty : id;
				var response = await client.PutAsync(requestUri, RestProviderHelpers.CreateContent(content));
				return await RestProviderHelpers.GetResponse(response);
			}
			catch (Exception ex)
			{
				var requestUri = string.IsNullOrEmpty(id) ? string.Empty : id;
				throw RestProviderHelpers.ThrowError(endpoint, requestUri, ex);
			}
		}

		/// <summary>
		/// Deletes a resource asynchronously.
		/// </summary>
		/// <param name="endpoint">The endpoint for the DELETE request.</param>
		/// <param name="id">The ID of the resource to delete.</param>
		/// <returns>A task that represents the asynchronous operation, containing the response as a string.</returns>
		public async Task<string> DeleteAsync(string endpoint, string id)
		{
			try
			{
				using var client = RestProviderHelpers.CreateHttpClient(endpoint);
				var requestUri = string.IsNullOrEmpty(id) ? string.Empty : id;
				var response = await client.DeleteAsync(requestUri);
				return await RestProviderHelpers.GetResponse(response);
			}
			catch (Exception ex)
			{
				var requestUri = string.IsNullOrEmpty(id) ? string.Empty : id;
				throw RestProviderHelpers.ThrowError(endpoint, requestUri, ex);
			}
		}
	}
}
