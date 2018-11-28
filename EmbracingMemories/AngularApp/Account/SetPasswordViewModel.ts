module Account {
    export class SetPasswordModel {
        Code: String;
        EmailAddress: String;
        Password: String;
        PasswordConfirmation: String;
        Errors: Object[];


        // Constructor 
        // (accepts a value so you can initialize engine)
        constructor() {
            this.Code = '';
            this.EmailAddress = '';
            this.Password = '';
            this.PasswordConfirmation = '';
        }
    }

    export interface IChangePasswordScope {
        RequestInProgress: boolean;
        IsSuccess: boolean;
        model: SetPasswordModel;
        beginResetPasswordProcess: Function;
        completePasswordResetProcess: Function;
        setPassword: Function;
        Errors: Object[];
    }

    export class ChangePasswordViewModel {
        constructor($scope: IChangePasswordScope, authService: Application.AuthService, keys: any) {
            $scope.model = new SetPasswordModel();
            $scope.beginResetPasswordProcess = function () {
                $scope.RequestInProgress = true;
                $scope.Errors = [];
                authService.beginResetPassword({ emailAddress: $scope.model.EmailAddress }).then(function () {
                    $scope.RequestInProgress = false;
                    $scope.IsSuccess = true;
                    $scope.Errors = [];
                }, function (err, status) {
                    $scope.RequestInProgress = false;
                    for (var errorKey in err.ModelState) {
                        var errorsArray = err.ModelState[errorKey];
                        for (var i = 0; i < errorsArray.length; i++) {
                            $scope.Errors.push(errorsArray[i]);
                        }
                    }
                });
            };
            $scope.completePasswordResetProcess = function () {
                $scope.RequestInProgress = true;
                $scope.Errors = [];
                authService.setPassword({ EmailAddress: keys.EmailAddress, Code: keys.Code, NewPassword: $scope.model.Password, ConfirmPassword: $scope.model.PasswordConfirmation }).then(function () {
                    $scope.RequestInProgress = false;
                    $scope.IsSuccess = true;
                    $scope.Errors = [];
                }, function (err, status) {
                    $scope.RequestInProgress = false;
                    for (var errorKey in err.ModelState) {
                        var errorsArray = err.ModelState[errorKey];
                        for (var i = 0; i < errorsArray.length; i++) {
                            $scope.Errors.push(errorsArray[i]);
                        }
                    }
                });
            };
        }
    }
}