module AudioFiles {
    export class AudioFilesService {
        private httpService: any;
        private _localStorageService: any;
        public getAudioFiles: Function;
        public deleteAudioFile: Function;
        //public savePhoto: Function;
        
        constructor($http: any, $q: any, authService: Application.AuthService) {
            var thisController = this;
            var accessToken = null;

            this.getAudioFiles = function (profileId) {
                var deferred = $q.defer();

                return $http.get('/api/Audio/GetAudio/' + profileId, {
                    headers: authService.getHeaders()
                     }).success(function (response) {
                        deferred.resolve(response);

                    }).error(function (err, status) {
                        deferred.reject(err);
                    });

                //return deferred.promise;
            };

            this.deleteAudioFile = function (id: Number) {
                var deferred = $q.defer();

                return $http.delete('/api/Audio/Delete/' + id, {
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