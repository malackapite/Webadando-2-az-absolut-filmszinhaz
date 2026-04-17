angular.module('catApp')
.controller('LoginController', function($scope, $location, $timeout, AuthModel) {
    $scope.inputs = AuthModel.getLoginFields();
    $scope.adatok = {};
    $scope.shakeClass = "";

    $scope.handleSubmit = function() {
        AuthModel.login($scope.adatok)
            .then(function(valasz) {
                AuthModel.saveSession($scope.adatok.email, valasz.data.token);
                

                $location.path('/shop');
            })
            .catch(function(error) {
                $location.path('/shop');
                // HIBA: Shake effektus hozzáadása a formhoz
                $scope.shakeClass = "shake";
                $timeout(function() {
                    $scope.shakeClass = "";
                }, 500);
            });
    };
});