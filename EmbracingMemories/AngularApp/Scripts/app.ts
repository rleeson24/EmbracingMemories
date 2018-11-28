/// <reference path='angular.d.ts' /> 
/// <reference path='models.ts' />


module Application {

	export interface AccountOperationsScope {
		EmailAddress: string;
		Password: string;
		rememberMe: boolean;
		RequestInProgress: boolean;
		IsSuccess: boolean;
		Errors: Object[];

		externalLogins: Models.ExternalLogin[];
		loginInternal: Function;
		loginToExternalProvider: Function;
		Logout: Function;
	}


	export class AccountOperationsController {
		private httpService: any;
		private _window: any;
		private _localStorageService: any;

		constructor($scope: AccountOperationsScope, $http: any, $window: any, authService: AuthService, localStorageService: any) {
			this.httpService = $http;
			this._window = $window;
			this._localStorageService = localStorageService;
			var thisController = this;
			var accessToken = null;

			$scope.Errors = [];
			$scope.RequestInProgress = false;
			$scope.IsSuccess = false;
			$scope.externalLogins = [];
			$scope.EmailAddress = '';
			$scope.Password = '';
			$scope.rememberMe = false;
			this.httpService.get('/api/Account/ExternalLogins?returnUrl=/&generateState=true').success(function (data) {
				$scope.externalLogins = data;
			});
			$scope.loginInternal = function () {
				$scope.RequestInProgress = true;
				$scope.Errors = [];
				authService.loginInternal({
					userName: $scope.EmailAddress, password: $scope.Password, RememberMe: $scope.rememberMe
				}).then(function (data) {
					var returnUrl = thisController._localStorageService.get('returnUrl');
					if (returnUrl) {
						thisController._localStorageService.remove('returnUrl');
						thisController._window.location.assign(returnUrl);
					}
					else {
						thisController._window.location.assign("/");
					}
				}
					, function (err, status) {
						$scope.RequestInProgress = false;
						if (err.ModelState) {
							for (var errorKey in err.ModelState) {
								var errorsArray = err.ModelState[errorKey];
								for (var i = 0; i < errorsArray.length; i++) {
									$scope.Errors.push(errorsArray[i]);
								}
							}
						}
						else if (err.error_description) {
							$scope.Errors.push(err.error_description);
						}
						else if (err.error) {
							$scope.Errors.push(err.error);
						}
					});
			};
			$scope.loginToExternalProvider = function (url) {
				thisController.httpService.get(url).success(function (data) {
					$scope.IsSuccess = true;
				});
			};
			$scope.Logout = function () {
				authService.LogOut();
			};
		}
	}

	export interface AuthService {
		beginResetPassword: Function;
		saveRegistration: Function;
		setPassword: Function;
		loginInternal: Function;
		LogOut: Function;
		FillAuthData: Function;
		Authentication: Object;
		getHeaders: Function;
		ResendEmailConfirmationToken: Function;
	}

	export class AuthenticationService implements AuthService {
		private httpService: any;
		private thisController: AuthenticationService;
		private serviceBase = '/';
		private _localStorageService = null;
		private _q: any;
		private _location: any;
		private _window: any;

		constructor($http: any, $q: any, $location: any, $window: any, localStorageService: any) {
			this.thisController = this;
			this._localStorageService = localStorageService;
			this.httpService = $http;
			this._q = $q;
			this._location = $location;
			this._window = $window;
			return this;
		}

		Authentication = {
			userName: ""
		};

		saveRegistration = function (registration) {
			var deferred = this._q.defer();

			this.httpService.post(this.serviceBase + 'api/account/register', registration, {
				headers: this.getHeaders()
				}).success(function (response) {
					deferred.resolve(response);

				}).error(function (err, status) {
					//this.thisController.LogOut();
					deferred.reject(err);
				});

			return deferred.promise;
		};

		ResendEmailConfirmationToken = (url) => {
			var deferred = this._q.defer();

			this.httpService.post(url)
				.success(function (response) {
					deferred.resolve(response);
				}).error(function (err, status) {
					deferred.reject(err);
				});

			return deferred.promise;
		};

