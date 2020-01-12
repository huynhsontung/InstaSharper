using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using InstaSharper.API.Processors;
using InstaSharper.API.Push;
using InstaSharper.Classes;
using InstaSharper.Classes.DeviceInfo;
using InstaSharper.Classes.Models.Account;
using InstaSharper.Classes.Models.Challenge;
using InstaSharper.Classes.Models.Other;
using InstaSharper.Classes.Models.User;
using InstaSharper.Classes.ResponseWrappers.BaseResponse;
using InstaSharper.Classes.ResponseWrappers.Errors;
using InstaSharper.Classes.ResponseWrappers.Login;
using InstaSharper.Classes.ResponseWrappers.Other;
using InstaSharper.Classes.ResponseWrappers.User;
using InstaSharper.Classes.SessionHandlers;
using InstaSharper.Converters;
using InstaSharper.Enums;
using InstaSharper.Helpers;
using InstaSharper.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstaSharper.API
{
    /// <summary>
    ///     Base of everything that you want.
    /// </summary>
    public class InstaApi
    {
        #region Variables and properties

        private IRequestDelay _delay = RequestDelay.Empty();
        private readonly IInstaLogger _logger;
        private InstaTwoFactorLoginInfo _twoFactorInfo;
        private InstaChallengeLoginInfo _challengeInfo;
        private UserSessionData _userSession;
        private UserSessionData _user
        {
            get => _userSession;
            set { _userSession = value; _userAuthValidate.User = value; }
        }
        private UserAuthValidate _userAuthValidate;

        private string _waterfallIdReg = "", _deviceIdReg = "", _phoneIdReg = "", _guidReg = "";
        private InstaAccountRegistrationPhoneNumber _signUpPhoneNumberInfo;

        private bool _isUserAuthenticated;
        public bool IsUserAuthenticated
        {
            get => _isUserAuthenticated;
            internal set { _isUserAuthenticated = value; _userAuthValidate.IsUserAuthenticated = value; }
        }
        public IHttpRequestProcessor RequestProcessor { get; }
        public HttpClient HttpClient => RequestProcessor.Client;
        public AndroidDevice DeviceInfo { get; private set; }
        public FbnsClient PushClient { get; private set; }

        #endregion Variables and properties

        #region SessionHandler

        public ISessionHandler SessionHandler { get; set; }

        #endregion

        #region Processors

        private ICollectionProcessor _collectionProcessor;
        private ICommentProcessor _commentProcessor;
        private IFeedProcessor _feedProcessor;
        private IHashtagProcessor _hashtagProcessor;
        private ILocationProcessor _locationProcessor;
        private IMediaProcessor _mediaProcessor;
        private IMessagingProcessor _messagingProcessor;
        private IStoryProcessor _storyProcessor;
        private IUserProcessor _userProcessor;
        private ILiveProcessor _liveProcessor;
        private IDiscoverProcessor _discoverProcessor;
        private IAccountProcessor _accountProcessor;


        ITVProcessor _tvProcessor;
        HelperProcessor _helperProcessor;
        IBusinessProcessor _businessProcessor;
        IShoppingProcessor _shoppingProcessor;
        IWebProcessor _webProcessor;

        /// <summary>
        ///     Live api functions.
        /// </summary>
        public ILiveProcessor LiveProcessor => _liveProcessor;
        /// <summary>
        ///     Discover api functions.
        /// </summary>
        public IDiscoverProcessor DiscoverProcessor => _discoverProcessor;
        /// <summary>
        ///     Account api functions.
        /// </summary>
        public IAccountProcessor AccountProcessor => _accountProcessor;
        /// <summary>
        ///     Comments api functions.
        /// </summary>
        public ICommentProcessor CommentProcessor => _commentProcessor;
        /// <summary>
        ///     Story api functions.
        /// </summary>
        public IStoryProcessor StoryProcessor => _storyProcessor;
        /// <summary>
        ///     Media api functions.
        /// </summary>
        public IMediaProcessor MediaProcessor => _mediaProcessor;
        /// <summary>
        ///     Messaging (direct) api functions.
        /// </summary>
        public IMessagingProcessor MessagingProcessor => _messagingProcessor;
        /// <summary>
        ///     Feed api functions.
        /// </summary>
        public IFeedProcessor FeedProcessor => _feedProcessor;
        /// <summary>
        ///     Collection api functions.
        /// </summary>
        public ICollectionProcessor CollectionProcessor => _collectionProcessor;
        /// <summary>
        /// Location api functions.
        /// </summary>
        public ILocationProcessor LocationProcessor => _locationProcessor;
        /// <summary>
        ///     Hashtag api functions.
        /// </summary>
        public IHashtagProcessor HashtagProcessor => _hashtagProcessor;
        /// <summary>
        ///     User api functions.
        /// </summary>
        public IUserProcessor UserProcessor => _userProcessor;
        /// <summary>
        ///     Helper processor for other processors
        /// </summary>
        internal HelperProcessor HelperProcessor => _helperProcessor;
        /// <summary>
        ///     Instagram TV api functions
        /// </summary>
        public ITVProcessor TVProcessor => _tvProcessor;
        /// <summary>
        ///     Business api functions
        ///     <para>Note: All functions of this interface only works with business accounts!</para>
        /// </summary>
        public IBusinessProcessor BusinessProcessor => _businessProcessor;
        /// <summary>
        ///     Shopping and commerce api functions
        /// </summary>
        public IShoppingProcessor ShoppingProcessor => _shoppingProcessor;
        /// <summary>
        ///     Instagram Web api functions.
        ///     <para>It's related to https://instagram.com/accounts/ </para>
        /// </summary>
        public IWebProcessor WebProcessor => _webProcessor;

        #endregion Processors

        #region Constructor

        public InstaApi(UserSessionData user, IInstaLogger logger, AndroidDevice deviceInfo,
            IHttpRequestProcessor httpRequestProcessor, FbnsConnectionData fbnsData, ApiVersion apiVersion)
        {
            _userAuthValidate = new UserAuthValidate();
            _user = user;
            _logger = logger;
            DeviceInfo = deviceInfo;
            RequestProcessor = httpRequestProcessor;
            if (apiVersion != null) ApiVersion.CurrentApiVersion = apiVersion;
            PushClient = new FbnsClient(DeviceInfo, _user, RequestProcessor, fbnsData);
        }

        #endregion Constructor

        #region Register new account with Phone number and email

        /// <summary>
        ///     Check email availability
        /// </summary>
        /// <param name="email">Email to check</param>
        public async Task<IResult<InstaCheckEmailRegistration>> CheckEmailAsync(string email)
        {
            return await CheckEmail(email);
        }
        private async Task<IResult<InstaCheckEmailRegistration>> CheckEmail(string email, bool useNewWaterfall = true)
        {
            try
            {
                if (_waterfallIdReg == null || useNewWaterfall)
                    _waterfallIdReg = Guid.NewGuid().ToString();

                var firstResponse = await RequestProcessor.GetAsync(RequestProcessor.Client.BaseAddress);

                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                _user.CsrfToken = csrftoken;

                var postData = new Dictionary<string, string>
                {
                    {"_csrftoken",      csrftoken},
                    {"login_nonces",    "[]"},
                    {"email",           email},
                    {"qe_id",           Guid.NewGuid().ToString()},
                    {"waterfall_id",    _waterfallIdReg},
                };
                var instaUri = UriCreator.GetCheckEmailUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var obj = JsonConvert.DeserializeObject<InstaCheckEmailRegistration>(json);
                    if (obj.ErrorType == "fail")
                        return Result.UnExpectedResponse<InstaCheckEmailRegistration>(response, json);
                    if (obj.ErrorType == "email_is_taken")
                        return Result.Fail("Email is taken.", (InstaCheckEmailRegistration)null);
                    if (obj.ErrorType == "invalid_email")
                        return Result.Fail("Please enter a valid email address.", (InstaCheckEmailRegistration)null);

                    return Result.UnExpectedResponse<InstaCheckEmailRegistration>(response, json);
                }
                else
                {
                    var obj = JsonConvert.DeserializeObject<InstaCheckEmailRegistration>(json);
                    if (obj.ErrorType == "fail")
                        return Result.UnExpectedResponse<InstaCheckEmailRegistration>(response, json);
                    if (obj.ErrorType == "email_is_taken")
                        return Result.Fail("Email is taken.", (InstaCheckEmailRegistration)null);
                    if (obj.ErrorType == "invalid_email")
                        return Result.Fail("Please enter a valid email address.", (InstaCheckEmailRegistration)null);

                    return Result.Success(obj);
                }
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaCheckEmailRegistration), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaCheckEmailRegistration>(exception);
            }
        }
        /// <summary>
        ///     Check phone number availability
        /// </summary>
        /// <param name="phoneNumber">Phone number to check</param>
        public async Task<IResult<bool>> CheckPhoneNumberAsync(string phoneNumber)
        {
            try
            {
                _deviceIdReg = ApiRequestMessage.GenerateDeviceId();

                var firstResponse = await RequestProcessor.GetAsync(RequestProcessor.Client.BaseAddress);
                
                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                _user.CsrfToken = csrftoken;

                var postData = new Dictionary<string, string>
                {
                    {"_csrftoken",      csrftoken},
                    {"login_nonces",    "[]"},
                    {"phone_number",    phoneNumber},
                    {"device_id",    DeviceInfo.DeviceId},
                };
                var instaUri = UriCreator.GetCheckPhoneNumberUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return Result.UnExpectedResponse<bool>(response, json);
                }

                return Result.Success(true);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }
        /// <summary>
        ///     Check username availablity. 
        /// </summary>
        /// <param name="username">Username</param>
        public async Task<IResult<InstaAccountCheck>> CheckUsernameAsync(string username)
        {
            try
            {
                var instaUri = UriCreator.GetCheckUsernameUri();
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"username", username}
                };
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<InstaAccountCheck>(json);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaAccountCheck), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaAccountCheck>(exception);
            }
        }
        /// <summary>
        ///     Send sign up sms code
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        public async Task<IResult<bool>> SendSignUpSmsCodeAsync(string phoneNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(_waterfallIdReg))
                    _waterfallIdReg = Guid.NewGuid().ToString();

                await CheckPhoneNumberAsync(phoneNumber);

                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                _user.CsrfToken = csrftoken;
                var postData = new Dictionary<string, string>
                {
                    {"phone_id",        DeviceInfo.PhoneId.ToString()},
                    {"phone_number",    phoneNumber},
                    {"_csrftoken",      csrftoken},
                    {"guid",            DeviceInfo.Uuid.ToString()},
                    {"device_id",       DeviceInfo.DeviceId},
                    {"waterfall_id",    _waterfallIdReg},
                };
                var instaUri = UriCreator.GetSignUpSMSCodeUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var o = JsonConvert.DeserializeObject<InstaAccountRegistrationPhoneNumber>(json);

                    return Result.UnExpectedResponse<bool>(response, o.Message?.Errors?[0], json);
                }
                _signUpPhoneNumberInfo = JsonConvert.DeserializeObject<InstaAccountRegistrationPhoneNumber>(json);
                return Result.Success(true);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }
        /// <summary>
        ///     Verify sign up sms code
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="verificationCode">Verification code</param>
        public async Task<IResult<InstaPhoneNumberRegistration>> VerifySignUpSmsCodeAsync(string phoneNumber, string verificationCode)
        {
            try
            {
                if (string.IsNullOrEmpty(_waterfallIdReg))
                    throw new ArgumentException("You should call SendSignUpSmsCodeAsync function first.");

                var cookies =
                    RequestProcessor.HttpHandler.CookieContainer.GetCookies(RequestProcessor.Client
                    .BaseAddress);
                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                _user.CsrfToken = csrftoken;
                var postData = new Dictionary<string, string>
                {
                    {"verification_code",         verificationCode},
                    {"phone_number",              phoneNumber},
                    {"_csrftoken",                csrftoken},
                    {"guid",                      DeviceInfo.Uuid.ToString()},
                    {"device_id",                 DeviceInfo.DeviceId},
                    {"waterfall_id",              _waterfallIdReg},
                };
                var instaUri = UriCreator.GetValidateSignUpSMSCodeUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var o = JsonConvert.DeserializeObject<InstaAccountRegistrationPhoneNumberVerifySms>(json);

                    return Result.Fail(o.Errors?.Nonce?[0], (InstaPhoneNumberRegistration)null);
                }

                var r = JsonConvert.DeserializeObject<InstaAccountRegistrationPhoneNumberVerifySms>(json);
                if (r.ErrorType == "invalid_nonce")
                    return Result.Fail(r.Errors?.Nonce?[0], (InstaPhoneNumberRegistration)null);

                await GetRegistrationStepsAsync();
                var obj = JsonConvert.DeserializeObject<InstaPhoneNumberRegistration>(json);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaPhoneNumberRegistration), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaPhoneNumberRegistration>(exception);
            }
        }
        /// <summary>
        ///     Get username suggestions
        /// </summary>
        /// <param name="name">Name</param>
        public async Task<IResult<InstaRegistrationSuggestionResponse>> GetUsernameSuggestionsAsync(string name)
        {
            return await GetUsernameSuggestions(name);
        }
        public async Task<IResult<InstaRegistrationSuggestionResponse>> GetUsernameSuggestions(string name, bool useNewIds = true)
        {
            try
            {
                if (string.IsNullOrEmpty(_deviceIdReg))
                    _deviceIdReg = ApiRequestMessage.GenerateDeviceId();
                if (useNewIds)
                {
                    _phoneIdReg = Guid.NewGuid().ToString();
                    _waterfallIdReg = Guid.NewGuid().ToString();
                    _guidReg = Guid.NewGuid().ToString();
                }
                var cookies =
                    RequestProcessor.HttpHandler.CookieContainer.GetCookies(RequestProcessor.Client
                    .BaseAddress);
                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                _user.CsrfToken = csrftoken;
                var postData = new Dictionary<string, string>
                {
                    {"name",            name},
                    {"_csrftoken",      csrftoken},
                    {"email",           ""}
                };
                if (useNewIds)
                {
                    postData.Add("phone_id", _phoneIdReg);
                    postData.Add("guid", _guidReg);
                    postData.Add("device_id", _deviceIdReg);
                    postData.Add("waterfall_id", _waterfallIdReg);
                }
                else
                {
                    postData.Add("phone_id", DeviceInfo.PhoneId.ToString());
                    postData.Add("guid", DeviceInfo.Uuid.ToString());
                    postData.Add("device_id", DeviceInfo.DeviceId.ToString());
                    postData.Add("waterfall_id", _waterfallIdReg ?? Guid.NewGuid().ToString());
                }
                var instaUri = UriCreator.GetUsernameSuggestionsUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var o = JsonConvert.DeserializeObject<InstaAccountRegistrationPhoneNumber>(json);

                    return Result.Fail(o.Message?.Errors?[0], (InstaRegistrationSuggestionResponse)null);
                }

                var obj = JsonConvert.DeserializeObject<InstaRegistrationSuggestionResponse>(json);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaRegistrationSuggestionResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaRegistrationSuggestionResponse>(exception);
            }
        }
        /// <summary>
        ///     Validate new account creation with phone number
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="verificationCode">Verification code</param>
        /// <param name="username">Username to set</param>
        /// <param name="password">Password to set</param>
        /// <param name="firstName">First name to set</param>
        public async Task<IResult<InstaAccountCreation>> ValidateNewAccountWithPhoneNumberAsync(string phoneNumber, string verificationCode, string username, string password, string firstName)
        {
            try
            {
                if (string.IsNullOrEmpty(_waterfallIdReg) || _signUpPhoneNumberInfo == null)
                    throw new ArgumentException("You should call SendSignUpSmsCodeAsync function first.");

                if (_signUpPhoneNumberInfo.GdprRequired)
                {
                    var acceptGdpr = await AcceptConsentRequiredAsync(null, phoneNumber);
                    if (!acceptGdpr.Succeeded)
                        return Result.Fail(acceptGdpr.Info.Message, (InstaAccountCreation)null);
                }
                var cookies =
                    RequestProcessor.HttpHandler.CookieContainer.GetCookies(RequestProcessor.Client
                    .BaseAddress);
                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                _user.CsrfToken = csrftoken;

                var postData = new Dictionary<string, string>
                {
                    {"allow_contacts_sync",       "true"},
                    {"verification_code",         verificationCode},
                    {"sn_result",                 "API_ERROR:+null"},
                    {"phone_id",                  DeviceInfo.PhoneId.ToString()},
                    {"phone_number",              phoneNumber},
                    {"_csrftoken",                csrftoken},
                    {"username",                  username},
                    {"first_name",                firstName},
                    {"adid",                      Guid.NewGuid().ToString()},
                    {"guid",                      DeviceInfo.Uuid.ToString()},
                    {"device_id",                 DeviceInfo.DeviceId},
                    {"sn_nonce",                  ""},
                    {"force_sign_up_code",        ""},
                    {"waterfall_id",              _waterfallIdReg},
                    {"qs_stamp",                  ""},
                    {"password",                  password},
                    {"has_sms_consent",           "true"},
                };
                if (_signUpPhoneNumberInfo.GdprRequired)
                    postData.Add("gdpr_s", "[0,2,0,null]");

                var instaUri = UriCreator.GetCreateValidatedUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var o = JsonConvert.DeserializeObject<InstaAccountCreationResponse>(json);

                    return Result.Fail(o.Errors?.Username?[0], (InstaAccountCreation)null);
                }

                var r = JsonConvert.DeserializeObject<InstaAccountCreationResponse>(json);
                if (r.ErrorType == "username_is_taken")
                    return Result.Fail(r.Errors?.Username?[0], (InstaAccountCreation)null);

                var obj = JsonConvert.DeserializeObject<InstaAccountCreation>(json);
                if (obj.AccountCreated && obj.CreatedUser != null)
                    ValidateUserAsync(obj.CreatedUser, csrftoken, true, password);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaAccountCreation), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaAccountCreation>(exception);
            }
        }


        private async Task<IResult<object>> GetRegistrationStepsAsync()
        {
            try
            {
                var cookies =
                    RequestProcessor.HttpHandler.CookieContainer.GetCookies(RequestProcessor.Client
                    .BaseAddress);
                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                _user.CsrfToken = csrftoken;
                var postData = new Dictionary<string, string>
                {
                    {"fb_connected",            "false"},
                    {"seen_steps",            "[]"},
                    {"phone_id",        _phoneIdReg},
                    {"fb_installed",            "false"},
                    {"locale",            "en_US"},
                    {"timezone_offset",            "16200"},
                    {"network_type",            "WIFI-UNKNOWN"},
                    {"_csrftoken",      csrftoken},
                    {"guid",            _guidReg},
                    {"is_ci",            "false"},
                    {"android_id",       _deviceIdReg},
                    {"reg_flow_taken",           "phone"},
                    {"tos_accepted",    "false"},
                };
                var instaUri = UriCreator.GetOnboardingStepsUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var o = JsonConvert.DeserializeObject<InstaAccountRegistrationPhoneNumber>(json);

                    return Result.Fail(o.Message?.Errors?[0], (InstaRegistrationSuggestionResponse)null);
                }

                var obj = JsonConvert.DeserializeObject<InstaRegistrationSuggestionResponse>(json);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaRegistrationSuggestionResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaRegistrationSuggestionResponse>(exception);
            }
        }

        /// <summary>
        ///     Create a new instagram account [NEW FUNCTION, BUT NOT WORKING?!!!!!!!!!!]
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="email">Email</param>
        /// <param name="firstName">First name (optional)</param>
        /// <param name="delay">Delay between requests. null = 2.5 seconds</param>
        private async Task<IResult<InstaAccountCreation>> CreateNewAccountAsync(string username, string password, string email, string firstName = "", TimeSpan? delay = null)
        {
            var createResponse = new InstaAccountCreation();
            try
            {
                if (delay == null)
                    delay = TimeSpan.FromSeconds(2.5);

                var firstResponse = await RequestProcessor.GetAsync(RequestProcessor.Client.BaseAddress);
                await firstResponse.Content.ReadAsStringAsync();
                var cookies =
                        RequestProcessor.HttpHandler.CookieContainer.GetCookies(RequestProcessor.Client
                        .BaseAddress);
                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                var checkEmail = await CheckEmail(email, false);
                if (!checkEmail.Succeeded)
                    return Result.Fail(checkEmail.Info.Message, (InstaAccountCreation)null);


                await Task.Delay((int)delay.Value.TotalMilliseconds);
                if (checkEmail.Value.GdprRequired)
                {
                    var acceptGdpr = await AcceptConsentRequiredAsync(email);
                    if (!acceptGdpr.Succeeded)
                        return Result.Fail(acceptGdpr.Info.Message, (InstaAccountCreation)null);
                }

                await Task.Delay((int)delay.Value.TotalMilliseconds);
                if (username.Length > 6)
                {
                    await GetUsernameSuggestions(username.Substring(0, 4), false);
                    await Task.Delay(1000);
                    await GetUsernameSuggestions(username.Substring(0, 5), false);
                }
                else
                {
                    await GetUsernameSuggestions(username, false);
                    await Task.Delay(1000);
                    await GetUsernameSuggestions(username, false);
                }

                await Task.Delay((int)delay.Value.TotalMilliseconds);
                var postData = new Dictionary<string, string>
                {
                    {"allow_contacts_sync",       "true"},
                    {"sn_result",                 "API_ERROR:+null"},
                    {"phone_id",                  DeviceInfo.PhoneId.ToString()},
                    {"_csrftoken",                csrftoken},
                    {"username",                  username},
                    {"first_name",                firstName},
                    {"adid",                      Guid.NewGuid().ToString()},
                    {"guid",                      DeviceInfo.Uuid.ToString()},
                    {"device_id",                 DeviceInfo.DeviceId.ToString()},
                    {"email",                     email},
                    {"sn_nonce",                  ""},
                    {"force_sign_up_code",        ""},
                    {"waterfall_id",              _waterfallIdReg ?? Guid.NewGuid().ToString()},
                    {"qs_stamp",                  ""},
                    {"password",                  password},
                };
                if (checkEmail.Value.GdprRequired)
                    postData.Add("gdpr_s", "[0,2,0,null]");

                var instaUri = UriCreator.GetCreateAccountUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaAccountCreation>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaAccountCreation>(json);
                //{"account_created": false, "errors": {"email": ["Another account is using iranramtin73jokar@live.com."], "username": ["This username isn't available. Please try another."]}, "allow_contacts_sync": true, "status": "ok", "error_type": "email_is_taken, username_is_taken"}
                //{"message": "feedback_required", "spam": true, "feedback_title": "Signup Error", "feedback_message": "Sorry! There\u2019s a problem signing you up right now. Please try again later. We restrict certain content and actions to protect our community. Tell us if you think we made a mistake.", "feedback_url": "repute/report_problem/instagram_signup/", "feedback_appeal_label": "Report problem", "feedback_ignore_label": "OK", "feedback_action": "report_problem", "status": "fail", "error_type": "signup_block"}

                if (obj.AccountCreated && obj.CreatedUser != null)
                    ValidateUserAsync(obj.CreatedUser, csrftoken, true, password);

                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaAccountCreation), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaAccountCreation>(exception);
            }
        }

        /// <summary>
        ///     Create a new instagram account
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="email">Email</param>
        /// <param name="firstName">First name (optional)</param>
        /// <returns></returns>
        public async Task<IResult<InstaAccountCreation>> CreateNewAccountAsync(string username, string password, string email, string firstName)
        {
            InstaAccountCreation createResponse = new InstaAccountCreation();
            try
            {
                var _deviceIdReg = ApiRequestMessage.GenerateDeviceId();
                var _phoneIdReg = Guid.NewGuid().ToString();
                var _waterfallIdReg = Guid.NewGuid().ToString();
                var _guidReg = Guid.NewGuid().ToString();
                var firstResponse = await RequestProcessor.GetAsync(RequestProcessor.Client.BaseAddress);
                await firstResponse.Content.ReadAsStringAsync();

                var cookies =
                    RequestProcessor.HttpHandler.CookieContainer.GetCookies(RequestProcessor.Client
                    .BaseAddress);
                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);

                var postData = new Dictionary<string, string>
                {
                    {"allow_contacts_sync",       "true"},
                    {"sn_result",                 "API_ERROR:+null"},
                    {"phone_id",                  _phoneIdReg},
                    {"_csrftoken",                csrftoken},
                    {"username",                  username},
                    {"first_name",                firstName},
                    {"adid",                      Guid.NewGuid().ToString()},
                    {"guid",                      _guidReg},
                    {"device_id",                 _deviceIdReg},
                    {"email",                     email},
                    {"sn_nonce",                  ""},
                    {"force_sign_up_code",        ""},
                    {"waterfall_id",              _waterfallIdReg},
                    {"qs_stamp",                  ""},
                    {"password",                  password},
                };
                var instaUri = UriCreator.GetCreateAccountUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaAccountCreation>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaAccountCreation>(json);
                if (obj.AccountCreated && obj.CreatedUser != null)
                    ValidateUserAsync(obj.CreatedUser, csrftoken, true, password);

                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaAccountCreation), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaAccountCreation>(exception);
            }
        }
        /// <summary>
        ///     Accept consent require (for GDPR countries) 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        private async Task<IResult<bool>> AcceptConsentRequiredAsync(string email, string phone = null)
        {
            try
            {
                var delay = TimeSpan.FromSeconds(2);

                //{"message": "consent_required", "consent_data": {"headline": "Updates to Our Terms and Data Policy", "content": "We've updated our Terms and made some changes to our Data Policy. Please take a moment to review these changes and let us know that you agree to them.\n\nYou need to finish reviewing this information before you can use Instagram.", "button_text": "Review Now"}, "status": "fail"}
                await Task.Delay((int)delay.TotalMilliseconds);
                var instaUri = UriCreator.GetConsentNewUserFlowBeginsUri();
                var data = new JObject
                {
                    {"phone_id", DeviceInfo.PhoneId},
                    {"_csrftoken", _user.CsrfToken}
                };
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                await Task.Delay((int)delay.TotalMilliseconds);

                instaUri = UriCreator.GetConsentNewUserFlowUri();
                data = new JObject
                {
                    {"phone_id", DeviceInfo.PhoneId},
                    {"gdpr_s", ""},
                    {"_csrftoken", _user.CsrfToken},
                    {"guid", DeviceInfo.Uuid},
                    {"device_id", DeviceInfo.DeviceId}
                };
                if (email != null)
                    data.Add("email", email);
                else
                {
                    if (phone != null && !phone.StartsWith("+"))
                        phone = $"+{phone}";

                    if (phone == null)
                        phone = string.Empty;
                    data.Add("phone", phone);
                }

                request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                response = await RequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                await Task.Delay((int)delay.TotalMilliseconds);

                data = new JObject
                {
                    {"current_screen_key", "age_consent_two_button"},
                    {"phone_id", DeviceInfo.PhoneId},
                    {"gdpr_s", "[0,0,0,null]"},
                    {"_csrftoken", _user.CsrfToken},
                    {"updates", "{\"age_consent_state\":\"2\"}"},
                    {"guid", DeviceInfo.Uuid},
                    {"device_id", DeviceInfo.DeviceId}
                };
                request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                response = await RequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                return Result.Success(true);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, false);
            }
        }
        #endregion Register new account with Phone number and email

        #region Authentication and challenge functions

        /// <summary>
        ///     Login using given credentials asynchronously
        /// </summary>
        /// <param name="isNewLogin"></param>
        /// <returns>
        ///     Success --> is succeed
        ///     TwoFactorRequired --> requires 2FA login.
        ///     BadPassword --> Password is wrong
        ///     InvalidUser --> User/phone number is wrong
        ///     Exception --> Something wrong happened
        ///     ChallengeRequired --> You need to pass Instagram challenge
        /// </returns>
        public async Task<IResult<InstaLoginResult>> LoginAsync(bool isNewLogin = true)
        {
            ValidateUser();
            ValidateRequestMessage();
            try
            {
                bool needsRelogin = false;
                ReloginLabel:
                if (isNewLogin)
                {
                    var firstResponse = await RequestProcessor.GetAsync(RequestProcessor.Client.BaseAddress);
                    var html = await firstResponse.Content.ReadAsStringAsync();
                    _logger?.LogResponse(firstResponse);
                }

                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                _user.CsrfToken = csrftoken;
                var instaUri = UriCreator.GetLoginUri();
                var signature = string.Empty;
                var devid = string.Empty;
                var apiVersion = ApiVersion.CurrentApiVersion;
                // if (isNewLogin)
                //     signature = $"{RequestProcessor.RequestMessage.GenerateSignature(apiVersion, apiVersion.SignatureKey, out devid)}.{RequestProcessor.RequestMessage.GetMessageString()}";
                // else
                signature = $"{RequestProcessor.RequestMessage.GenerateChallengeSignature(apiVersion, apiVersion.SignatureKey, csrftoken, out devid)}.{RequestProcessor.RequestMessage.GetChallengeMessageString(csrftoken)}";
                DeviceInfo.DeviceId = devid;
                var fields = new Dictionary<string, string>
                {
                    {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
                    {InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION}
                };
                var request = HttpHelper.GetDefaultPostRequest(instaUri, DeviceInfo, fields);
                request.Headers.Add("Host", "i.instagram.com");
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var loginFailReason = JsonConvert.DeserializeObject<InstaLoginBaseResponse>(json);

                    if (loginFailReason.InvalidCredentials)
                        return Result.Fail("Invalid Credentials",
                            loginFailReason.ErrorType == "bad_password"
                                ? InstaLoginResult.BadPassword
                                : InstaLoginResult.InvalidUser);
                    if (loginFailReason.TwoFactorRequired)
                    {
                        if (loginFailReason.TwoFactorLoginInfo != null)
                            RequestProcessor.RequestMessage.Username = loginFailReason.TwoFactorLoginInfo.Username;
                        _twoFactorInfo = loginFailReason.TwoFactorLoginInfo;
                        //2FA is required!
                        return Result.Fail("Two Factor Authentication is required", InstaLoginResult.TwoFactorRequired);
                    }
                    if (loginFailReason.ErrorType == "checkpoint_challenge_required"
                       /* || !string.IsNullOrEmpty(loginFailReason.Message) && loginFailReason.Message == "challenge_required"*/)
                    {
                        _challengeInfo = loginFailReason.Challenge;

                        return Result.Fail("Challenge is required", InstaLoginResult.ChallengeRequired);
                    }
                    if (loginFailReason.ErrorType == "rate_limit_error")
                    {
                        return Result.Fail("Please wait a few minutes before you try again.", InstaLoginResult.LimitError);
                    }
                    if (loginFailReason.ErrorType == "inactive user" || loginFailReason.ErrorType == "inactive_user")
                    {
                        return Result.Fail($"{loginFailReason.Message}\r\nHelp url: {loginFailReason.HelpUrl}", InstaLoginResult.InactiveUser);
                    }
                    if (loginFailReason.ErrorType == "checkpoint_logged_out")
                    {
                        if (!needsRelogin)
                        {
                            needsRelogin = true;
                            goto ReloginLabel;
                        }
                        return Result.Fail($"{loginFailReason.ErrorType} {loginFailReason.CheckpointUrl}", InstaLoginResult.CheckpointLoggedOut);
                    }
                    return Result.UnExpectedResponse<InstaLoginResult>(response, json);
                }
                var loginInfo = JsonConvert.DeserializeObject<InstaLoginResponse>(json);
                _user.UserName = loginInfo.User?.UserName;
                IsUserAuthenticated = loginInfo.User != null;
                if (loginInfo.User != null)
                    RequestProcessor.RequestMessage.Username = loginInfo.User.UserName;
                var converter = ConvertersFabric.Instance.GetUserShortConverter(loginInfo.User);
                _user.LoggedInUser = converter.Convert();
                _user.RankToken = $"{_user.LoggedInUser.Pk}_{RequestProcessor.RequestMessage.PhoneId}";
                if (string.IsNullOrEmpty(_user.CsrfToken))
                {
                    _user.CsrfToken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                }
                return Result.Success(InstaLoginResult.Success);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, InstaLoginResult.Exception, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Result.Fail(exception, InstaLoginResult.Exception);
            }
            finally
            {
                InvalidateProcessors();
            }
        }
        /// <summary>
        ///     Login using cookies
        ///     <para>Note: You won't be able to change password, if you use <see cref="IInstaApi.LoginWithCookiesAsync(string)"/> function for logging in!</para>
        /// </summary>
        /// <param name="cookies">Cookies</param>
        public async Task<IResult<bool>> LoginWithCookiesAsync(string cookies)
        {
            try
            {
                if (cookies.Contains("Cookie:"))
                    cookies = cookies.Substring(8);

                var parts = cookies.Split(';')
                    .Where(xx => xx.Contains("="))
                    .Select(xx => xx.Trim().Split('='))
                    .Select(xx => new { Name = xx.First(), Value = xx.Last() });

                var user = parts.FirstOrDefault(u => u.Name.ToLower() == "ds_user")?.Value?.ToLower();
                var userId = parts.FirstOrDefault(u => u.Name.ToLower() == "ds_user_id")?.Value;
                var csrfToken = parts.FirstOrDefault(u => u.Name.ToLower() == "csrftoken")?.Value;

                if (string.IsNullOrEmpty(csrfToken))
                    return Result.Fail<bool>("Cannot find 'csrftoken' in cookies!");

                if (string.IsNullOrEmpty(userId))
                    return Result.Fail<bool>("Cannot find 'ds_user_id' in cookies!");

                var uri = new Uri(InstaApiConstants.INSTAGRAM_URL);
                cookies = cookies.Replace(';', ',');
                RequestProcessor.HttpHandler.CookieContainer.SetCookies(uri, cookies);
                _user = UserSessionData.Empty;
                user = user ?? "AlakiMasalan";
                _user.UserName = RequestProcessor.RequestMessage.Username = user;
                _user.Password = "AlakiMasalan";
                _user.LoggedInUser = new InstaUserShort
                {
                    UserName = user
                };
                try
                {
                    _user.LoggedInUser.Pk = long.Parse(userId);
                }
                catch { }
                _user.CsrfToken = csrfToken;
                _user.RankToken = $"{DeviceInfo.RankToken}_{userId}";

                IsUserAuthenticated = true;
                InvalidateProcessors();

                var us = await UserProcessor.GetUserInfoByIdAsync(long.Parse(userId));
                if (!us.Succeeded)
                {
                    IsUserAuthenticated = false;
                    return Result.Fail(us.Info, false);
                }
                _user.UserName = RequestProcessor.RequestMessage.Username = _user.LoggedInUser.UserName = us.Value.Username;
                _user.LoggedInUser.FullName = us.Value.FullName;
                _user.LoggedInUser.IsPrivate = us.Value.IsPrivate;
                _user.LoggedInUser.IsVerified = us.Value.IsVerified;
                _user.LoggedInUser.ProfilePictureUrl = us.Value.ProfilePicUrl;

                return Result.Success(true);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Result.Fail(exception, false);
            }
        }
        /// <summary>
        ///     2-Factor Authentication Login using a verification code
        ///     Before call this method, please run LoginAsync first.
        /// </summary>
        /// <param name="verificationCode">Verification Code sent to your phone number</param>
        /// <returns>
        ///     Success --> is succeed
        ///     InvalidCode --> The code is invalid
        ///     CodeExpired --> The code is expired, please request a new one.
        ///     Exception --> Something wrong happened
        /// </returns>
        public async Task<IResult<InstaLoginTwoFactorResult>> TwoFactorLoginAsync(string verificationCode)
        {
            if (_twoFactorInfo == null)
                return Result.Fail<InstaLoginTwoFactorResult>("Run LoginAsync first");

            try
            {
                var twoFactorRequestMessage = new ApiTwoFactorRequestMessage(verificationCode,
                    RequestProcessor.RequestMessage.Username,
                    RequestProcessor.RequestMessage.DeviceId,
                    _twoFactorInfo.TwoFactorIdentifier);

                var instaUri = UriCreator.GetTwoFactorLoginUri();
                var apiVersion = ApiVersion.CurrentApiVersion;
                var hash = twoFactorRequestMessage.GenerateSignature(apiVersion, apiVersion.SignatureKey);
                var payload = twoFactorRequestMessage.GetMessageString();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, hash, payload);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var loginInfo =
                        JsonConvert.DeserializeObject<InstaLoginResponse>(json);
                    _user.UserName = loginInfo.User?.UserName;
                    IsUserAuthenticated = loginInfo.User != null;
                    RequestProcessor.RequestMessage.Username = loginInfo.User?.UserName;
                    var converter = ConvertersFabric.Instance.GetUserShortConverter(loginInfo.User);
                    _user.LoggedInUser = converter.Convert();
                    _user.RankToken = $"{_user.LoggedInUser.Pk}_{RequestProcessor.RequestMessage.PhoneId}";

                    return Result.Success(InstaLoginTwoFactorResult.Success);
                }

                var loginFailReason = JsonConvert.DeserializeObject<InstaLoginTwoFactorBaseResponse>(json);

                if (loginFailReason.ErrorType == "sms_code_validation_code_invalid")
                    return Result.Fail("Please check the security code.", InstaLoginTwoFactorResult.InvalidCode);
                else if (loginFailReason.Message.ToLower().Contains("challenge"))
                {
                    _challengeInfo = loginFailReason.Challenge;

                    return Result.Fail("Challenge is required", InstaLoginTwoFactorResult.ChallengeRequired);

                }
                return Result.Fail("This code is no longer valid, please, call LoginAsync again to request a new one",
                    InstaLoginTwoFactorResult.CodeExpired);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaLoginTwoFactorResult), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Result.Fail(exception, InstaLoginTwoFactorResult.Exception);
            }
        }

        /// <summary>
        ///     Get Two Factor Authentication details
        /// </summary>
        /// <returns>
        ///     An instance of TwoFactorInfo if success.
        ///     A null reference if not success; in this case, do LoginAsync first and check if Two Factor Authentication is
        ///     required, if not, don't run this method
        /// </returns>
        public async Task<IResult<InstaTwoFactorLoginInfo>> GetTwoFactorInfoAsync()
        {
            return await Task.Run(() =>
                _twoFactorInfo != null
                    ? Result.Success(_twoFactorInfo)
                    : Result.Fail<InstaTwoFactorLoginInfo>("No Two Factor info available."));
        }

        /// <summary>
        ///     Logout from instagram asynchronously
        /// </summary>
        /// <returns>
        ///     True if logged out without errors
        /// </returns>
        public async Task<IResult<bool>> LogoutAsync()
        {
            ValidateUser();
            ValidateLoggedIn();
            try
            {
                var instaUri = UriCreator.GetLogoutUri();
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, DeviceInfo);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<bool>(response, json);
                var logoutInfo = JsonConvert.DeserializeObject<BaseStatusResponse>(json);
                if (logoutInfo.Status == "ok")
                    IsUserAuthenticated = false;
                return Result.Success(!IsUserAuthenticated);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Result.Fail(exception, false);
            }
        }

        /// <summary>
        ///     Get user lookup for recovery options
        /// </summary>
        /// <param name="usernameOrEmailOrPhoneNumber">Username or email or phone number</param>
        public async Task<IResult<InstaUserLookup>> GetRecoveryOptionsAsync(string usernameOrEmailOrPhoneNumber)
        {
            try
            {
                var csrfToken = "";
                if (!string.IsNullOrEmpty(_user.CsrfToken))
                    csrfToken = _user.CsrfToken;
                else
                {
                    var firstResponse = await RequestProcessor.GetAsync(RequestProcessor.Client.BaseAddress);
                    var cookies =
                        RequestProcessor.HttpHandler.CookieContainer.GetCookies(RequestProcessor.Client
                            .BaseAddress);
                    _logger?.LogResponse(firstResponse);
                    csrfToken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                }
                var data = new JObject
                {
                    {"_csrftoken", csrfToken},
                    {"q", usernameOrEmailOrPhoneNumber},
                    {"guid",  DeviceInfo.Uuid.ToString()},
                    {"device_id", DeviceInfo.DeviceId},
                    {"directly_sign_in", "true"},
                };

                var instaUri = UriCreator.GetUsersLookupUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);

                var response = await RequestProcessor.SendAsync(request);

                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<InstaUserLookupResponse>(json);
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.Fail<InstaUserLookup>(obj.Message);

                return Result.Success(ConvertersFabric.Instance.GetUserLookupConverter(obj).Convert());
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaUserLookup), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                return Result.Fail<InstaUserLookup>(exception);
            }
        }
        /// <summary>
        ///     Send recovery code by Username
        /// </summary>
        /// <param name="username">Username</param>
        public async Task<IResult<InstaRecovery>> SendRecoveryByUsernameAsync(string username)
        {
            return await SendRecoveryByEmailAsync(username);
        }

        /// <summary>
        ///     Send recovery code by Email
        /// </summary>
        /// <param name="email">Email Address</param>
        public async Task<IResult<InstaRecovery>> SendRecoveryByEmailAsync(string email)
        {
            try
            {
                var token = "";
                if (!string.IsNullOrEmpty(_user.CsrfToken))
                    token = _user.CsrfToken;
                else
                {
                    var firstResponse = await RequestProcessor.GetAsync(RequestProcessor.Client.BaseAddress);
                    var cookies =
                        RequestProcessor.HttpHandler.CookieContainer.GetCookies(RequestProcessor.Client
                            .BaseAddress);
                    _logger?.LogResponse(firstResponse);
                    token = InstaApiHelper.GetCsrfToken(RequestProcessor);
                }

                var postData = new JObject
                {
                    {"query", email },
                    {"adid", DeviceInfo.GoogleAdId },
                    {"device_id",  ApiRequestMessage.GenerateDeviceId()},
                    {"guid",  DeviceInfo.Uuid.ToString()},
                    {"_csrftoken", token },
                };

                var instaUri = UriCreator.GetAccountRecoveryEmailUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);

                var response = await RequestProcessor.SendAsync(request);

                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var error = JsonConvert.DeserializeObject<MessageErrorsResponseRecoveryEmail>(result);
                    return Result.Fail<InstaRecovery>(error.Message);
                }

                return Result.Success(JsonConvert.DeserializeObject<InstaRecovery>(result));
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaRecovery), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                return Result.Fail<InstaRecovery>(exception);
            }
        }

        /// <summary>
        ///     Send recovery code by Phone
        /// </summary>
        /// <param name="phone">Phone Number</param>
        public async Task<IResult<InstaRecovery>> SendRecoveryByPhoneAsync(string phone)
        {
            try
            {
                var token = "";
                if (!string.IsNullOrEmpty(_user.CsrfToken))
                    token = _user.CsrfToken;
                else
                {
                    var firstResponse = await RequestProcessor.GetAsync(RequestProcessor.Client.BaseAddress);
                    var cookies =
                        RequestProcessor.HttpHandler.CookieContainer.GetCookies(RequestProcessor.Client
                            .BaseAddress);
                    _logger?.LogResponse(firstResponse);
                    token = InstaApiHelper.GetCsrfToken(RequestProcessor);
                }

                var postData = new JObject
                {
                    {"query",  phone},
                    {"_csrftoken",  _user.CsrfToken},
                };

                var instaUri = UriCreator.GetAccountRecoverPhoneUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);

                var response = await RequestProcessor.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var error = JsonConvert.DeserializeObject<BadStatusErrorsResponse>(result);
                    var errors = "";
                    error.Message.Errors.ForEach(errorContent => errors += errorContent + "\n");
                    return Result.Fail<InstaRecovery>(errors);
                }

                if (result.Contains("errors"))
                {
                    var error = JsonConvert.DeserializeObject<BadStatusErrorsResponseRecovery>(result);
                    var errors = "";
                    error.PhoneNumber.Errors.ForEach(errorContent => errors += errorContent + "\n");

                    return Result.Fail<InstaRecovery>(errors);
                }
                return Result.Success(JsonConvert.DeserializeObject<InstaRecovery>(result));
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaRecovery), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                return Result.Fail<InstaRecovery>(exception);
            }
        }


        /// <summary>
        ///    Send Two Factor Login SMS Again
        /// </summary>
        public async Task<IResult<TwoFactorLoginSMS>> SendTwoFactorLoginSMSAsync()
        {
            try
            {
                if (_twoFactorInfo == null)
                    return Result.Fail<TwoFactorLoginSMS>("Run LoginAsync first");

                var postData = new Dictionary<string, string>
                {
                    { "two_factor_identifier",  _twoFactorInfo.TwoFactorIdentifier },
                    { "username",    RequestProcessor.RequestMessage.Username},
                    { "device_id",   RequestProcessor.RequestMessage.DeviceId},
                    { "guid",        DeviceInfo.Uuid.ToString()},
                    { "_csrftoken",    _user.CsrfToken }
                };

                var instaUri = UriCreator.GetAccount2FALoginAgainUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);
                var response = await RequestProcessor.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                var T = JsonConvert.DeserializeObject<TwoFactorLoginSMS>(result);
                if (!string.IsNullOrEmpty(T.TwoFactorInfo.TwoFactorIdentifier))
                    _twoFactorInfo.TwoFactorIdentifier = T.TwoFactorInfo.TwoFactorIdentifier;
                return Result.Success(T);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(TwoFactorLoginSMS), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<TwoFactorLoginSMS>(exception);
            }
        }

        #region Challenge part

        /// <summary>
        ///     Get challenge data for logged in user
        ///     <para>This will pop-on, if some suspecious login happend</para>
        /// </summary>
        public async Task<IResult<InstaLoggedInChallengeDataInfo>> GetLoggedInChallengeDataInfoAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);

            try
            {
                var instaUri = UriCreator.GetChallengeRequireFirstUri("/challenge/", DeviceInfo.Uuid.ToString(), DeviceInfo.DeviceId);
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, DeviceInfo);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaLoggedInChallengeDataInfo>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaLoggedInChallengeDataInfoContainer>(json);
                return Result.Success(obj?.StepData);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaLoggedInChallengeDataInfo), ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, (InstaLoggedInChallengeDataInfo)null);
            }
        }

        /// <summary>
        ///     Accept challlenge, it is THIS IS ME feature!!!!
        ///     <para>You must call <see cref="IInstaApi.GetLoggedInChallengeDataInfoAsync"/> first,
        ///     if you across to <see cref="ResultInfo.ResponseType"/> equals to <see cref="ResponseType.ChallengeRequired"/> while you logged in!</para>
        /// </summary>
        public async Task<IResult<bool>> AcceptChallengeAsync()
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetChallengeUri();

                var data = new JObject
                {
                    {"choice", "0"},
                    {"_csrftoken", _user.CsrfToken},
                    {"_uid", _user.LoggedInUser.Pk},
                    {"guid", DeviceInfo.Uuid.ToString()},
                    {"device_id", DeviceInfo.DeviceId},
                    {"_uuid", DeviceInfo.Uuid.ToString()}
                };

                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaChallengeRequireVerifyCode>(json);
                return obj.Action.ToLower() == "close" ? Result.Success(true) : Result.Success(false);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, false, ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail<bool>(ex);
            }
        }

        /// <summary>
        ///     Get challenge require (checkpoint required) options
        /// </summary>
        public async Task<IResult<InstaChallengeRequireVerifyMethod>> GetChallengeRequireVerifyMethodAsync()
        {
            if (_challengeInfo == null)
                return Result.Fail("challenge require info is empty.\r\ntry to call LoginAsync function first.", (InstaChallengeRequireVerifyMethod)null);

            try
            {
                var instaUri = UriCreator.GetChallengeRequireFirstUri(_challengeInfo.ApiPath, DeviceInfo.Uuid.ToString(), DeviceInfo.DeviceId);
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, DeviceInfo);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaChallengeRequireVerifyMethod>(response, json);

                var obj = JsonConvert.DeserializeObject<InstaChallengeRequireVerifyMethod>(json);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaChallengeRequireVerifyMethod), ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, (InstaChallengeRequireVerifyMethod)null);
            }
        }
        /// <summary>
        ///     Reset challenge require (checkpoint required) method
        /// </summary>
        public async Task<IResult<InstaChallengeRequireVerifyMethod>> ResetChallengeRequireVerifyMethodAsync()
        {
            if (_challengeInfo == null)
                return Result.Fail("challenge require info is empty.\r\ntry to call LoginAsync function first.", (InstaChallengeRequireVerifyMethod)null);

            try
            {
                var instaUri = UriCreator.GetResetChallengeRequireUri(_challengeInfo.ApiPath);
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"guid", DeviceInfo.Uuid.ToString()},
                    {"device_id", DeviceInfo.DeviceId},
                };
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var msg = "";
                    try
                    {
                        var j = JsonConvert.DeserializeObject<InstaChallengeRequireVerifyMethod>(json);
                        msg = j.Message;
                    }
                    catch { }
                    return Result.UnExpectedResponse<InstaChallengeRequireVerifyMethod>(response, json);
                }

                var obj = JsonConvert.DeserializeObject<InstaChallengeRequireVerifyMethod>(json);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaChallengeRequireVerifyMethod), ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, (InstaChallengeRequireVerifyMethod)null);
            }
        }
        /// <summary>
        ///     Request verification code sms for challenge require (checkpoint required)
        /// </summary>
        /// <param name="replayChallenge">true if Instagram should resend verification code to you</param>
        public async Task<IResult<InstaChallengeRequireSMSVerify>> RequestVerifyCodeToSMSForChallengeRequireAsync(bool replayChallenge)
        {
            return await RequestVerifyCodeToSMSForChallengeRequire(replayChallenge);
        }
        /// <summary>
        ///     Submit phone number for challenge require (checkpoint required)
        ///     <para>Note: This only needs , when you calling <see cref="IInstaApi.GetChallengeRequireVerifyMethodAsync"/> or
        ///     <see cref="IInstaApi.ResetChallengeRequireVerifyMethodAsync"/> and
        ///     <see cref="InstaChallengeRequireVerifyMethod.SubmitPhoneRequired"/> property is true.</para>
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        public async Task<IResult<InstaChallengeRequireSMSVerify>> SubmitPhoneNumberForChallengeRequireAsync(string phoneNumber, bool replayChallenge)
        {
            return await RequestVerifyCodeToSMSForChallengeRequire(replayChallenge, phoneNumber);
        }

        private async Task<IResult<InstaChallengeRequireSMSVerify>> RequestVerifyCodeToSMSForChallengeRequire(bool replayChallenge, string phoneNumber = null)
        {
            if (_challengeInfo == null)
                return Result.Fail("challenge require info is empty.\r\ntry to call LoginAsync function first.", (InstaChallengeRequireSMSVerify)null);

            try
            {
                Uri instaUri;

                if (replayChallenge)
                {
                    instaUri = UriCreator.GetChallengeReplayUri(_challengeInfo.ApiPath);
                }
                else
                {
                    instaUri = UriCreator.GetChallengeRequireUri(_challengeInfo.ApiPath);
                }

                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"guid", DeviceInfo.Uuid.ToString()},
                    {"device_id", DeviceInfo.DeviceId},
                };
                if (!string.IsNullOrEmpty(phoneNumber))
                    data.Add("phone_number", phoneNumber);
                else
                    data.Add("choice", "0");

                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                request.Headers.Add("Host", "i.instagram.com");
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var msg = "";
                    try
                    {
                        var j = JsonConvert.DeserializeObject<InstaChallengeRequireSMSVerify>(json);
                        msg = j.Message;
                    }
                    catch { }
                    return Result.Fail(msg, (InstaChallengeRequireSMSVerify)null);
                }

                var obj = JsonConvert.DeserializeObject<InstaChallengeRequireSMSVerify>(json);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaChallengeRequireSMSVerify), ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, (InstaChallengeRequireSMSVerify)null);
            }
        }
        /// <summary>
        ///     Request verification code email for challenge require (checkpoint required)
        /// </summary>
        /// <param name="replayChallenge">true if Instagram should resend verification code to you</param>
        public async Task<IResult<InstaChallengeRequireEmailVerify>> RequestVerifyCodeToEmailForChallengeRequireAsync(bool replayChallenge)
        {
            if (_challengeInfo == null)
                return Result.Fail("challenge require info is empty.\r\ntry to call LoginAsync function first.", (InstaChallengeRequireEmailVerify)null);

            try
            {
                Uri instaUri;

                if (replayChallenge)
                {
                    instaUri = UriCreator.GetChallengeReplayUri(_challengeInfo.ApiPath);
                }
                else
                {
                    instaUri = UriCreator.GetChallengeRequireUri(_challengeInfo.ApiPath);
                }

                var data = new JObject
                {
                    {"choice", "1"},
                    {"_csrftoken", _user.CsrfToken},
                    {"guid", DeviceInfo.Uuid.ToString()},
                    {"device_id", DeviceInfo.DeviceId},
                };
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                request.Headers.Add("Host", "i.instagram.com");
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var msg = "";
                    try
                    {
                        var j = JsonConvert.DeserializeObject<InstaChallengeRequireEmailVerify>(json);
                        msg = j.Message;
                    }
                    catch { }
                    return Result.Fail(msg, (InstaChallengeRequireEmailVerify)null);
                }

                var obj = JsonConvert.DeserializeObject<InstaChallengeRequireEmailVerify>(json);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaChallengeRequireEmailVerify), ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, (InstaChallengeRequireEmailVerify)null);
            }
        }
        /// <summary>
        ///     Verify verification code for challenge require (checkpoint required)
        /// </summary>
        /// <param name="verifyCode">Verification code</param>
        public async Task<IResult<InstaLoginResult>> VerifyCodeForChallengeRequireAsync(string verifyCode)
        {
            if (_challengeInfo == null)
                return Result.Fail("challenge require info is empty.\r\ntry to call LoginAsync function first.", InstaLoginResult.Exception);

            if (verifyCode.Length != 6)
                return Result.Fail("Verify code must be an 6 digit number.", InstaLoginResult.Exception);

            try
            {
                var cookies =
            RequestProcessor.HttpHandler.CookieContainer.GetCookies(RequestProcessor.Client
                .BaseAddress);
                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                _user.CsrfToken = csrftoken;
                var instaUri = UriCreator.GetChallengeRequireUri(_challengeInfo.ApiPath);

                var data = new JObject
                {
                    {"security_code", verifyCode},
                    {"_csrftoken", _user.CsrfToken},
                    {"guid", DeviceInfo.Uuid.ToString()},
                    {"device_id", DeviceInfo.DeviceId},
                };
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                request.Headers.Add("Host", "i.instagram.com");
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var msg = "";
                    try
                    {
                        var j = JsonConvert.DeserializeObject<InstaChallengeRequireVerifyCode>(json);
                        msg = j.Message;
                    }
                    catch { }
                    return Result.Fail(msg, InstaLoginResult.Exception);
                }

                var obj = JsonConvert.DeserializeObject<InstaChallengeRequireVerifyCode>(json);
                if (obj != null)
                {
                    if (obj.LoggedInUser != null)
                    {
                        ValidateUserAsync(obj.LoggedInUser, csrftoken);
                        await Task.Delay(3000);
                        await _messagingProcessor.GetInboxAsync(PaginationParameters.MaxPagesToLoad(1));
                        await _feedProcessor.GetRecentActivityFeedAsync(PaginationParameters.MaxPagesToLoad(1));

                        return Result.Success(InstaLoginResult.Success);
                    }

                    if (!string.IsNullOrEmpty(obj.Action))
                    {
                        // we should wait at least 15 seconds and then trying to login again
                        await Task.Delay(15000);
                        return await LoginAsync(false);
                    }
                }
                return Result.Fail(obj?.Message, InstaLoginResult.Exception);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaLoginResult), ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Result.Fail(ex, InstaLoginResult.Exception);
            }
        }
        #endregion Challenge part

        #endregion Authentication and challenge functions

        #region ORIGINAL FACEBOOK LOGIN

        string _facebookAccessToken = null;
        /// <summary>
        ///     Login with Facebook access token
        /// </summary>
        /// <param name="fbAccessToken">Facebook access token</param>
        /// <param name="cookiesContainer">Cookies</param>
        /// <returns>
        ///     Success --> is succeed
        ///     TwoFactorRequired --> requires 2FA login.
        ///     BadPassword --> Password is wrong
        ///     InvalidUser --> User/phone number is wrong
        ///     Exception --> Something wrong happened
        ///     ChallengeRequired --> You need to pass Instagram challenge
        /// </returns>
        public async Task<IResult<InstaLoginResult>> LoginWithFacebookAsync(string fbAccessToken, string cookiesContainer)
        {
            return await LoginWithFacebookAsync(fbAccessToken, cookiesContainer, true, null,null,null, true);
        }

        public async Task<IResult<InstaLoginResult>> LoginWithFacebookAsync(string fbAccessToken, string cookiesContainer, 
            bool dryrun = true, string username = null, string waterfallId = null, string adId = null, bool newToken = true)
        {
            try
            {
                _facebookAccessToken = null;
                if (newToken)
                {
                    var firstResponse = await RequestProcessor.GetAsync(RequestProcessor.Client.BaseAddress);
                    await firstResponse.Content.ReadAsStringAsync();
                }
                else
                    System.Diagnostics.Debug.WriteLine("--------------------RELOGIN-------------------------");
                
                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                var uri = new Uri(InstaApiConstants.INSTAGRAM_URL);

                cookiesContainer = cookiesContainer.Replace(';', ',');
                RequestProcessor.HttpHandler.CookieContainer.SetCookies(uri, cookiesContainer);

                if (adId.IsEmpty())
                    adId = Guid.NewGuid().ToString();

                if (waterfallId.IsEmpty())
                    waterfallId = Guid.NewGuid().ToString();

                var instaUri = UriCreator.GetFacebookSignUpUri();

                var data = new JObject
                {
                    {"dryrun", dryrun.ToString().ToLower()},
                    {"phone_id", DeviceInfo.PhoneId.ToString()},
                    {"_csrftoken", csrftoken},
                    {"adid", adId},
                    {"guid",  DeviceInfo.Uuid.ToString()},
                    {"_uuid",  DeviceInfo.Uuid.ToString()},
                    {"device_id", DeviceInfo.DeviceId},
                    {"waterfall_id", waterfallId},
                    {"fb_access_token", fbAccessToken},
                };
                if (username.IsNotEmpty())
                    data.Add("username", username);

                _facebookAccessToken = fbAccessToken;
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var loginFailReason = JsonConvert.DeserializeObject<InstaLoginBaseResponse>(json);

                    if (loginFailReason.InvalidCredentials)
                        return Result.Fail("Invalid Credentials",
                            loginFailReason.ErrorType == "bad_password"
                                ? InstaLoginResult.BadPassword
                                : InstaLoginResult.InvalidUser);
                    if (loginFailReason.TwoFactorRequired)
                    {
                        _twoFactorInfo = loginFailReason.TwoFactorLoginInfo;
                        RequestProcessor.RequestMessage.Username = _twoFactorInfo.Username;
                        RequestProcessor.RequestMessage.DeviceId = DeviceInfo.DeviceId;
                        return Result.Fail("Two Factor Authentication is required", InstaLoginResult.TwoFactorRequired);
                    }
                    if (loginFailReason.ErrorType == "checkpoint_challenge_required")
                    {
                        _challengeInfo = loginFailReason.Challenge;

                        return Result.Fail("Challenge is required", InstaLoginResult.ChallengeRequired);
                    }
                    if (loginFailReason.ErrorType == "rate_limit_error")
                    {
                        return Result.Fail("Please wait a few minutes before you try again.", InstaLoginResult.LimitError);
                    }
                    if (loginFailReason.ErrorType == "inactive user" || loginFailReason.ErrorType == "inactive_user")
                    {
                        return Result.Fail($"{loginFailReason.Message}\r\nHelp url: {loginFailReason.HelpUrl}", InstaLoginResult.InactiveUser);
                    }
                    if (loginFailReason.ErrorType == "checkpoint_logged_out")
                        return Result.Fail($"{loginFailReason.ErrorType} {loginFailReason.CheckpointUrl}", InstaLoginResult.CheckpointLoggedOut);
                    
                    return Result.UnExpectedResponse<InstaLoginResult>(response, json);
                }

                var fbUserId = string.Empty;
                InstaUserShortResponse loginInfoUser = null;
                if (json.Contains("\"account_created\""))
                {
                    var rmt = JsonConvert.DeserializeObject<InstaFacebookRegistrationResponse>(json);
                    if(rmt?.AccountCreated != null)
                    {
                        fbUserId = rmt?.FbUserId;
                        if (rmt.AccountCreated.Value)
                        {
                            loginInfoUser = JsonConvert.DeserializeObject<InstaFacebookLoginResponse>(json)?.CreatedUser;
                        }
                        else
                        {
                            var desireUsername = rmt?.UsernameSuggestionsWithMetadata?.Suggestions?.LastOrDefault()?.Username;
                            await Task.Delay(4500);
                            await GetFacebookOnboardingStepsAsync();
                            await Task.Delay(12000);

                            return await LoginWithFacebookAsync(fbAccessToken, cookiesContainer, false, desireUsername, waterfallId, adId, false);
                        }
                    }
                }

                if(loginInfoUser == null)
                {
                    var obj = JsonConvert.DeserializeObject<InstaFacebookLoginResponse>(json);
                    fbUserId = obj?.FbUserId;
                    loginInfoUser = obj?.LoggedInUser;
                }
                
                IsUserAuthenticated = true;
                var converter = ConvertersFabric.Instance.GetUserShortConverter(loginInfoUser);
                _user.LoggedInUser = converter.Convert();
                _user.RankToken = $"{_user.LoggedInUser.Pk}_{RequestProcessor.RequestMessage.PhoneId}";
                _user.CsrfToken = csrftoken;
                _user.FacebookUserId = fbUserId;
                _user.UserName = _user.LoggedInUser.UserName;
                _user.FacebookAccessToken = fbAccessToken;
                _user.Password = "ALAKIMASALAN";

                InvalidateProcessors();

                _user.RankToken = $"{_user.LoggedInUser.Pk}_{RequestProcessor.RequestMessage.PhoneId}";
                if (string.IsNullOrEmpty(_user.CsrfToken))
                {
                    _user.CsrfToken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                }
                return Result.Success(InstaLoginResult.Success);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, InstaLoginResult.Exception, ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Result.Fail(exception, InstaLoginResult.Exception);
            }
        }

        private async Task<IResult<object>> GetFacebookOnboardingStepsAsync()
        {
            try
            {
                var cookies =
                    RequestProcessor.HttpHandler.CookieContainer.GetCookies(RequestProcessor.Client
                    .BaseAddress);
                var csrftoken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                _user.CsrfToken = csrftoken;

                //{
                //  "fb_connected": "true",
                //  "seen_steps": "[]",
                //  "phone_id": "d46328c2-01af-4457-9da2-bc60637abde6",
                //  "fb_installed": "false",
                //  "locale": "en_US",
                //  "timezone_offset": "12600",
                //  "_csrftoken": "2YmsoSkHtIknBA8maAqb1QSk92nrM6xo",
                //  "network_type": "WIFI-UNKNOWN",
                //  "_uid": "9013775990",
                //  "guid": "6324ecb2-e663-4dc8-a3a1-289c699cc876",
                //  "_uuid": "6324ecb2-e663-4dc8-a3a1-289c699cc876",
                //  "is_ci": "false",
                //  "android_id": "android-21c311d494a974fe",
                //  "reg_flow_taken": "facebook",
                //  "tos_accepted": "false"
                //}

                var postData = new Dictionary<string, string>
                {
                    {"fb_connected",        "true"},
                    {"seen_steps",          "[]"},
                    {"phone_id",            DeviceInfo.PhoneId.ToString()},
                    {"fb_installed",        "false"},
                    {"locale",              InstaApiConstants.ACCEPT_LANGUAGE.Replace("-","_")},
                    {"timezone_offset",     InstaApiConstants.TIMEZONE_OFFSET.ToString()},
                    {"_csrftoken",          csrftoken},
                    {"network_type",        "WIFI-UNKNOWN"},
                    {"guid",                DeviceInfo.Uuid.ToString()},
                    {"_uuid",               DeviceInfo.Uuid.ToString()},
                    {"is_ci",               "false"},
                    {"android_id",          DeviceInfo.DeviceId},
                    {"reg_flow_taken",      "facebook"},
                    {"tos_accepted",        "false"}
                };


                var instaUri = UriCreator.GetOnboardingStepsUri();
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, postData);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var o = JsonConvert.DeserializeObject<InstaAccountRegistrationPhoneNumber>(json);

                    return Result.Fail(o.Message?.Errors?[0], (InstaRegistrationSuggestionResponse)null);
                }

                var obj = JsonConvert.DeserializeObject<InstaRegistrationSuggestionResponse>(json);
                return Result.Success(obj);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(InstaRegistrationSuggestionResponse), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<InstaRegistrationSuggestionResponse>(exception);
            }
        }

        private async Task<IResult<bool>> AcceptFacebookConsentRequiredAsync(string email, string phone = null)
        {
            try
            {
                var delay = TimeSpan.FromSeconds(2);

                //{"message": "consent_required", "consent_data": {"headline": "Updates to Our Terms and Data Policy", "content": "We've updated our Terms and made some changes to our Data Policy. Please take a moment to review these changes and let us know that you agree to them.\n\nYou need to finish reviewing this information before you can use Instagram.", "button_text": "Review Now"}, "status": "fail"}
                await Task.Delay((int)delay.TotalMilliseconds);
                var instaUri = UriCreator.GetConsentNewUserFlowBeginsUri();
                var data = new JObject
                {
                    {"phone_id", DeviceInfo.PhoneId},
                    {"_csrftoken", _user.CsrfToken}
                };
                var request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                await Task.Delay((int)delay.TotalMilliseconds);

                instaUri = UriCreator.GetConsentNewUserFlowUri();
                data = new JObject
                {
                    {"phone_id", DeviceInfo.PhoneId},
                    {"gdpr_s", ""},
                    {"_csrftoken", _user.CsrfToken},
                    {"guid", DeviceInfo.Uuid},
                    {"device_id", DeviceInfo.DeviceId}
                };
                if (email != null)
                    data.Add("email", email);
                else
                {
                    if (phone != null && !phone.StartsWith("+"))
                        phone = $"+{phone}";

                    if (phone == null)
                        phone = string.Empty;
                    data.Add("phone", phone);
                }

                request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                response = await RequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                await Task.Delay((int)delay.TotalMilliseconds);

                data = new JObject
                {
                    {"current_screen_key", "age_consent_two_button"},
                    {"phone_id", DeviceInfo.PhoneId},
                    {"gdpr_s", "[0,0,0,null]"},
                    {"_csrftoken", _user.CsrfToken},
                    {"updates", "{\"age_consent_state\":\"2\"}"},
                    {"guid", DeviceInfo.Uuid},
                    {"device_id", DeviceInfo.DeviceId}
                };
                request = HttpHelper.GetSignedRequest(instaUri, DeviceInfo, data);
                response = await RequestProcessor.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);

                return Result.Success(true);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, false);
            }
        }
        #endregion ORIGINAL FACEBOOK LOGIN

        #region Other public functions
        /// <summary>
        ///     Get current API version info (signature key, api version info, app id)
        /// </summary>
        public ApiVersion GetApiVersionInfo() => ApiVersion.CurrentApiVersion;

        /// <summary>
        ///     Get user agent of current <see cref="IInstaApi"/>
        /// </summary>
        public string GetUserAgent() => DeviceInfo.UserAgent;
        /// <summary>
        ///     Set timeout to <see cref="HttpClient"/>
        ///     <para>Note: Set timeout more than 100 seconds!</para>
        /// </summary>
        /// <param name="timeout">Timeout (set more than 100 seconds!)</param>
        public void SetTimeout(TimeSpan timeout)
        {
            if (timeout == null)
                timeout = TimeSpan.FromSeconds(350);

            HttpClient.Timeout = timeout;
        }
        /// <summary>
        ///     Set custom HttpClientHandler to be able to use certain features, e.g Proxy and so on
        /// </summary>
        /// <param name="handler">HttpClientHandler</param>
        public void UseHttpClientHandler(HttpClientHandler handler)
        {
            RequestProcessor.SetHttpClientHandler(handler);
        }
        /// <summary>
        /// Sets user credentials
        /// </summary>
        public void SetUser(string username, string password)
        {
            _user.UserName = username;
            _user.Password = password;

            RequestProcessor.RequestMessage.Username = username;
            RequestProcessor.RequestMessage.Password = password;
        }

        /// <summary>
        /// Sets user credentials
        /// </summary>
        public void SetUser(UserSessionData user)
        {
            SetUser(user.UserName, user.Password);
        }

        /// <summary>
        ///     Gets current device
        /// </summary>
        public AndroidDevice GetCurrentDevice()
        {
            return DeviceInfo;
        }
        /// <summary>
        ///     Gets logged in user
        /// </summary>
        public UserSessionData GetLoggedUser()
        {
            return _user;
        }
        /// <summary>
        ///     Get currently logged in user info asynchronously
        /// </summary>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaCurrentUser" />
        /// </returns>
        public async Task<IResult<InstaCurrentUser>> GetCurrentUserAsync()
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _userProcessor.GetCurrentUserAsync();
        }
        /// <summary>
        ///     Get Accept Language
        /// </summary>
        public string GetAcceptLanguage()
        {
            try
            {
                return InstaApiConstants.ACCEPT_LANGUAGE;
            }
            catch (Exception exception)
            {
                return Result.Fail<string>(exception).Value;
            }
        }
        /// <summary>
        ///     Get current time zone
        ///     <para>Returns something like: Asia/Tehran</para>
        /// </summary>
        /// <returns>Returns something like: Asia/Tehran</returns>
        public string GetTimezone() => InstaApiConstants.TIMEZONE;
        /// <summary>
        ///     Get current time zone offset
        ///     <para>Returns something like this: 16200</para>
        /// </summary>
        /// <returns>Returns something like this: 16200</returns>
        public int GetTimezoneOffset() => InstaApiConstants.TIMEZONE_OFFSET;
        /// <summary>
        ///     Set delay between requests. Useful when API supposed to be used for mass-bombing.
        /// </summary>
        /// <param name="delay">Timespan delay</param>
        public void SetRequestDelay(IRequestDelay delay)
        {
            if (delay == null)
                delay = RequestDelay.Empty();
            _delay = delay;
            RequestProcessor.Delay = _delay;
        }

        internal IRequestDelay GetRequestDelay() => _delay;

        /// <summary>
        ///     Set instagram api version (for user agent version)
        /// </summary>
        /// <param name="apiVersion">Api version</param>
        public void SetApiVersion(ApiVersionNumber apiVersion)
        {
            ApiVersion.CurrentApiVersion = ApiVersion.GetApiVersion(apiVersion);
        }
        /// <summary>
        ///     Set custom android device.
        ///     <para>Note 1: If you want to use this method, you should call it before you calling <seealso cref="IInstaApi.LoadStateDataFromStream(Stream)"/> or <seealso cref="IInstaApi.LoadStateDataFromString(string)"/></para>
        ///     <para>Note 2: this is optional, if you didn't set this, InstagramApiSharp will choose random device.</para>
        /// </summary>
        /// <param name="device">Android device</param>
        public void SetDevice(AndroidDevice device)
        {
            if (device == null)
                return;
            DeviceInfo = device;
        }
        /// <summary>
        ///     Set Accept Language
        /// </summary>
        /// <param name="languageCodeAndCountryCode">Language Code and Country Code. For example:
        /// <para>en-US for united states</para>
        /// <para>fa-IR for IRAN</para>
        /// </param>
        public bool SetAcceptLanguage(string languageCodeAndCountryCode)
        {
            try
            {
                InstaApiConstants.ACCEPT_LANGUAGE = languageCodeAndCountryCode;
                return true;
            }
            catch (Exception exception)
            {
                return Result.Fail<bool>(exception).Value;
            }
        }
        /// <summary>
        ///     Set time zone
        ///     <para>I.e: Asia/Tehran for Iran</para>
        /// </summary>
        /// <param name="timezone">
        ///     time zone
        ///     <para>I.e: Asia/Tehran for Iran</para>
        /// </param>
        public void SetTimezone(string timezone)
        {
            if (string.IsNullOrEmpty(timezone))
                return;
            InstaApiConstants.TIMEZONE = timezone;
        }
        /// <summary>
        ///     Set time zone offset
        ///     <para>I.e: 16200 for Iran/Tehran</para>
        /// </summary>
        /// <param name="timezoneOffset">
        ///     time zone offset
        ///     <para>I.e: 16200 for Iran/Tehran</para>
        /// </param>
        public void SetTimezoneOffset(int timezoneOffset)
        {
            InstaApiConstants.TIMEZONE_OFFSET = timezoneOffset;
        }

        public async Task<HttpResponseMessage> SendGetRequestAsync(Uri uri, HttpCompletionOption completionOption)
        {
            try
            {
                if (uri == null)
                    throw new ArgumentNullException(nameof(uri));

                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, uri, DeviceInfo);
                var response = await RequestProcessor.SendAsync(request, completionOption);
                return response;
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                throw;
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                throw;
            }
        }

        /// <summary>
        ///     Send get request
        /// </summary>
        /// <param name="uri">Desire uri (must include https://i.instagram.com/api/v...) </param>
        public async Task<HttpResponseMessage> SendGetRequestAsync(Uri uri)
        {
            return await SendGetRequestAsync(uri, HttpCompletionOption.ResponseContentRead);
        }
        
        /// <summary>
        ///     Send signed post request (include signed signature) 
        /// </summary>
        /// <param name="uri">Desire uri (must include https://i.instagram.com/api/v...) </param>
        /// <param name="data">Data to post</param>
        public async Task<IResult<string>> SendSignedPostRequestAsync(Uri uri, Dictionary<string, string> data)
        {
            return await SendSignedPostRequest(uri, null, data);
        }
        /// <summary>
        ///     Send signed post request (include signed signature) 
        /// </summary>
        /// <param name="uri">Desire uri (must include https://i.instagram.com/api/v...) </param>
        /// <param name="data">Data to post</param>
        public async Task<IResult<string>> SendSignedPostRequestAsync(Uri uri, JObject data)
        {
            return await SendSignedPostRequest(uri, data, null);
        }
        private async Task<IResult<string>> SendSignedPostRequest(Uri uri, JObject JData, Dictionary<string, string> DicData)
        {
            try
            {
                if (uri == null)
                    return Result.Fail("Uri cannot be null!", default(string));

                HttpRequestMessage request;
                if (JData != null)
                {
                    JData.Add("_uuid", DeviceInfo.Uuid.ToString());
                    JData.Add("_uid", _user.LoggedInUser.Pk.ToString());
                    JData.Add("_csrftoken", _user.CsrfToken);
                    request = HttpHelper.GetSignedRequest(uri, DeviceInfo, JData);
                }
                else
                {
                    DicData.Add("_uuid", DeviceInfo.Uuid.ToString());
                    DicData.Add("_uid", _user.LoggedInUser.Pk.ToString());
                    DicData.Add("_csrftoken", _user.CsrfToken);
                    request = HttpHelper.GetSignedRequest(uri, DeviceInfo, DicData);
                }

                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<string>(response, json);

                return Result.Success(json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(string), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, default(string));
            }
        }
        /// <summary>
        ///     Send post request
        /// </summary>
        /// <param name="uri">Desire uri (must include https://i.instagram.com/api/v...) </param>
        /// <param name="data">Data to post</param>
        public async Task<IResult<string>> SendPostRequestAsync(Uri uri, Dictionary<string, string> data)
        {
            try
            {
                if (uri == null)
                    return Result.Fail("Uri cannot be null!", default(string));

                data.Add("_uuid", DeviceInfo.Uuid.ToString());
                data.Add("_uid", _user.LoggedInUser.Pk.ToString());
                data.Add("_csrftoken", _user.CsrfToken);
                var request = HttpHelper.GetDefaultPostRequest(uri, DeviceInfo, data);
                var response = await RequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<string>(response, json);

                return Result.Success(json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(string), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail(exception, default(string));
            }
        }

        #endregion Other public functions

        #region State data

        /// <summary>
        ///     Get current state info
        /// </summary>
        /// <returns>State data</returns>
        public StateData GetStateData()
        {
            return new StateData
            {
                DeviceInfo = DeviceInfo,
                IsAuthenticated = IsUserAuthenticated,
                UserSession = _user,
                Cookies = RequestProcessor.HttpHandler.CookieContainer,
                FbnsConnectionData = PushClient.ConnectionData,
                CurrentApiVersion = ApiVersion.CurrentApiVersion
            };
        }

        /// <summary>
        ///     Get current state info as Memory stream
        /// </summary>
        /// <returns>
        ///     State data
        /// </returns>
        public Stream GetStateDataAsStream()
        {
            var state = new StateData
            {
                DeviceInfo = DeviceInfo,
                IsAuthenticated = IsUserAuthenticated,
                UserSession = _user,
                Cookies = RequestProcessor.HttpHandler.CookieContainer,
                FbnsConnectionData = PushClient.ConnectionData,
                CurrentApiVersion = ApiVersion.CurrentApiVersion
            };
            return SerializationHelper.SerializeToStream(state);
        }

        /// <summary>
        ///     Get current state info as Json string
        /// </summary>
        /// <returns>
        ///     State data
        /// </returns>
        public string GetStateDataAsString()
        {
            var state = new StateData
            {
                DeviceInfo = DeviceInfo,
                IsAuthenticated = IsUserAuthenticated,
                UserSession = _user,
                Cookies = RequestProcessor.HttpHandler.CookieContainer,
                FbnsConnectionData = PushClient.ConnectionData,
                CurrentApiVersion = ApiVersion.CurrentApiVersion
            };
            return SerializationHelper.SerializeToString(state);
        }

        /// <summary>
        ///     Get current state info as Memory stream asynchronously
        /// </summary>
        /// <returns>
        ///     State data
        /// </returns>
        public async Task<Stream> GetStateDataAsStreamAsync()
        {
            return await Task.Run(() =>
            {
                var state = GetStateDataAsStream();
                return state;
            });
        }
        /// <summary>
        ///     Get current state info as Json string asynchronously
        /// </summary>
        /// <returns>
        ///     State data
        /// </returns>
        public async Task<string> GetStateDataAsStringAsync()
        {
            return await Task.Run(() =>
            {
                var state = GetStateDataAsString();
                return state;
            });
        }
        /// <summary>
        ///     Loads the state data from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void LoadStateDataFromStream(Stream stream)
        {
            var data = SerializationHelper.DeserializeFromStream<StateData>(stream);
            DeviceInfo = data.DeviceInfo;
            _user = data.UserSession;

            RequestProcessor.RequestMessage.Username = data.UserSession.UserName;
            RequestProcessor.RequestMessage.Password = data.UserSession.Password;

            RequestProcessor.RequestMessage.DeviceId = data.DeviceInfo.DeviceId;
            RequestProcessor.RequestMessage.PhoneId = data.DeviceInfo.PhoneId.ToString();
            RequestProcessor.RequestMessage.Guid = data.DeviceInfo.Uuid;
            RequestProcessor.RequestMessage.AdId = data.DeviceInfo.AdId.ToString();
            RequestProcessor.HttpHandler.CookieContainer = data.Cookies;

            ApiVersion.CurrentApiVersion = data.CurrentApiVersion;

            Task.Run(async () => { await PushClient.Shutdown(); });
            PushClient = new FbnsClient(DeviceInfo, _user, RequestProcessor, data.FbnsConnectionData);

            IsUserAuthenticated = data.IsAuthenticated;
            InvalidateProcessors();
        }
        /// <summary>
        ///     Set state data from provided json string
        /// </summary>
        public void LoadStateDataFromString(string json)
        {
            var data = SerializationHelper.DeserializeFromString<StateData>(json);
            DeviceInfo = data.DeviceInfo;
            _user = data.UserSession;

            //Load Stream Edit 
            RequestProcessor.RequestMessage.Username = data.UserSession.UserName;
            RequestProcessor.RequestMessage.Password = data.UserSession.Password;

            RequestProcessor.RequestMessage.DeviceId = data.DeviceInfo.DeviceId;
            RequestProcessor.RequestMessage.PhoneId = data.DeviceInfo.PhoneId.ToString();
            RequestProcessor.RequestMessage.Guid = data.DeviceInfo.Uuid;
            RequestProcessor.RequestMessage.AdId = data.DeviceInfo.AdId.ToString();
            RequestProcessor.HttpHandler.CookieContainer = data.Cookies;

            ApiVersion.CurrentApiVersion = data.CurrentApiVersion;

            Task.Run(async () => { await PushClient.Shutdown(); });
            PushClient = new FbnsClient(DeviceInfo, _user, RequestProcessor, data.FbnsConnectionData);

            IsUserAuthenticated = data.IsAuthenticated;
            InvalidateProcessors();
        }


        /// <summary>
        ///     Set state data from provided stream asynchronously
        /// </summary>
        public async Task LoadStateDataFromStreamAsync(Stream stream)
        {
            await Task.Run(() =>
            {
                LoadStateDataFromStream(stream);
            });
        }
        /// <summary>
        ///     Set state data from provided json string asynchronously
        /// </summary>
        public async Task LoadStateDataFromStringAsync(string json)
        {
            await Task.Run(() =>
            {
                LoadStateDataFromString(json);
            });
        }

        #endregion State data

        #region private part

        internal void InvalidateProcessors()
        {
            _hashtagProcessor = new HashtagProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate);
            _locationProcessor = new LocationProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate);
            _collectionProcessor = new CollectionProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate);
            _mediaProcessor = new MediaProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _userProcessor = new UserProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _storyProcessor = new StoryProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _commentProcessor = new CommentProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _messagingProcessor = new MessagingProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _feedProcessor = new FeedProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);

            _liveProcessor = new LiveProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _discoverProcessor = new DiscoverProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _accountProcessor = new AccountProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _helperProcessor = new HelperProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _tvProcessor = new TVProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _businessProcessor = new BusinessProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _shoppingProcessor = new ShoppingProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate, this);
            _webProcessor = new WebProcessor(DeviceInfo, _user, RequestProcessor, _logger, _userAuthValidate);

        }

        private void ValidateUserAsync(InstaUserShortResponse user, string csrfToken, bool validateExtra = true, string password = null)
        {
            try
            {
                var converter = ConvertersFabric.Instance.GetUserShortConverter(user);
                _user.LoggedInUser = converter.Convert();
                if (password != null)
                    _user.Password = password;
                _user.UserName = _user.UserName;
                if (validateExtra)
                {
                    _user.RankToken = $"{_user.LoggedInUser.Pk}_{RequestProcessor.RequestMessage.PhoneId}";
                    _user.CsrfToken = csrfToken;
                    if (string.IsNullOrEmpty(_user.CsrfToken))
                    {
                        _user.CsrfToken = InstaApiHelper.GetCsrfToken(RequestProcessor);
                    }
                    IsUserAuthenticated = true;
                    InvalidateProcessors();
                }

            }
            catch { }
        }

        private void ValidateUser()
        {
            if (string.IsNullOrEmpty(_user.UserName) || string.IsNullOrEmpty(_user.Password))
                throw new ArgumentException("user name and password must be specified");
        }

        private void ValidateLoggedIn()
        {
            if (!IsUserAuthenticated)
                throw new ArgumentException("user must be authenticated");
        }

        private void ValidateRequestMessage()
        {
            if (RequestProcessor.RequestMessage == null || RequestProcessor.RequestMessage.IsEmpty())
                throw new ArgumentException("API request message null or empty");
        }

        private void LogException(Exception exception)
        {
            _logger?.LogException(exception);
        }

        #endregion
    }
}