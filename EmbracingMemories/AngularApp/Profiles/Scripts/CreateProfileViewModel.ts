declare var SqPaymentForm;
declare var Stripe;

module Profiles {
    export class CreateProfileViewModel {
        private httpService: any;
        private _window: any;
        private _localStorageService: any;
        private _paymentForm: any;
        private stripe: any;
        private card: any;

        constructor($scope: ICreateProfileScope, $http: any, $window: any, $routeParams: any, usStates: Geography.StateDefinition[], authService: Application.AuthService) {
            this.httpService = $http;
            var thisController = this;
            var accessToken = null;
            $scope.Errors = [];
            $scope.ShowPageMessage = false;
            $scope.StatesList = usStates;
            $scope.Message = '';

            $scope.model = new CreateProfileModel();
            $scope.model.FirstName = '';
            $scope.model.LastName = '';
            $scope.model.PaymentError = '';
            $scope.showPaymentForm = true;
            $scope.submitPaymentData = function (e) {
                $scope.model.CardToken = '';
                
                var formElement = angular.element(e.target);

                // Request a token from Stripe:
                this.stripe.createToken(this.card).then(function (response) {
                    if (response.error) { // Problem!

                        // Show the errors on the form:
                        $scope.$apply(function () {
                            $scope.model.PaymentError = response.error.message;
                            $scope.ShowPageMessage = true;
                            $scope.Message = "A payment error has occurred";
                        });

                    } else { // Token was created!

                        // Get the token ID:
                        $scope.model.CardToken = response.token.id;
                        CreateProfile();
                    }
                });

                // Prevent the form from being submitted:
                return false;
            };

            function CreateProfile() {
                $scope.Errors = [];

                thisController.httpService.post('/api/QrProfiles/', $scope.model, {
                    headers: authService.getHeaders()
                }).then(function (response) {
                    $window.location.assign("/Profiles/" + response.data.Id + "/edit");
                },
                    function (err, status) {
                        $scope.ShowPageMessage = true;
                        $scope.Message = "An error has occurred saving your profile";
                        $scope.RequestInProgress = false;
                        Application.ProcessErrorResponse(err, $scope.Errors);
                    });
            };

            $scope.init = function (user: any) {
                $scope.model.UserFirstName = user.FirstName;
                $scope.model.UserLastName = user.LastName;
                $scope.model.UserAddressLine1 = user.AddressLine1;
                $scope.model.UserAddressLine2 = user.AddressLine2;
                $scope.model.UserCity = user.City;
                $scope.model.UserState = user.State;
                $scope.model.UserCountry = user.Country;
                $scope.model.UserPostalCode = user.PostalCode;
                if (user.PhoneNumber != null) {
                    $scope.model.UserPhone = user.PhoneNumber.replace(/[^0-9]/g, "");
                }
                else {
                    $scope.model.UserPhone = null;
                }

                $scope.model.UserEmail = user.Email;
                // Create an instance of Elements
                this.stripe = new Stripe($window._stripePublicKey);
                var elements = this.stripe.elements();

                // Custom styling can be passed to options when creating an Element.
                // (Note that this demo uses a wider set of styles than the guide below.)
                var style = {
                    base: {
                        color: '#32325d',
                        lineHeight: '18px',
                        fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
                        fontSmoothing: 'antialiased',
                        fontSize: '16px',
                        '::placeholder': {
                            color: '#aab7c4'
                        }
                    },
                    invalid: {
                        color: '#fa755a',
                        iconColor: '#fa755a'
                    }
                };

                // Create an instance of the card Element
                this.card = elements.create('card', { style: style });

                // Add an instance of the card Element into the `card-element` <div>
                this.card.mount('#card-element');

                // Handle real-time validation errors from the card Element.
                this.card.addEventListener('change', function (event) {
                    var displayError = document.getElementById('card-errors');
                    if (event.error) {
                        displayError.textContent = event.error.message;
                    } else {
                        displayError.textContent = '';
                    }
                });
            };

            $scope.saveProfile = function () {
                //this._paymentForm.requestCardNonce();
                $scope.Errors = [];
                thisController.httpService.post('/api/QrProfiles/', $scope.model, {
                    headers: authService.getHeaders()
                }).then(function (response) {
                    $window.location.assign("/Profiles/" + response.data.Id + "/edit");
                },
                    function (err, status) {
                        $scope.ShowPageMessage = true;
                        $scope.Message = "An error has occurred saving your profile";
                        $scope.RequestInProgress = false;
                        Application.ProcessErrorResponse(err, $scope.Errors);
                    });
            };


        }
    }
}