module Profiles {
    export class QrProfileService {
        private httpService: any;
        private _localStorageService: any;
        public getProfile: Function;
        public saveProfile: Function;
        public getProfiles: Function;

        constructor($http: any, authService: Application.AuthService) {
            var thisController = this;
            var accessToken = null;
            this.getProfile = function (id) {
                return $http.get('/api/QrProfiles/' + id, {
                    headers: authService.getHeaders()
                }).then(function (response) {
                    return response.data;
                },
                    function (httpError) {
                        // translate the error
                        throw httpError.status + " : " +
                        httpError.data;
                    });
            };

            this.saveProfile = function (id: Number, model: QrProfileModel) {
                return $http.put('/api/QrProfiles/' + id, model, {
                    headers: authService.getHeaders()
                }).then(function (response) {
                    return response.data;
                },
                    function (httpError) {
                        // translate the error
                        throw httpError.status + " : " +
                        httpError.data;
                    });
            };

            this.getProfiles = function () {
                return $http.get('/api/QrProfiles', {
                    headers: authService.getHeaders()
                }).then(function (response) {
                    return response.data;
                },
                    function (httpError) {
                        // translate the error
                        throw httpError.status + " : " +
                        httpError.data;
                    });
            };
            return this;
        }
    }
}