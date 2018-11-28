module Account {
    export interface RegistrationScope {
        RoleCode: string;
        EmailAddress: string;
        Password: string;
        PasswordConfirmation: string;
        FirstName: string;
        LastName: string;
        Company: string;
        AddressLine1: string;
        AddressLine2: string;
        City: string;
        State: string;
        PostalCode: string;
        Country: string;
        Phone: string;
        RequestInProgress: boolean;
        IsSuccess: boolean;
        Errors: Object[];
        StatesList: Geography.StateDefinition[];

        registerInternalUser: Function;
        Logout: Function;
    }

    export class RegisterViewModel {
        private httpService: any;
        private _window: any;
        private _localStorageService: any;

        constructor($scope: RegistrationScope, usStates: Geography.StateDefinition[], authService: Application.AuthService) {
            var thisController = this;
            var accessToken = null;
            
            $scope.Errors = [];
            $scope.RequestInProgress = false;
            $scope.IsSuccess = false;
            $scope.EmailAddress = '';
            $scope.Password = '';
            $scope.PasswordConfirmation = '';
            $scope.StatesList = usStates;
            $scope.Logout = function () {
                authService.LogOut();
            };
            $scope.registerInternalUser = function () {
                $scope.RequestInProgress = true;
                $scope.Errors = [];
                authService.saveRegistration({
                    RoleCode: $scope.RoleCode,
                    Email: $scope.EmailAddress,
                    Password: $scope.Password,
                    ConfirmPassword: $scope.PasswordConfirmation,
                    FirstName: $scope.FirstName,
                    LastName: $scope.LastName,
                    Company: $scope.Company,
                    AddressLine1: $scope.AddressLine1,
                    AddressLine2: $scope.AddressLine2,
                    City: $scope.City,
                    State: $scope.State,
                    PostalCode: $scope.PostalCode,
                    Country: $scope.Country,
                    Phone: $scope.Phone
                }).then(function (data) {
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