		loginInternal = (loginData) => {
			var data = "grant_type=password&username=" +
				loginData.userName + "&password=" + loginData.password;
			//var data = { Email: loginData.userName, Password: loginData.password, RememberMe: loginData.RememberMe };
			var deferred = this._q.defer();

			this.httpService.post(this.serviceBase + 'token', data, {
				headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
			}).success((response) => {
				this._window.localStorage.setItem('authorizationData',
					JSON.stringify({ token: response.access_token, userName: loginData.userName }));
				
				this.thisController.Authentication.userName = loginData.userName;

				deferred.resolve(response);

			}).error((err, status) => {
				//this.thisController.LogOut();
				deferred.reject(err);
			});

			return deferred.promise;
		};

		LogOut = () => {
			this.httpService.post(this.serviceBase + 'api/Account/Logout', null, {
				headers: this.getHeaders()
			}).success((response) => {
				this._window.localStorage.removeItem('authorizationData');

				this.thisController.Authentication.userName = "";
				this._window.location.assign('/');
			});
		};

		beginResetPassword = (registration) => {
			var deferred = this._q.defer();

			this.httpService.post(this.serviceBase + 'api/account/GenerateResetToken', { EmailAddress: registration.emailAddress })
				.success(function (response) {
					deferred.resolve(response);

				}).error(function (err, status, a, b, c, d) {
					//this.thisController.LogOut();
					deferred.reject(err);
				});

			return deferred.promise;
		};

		setPassword = (registration) => {
			var deferred = this._q.defer();

			this.httpService.post(this.serviceBase + 'api/Account/SetPassword', registration)
				.success(function (response) {
					deferred.resolve(response);

				}).error(function (err, status, a, b, c, d) {
					//this.thisController.LogOut();
					deferred.reject(err);
				});

			return deferred.promise;
		};

		FillAuthData = () => {
			var authData = JSON.parse(this._window.localStorage.getItem('authorizationData'));
			if (authData) {
				this.Authentication.userName = authData.userName;
			}
		}
		// we have to include the Bearer token with each call to the Web API controllers. 
		getHeaders = () => {
			this.FillAuthData();
			if (this._window.localStorage.getItem('authorizationData')) {
				return {
					Authorization: "Bearer " + JSON.parse(this._window.localStorage.getItem('authorizationData')).token
				};
			}
			return null;
		}
	}

