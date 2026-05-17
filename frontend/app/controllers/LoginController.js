angular.module('catApp')
.controller('LoginController', function($http, $scope, $location, $timeout, AuthModel) {
    $scope.inputs = AuthModel.getLoginFields();
    $scope.adatok = {};
    $scope.shakeClass = "";
    $scope.isRegisterMode = false;
    const API_BASE_URL = "https://localhost:7063";

    $scope.toggleMode = function() {
        $scope.isRegisterMode = !$scope.isRegisterMode;
        $scope.adatok = {};
        $scope.shakeClass = "";
    };


    $scope.handleSubmit = function() {
        if ($scope.isRegisterMode) {
            $http.post(`${API_BASE_URL}/felhasznalo`, {
                email: $scope.adatok["Email-cím"],
                jelszo: $scope.adatok["Jelszó"],
                engedelyek: [
                    "RendelesekKeszitese",
                ]
            })
            .then(function(resp) {
                const toastElem = document.getElementById('regisztracioToast');
                if (toastElem) {
                    const toast = new bootstrap.Toast(toastElem);
                    toast.show();
                }
                $scope.toggleMode();
            })
            .catch(function(err) {
                console.error("Regisztrációs hiba:", err);
                $scope.shakeClass = "shake"; 
            });
        } else {
            AuthModel.login($scope.adatok)
            .then(function(valasz) {
                AuthModel.saveSession($scope.adatok["Email-cím"], valasz.data);
                const decodedToken = jwtDecode(valasz.data.token);
                if (decodedToken && decodedToken["TermekekKezelese"] === "true") {
                    $location.path('/admin');
                }
                else if (decodedToken) {
                    $location.path('/shop');
                }
            })
            .catch(function(error) {
                // HIBA: Shake effektus hozzáadása a formhoz
                $scope.shakeClass = "shake";
                $timeout(function() {
                    $scope.shakeClass = "";
                }, 500);
            });
        }
    };
});