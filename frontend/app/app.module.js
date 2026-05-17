angular.module('catApp', ['ngRoute'])
.config(function($routeProvider, $httpProvider) {
    $routeProvider
    .when('/login', {
        templateUrl: 'views/login.html',
        controller: 'LoginController',
    })
    .when('/shop', {
        templateUrl: 'views/shop.html',
        controller: 'CatController',
    })
    .when('/checkout', {
        templateUrl: 'views/checkout.html',
        controller: 'StepperController',
        resolve: {
            auth: ['$q', '$location', 'AuthService', function($q, $location, AuthService) {
                return checkPermission('RendelesekKeszitese', '/login')($q, $location, AuthService);
            }]
        }
    })
    .when('/admin', {
        templateUrl: 'views/admin.html',
        controller: 'AdminController',
        resolve: {
            auth: ['$q', '$location', 'AuthService', function($q, $location, AuthService) {
                return checkPermission('TermekekKezelese', '/login')($q, $location, AuthService);
            }]
        }
    })
    .otherwise({ redirectTo: '/shop' });

    // HTTP interceptor hozzáadása a token automatikus küldéséhez
    $httpProvider.interceptors.push(function($q, $location) {
        return {
            request: function(config) {
                const token = localStorage.getItem("token");
                if (token) {
                    config.headers["Authorization"] = "Bearer " + token;
                }
                return config;
            },
        };
    });
}).factory('AuthService', function() {
    return {
        hasPermission: function(permission) {
            const token = localStorage.getItem("token");
            if (!token) return false;
            const decoded = jwtDecode(token);
            return decoded && decoded[permission] === "true";
        }
    };
});

function checkAuth(permission) {
    return function($q, $location) {
        const token = localStorage.getItem("token");
        const lejaratStr = localStorage.getItem("lejaratiIdopont");
        
        if (token && lejaratStr) {
            const lejarat = new Date(lejaratStr);
            const most = new Date();
            
            if (lejarat - most > 0) return true; 
        }
        if (jwtDecode(token)[permission] === "true") return true;
        
        $location.path('/login');
        return $q.reject('Bejelentkezés szükséges');
    };
}

function checkPermission(permission, redirectPath) {
    return function($q, $location, AuthService) {
        if (AuthService.hasPermission(permission)) {
            return true;
        }
        $location.path(redirectPath);
        return $q.reject('Nincs jogosultság');
    };
}

function jwtDecode(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/'); 
        return JSON.parse(window.atob(base64));
    } catch (e) {
        console.error("JWT dekódolási hiba:", e);
        return null;
    }
};