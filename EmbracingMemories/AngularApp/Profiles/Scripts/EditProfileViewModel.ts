declare var scope;

module Profiles {
	export class EditProfileViewModel {
		private _window: any;
		constructor($scope: IEditProfileScope, $window: any, $routeParams: any, $sce: any, profileService: QrProfileService, fileUpload: Application.FileUploadService, photoService: Photos.PhotosService, audioFileService: AudioFiles.AudioFilesService, videoService: Video.VideoService) {
			var thisController = this;
			var accessToken = null;

			$scope.model = new QrProfileModel();

			$scope.ShowPageMessage = false;
			$scope.model.Links = [];
			$scope.model.FirstName = '';
			$scope.model.LastName = '';
			$scope.model.Obituary = '';
			$scope.model.LifeHistory = '';
			$scope.model.Sex = 'M';
			$scope.showSaveComplete = false;
			profileService.getProfile($routeParams.profileId)
				.then(function (response) {
					$scope.model = response;
					for (var i in $scope.model.AudioFiles) {
						$scope.model.AudioFiles[i].Url = '/api/Audio/Content/' + $scope.model.AudioFiles[i].Id;
					}
					for (var i in $scope.model.Videos) {
						$scope.model.Videos[i].Url = $sce.trustAsResourceUrl('https://player.vimeo.com' + $scope.model.Videos[i].Url);
					}
				},
				function () {
					$scope.ShowPageMessage = true;
					$scope.Message = "An error has occurred retrieving your profile";
				});
			//data.Birthday = new Date(data.Birthday);
			//data.DateOfDeath = new Date(data.DateOfDeath);
			

			var uploadPhoto = function(fileData) {
				var uploadUrl = "/api/Photos/Upload/" + $scope.model.Id;
				angular.forEach(fileData, function (file, key) {
					console.log('file is ');
					console.dir(file);
					fileUpload.uploadFileToUrl(file, uploadUrl)
						.then(function () {
							photoService.getPhotos($scope.model.Id)
								.then(function (response) {
									$scope.model.Photos = response.data;
								}, function (err) {
								});
						},
						function () {
						});
				});
			};

			$scope.$watch(function (scope) {
				return scope.model.newPhoto;
			},
			function (newValue, oldValue) {
				uploadPhoto(newValue);
			});

			var uploadAudioFile = function (fileData) {
				var uploadUrl = "/api/Audio/Upload/" + $scope.model.Id;
				angular.forEach(fileData, function (file, key) {
					console.log('file is ');
					console.dir(file);
					fileUpload.uploadFileToUrl(file, uploadUrl)
						.then(function (data) {
							angular.forEach(data, function (item) {
								item.Url = '/api/Audio/Content/' + item.Id;
								$scope.model.AudioFiles.push(item);
							});
						},
						function () {
						});
				});
			};

			$scope.$watch(function (scope) {
				return scope.model.audioFile;
			},
			function (newValue, oldValue) {
				uploadAudioFile(newValue);
			});

			var uploadVideoFile = function (fileData) {
				var uploadUrl = "/api/Video/Upload/" + $scope.model.Id;
				angular.forEach(fileData, function (file, key) {
					console.log('file is ');
					console.dir(file);
					fileUpload.uploadFileToUrl(file, uploadUrl)
						.then(function (data) {
							angular.forEach(data, function (item) {
								item.Url = $sce.trustAsResourceUrl('https://player.vimeo.com' + item.Url );
								$scope.model.Videos.push(item);
							});
						},
						function () {
						});
				});
			};

			$scope.$watch(function (scope) {
				return scope.model.videoFile;
			},
			function (newValue, oldValue) {
				uploadVideoFile(newValue);
			});

			$scope.saveProfile = function () {
				$scope.showSaveComplete = false;
				$scope.RequestInProgress = true;
				profileService.saveProfile($routeParams.profileId, $scope.model)
					.then(function (data) {
						$scope.RequestInProgress = false;
						$scope.showSaveComplete = true;
						$scope.model = data;
						setTimeout(function () {
							$scope.showSaveComplete = false;
						}, 3000);
					},
					function () {
						$scope.ShowPageMessage = true;
						$scope.Message = "An error has occurred saving your profile";
						$scope.RequestInProgress = false;
					});
			};


			$scope.addLink = function () {
				$scope.model.Links.push({ label: '', url: '' });
			};


			$scope.deleteLink = function (element: ILink) {
				$scope.model.Links.splice($scope.model.Links.indexOf(element),1);
			};

			$scope.deletePhoto = function (photoId: String) {
				photoService.deletePhoto(photoId).
					then(function () {
						//remove the photo from the view model
						var index = -1;
						for (var i = 0; i < $scope.model.Photos.length; i++) {
							if ($scope.model.Photos[i].Id == photoId) {
								index = i;
								break;
							}
						}
						$scope.model.Photos.splice(index, 1);
					}, function () {
					});
			};

			$scope.deleteAudioFile = function (audioFileId: String) {
				audioFileService.deleteAudioFile(audioFileId).
					then(function () {
						//remove the audio file from the view model
						var index = -1;
						for (var i = 0; i < $scope.model.AudioFiles.length; i++) {
							if ($scope.model.AudioFiles[i].Id == audioFileId) {
								index = i;
								break;
							}
						}
						$scope.model.AudioFiles.splice(index, 1);
					}, function () {
					});
			};

			$scope.deleteVideo = function (videoId: String) {
				videoService.deleteVideo(videoId).
					then(function () {
						//remove the video from the view model
						var index = -1;
						for (var i = 0; i < $scope.model.Videos.length; i++) {
							if ($scope.model.Videos[i].Id == videoId) {
								index = i;
								break;
							}
						}
						$scope.model.Videos.splice(index, 1);
					}, function () {
					});
			};
		}
	}
}