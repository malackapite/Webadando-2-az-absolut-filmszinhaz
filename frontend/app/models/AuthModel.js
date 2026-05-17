angular.module('catApp')
.factory('AuthModel', function($http) {
    const API_BASE_URL = "https://localhost:7063";
    return {
        
        getLoginFields: function() {
            return [
                { columnName: "Email-cím", dataType: "email" },
                { columnName: "Jelszó", dataType: "password" }
            ];
        },
        
        login: function(adatok) {
            const backendAdatok = {
                email: adatok["Email-cím"],
                password: adatok["Jelszó"]
            };
            return $http.post(`${API_BASE_URL}/felhasznalo/login`, backendAdatok);
        },

        saveSession: function(email, responseData) {
            localStorage.setItem("felhasznalo", email);
            localStorage.setItem("token", responseData.token);
            localStorage.setItem("lejaratiIdopont", responseData.lejaratiIdopont);
        }
    };
});