	export class LocalizationService// implements ILocalizationService 
	{
		constructor($http: any, $rootScope: any, $window: any, $filter: any) {
			var localize = {
				// use the $window service to get the language of the user's browser
				language: '',
				// array to hold the localized resource string entries
				dictionary: [],
				// location of the resource file
				url: undefined,
				// flag to indicate if the service hs loaded the resource file
				resourceFileLoaded: false,

				// success handler for all server communication
				successCallback: function (data) {
					// store the returned array in the dictionary
					localize.dictionary = data;
					// set the flag that the resource are loaded
					localize.resourceFileLoaded = true;
					// broadcast that the file has been loaded
					$rootScope.$broadcast('localizeResourcesUpdated');
				},

				// allows setting of language on the fly
				setLanguage: function (value) {
					localize.language = value;
					localize.initLocalizedResources();
				},

				// allows setting of resource url on the fly
				setUrl: function (value) {
					localize.url = value;
					localize.initLocalizedResources();
				},

				// builds the url for locating the resource file
				buildUrl: function () {
					if (!localize.language) {
						var lang, androidLang;
						// works for earlier version of Android (2.3.x)
						if ($window.navigator && $window.navigator.userAgent && (androidLang = $window.navigator.userAgent.match(/android.*\W(\w\w)-(\w\w)\W/i))) {
							lang = androidLang[1];
						} else {
							// works for iOS, Android 4.x and other devices
							lang = $window.navigator.userLanguage || $window.navigator.language;
						}
						// set language
						localize.language = lang;
					}
					if (localize.language.indexOf('es-') == 0) {
						localize.language = 'es';
					}
					if (localize.language.indexOf('en-') == 0) {
						localize.language = 'en';
					}
					//localize.language = 'en';
					return '/i18n/resources-locale_' + localize.language + '.js';
				},

				// loads the language resource file from the server
				initLocalizedResources: function () {
					// build the url to retrieve the localized resource file
					var url = localize.url || localize.buildUrl();
					// request the resource file
					$http({ method: "GET", url: url, cache: false }).success(localize.successCallback).error(function () {
						// the request failed set the url to the default resource file
						var url = '/i18n/resources-locale_default.js';
						// request the default resource file
						$http({ method: "GET", url: url, cache: false }).success(localize.successCallback);
					});
				},

				// checks the dictionary for a localized resource string
				getLocalizedString: function (value) {
					// default the result to an empty string
					var result = '';

					// make sure the dictionary has valid data
					if ((localize.dictionary !== []) && (localize.dictionary.length > 0)) {
						// use the filter service to only return those entries which match the value
						// and only take the first result
						var entry = $filter('filter')(localize.dictionary, function (element) {
							return element.key === value;
						}
						)[0];

						try {
							// set the result
							result = entry.value;
						}
						catch (ex) {
							throw 'Unable to retrieve localized value for ' + value;
						}
					}
					// return the value to the call
					return result;
				}
			};

			// force the load of the resource file
			localize.initLocalizedResources();

			// return the local instance when called
			return localize;
		}
	}


	export class ContactUsModel {
		EmailAddress: string;
		Subject: string;
		Message: string;
		EnquiryType: String;
		Name: String;
		Phone: String;
	}

	export interface ContactUsScope {
		model: ContactUsModel;
		RequestInProgress: boolean;
		IsSuccess: boolean;
		Errors: Object[];
		Submit: Function;
	}


	export class ContactUsController {
		private httpService: any;
		private _window: any;
		private _localStorageService: any;

		constructor($scope: ContactUsScope, $http: any, $window: any) {
			this.httpService = $http;
			this._window = $window;
			var thisController = this;

			$scope.Errors = [];
			$scope.RequestInProgress = false;
			$scope.IsSuccess = false;
			$scope.model = new Application.ContactUsModel();
			$scope.model.Name = '';
			$scope.model.Phone = '';
			$scope.model.EnquiryType = '';
			$scope.model.Message = '';
			$scope.model.EmailAddress = '';
			$scope.model.Subject = '';
			$scope.Submit = function () {
				$scope.Errors = [];
				$http.post('/api/ContactUs/Contact/', $scope.model, {}).then(function (response) {
					$scope.IsSuccess = true;
				},
					function (err, status) {
						$scope.RequestInProgress = false;
						if (err.ModelState) {
							for (var errorKey in err.ModelState) {
								var errorsArray = err.ModelState[errorKey];
								for (var i = 0; i < errorsArray.length; i++) {
									$scope.Errors.push(errorsArray[i]);
								}
							}
						}
						if (err.data && err.data.ModelState) {
							err = err.data;
							for (var errorKey in err.ModelState) {
								var errorsArray = err.ModelState[errorKey];
								for (var i = 0; i < errorsArray.length; i++) {
									$scope.Errors.push(errorsArray[i]);
								}
							}
						}
						else if (err.error_description) {
							$scope.Errors.push(err.error_description);
						}
						else if (err.error) {
							$scope.Errors.push(err.error);
						}
					});
			};
		}
	}
	var regexIso8601 = /^\d{4}(-\d\d(-\d\d(T\d\d:\d\d(:\d\d)?(\.\d+)?(([+-]\d\d:\d\d)|Z)?)?)?)?$/;
	var regexIso8601Date = /^(\d{4}|\+\d{6})(?:-(\d{2})(?:-(\d{2})(?:T00:00:00)?)?)?$/;

