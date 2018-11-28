module Profiles {
    export class ViewProfileViewModel {
        private _window: any;

        constructor($scope: IViewProfileScope, $window: any, $sce: any, data: QrProfileModel) {
            var thisController = this;
            var accessToken = null;
            //data.Birthday = new Date(parseInt(data.Birthday.substr(6)));
            //data.DateOfDeath = new Date(parseInt(data.DateOfDeath.substr(6)));
            $scope.model = data;
            for (var iFile in $scope.model.AudioFiles) {
                $scope.model.AudioFiles[iFile].Url = '/api/Audio/Content/' + $scope.model.AudioFiles[iFile].Id;
            }
            for (var iFile in $scope.model.Videos) {
                $scope.model.Videos[iFile].Url = $sce.trustAsResourceUrl('https://player.vimeo.com' + $scope.model.Videos[iFile].Url );
            }
            $scope.images = [];
            for (var i = 0; i < data.Photos.length; i++ )
            {
                $scope.images.push({ src: '/api/Photos/Content/' + data.Photos[i].Id, alt: '' });
            }
        }
    }
}