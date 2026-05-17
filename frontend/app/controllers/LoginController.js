angular.module('catApp')
.controller('LoginController', function($scope, $location, $timeout, AuthModel) {
    $scope.inputs = AuthModel.getLoginFields();
    $scope.adatok = {};
    $scope.shakeClass = "";

    $scope.handleSubmit = function() {
        AuthModel.login($scope.adatok)
            .then(function(valasz) {
                AuthModel.saveSession($scope.adatok["Email-cím"], valasz.data);
                const decodedToken = $scope.jwtDecode(valasz.data.token);
                if (decodedToken && decodedToken["RendelesekKeszitese"] === "false") {
                    $location.path('/shop');
                } else if (decodedToken /* && decodedToken["TermekekKezelese"] === "true" */) {
                    $location.path('/admin');
                }
            })
            .catch(function(error) {
                // HIBA: Shake effektus hozzáadása a formhoz
                $scope.shakeClass = "shake";
                $timeout(function() {
                    $scope.shakeClass = "";
                }, 500);
            });
    };

    $scope.jwtDecode = function(token) {
        try {
            const base64Url = token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            return JSON.parse(window.atob(base64));
        } catch (e) {
            console.error("JWT dekódolási hiba:", e);
            return null;
        }
    };
});