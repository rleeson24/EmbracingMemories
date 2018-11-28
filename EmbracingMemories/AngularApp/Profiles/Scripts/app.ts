declare var angular;
angular.module('fileUpload', ['ngFileUpload']);
var profileModule = angular.module('profile', ['common', 'ui.bootstrap.datetimepicker', 'ui.bootstrap.tabs', 'localization'])
	.value('usStates', Geography.UsStatesList)
	.service('fileUpload', ['$http', 'authService', Application.FileUploadService])
	.directive('fileModel', ['$parse', 'fileUpload', Application.FileUploadDirective])
	.factory('profileService', ['$http', 'authService', 'blockUI', Profiles.QrProfileService])
	.factory('photoService', ['$http', '$q', 'authService', Photos.PhotosService])
	.factory('audioFileService', ['$http', '$q', 'authService', AudioFiles.AudioFilesService])
	.factory('videoService', ['$http', '$q', 'authService', Video.VideoService])
	.controller('ProfileListViewModel', ['$scope', '$window', 'profileService', Profiles.ProfileListViewModel])
	//.controller('ProfileRegistrationViewModel', Profiles.ProfileRegistrationViewModel)
	.controller('CreateProfileViewModel', ['$scope', '$http', '$window', '$routeParams', 'usStates', 'authService', Profiles.CreateProfileViewModel])
	.controller('ViewProfileViewModel', ['$scope', '$window', '$sce', 'data', Profiles.ViewProfileViewModel])
	.controller('EditProfileViewModel', ['$scope', '$window', '$routeParams', '$sce', 'profileService', 'fileUpload', 'photoService', 'audioFileService','videoService', Profiles.EditProfileViewModel])
	.config(['$httpProvider', '$routeProvider', '$locationProvider', function ($httpProvider, $routeProvider, $locationProvider) {
		$httpProvider.defaults.transformResponse.push(function (responseData) {
			Application.ConvertDateStringsToDates(responseData);
			return responseData;
		});
		$httpProvider.interceptors.push('authInterceptorService');
		$routeProvider.
			when('/Profiles/list', {
				templateUrl: '/ProfileViews/List',
				controller: 'ProfileListViewModel',
				caseInsensitiveMatch: true
			}).
			when('/Profiles/Create', {
				templateUrl: '/ProfileViews/Create',
				controller: 'CreateProfileViewModel',
				caseInsensitiveMatch: true
			}).
			when('/Profiles/:profileId', {
				templateUrl: '/ProfileViews/ReadOnlyView',
				controller: 'ViewProfileViewModel',
				resolve: {
					data: ['$route', 'profileService', function ($route, profileService) {
						return profileService.getProfile($route.current.params.profileId);
					}]
				}
			}).
			when('/Profiles/:profileId/edit', {
				templateUrl: '/ProfileViews/Edit',
				controller: 'EditProfileViewModel',
				caseInsensitiveMatch: true
			}).
			otherwise({ redirectTo: '/Profiles/list' });
		$locationProvider.html5Mode({
			enabled: true,
			requireBase: false
		});
	}]).factory('authInterceptorService', ['$q', '$location',
		'localStorageService', function ($q, $location, localStorageService) {

			var authInterceptorServiceFactory = {
				//request: null,
				responseError: null
			};

			//var _request = function (config) {

			//	config.headers = config.headers || {};

			//	var authData = localStorageService.get('authorizationData');
			//	if (authData) {
			//		config.headers.Authorization = 'Bearer ' + authData.token;
			//	}

			//	return config;
			//}

			var _responseError = function (rejection) {
				if (rejection.status === 401) {
					$location.assign('/account/login');
				}
				return $q.reject(rejection);
			}

			//authInterceptorServiceFactory.request = _request;
			authInterceptorServiceFactory.responseError = _responseError;

			return authInterceptorServiceFactory;
		}]);



//$rootScope.$on("$routeChangeError", function () {
//    console.log("failed to change routes");
//});