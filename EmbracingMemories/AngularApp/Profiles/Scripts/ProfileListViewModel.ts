module Profiles {
	export class ProfileListViewModel {
		private _window: any;

		constructor($scope: IProfileListScope, $window: any, profileService: QrProfileService) {
			var thisController = this;

			$scope.Busy = true;
			//$scope.$on('$viewContentLoaded', function () {
			//});
			profileService.getProfiles()
				.then(function (response) {
					$scope.Profiles = response;
					$scope.Busy = false;
				},
				function () {
					$scope.Busy = false;
					$scope.ShowPageMessage = true;
					$scope.Message = "An error has occurred retrieving your profiles";
				});
			
			$scope.createNew = function () {
				$window.location.assign('/Profiles/create');
			};
			//profileService.getProfiles()
			//    .then(function (profiles) {
			//        $scope.Profiles = profiles;
			//    });
		}
	}
}