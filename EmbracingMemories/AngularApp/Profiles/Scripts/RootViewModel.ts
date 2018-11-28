declare var profileModule;
profileModule.controller("rootViewModel",
    function ($scope: any) {

        // This is the parent controller/viewmodel for 'customerModule' and its $scope is accesible
        // down controllers set by the routing engine. This controller is bound to the Customer.cshtml in the
        // Home view-folder.

        var initialize = function () {
            $scope.pageHeading = "Customer Section";
        }

        initialize();
    });
