module BindQrCode {
	export class BindQrCodeViewModel {
		private httpService: any;
		private _window: any;
		private _localStorageService: any;
		private _paymentForm: any;

		constructor($scope: IBindQrCodeScope, $http: any, $window: any, data: QrProfileModel[], QrGuid: String, authService: Application.AuthService) {
			this.httpService = $http;
			var thisController = this;
			var accessToken = null;
			$scope.Errors = [];
			$scope.ShowPageMessage = false;
			$scope.Message = '';

			$scope.model = data;

			$scope.BindToCode = function(profile: QrProfileModel) {
				$scope.Errors = [];
				var model = {
					ProfileId: profile.Id,
					QrCode: QrGuid
				};
				thisController.httpService.post('/api/QrProfiles/BindQrCode', model, {
					headers: authService.getHeaders()
				}).then(function (response) {
					$scope.showSaveComplete = true;
					$scope.disableButtons = true;
					$scope.RequestInProgress = false;

				},
					function (err, status) {
						$scope.ShowPageMessage = true;
						$scope.Message = "An error has occurred binding the QR plaque to the profile";
						$scope.RequestInProgress = false;
						Application.ProcessErrorResponse(err, $scope.Errors);
					});
			};
		}
	}

	export class QrProfileModel {
		FirstName: String;
		MiddleName: String;
		LastName: String;
		Id: String;
	}

	export interface IBindQrCodeScope {
		BindToCode: Function;
		QrCode: String;
		model: QrProfileModel[];
		ShowPageMessage: Boolean;
		Message: String;
		RequestInProgress: Boolean;
		Errors: Object[];
		showSaveComplete: Boolean;
		disableButtons: Boolean;
	}
}