	export function ConvertDateStringsToDates(input) {
		// Ignore things that aren't objects.
		if (typeof input !== "object") return input;

		for (var key in input) {
			if (!input.hasOwnProperty(key)) continue;

			var value = input[key];
			var match;
			// Check for string properties which look like dates.
			if (typeof value === "string" && (match = value.match(regexIso8601Date))) {
				input[key] = new Date(value.substring(0, 8));
			} else if (typeof value === "string" && (match = value.match(regexIso8601))) {
				var milliseconds = Date.parse(match[0])
				if (!isNaN(milliseconds)) {
					input[key] = new Date(milliseconds);
				}
			} else if (typeof value === "object") {
				// Recurse into object
				ConvertDateStringsToDates(value);
			}
		}
	}

	export class PhoneFormatter {
		constructor() {
			return (function () {
				return {
					require: 'ngModel',
					link: function (scope, element, attrs, controller) {
						controller.$formatters.unshift(function (tel) {
							if (!tel) { return ''; }

							var value = tel.toString().trim().replace(/^\+/, '');

							if (value.match(/[^0-9]\-/)) {
								return tel;
							}

							var country, city, number;

							switch (value.length) {
								case 10: // +1ppp####### -> c (ppp) ###-####
									country = 1;
									city = value.slice(0, 3);
									number = value.slice(3);
									break;

								case 11: // +cppp####### -> ccc (pp) ###-####
									country = value[0];
									city = value.slice(1, 4);
									number = value.slice(4);
									break;

								case 12: // +ccppp####### -> cc ppp ###-####
									country = value.slice(0, 2);
									city = value.slice(2, 5);
									number = value.slice(5);
									break;

								default:
									return tel;
							}

							if (country == 1) {
								country = "";
							}

							number = number.slice(0, 3) + '-' + number.slice(3);

							return (country + " (" + city + ") " + number).trim();
						});
						controller.$parsers.unshift(function (tel) {
							if (!tel) { return ''; }

							var value = tel.toString().trim().replace(/^\+/, '').replace(/[^0-9]/g, '');

							if (value.match(/[^0-9]\-/)) {
								return tel;
							}

							var country, city, number;

							switch (value.length) {
								case 10: // +1ppp####### -> c (ppp) ###-####
									country = 1;
									city = value.slice(0, 3);
									number = value.slice(3);
									break;

								case 11: // +cppp####### -> ccc (pp) ###-####
									country = value[0];
									city = value.slice(1, 4);
									number = value.slice(4);
									break;

								case 12: // +ccppp####### -> cc ppp ###-####
									country = value.slice(0, 2);
									city = value.slice(2, 5);
									number = value.slice(5);
									break;

								default:
									return tel;
							}

							if (country == 1) {
								country = "";
							}

							number = number.slice(0, 3) + '-' + number.slice(3);

							element.val((country + " (" + city + ") " + number).trim());

							return tel;
						});
					}
				}
			})();
		}
	}

	export function ProcessErrorResponse(errorResponse: any, errorList: Object[]) {
		if (errorResponse.ModelState) {
			for (var errorKey in errorResponse.ModelState) {
				var errorsArray = errorResponse.ModelState[errorKey];
				for (var i = 0; i < errorsArray.length; i++) {
					errorList.push(errorsArray[i]);
				}
			}
		}
		if (errorResponse.data && errorResponse.data.ModelState) {
			errorResponse = errorResponse.data;
			for (var errorKey in errorResponse.ModelState) {
				var errorsArray = errorResponse.ModelState[errorKey];
				for (var i = 0; i < errorsArray.length; i++) {
					errorList.push(errorsArray[i]);
				}
			}
		}
		if (errorResponse.data && errorResponse.data.Message) {
			errorList.push(errorResponse.data.Message);
		}
		else if (errorResponse.error_description) {
			errorList.push(errorResponse.error_description);
		}
		else if (errorResponse.error) {
			errorList.push(errorResponse.error);
		}
		else if (errorResponse.Message) {
			errorList.push(errorResponse.Message);
		}
	}
}
