angular.module('catApp')
.factory('AuthModel', function($http) {
    return {
        
        getLoginFields: function() {
            return [
                { columnName: "Email-cím", dataType: "email" },
                { columnName: "Jelszó", dataType: "password" }
            ];
        },
        
        login: function(adatok) {
            return $http.post("kezelok/login", adatok);
        },

        saveSession: function(email, responseData) {
            localStorage.setItem("felhasznalo", email);
            localStorage.setItem("token", responseData.token);
            localStorage.setItem("lejaratiIdopont", responseData.lejaratiIdopont);
        }
    };
});