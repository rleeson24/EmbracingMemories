module Account {
    export class ConfirmEmailModel {
        Code: String;
        EmailAddress: String;
        Password: String;
        PasswordConfirmation: String;


        // Constructor 
        // (accepts a value so you can initialize engine)
        constructor() {
            this.Code = '';
            this.EmailAddress = '';
            this.Password = '';
            this.PasswordConfirmation = '';
        }
    }

    export interface IConfirmEmailScope {
        RequestInProgress: boolean;
        IsSuccess: boolean;
        setPassword: Function;
        model: ConfirmEmailModel;
        ResendEmailConfirmationToken: Function;
        Errors: Object[];
    }

    export class ConfirmEmailViewModel {
        constructor($scope: IConfirmEmailScope, authService: Application.AuthService, keys: ConfirmEmailModel, ResendEmailConfirmationTokenLink: String) {
            $scope.Errors = [];

            $scope.setPassword = function () {
                $scope.RequestInProgress = true;
                authService.setPassword({ Code: keys.Code, EmailAddress: keys.EmailAddress, NewPassword: $scope.model.Password, ConfirmPassword: $scope.model.PasswordConfirmation })
                    .then(function (data) {
                        $scope.RequestInProgress = false;
                        $scope.IsSuccess = true;
                    },
                    function (err, status) {
                        $scope.RequestInProgress = false;
                        for (var errorKey in err.ModelState) {
                            var errorsArray = err.ModelState[errorKey];
                            for (var i = 0; i < errorsArray.length; i++) {
                                $scope.Errors.push(errorsArray[i]);
                            }
                        }
                    });
            };

            $scope.ResendEmailConfirmationToken = function () {
                $scope.RequestInProgress = true;
                authService.ResendEmailConfirmationToken(ResendEmailConfirmationTokenLink)
                    .then(function (data) {
                    },
                    function (err, status) {
                    });
            };
        };
    }
}