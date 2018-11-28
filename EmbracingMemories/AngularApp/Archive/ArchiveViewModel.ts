declare var angular;
declare var SqPaymentForm;
declare var Stripe;

module Archive {
	export class ArchiveViewModel {
		private httpService: any;
		private _window: any;
		private _localStorageService: any;
		private _paymentForm: any;

		constructor($scope: IArchiveScope, $http: any, $window: any, authService: Application.AuthService, $uibModal: any, $sce: any, fileUpload: Application.FileUploadService, videoService: Video.VideoService ) {
			this.httpService = $http;
			var thisController = this;
			var accessToken = null;
			$scope.Errors = [];
			$scope.ShowPageMessage = false;
			$scope.Message = '';

			$scope.ListModel = new ListModel();
			$scope.ListModel.SelectedEntry = null;
			$scope.ListModel.NewEntry = new ArchiveModel();
			$scope.ListModel.ArchiveEntries = new Array<ArchiveModel>();
			$scope.saveNew = CreateArchiveEntry;

			
			$scope.PaymentModel = new PaymentModel();
			$scope.PaymentModel.PaymentError = '';

			$http.get('/api/Archive/', {
				headers: authService.getHeaders()
			}).then(function (response) {
				$scope.ListModel.ArchiveEntries = response.data;
				angular.forEach($scope.ListModel.ArchiveEntries, function (item) {
					setupFileUploadWatch(item);
					if (item.VideoUrl > '') {
						item.VideoUrl = $sce.trustAsResourceUrl('https://player.vimeo.com' + item.VideoUrl);
					}
				});
			},
			function (httpError) {
				$scope.ShowPageMessage = true;
				$scope.Message = "An error has occurred retrieving your archive entries";
			});

			$scope.submitPaymentData = function (e) {
				$scope.PaymentModel.CardToken = '';
				Stripe.setPublishableKey($window._stripePublicKey);
				var formElement = angular.element(e.target);
				
				// Request a token from Stripe:
				Stripe.card.createToken(formElement, function (status, response) {
					if (response.error) { // Problem!

						// Show the errors on the form:
						$scope.$apply(function () {
							$scope.PaymentModel.PaymentError = response.error.message;
							$scope.ShowPageMessage = true;
							$scope.Message = "A payment error has occurred";
						});

					} else { // Token was created!

						// Get the token ID:
						$scope.PaymentModel.CardToken = response.id;
						PayForArchiveFeature();
					}
				});

				// Prevent the form from being submitted:
				return false;
			};

			function PayForArchiveFeature() {
				$scope.Errors = [];
				thisController.httpService.post('/api/Archive/PayForArchiveFeature', $scope.PaymentModel, {
					headers: authService.getHeaders()
				}).then(function (response) {
					var modalInstance = $uibModal.open({
						animation: true,
						ariaLabelledBy: 'modal-title',
						ariaDescribedBy: 'modal-body',
						templateUrl: 'loggingOffDialog.html',
						controller: 'ArchiveViewModel',
						controllerAs: 'ArchiveViewModel',
						size: 'sm'
					});
				},
					function (err, status) {
						$scope.ShowPageMessage = true;
						$scope.Message = "An error has occurred paying for the Archive feature";
						$scope.RequestInProgress = false;
						Application.ProcessErrorResponse(err, $scope.Errors);
					});
			};

			function CreateArchiveEntry() {
				$scope.Errors = [];
				thisController.httpService.post('/api/Archive/', $scope.ListModel.NewEntry, {
					headers: authService.getHeaders()
				}).then(function (response) {
					$scope.ListModel.ArchiveEntries.push(response.data);
					setupFileUploadWatch(response.data);
				},
				function (err, status) {
					$scope.ShowPageMessage = true;
					$scope.Message = "An error has occurred saving your profile";
					$scope.RequestInProgress = false;
					Application.ProcessErrorResponse(err, $scope.Errors);
				});
			};

			$scope.logoffDialogOk = function () {
				authService.LogOut();
			};

			$scope.setSelectedEntry = function (entryToSelect: any) {
				$scope.ListModel.SelectedEntry = entryToSelect;
			};

			var uploadVideoFile = function (fileData) {
				var uploadUrl = "/api/Archive/UploadVideo/" + $scope.ListModel.SelectedEntry.Id;
				angular.forEach(fileData, function (file, key) {
					console.log('file is ');
					console.dir(file);
					fileUpload.uploadFileToUrl(file, uploadUrl)
						.then(function (data) {
							data.VideoUrl = $sce.trustAsResourceUrl('https://player.vimeo.com' + data.VideoUrl);
							$scope.ListModel.SelectedEntry.VideoUrl = data.VideoUrl;
						},
						function () {
						});
				});
			};

			function setupFileUploadWatch(listItem) {
				$scope.$watch(function (scope) {
					return listItem.videoFile;
				},
				function (newValue, oldValue) {
					uploadVideoFile(newValue);
				});
			}
		}
	}
}

var archiveModule = angular.module('archive', ['common', 'localization'])
	.service('fileUpload', ['$http', 'authService', Application.FileUploadService])
	.directive('fileModel', ['$parse', 'fileUpload', Application.FileUploadDirective])
	.factory('videoService', ['$http', '$q', 'authService', Video.VideoService])
	.controller('ArchiveViewModel', ['$scope', '$http', '$window', 'authService', '$uibModal', '$sce', 'fileUpload', 'videoService', Archive.ArchiveViewModel]);
