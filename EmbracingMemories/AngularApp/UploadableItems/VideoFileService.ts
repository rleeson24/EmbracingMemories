module Video {
    export class VideoService {
        private httpService: any;
        private _localStorageService: any;
        public getVideos: Function;
        public deleteVideo: Function;
        //public savePhoto: Function;
        
        constructor($http: any, $q: any, authService: Application.AuthService) {
            var thisController = this;
            var accessToken = null;

            this.getVideos = function (profileId) {
                var deferred = $q.defer();

                return $http.get('/api/Video/GetVideos/' + profileId, {
                    headers: authService.getHeaders()
                     }).success(function (response) {
                        deferred.resolve(response);

                    }).error(function (err, status) {
                        deferred.reject(err);
                    });

                //return deferred.promise;
            };

            this.deleteVideo = function (id: Number) {
                var deferred = $q.defer();

                return $http.delete('/api/Video/Delete/' + id, {
                    headers: authService.getHeaders()
                }).success(function (response) {
                    deferred.resolve(response);
                }).error(function (err, status) {
                    deferred.reject(err);
                });

                //return deferred.promise;
            };
            return this;
        }
    }
}