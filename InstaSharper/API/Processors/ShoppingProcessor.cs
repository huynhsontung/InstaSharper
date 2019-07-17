/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using InstaSharper.Classes;
using InstaSharper.Classes.DeviceInfo;
using InstaSharper.Classes.Models.Media;
using InstaSharper.Classes.Models.Shopping;
using InstaSharper.Classes.ResponseWrappers.Media;
using InstaSharper.Classes.ResponseWrappers.Shopping;
using InstaSharper.Converters;
using InstaSharper.Converters.Json;
using InstaSharper.Helpers;
using InstaSharper.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstaSharper.API.Processors
{
    /// <summary>
    ///     Shopping and commerce api functions.
    /// </summary>
    internal class ShoppingProcessor : IShoppingProcessor
    {
        #region Properties and constructor
        private readonly AndroidDevice _deviceInfo;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        private readonly InstaApi _instaApi;

        public ShoppingProcessor(AndroidDevice deviceInfo, UserSessionData user,
            IHttpRequestProcessor httpRequestProcessor, IInstaLogger logger,
            UserAuthValidate userAuthValidate, InstaApi instaApi)
        {
            _deviceInfo = deviceInfo;
            _user = user;
            _httpRequestProcessor = httpRequestProcessor;
            _logger = logger;
            _userAuthValidate = userAuthValidate;
            _instaApi = instaApi;
        }
        #endregion Properties and constructor


        /// <summary>
        ///     Get all user shoppable media by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetUserShoppableMediaAsync(string username,
             PaginationParameters paginationParameters)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            var user = await _instaApi.UserProcessor.GetUserAsync(username);
            if (!user.Succeeded)
                return Result.Fail<InstaMediaList>("Unable to get user to load shoppable media");
            return await GetUserShoppableMedia(user.Value.Pk, paginationParameters);
        }

        /// <summary>
        ///     Get all user shoppable media by user id (pk)
        /// </summary>
        /// <param name="userId">User id (pk)</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetUserShoppableMediaByIdAsync(long userId,
            PaginationParameters paginationParameters)
        {
            return await GetUserShoppableMedia(userId, paginationParameters);
        }

        /// <summary>
        ///     Get product info
        /// </summary>
        /// <param name="productId">Product id (get it from <see cref="InstaProduct.ProductId"/> )</param>
        /// <param name="mediaPk">Media Pk (get it from <see cref="InstaMedia.Pk"/>)</param>
        /// <param name="deviceWidth">Device width (pixel)</param>
        public async Task<IResult<InstaProductInfo>> GetProductInfoAsync(long productId, string mediaPk, int deviceWidth = 720)
        {
            try
            {
                var instaUri = UriCreator.GetProductInfoUri(productId, mediaPk, deviceWidth);
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaProductInfo>(response, json);

                var productInfoResponse = JsonConvert.DeserializeObject<InstaProductInfoResponse>(json);
                var converted = ConvertersFabric.Instance.GetProductInfoConverter(productInfoResponse).Convert();

                return Result.Success(converted);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaProductInfo), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaProductInfo>(exception);
            }
        }


        public async Task<IResult<InstaProductInfo>> GetCatalogsAsync()
        {
            try
            {
                var instaUri = new Uri($"https://i.instagram.com/api/v1/wwwgraphql/ig/query/?locale={InstaApiConstants.ACCEPT_LANGUAGE.Replace("-","_")}");

                var sources = new JObject
                {
                    {"sources", null}
                };

                var data = new Dictionary<string, string>
                {
                    {"access_token", "undefined"},
                    {"fb_api_caller_class", "RelayModern"},
                    {"variables", sources.ToString(Formatting.Indented)},
                    {"doc_id", "1742970149122229"},
                };

                var request = HttpHelper.GetDefaultRequest(instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                //{"data":{"me":{"taggable_catalogs":{"edges":[],"page_info":{"has_next_page":false,"end_cursor":null}},"id":"17841407343005740"}}}
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaProductInfo>(response, json);

                var productInfoResponse = JsonConvert.DeserializeObject<InstaProductInfoResponse>(json);
                var converted = ConvertersFabric.Instance.GetProductInfoConverter(productInfoResponse).Convert();

                return Result.Success(converted);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaProductInfo), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaProductInfo>(exception);
            }
        }





        private async Task<IResult<InstaMediaList>> GetUserShoppableMedia(long userId,
                                            PaginationParameters paginationParameters)
        {
            var mediaList = new InstaMediaList();
            try
            {
                if (paginationParameters == null)
                    paginationParameters = PaginationParameters.MaxPagesToLoad(1);

                InstaMediaList Convert(InstaMediaListResponse mediaListResponse)
                {
                    return ConvertersFabric.Instance.GetMediaListConverter(mediaListResponse).Convert();
                }

                var mediaResult = await GetShoppableMedia(userId, paginationParameters);
                if (!mediaResult.Succeeded)
                {
                    if (mediaResult.Value != null)
                        return Result.Fail(mediaResult.Info, Convert(mediaResult.Value));
                    else
                        return Result.Fail(mediaResult.Info, mediaList);
                }
                var mediaResponse = mediaResult.Value;
                mediaList = ConvertersFabric.Instance.GetMediaListConverter(mediaResponse).Convert();
                mediaList.NextMaxId = paginationParameters.NextMaxId = mediaResponse.NextMaxId;
                paginationParameters.PagesLoaded++;

                while (mediaResponse.MoreAvailable
                       && !string.IsNullOrEmpty(paginationParameters.NextMaxId)
                       && paginationParameters.PagesLoaded < paginationParameters.MaximumPagesToLoad)
                {
                    var nextMedia = await GetShoppableMedia(userId, paginationParameters);
                    if (!nextMedia.Succeeded)
                        return Result.Fail(nextMedia.Info, mediaList);
                    mediaList.NextMaxId = paginationParameters.NextMaxId = nextMedia.Value.NextMaxId;
                    mediaList.AddRange(Convert(nextMedia.Value));
                    mediaResponse.MoreAvailable = nextMedia.Value.MoreAvailable;
                    mediaResponse.ResultsCount += nextMedia.Value.ResultsCount;
                    paginationParameters.PagesLoaded++;
                }

                mediaList.Pages = paginationParameters.PagesLoaded;
                mediaList.PageSize = mediaResponse.ResultsCount;
                return Result.Success(mediaList);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMediaList), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, mediaList);
            }
        }

        private async Task<IResult<InstaMediaListResponse>> GetShoppableMedia(long userId,
                                    PaginationParameters paginationParameters)
        {
            try
            {
                var instaUri = UriCreator.GetUserShoppableMediaListUri(userId, paginationParameters.NextMaxId);
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaMediaListResponse>(response, json);

                var mediaResponse = JsonConvert.DeserializeObject<InstaMediaListResponse>(json,
                    new InstaMediaListDataConverter());

                return Result.Success(mediaResponse);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaMediaListResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, default(InstaMediaListResponse));
            }
        }
    }
}
