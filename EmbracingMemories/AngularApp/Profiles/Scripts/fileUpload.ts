declare var ng;

module Application {
    export class FileUploadService {
        public uploadFileToUrl: Function;
        constructor($http: any, authService: Application.AuthService) {
            this.uploadFileToUrl = function (file, uploadUrl) {
                var fd = new FormData();
                fd.append('file', file);
                var headers = authService.getHeaders();
                headers['Content-Type'] = undefined;
                return $http.post(uploadUrl, fd, {
                    transformRequest: angular.identity,
                    headers: headers
                })
                .then(function (response) {
                    return response.data;
                },
                function (httpError) {
                    // translate the error
                    throw httpError.status + " : " +
                    httpError.data;
                });
            }
        }
    }

    export class FileUploadDirective {
        public restrict: String;
        public link: Function;
        constructor($parse: any) {
            var directive = {
                restrict: "A",
                link: function (scope, element, attrs) {
                    var model = $parse(attrs.fileModel);
                    var modelSetter = model.assign;

                    element.bind('change', function () {
                        scope.$apply(function () {
                            modelSetter(scope, element[0].files);
                        });
                    });
                }
            };
            return directive;
        }
    }
}